using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Books;
using LMS.Application.Contracts.DTOs.Configs;
using LMS.Application.Contracts.DTOs.UserMembershipMapping;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Application.Services.Constants;
using LMS.Common.ErrorHandling.CustomException;
using LMS.Common.Helpers;
using LMS.Common.Models;
using LMS.Core.Entities;
using LMS.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace LMS.Application.Services.Services;

internal class BookService : IBookService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly HttpContext? _httpContext;
    private readonly IMapper _mapper;
    private readonly IValidationService _validationService;

    public BookService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, IValidationService validationService)
    {
        _repositoryManager = repositoryManager;
        _mapper = mapper;
        _httpContext = httpContextAccessor.HttpContext;
        _validationService = validationService;
    }

    public async Task<PaginatedResponseDto<T>> GetAllBookAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null, long? genreId = null) where T : class
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        var bookQuery = _repositoryManager.BooksRepository
            .GetAllAsync(isActive: authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase) ? isActive : true)
            .Where(x => !genreId.HasValue || x.GenreId == genreId);

        var totalCount = await bookQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var books = await bookQuery
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .Sort<T>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<T>()
        {
            Pagination = paginationModel,
            Data = books
        };
    }

    public async Task<PaginatedResponseDto<GetBookDto>> GetAllBookByNameFilterAsync(string nameStartWith, int? pageNumber = null, int? pageSize = null, int? latestDataCount = null)
    {
        var isLatter = char.IsLetter(nameStartWith.First());

        var bookQuery = _repositoryManager.BooksRepository
            .FindByCondition(x => x.IsActive && (!isLatter || x.Title.ToLower().StartsWith(nameStartWith.ToLower())));

        var totalCount = await bookQuery.CountAsync();

        var paginationModel = new PaginationModel(pageNumber: (int)(pageNumber ?? 1), pageSize: (int)(pageSize ?? 10), totalCount: totalCount);

        var books = await bookQuery
            .OrderBy(x => x.Title)
            .ProjectTo<GetBookDto>(_mapper.ConfigurationProvider)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        if (!isLatter)
            books = books.Where(x => !char.IsLetter(x.Title.FirstOrDefault())).ToList();

        var latestBook = await _repositoryManager.BooksRepository
            .GetAllAsync(isActive: true)
            .Sort<Books>(orderBy: "CreatedAt desc")
            .ProjectTo<GetBookDto>(_mapper.ConfigurationProvider)
            .Paginate(pageSize: (int)(latestDataCount ?? 9), pageNumber: 1)
            .ToListAsync();

        return new PaginatedResponseDto<GetBookDto>()
        {
            Pagination = paginationModel,
            Data = books,
            Data1 = latestBook
        };
    }

    public async Task<List<SelectListItem>> GetAllBookSelectionAsync(long[]? bookStatusList = null, long? bookId = null)
    {
        var bookSelection = await _repositoryManager.BooksRepository.GetAllAsync(isActive: true)
            .Where(x => (bookId == null || x.Id == bookId) && (bookStatusList == null || bookStatusList.Contains(x.StatusId)))
            .ProjectTo<SelectListItem>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return bookSelection ?? new List<SelectListItem>();
    }

    public async Task<T> GetBookByIdAsync<T>(long id) where T : class
    {
        var book = await _repositoryManager.BooksRepository
            .GetByIdAsync(id)
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return book ?? Activator.CreateInstance<T>();
    }

    public async Task<GetBookInfoDto> GetBookInfoByIdAsync(long id)
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        if (!isLogin)
            throw new BadRequestException("It is necessary for you to log in");

        var book = await _repositoryManager.BooksRepository
            .GetByIdAsync(id)
            .ProjectTo<GetBookInfoDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync() ?? new GetBookInfoDto();

        var loginUserMembership = await _repositoryManager.UserMembershipMappingRepository
            .FindByCondition(x => x.UserId == authUserId && x.IsActive && x.EffectiveStartDate < DateTimeOffset.UtcNow && x.ExpirationDate > DateTimeOffset.UtcNow)
            .ProjectTo<GetUserMembershipDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        var reservationQuery = _repositoryManager.ReservationRepository.FindByCondition(x => x.UserId == authUserId && x.IsActive && !StatusGroups.Reservation.Finalized.Contains(x.StatusId));

        var transectionQuery = _repositoryManager.TransectionRepository.FindByCondition(x => x.UserId == authUserId && x.IsActive && !StatusGroups.Transaction.Finalized.Contains(x.StatusId));

        bool hasNotExceededLimit = await transectionQuery.LongCountAsync() < loginUserMembership?.BorrowLimit;
        bool isAvailableOrReservedByUser = book.StatusLabel == nameof(BookStatusEnum.Available)
            || (book.StatusLabel == nameof(BookStatusEnum.Reserved) && await reservationQuery.AnyAsync(x => x.BookId == id));

        book.canBorrow = loginUserMembership?.BorrowLimit != null
                            ? hasNotExceededLimit && isAvailableOrReservedByUser
                            : null;

        book.canReserve = loginUserMembership?.ReservationLimit != null
                            ? await reservationQuery.LongCountAsync() < loginUserMembership?.ReservationLimit
                            : null;

        book.IsBorrowed = await transectionQuery.AnyAsync(x => x.BookId == id);
        book.IsReserved = await reservationQuery.AnyAsync(x => x.BookId == id);
        book.IsAllocated = reservationQuery.FirstOrDefault(x => x.BookId == id)?.IsAllocated ?? false;

        return book;
    }

    public async Task<byte[]> ExportBookData()
    {
        var books = await GetAllBookAsync<ExportBookDto>();

        return FileService.ExportMultipleSheets(new Dictionary<string, IEnumerable> { { "Books", books.Data } });
    }

    public async Task AddBookAsync(AddBookDto book)
    {
        _validationService.Validate<AddBookDto>(book);

        if (!Enum.IsDefined(typeof(BookStatusEnum), book.StatusId))
            throw new BadRequestException("Undefined Book status");

        if (book.CoverPage is null)
            throw new BadRequestException("Cover page is required");

        var mappedBook = _mapper.Map<Books>(book);

        if (book.CoverPage is not null)
            mappedBook.BookFileMappings.Add(await AddBookCoverPage(file: book.CoverPage));

        if (book.BookPreview is not null)
            mappedBook.BookFileMappings.Add(await AddBookPreviewFile(file: book.BookPreview));

        await _repositoryManager.BooksRepository.AddAsync(mappedBook);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task UpdateBookAsync(UpdateBookDto book)
    {
        _validationService.Validate<UpdateBookDto>(book);

        if (!Enum.IsDefined(typeof(BookStatusEnum), book.StatusId))
            throw new BadRequestException("Undefined Book status");

        var existBook = await _repositoryManager.BooksRepository.GetByIdAsync(id: book.Id).FirstOrDefaultAsync();

        if (existBook == null)
            throw new BadRequestException("This Book is not available");

        if ((book.CoverPage is not null && !book.IsDeletedCoverPage) || (book.BookPreview is not null && !book.IsDeletedBookPreview))
            throw new BadRequestException("Something wrong with the request");

        if (book.CoverPage is null && book.IsDeletedCoverPage)
            throw new BadRequestException("Cover page is required");

        var mappedBook = _mapper.Map(book, existBook);

        if (existBook.BookFileMappings is not null && existBook.BookFileMappings.Any())
        {
            if (book.IsDeletedCoverPage)
                mappedBook.BookFileMappings = await ClearBookCoverPageStorage(bookFileMappings: mappedBook.BookFileMappings);

            if (book.IsDeletedBookPreview)
                mappedBook.BookFileMappings = await ClearBookPreviewStorage(bookFileMappings: mappedBook.BookFileMappings);
        }

        if (book.CoverPage is not null)
            mappedBook.BookFileMappings.Add(await AddBookCoverPage(file: book.CoverPage));

        if (book.BookPreview is not null)
            mappedBook.BookFileMappings.Add(await AddBookPreviewFile(file: book.BookPreview));

        _repositoryManager.BooksRepository.Update(entity: mappedBook);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task DeleteBookAsync(long id)
    {
        var existBook = await _repositoryManager.BooksRepository.GetByIdAsync(id: id).FirstOrDefaultAsync();

        if (existBook == null)
            throw new BadRequestException("This Book is already not exist");

        if (await _repositoryManager.TransectionRepository.AnyAsync(x => x.IsActive && x.BookId == id && !StatusGroups.Transaction.Finalized.Contains(x.StatusId)))
        {
            existBook.StatusId = (long)BookStatusEnum.Removed;
        }
        else
        {
            existBook.IsActive = false;

            existBook.BookFileMappings = await ClearBookCoverPageStorage(bookFileMappings: existBook.BookFileMappings);
            existBook.BookFileMappings = await ClearBookPreviewStorage(bookFileMappings: existBook.BookFileMappings);
        }

        _repositoryManager.BooksRepository.Update(entity: existBook);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task PermanentDeleteBookAsync(long id)
    {
        var existBook = await _repositoryManager.BooksRepository.GetAllAsync(isActive: false).FirstOrDefaultAsync(x => x.Id == id);

        if (existBook == null)
            throw new BadRequestException("This Book is already not exist");

        _repositoryManager.BooksRepository.Remove(entity: existBook);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    #region Private section
    private async Task<List<BookFileMapping>> ClearBookCoverPageStorage(List<BookFileMapping> bookFileMappings)
    {
        var archiveDirectory = (await _repositoryManager.ConfigRepository
                .GetByKeyNameAsync("BookCoverPageArchiveDirectoryPath")
                .FirstOrDefaultAsync())?
                .KeyValue ?? throw new ArgumentException("Archive file directory path not found!");

        foreach (var item in bookFileMappings)
        {
            if (item.Label.Equals(nameof(BookFileTypeEnum.CoverPage), StringComparison.InvariantCultureIgnoreCase))
            {
                await FileService.MoveFileToArchive(sourceFile: item.fileLocation, archiveDirectory: archiveDirectory);
                item.IsActive = false;
            }
        }
        return bookFileMappings;
    }

    private async Task<List<BookFileMapping>> ClearBookPreviewStorage(List<BookFileMapping> bookFileMappings)
    {
        var archiveDirectory = (await _repositoryManager.ConfigRepository
                .GetByKeyNameAsync("BookPreviewArchiveDirectoryPath")
                .FirstOrDefaultAsync())?
                .KeyValue ?? throw new ArgumentException("Archive file directory path not found!");

        foreach (var item in bookFileMappings)
        {
            if (item.Label.Equals(nameof(BookFileTypeEnum.BookPreview), StringComparison.InvariantCultureIgnoreCase))
            {
                await FileService.MoveFileToArchive(sourceFile: item.fileLocation, archiveDirectory: archiveDirectory);
                item.IsActive = false;
            }
        }
        return bookFileMappings;
    }

    private async Task<BookFileMapping> AddBookCoverPage(IFormFile file)
    {
        var booksFileStorageInfo = await _repositoryManager.ConfigRepository
               .GetByKeyNameListAsync(new List<string>() { "BookCoverPageDirectoryPath", "BookCoverPageArchiveDirectoryPath", "ImageFileExtentions" })
               .ProjectTo<GetConfigsValueDto>(_mapper.ConfigurationProvider)
               .ToListAsync();

        var archiveDirectory = booksFileStorageInfo
                .FirstOrDefault(x => x.KeyName.Equals("BookCoverPageArchiveDirectoryPath", StringComparison.InvariantCultureIgnoreCase))?
                .KeyValue ?? throw new ArgumentException("Archive file directory path not found!");

        var imageFileExtentions = booksFileStorageInfo
                .FirstOrDefault(x => x.KeyName.Equals("ImageFileExtentions", StringComparison.InvariantCultureIgnoreCase))?.KeyValue
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ext => ext.Trim())
                .ToArray() ?? throw new ArgumentException("Valid file extension list not found!");

        var sourceFileDirectory = booksFileStorageInfo
            .FirstOrDefault(x => x.KeyName.Equals("BookCoverPageDirectoryPath", StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? throw new ArgumentException("Source file directory path not found!");

        return new BookFileMapping()
        {
            Id = 0,
            Label = nameof(BookFileTypeEnum.CoverPage),
            fileLocation = await FileService.FileUpload(file: file, sourceFileDirectory: sourceFileDirectory, archiveDirectory: archiveDirectory, allowedExtensions: imageFileExtentions)
        };
    }

    private async Task<BookFileMapping> AddBookPreviewFile(IFormFile file)
    {
        var booksFileStorageInfo = await _repositoryManager.ConfigRepository
               .GetByKeyNameListAsync(new List<string>() { "BookPreviewDirectoryPath", "BookPreviewArchiveDirectoryPath" })
               .ProjectTo<GetConfigsValueDto>(_mapper.ConfigurationProvider)
               .ToListAsync();

        var archiveDirectory = booksFileStorageInfo
                    .FirstOrDefault(x => x.KeyName.Equals("BookPreviewArchiveDirectoryPath", StringComparison.InvariantCultureIgnoreCase))?
                    .KeyValue ?? throw new ArgumentException("Archive file directory path not found!");

        var sourceFileDirectory = booksFileStorageInfo
                .FirstOrDefault(x => x.KeyName.Equals("BookPreviewDirectoryPath", StringComparison.InvariantCultureIgnoreCase))?
                .KeyValue ?? throw new ArgumentException("Source file directory path not found!");

        return new BookFileMapping()
        {
            Id = 0,
            Label = nameof(BookFileTypeEnum.BookPreview),
            fileLocation = await FileService.FileUpload(file: file, sourceFileDirectory: sourceFileDirectory, archiveDirectory: archiveDirectory, allowedExtensions: [".pdf"])
        };
    }
    #endregion
}
