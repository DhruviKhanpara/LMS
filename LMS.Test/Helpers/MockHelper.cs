using AutoMapper;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Application.Services.Mappings;
using LMS.Application.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Security.Claims;

namespace LMS.Tests.Helpers
{
    /// <summary>
    /// Helper class for creating and configuring mocks used across service tests
    /// Centralizes mock creation to maintain consistency and reduce duplication
    /// </summary>
    public static class MockHelper
    {
        #region Repository Mocks

        /// <summary>
        /// Creates a mock IRepositoryManager with all repository properties accessible
        /// By default, returns mock repositories that can be further configured in tests
        /// </summary>
        public static Mock<IRepositoryManager> CreateRepositoryManager()
        {
            var mock = new Mock<IRepositoryManager>();

            // Setup mock repositories - these can be further configured in individual tests
            // repo is injected as Lazy loading so when need to access something from perticular repo then need to setup here else it work fine
            mock.Setup(x => x.BooksRepository).Returns(CreateBookRepository().Object);
            mock.Setup(x => x.ConfigRepository).Returns(CreateConfigRepository().Object);
            mock.Setup(x => x.GenreRepository).Returns(CreateGenreRepository().Object);
            mock.Setup(x => x.MembershipRepository).Returns(CreateMembershipRepository().Object);
            mock.Setup(x => x.OutboxMessageRepository).Returns(CreateOutboxMessageRepository().Object);
            mock.Setup(x => x.PenaltyRepository).Returns(CreatePenaltyRepository().Object);
            mock.Setup(x => x.ReservationRepository).Returns(CreateReservationRepository().Object);
            mock.Setup(x => x.TransectionRepository).Returns(CreateTransectionRepository().Object);
            mock.Setup(x => x.UnitOfWork).Returns(CreateUnitOfWorkRepository().Object);
            mock.Setup(x => x.UserRepository).Returns(CreateUserRepository().Object);
            mock.Setup(x => x.UserMembershipMappingRepository).Returns(CreateUserMembershipRepository().Object);

            return mock;
        }

        /// <summary>
        /// Creates a mock IPenaltyRepository
        /// Configure specific behavior in individual tests
        /// </summary>
        public static Mock<IPenaltyRepository> CreatePenaltyRepository()
        {
            var mock = new Mock<IPenaltyRepository>();

            // Default setup - can be overridden in tests
            // Example: mock.Setup(x => x.GetByIdAsync(It.IsAny<long>()))
            //              .ReturnsAsync(TestDataBuilder.CreatePenaltyEntity());

            return mock;
        }

        public static Mock<IBookRepository> CreateBookRepository()
        {
            return new Mock<IBookRepository>();
        }

        public static Mock<IConfigRepository> CreateConfigRepository()
        {
            var mock = new Mock<IConfigRepository>();

            // Setup common configuration values
            // Example:
            // mock.Setup(x => x.GetPenaltyRateAsync()).ReturnsAsync(5m);
            // mock.Setup(x => x.GetBufferDaysAsync()).ReturnsAsync(5);

            return mock;
        }

        public static Mock<IGenreRepository> CreateGenreRepository()
        {
            return new Mock<IGenreRepository>();
        }

        public static Mock<IMembershipRepository> CreateMembershipRepository()
        {
            return new Mock<IMembershipRepository>();
        }

        public static Mock<IOutboxMessageRepository> CreateOutboxMessageRepository()
        {
            return new Mock<IOutboxMessageRepository>();
        }

        public static Mock<IReservationRepository> CreateReservationRepository()
        {
            return new Mock<IReservationRepository>();
        }

        public static Mock<ITransectionRepository> CreateTransectionRepository()
        {
            return new Mock<ITransectionRepository>();
        }

        public static Mock<IUnitOfWork> CreateUnitOfWorkRepository()
        {
            return new Mock<IUnitOfWork>();
        }

        public static Mock<IUserRepository> CreateUserRepository()
        {
            return new Mock<IUserRepository>();
        }

        public static Mock<IUserMembershipReporsitory> CreateUserMembershipRepository()
        {
            return new Mock<IUserMembershipReporsitory>();
        }

        #endregion

        #region Service Mocks

        public static Mock<IValidationService> CreateValidationService()
        {
            var mock = new Mock<IValidationService>();

            // Default: validation passes (does nothing)
            mock.Setup(x => x.Validate(It.IsAny<object>()));
            mock.Setup(x => x.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return mock;
        }

        #endregion

        #region Infrastructure Mocks

        /// <summary>
        /// Creates a REAL IMapper with AutoMapper configuration
        /// Uses actual mapping profiles to test real mapping logic
        /// This is better than mocking because it tests your actual mapping conditions
        /// </summary>
        public static IMapper CreateRealMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // Add your actual mapping profile here
                cfg.AddProfile<PenaltyMapperProfile>();
                cfg.AddProfile<ConfigsMapperProfile>();
                // Add other profiles as needed
            });

            return config.CreateMapper();
        }

        /// <summary>
        /// Creates a mock IMapper (use only if you don't want to test mapping logic)
        /// NOT RECOMMENDED - use CreateRealMapper() instead
        /// </summary>
        public static Mock<IMapper> CreateMapper()
        {
            var mock = new Mock<IMapper>();

            // Default behavior - override in specific tests
            // Example:
            // mock.Setup(x => x.Map<GetPenaltyDto>(It.IsAny<Penalty>()))
            //     .Returns(TestDataBuilder.CreateValidGetPenaltyDto());

            return mock;
        }

        /// <summary>
        /// Creates a mock IHttpContextAccessor with a default HttpContext
        /// Useful for services that need HttpContext (like for getting current user)
        /// </summary>
        /// <param name="userId">Optional user ID to set in claims</param>
        /// <param name="role">Optional role to set in claims (Admin, Librarian, User)</param>
        public static Mock<IHttpContextAccessor> CreateHttpContextAccessor(string? userId = null, string? role = null)
        {
            var mock = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();

            if (!string.IsNullOrEmpty(userId))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                };

                if (!string.IsNullOrEmpty(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var identity = new ClaimsIdentity(claims, "TestAuth");
                var principal = new ClaimsPrincipal(identity);
                httpContext.User = principal;
            }

            mock.Setup(x => x.HttpContext).Returns(httpContext);

            return mock;
        }

        public static Mock<ILogger<T>> CreateLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }

        #endregion

        #region Service Creation Helpers

        /// <summary>
        /// Creates a PenaltyService with all dependencies mocked
        /// Allows optional override of specific dependencies for custom test scenarios
        /// </summary>
        /// <param name="repositoryManager">Optional custom repository manager mock</param>
        /// <param name="validationService">Optional custom validation service mock</param>
        /// <param name="mapper">Optional custom mapper</param>
        /// <param name="httpContextAccessor">Optional custom HTTP context accessor mock</param>
        /// <param name="logger">Optional custom logger mock</param>
        /// <returns>Tuple of (PenaltyService instance, all mocks for verification)</returns>
        public static (
            PenaltyService service,
            Mock<IRepositoryManager> repositoryManager,
            Mock<IValidationService> validationService,
            IMapper mapper,
            Mock<IHttpContextAccessor> httpContextAccessor,
            Mock<ILogger<PenaltyService>> logger
        ) CreatePenaltyService(
            Mock<IRepositoryManager>? repositoryManager = null,
            Mock<IValidationService>? validationService = null,
            IMapper? mapper = null,
            Mock<IHttpContextAccessor>? httpContextAccessor = null,
            Mock<ILogger<PenaltyService>>? logger = null)
        {
            // Use provided mocks or create new ones
            var repoManagerMock = repositoryManager ?? CreateRepositoryManager();
            var validationServiceMock = validationService ?? CreateValidationService();
            var realMapper = mapper ?? CreateRealMapper();
            var httpContextAccessorMock = httpContextAccessor ?? CreateHttpContextAccessor();
            var loggerMock = logger ?? CreateLogger<PenaltyService>();

            var service = new PenaltyService(
                repoManagerMock.Object,
                httpContextAccessorMock.Object,
                realMapper,
                validationServiceMock.Object,
                loggerMock.Object
            );

            return (service, repoManagerMock, validationServiceMock, realMapper, httpContextAccessorMock, loggerMock);
        }

        #endregion

        #region Verification Helpers

        /// <summary>
        /// Verifies that a repository method was called with specific parameters
        /// Example usage in tests to verify correct repository interaction
        /// </summary>
        public static void VerifyRepositoryCall<TRepo>(
            Mock<TRepo> repositoryMock,
            Expression<Action<TRepo>> expression,
            Times times) where TRepo : class
        {
            repositoryMock.Verify(expression, times);
        }

        /// <summary>
        /// Verifies that validation was called for a specific DTO type
        /// </summary>
        public static void VerifyValidationCalled<TDto>(
            Mock<IValidationService> validationServiceMock,
            Times times) where TDto : class
        {
            validationServiceMock.Verify(
                x => x.ValidateAsync(It.IsAny<TDto>(), It.IsAny<CancellationToken>()),
                times
            );
        }

        #endregion
    }
}
