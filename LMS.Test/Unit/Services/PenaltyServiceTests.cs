using FluentAssertions;
using LMS.Application.Contracts.DTOs.Penalty;
using LMS.Core.Entities;
using MockQueryable;

namespace LMS.Tests.Unit.Services
{
    /// <summary>
    /// Unit tests for PenaltyService
    /// Demonstrates service layer testing with mocked dependencies
    /// This serves as a template for testing other services
    /// Pattern: Arrange dependencies → Setup mocks → Act → Assert result
    /// </summary>
    public class PenaltyServiceTests
    {
        #region GetAllPenaltyAsync Tests

        [Fact]
        public async Task GetAllPenaltyAsync_AsAdmin_WithoutFilters_ShouldReturnAllPenalties()
        {
            var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: "1", role: "Admin");
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService(httpContextAccessor: httpContextAccessor);

            var penalties = new List<Penalty>
            {
                TestDataBuilder.CreatePenaltyEntityWithId(1),
                TestDataBuilder.CreatePenaltyEntityWithId(2),
                TestDataBuilder.CreateRemovedPenaltyEntity()
            }.AsQueryable().BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetPenaltyOrderByDefault(It.IsAny<bool?>()))
                .Returns(penalties);

            var result = await service.GetAllPenaltyAsync<GetPenaltyDto>();

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(3);
            result.Pagination.TotalCount.Should().Be(3);
        }

        [Fact]
        public async Task GetAllPenaltyAsync_AsUser_WithUserFilter_ShouldOnlySeeOwnPenalties()
        {
            var currentUserId = 100L;

            var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: currentUserId.ToString(), role: "User");
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService(httpContextAccessor: httpContextAccessor);

            var penalty1 = TestDataBuilder.CreatePenaltyEntityWithId(id: 1);
            penalty1.UserId = currentUserId;
            penalty1.User = TestDataBuilder.CreateUserWithoutOptionalFieldsWithId(id: currentUserId);

            var penalty2 = TestDataBuilder.CreatePenaltyEntityWithId(2);
            penalty2.UserId = 200L;

            var penalties = new List<Penalty> { penalty1, penalty2 }
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetPenaltyOrderByDefault(It.IsAny<bool?>()))
                .Returns(penalties);

            var result = await service.GetAllPenaltyAsync<GetPenaltyDto>(userId: 200L);

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            result.Data.First().Id.Should().Be(1);
        }

        [Fact]
        public async Task GetAllPenaltyAsync_AsLibrarian_CanSeeOtherUsersPenalties()
        {
            var targetUserId = 200L;

            var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: "1", role: "Librarian");
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService(httpContextAccessor: httpContextAccessor);

            var penalty1 = TestDataBuilder.CreatePenaltyEntityWithId(id: 1);
            penalty1.User.Id = penalty1.UserId = 100L;

            var penalty2 = TestDataBuilder.CreatePenaltyEntityWithId(id: 2);
            penalty2.UserId = targetUserId;
            penalty2.User = TestDataBuilder.CreateUserWithoutOptionalFieldsWithId(id: targetUserId);

            var penalties = new List<Penalty> { penalty1, penalty2 }
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetPenaltyOrderByDefault(It.IsAny<bool?>()))
                .Returns(penalties);

            var result = await service.GetAllPenaltyAsync<GetPenaltyDto>(userId: targetUserId);

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            result.Data.First().UserName.Should().Be("Jane Smith");
        }

        [Fact]
        public async Task GetAllPenaltyAsync_WithPagination_ShouldReturnCorrectPage()
        {
            var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: "1", role: "Admin");
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService(httpContextAccessor: httpContextAccessor);

            var penalties = Enumerable.Range(1, 10)
                .Select(i => TestDataBuilder.CreatePenaltyEntityWithId(i))
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetPenaltyOrderByDefault(It.IsAny<bool?>()))
                .Returns(penalties);

            var result = await service.GetAllPenaltyAsync<GetPenaltyDto>(
                pageSize: 3,
                pageNumber: 2
            );

            result.Should().NotBeNull();
            result.Pagination.TotalCount.Should().Be(10);
            result.Pagination.PageSize.Should().Be(3);
            result.Pagination.PageNumber.Should().Be(2);
            result.Pagination.PageCount.Should().Be(4);
            result.Data.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAllPenaltyAsync_WithoutPagination_ShouldReturnAllItems()
        {
            var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: "1", role: "Admin");
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService(httpContextAccessor: httpContextAccessor);

            var penalties = Enumerable.Range(1, 5)
                .Select(i => TestDataBuilder.CreatePenaltyEntityWithId(i))
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetPenaltyOrderByDefault(It.IsAny<bool?>()))
                .Returns(penalties);

            var result = await service.GetAllPenaltyAsync<GetPenaltyDto>();

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(5);
            result.Pagination.TotalCount.Should().Be(5);
        }

        [Fact]
        public async Task GetAllPenaltyAsync_WithPenaltyTypeFilter_ShouldFilterCorrectly()
        {
            var targetPenaltyType = TestDataBuilder.CreateValidPenaltyTypeEntityWithId(2L);

            var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: "1", role: "Admin");
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService(httpContextAccessor: httpContextAccessor);

            var penalty1 = TestDataBuilder.CreatePenaltyEntityWithId(1);
            penalty1.PenaltyTypeId = 1L;

            var penalty2 = TestDataBuilder.CreatePenaltyEntityWithId(2);
            penalty2.PenaltyType = targetPenaltyType;
            penalty2.PenaltyTypeId = targetPenaltyType.Id;

            var penalty3 = TestDataBuilder.CreatePenaltyEntityWithId(3);
            penalty3.PenaltyType = targetPenaltyType;
            penalty3.PenaltyTypeId = targetPenaltyType.Id;

            var penalties = new List<Penalty> { penalty1, penalty2, penalty3 }
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetPenaltyOrderByDefault(It.IsAny<bool?>()))
                .Returns(penalties);

            var result = await service.GetAllPenaltyAsync<GetPenaltyDto>(
                penaltyTypeId: 2L
            );

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Data.Should().AllSatisfy(p => p.PenaltyTypeName.Should().Be(targetPenaltyType.Label));
        }

        [Fact]
        public async Task GetAllPenaltyAsync_AsAdmin_WithIsActiveFalse_ShouldReturnInactivePenalties()
        {
            var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: "1", role: "Admin");
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService(httpContextAccessor: httpContextAccessor);

            var inactivePenalty = TestDataBuilder.CreateRemovedPenaltyEntity();

            var activePenalty = TestDataBuilder.CreatePenaltyEntityWithId(2);
            activePenalty.IsActive = true;

            var penalties = new List<Penalty> { activePenalty, inactivePenalty }
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetPenaltyOrderByDefault(false))
                .Returns(penalties.Where(p => !p.IsActive));

            var result = await service.GetAllPenaltyAsync<GetPenaltyDto>(isActive: false);

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            result.Data.First().IsRemoved.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllPenaltyAsync_AsUser_ShouldOnlySeeActivePenalties()
        {
            var currentUserId = 100L;
            var currentUser = TestDataBuilder.CreateUserWithoutOptionalFieldsWithId(currentUserId);

            var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: currentUserId.ToString(), role: "User");
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService(httpContextAccessor: httpContextAccessor);

            var activePenalty = TestDataBuilder.CreatePenaltyEntityWithId(1);
            activePenalty.UserId = currentUserId;
            activePenalty.User = currentUser;

            var inactivePenalty = TestDataBuilder.CreateRemovedPenaltyEntity();
            inactivePenalty.Id = 2;
            inactivePenalty.UserId = currentUserId;
            inactivePenalty.User = currentUser;

            var penalties = new List<Penalty> { activePenalty, inactivePenalty }
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetPenaltyOrderByDefault(true))
                .Returns(penalties.Where(p => p.IsActive));

            var result = await service.GetAllPenaltyAsync<GetPenaltyDto>(isActive: false);

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            result.Data.First().IsRemoved.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllPenaltyAsync_WithSorting_ShouldApplySortOrder()
        {
            var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: "1", role: "Admin");
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService(httpContextAccessor: httpContextAccessor);

            var penalty1 = TestDataBuilder.CreatePenaltyEntityWithId(1);
            penalty1.Amount = 100m;

            var penalty2 = TestDataBuilder.CreatePenaltyEntityWithId(2);
            penalty2.Amount = 50m;

            var penalty3 = TestDataBuilder.CreatePenaltyEntityWithId(3);
            penalty3.Amount = 200m;

            var penalties = new List<Penalty> { penalty1, penalty2, penalty3 }
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetPenaltyOrderByDefault(It.IsAny<bool?>()))
                .Returns(penalties);

            var result = await service.GetAllPenaltyAsync<GetPenaltyDto>(
                orderBy: "Amount DESC"
            );

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(3);
            result.Data.First().Amount.Should().Be(200m);
            result.Data.Last().Amount.Should().Be(50m);
        }

        [Fact]
        public async Task GetAllPenaltyAsync_WithMultipleFilters_ShouldApplyAllFilters()
        {
            var targetUserId = 100L;
            var targetPenaltyTypeId = 2L;

            var targetUser = TestDataBuilder.CreateUserWithoutOptionalFieldsWithId(targetUserId);
            var targetPenaltyType = TestDataBuilder.CreateValidPenaltyTypeEntityWithId(targetPenaltyTypeId);

            var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: "1", role: "Librarian");
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService(httpContextAccessor: httpContextAccessor);

            var penalty1 = TestDataBuilder.CreatePenaltyEntityWithId(1);
            penalty1.UserId = targetUserId;
            penalty1.User = targetUser;
            penalty1.PenaltyTypeId = targetPenaltyTypeId;
            penalty1.PenaltyType = targetPenaltyType;

            var penalty2 = TestDataBuilder.CreatePenaltyEntityWithId(2);
            penalty1.UserId = targetUserId;
            penalty2.User = targetUser;
            penalty2.PenaltyTypeId = 1L;

            var penalty3 = TestDataBuilder.CreatePenaltyEntityWithId(3);
            penalty3.UserId = 200L;
            penalty3.PenaltyTypeId = targetPenaltyTypeId;
            penalty3.PenaltyType = targetPenaltyType;

            var penalties = new List<Penalty> { penalty1, penalty2, penalty3 }
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetPenaltyOrderByDefault(It.IsAny<bool?>()))
                .Returns(penalties);

            var result = await service.GetAllPenaltyAsync<GetPenaltyDto>(
                userId: targetUserId,
                penaltyTypeId: targetPenaltyTypeId,
                pageSize: 10,
                pageNumber: 1
            );

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            result.Data.First().UserName.Should().Be("Jane Smith");
            result.Data.First().PenaltyTypeName.Should().Be(targetPenaltyType.Label);
        }

        [Fact]
        public async Task GetAllPenaltyAsync_EmptyResult_ShouldReturnEmptyList()
        {
            var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: "1", role: "Admin");
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService(httpContextAccessor: httpContextAccessor);

            var emptyPenalties = new List<Penalty>().AsQueryable().BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetPenaltyOrderByDefault(It.IsAny<bool?>()))
                .Returns(emptyPenalties);

            var result = await service.GetAllPenaltyAsync<GetPenaltyDto>();

            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();
            result.Pagination.TotalCount.Should().Be(0);
            result.Pagination.PageCount.Should().Be(1);
        }

        #endregion

        #region GetPenaltyByIdAsync Tests

        /// <summary>
        /// Template test showing how to test a service method that returns data
        /// </summary>
        [Fact]
        public async Task GetPenaltyByIdAsync_WithValidId_ShouldReturnMappedPenaltyDto()
        {
            var penaltyId = 1L;

            var (service, repoManager, _, mapper, _, _) = MockHelper.CreatePenaltyService();

            var penaltyEntities = new List<Penalty> { TestDataBuilder.CreatePenaltyEntityWithId(penaltyId) }
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetByIdAsync(penaltyId))
                .Returns(penaltyEntities);

            var result = await service.GetPenaltyByIdAsync<GetPenaltyDto>(penaltyId);

            result.Should().NotBeNull();
            result.Id.Should().Be(penaltyId);

            result.UserName.Should().Be("John Michael Doe");
            result.StatusLabel.Should().Be("Unpaid");
            result.PenaltyTypeName.Should().Be("Overdue");
            result.StatusLabelBgColor.Should().EndWith("29");
            result.IsRemoved.Should().BeFalse();
            result.TransectionDueDate.Should().NotBeNull();
            result.Should().BeOfType<GetPenaltyDto>();

            repoManager.Verify(x => x.PenaltyRepository.GetByIdAsync(penaltyId), Times.Once);
        }

        [Fact]
        public async Task GetPenaltyByIdAsync_WithNonExistentId_ShouldReturnEmptyInstance()
        {
            var penaltyId = 999L;

            var (service, repoManager, _, mapper, _, _) = MockHelper.CreatePenaltyService();

            var emptyList = new List<Penalty>().AsQueryable().BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetByIdAsync(penaltyId))
                .Returns(emptyList);

            var result = await service.GetPenaltyByIdAsync<GetPenaltyDto>(penaltyId);

            result.Should().NotBeNull();
            result.Id.Should().Be(0);

            repoManager.Verify(x => x.PenaltyRepository.GetByIdAsync(penaltyId), Times.Once);
        }

        [Fact]
        public async Task GetPenaltyByIdAsync_WithZeroId_ShouldReturnEmptyInstance()
        {
            var penaltyId = 0L;

            var (service, repoManager, _, mapper, _, _) = MockHelper.CreatePenaltyService();

            var emptyList = new List<Penalty>().AsQueryable().BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetByIdAsync(penaltyId))
                .Returns(emptyList);

            var result = await service.GetPenaltyByIdAsync<GetPenaltyDto>(penaltyId);

            result.Should().NotBeNull();
            result.Id.Should().Be(0);

            repoManager.Verify(x => x.PenaltyRepository.GetByIdAsync(penaltyId), Times.Once);
        }

        [Fact]
        public async Task GetPenaltyByIdAsync_WithNegativeId_ShouldReturnEmptyInstance()
        {
            var penaltyId = -1L;

            var (service, repoManager, _, mapper, _, _) = MockHelper.CreatePenaltyService();

            var emptyList = new List<Penalty>().AsQueryable().BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetByIdAsync(penaltyId))
                .Returns(emptyList);

            var result = await service.GetPenaltyByIdAsync<GetPenaltyDto>(penaltyId);

            result.Should().NotBeNull();

            repoManager.Verify(x => x.PenaltyRepository.GetByIdAsync(penaltyId), Times.Once);
        }

        [Fact]
        public async Task GetPenaltyByIdAsync_WithNullMiddleName_ShouldMapUserNameCorrectly()
        {
            var penaltyId = 2L;

            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService();

            var penaltyEntity = TestDataBuilder.CreatePenaltyEntityWithId(penaltyId);
            penaltyEntity.User = TestDataBuilder.CreateUserWithoutOptionalFields();

            var penaltyEntities = new List<Penalty> { penaltyEntity }
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetByIdAsync(penaltyId))
                .Returns(penaltyEntities);

            var result = await service.GetPenaltyByIdAsync<GetPenaltyDto>(penaltyId);

            result.Should().NotBeNull();
            result.UserName.Should().Be("Jane Smith");
            result.UserName.Should().NotContain("  ");
            result.UserProfilePhoto.Should().StartWith("https://");
        }

        [Fact]
        public async Task GetPenaltyByIdAsync_ShouldMapStatusLabelBgColorWithOpacity()
        {
            var penaltyId = 5L;

            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService();

            var penaltyEntity = TestDataBuilder.CreatePenaltyEntityWithId(penaltyId);

            var penaltyEntities = new List<Penalty> { penaltyEntity }
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetByIdAsync(penaltyId))
                .Returns(penaltyEntities);

            var result = await service.GetPenaltyByIdAsync<GetPenaltyDto>(penaltyId);

            result.Should().NotBeNull();
            result.StatusLabelColor.Should().Be("#FF0000");
            result.StatusLabelBgColor.Should().Be("#FF000029");
        }

        [Fact]
        public async Task GetPenaltyByIdAsync_WithRemovedPenalty_ShouldMapIsRemovedCorrectly()
        {
            var penaltyId = 4L;

            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService();

            var penaltyEntity = TestDataBuilder.CreateRemovedPenaltyEntity();
            penaltyEntity.Id = penaltyId;

            var penaltyEntities = new List<Penalty> { penaltyEntity }
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetByIdAsync(penaltyId))
                .Returns(penaltyEntities);

            var result = await service.GetPenaltyByIdAsync<GetPenaltyDto>(penaltyId);

            result.Should().NotBeNull();
            result.IsRemoved.Should().BeTrue();
        }

        [Fact]
        public async Task GetPenaltyByIdAsync_CanReturnDifferentDtoTypes()
        {
            var penaltyId = 1L;

            var (service, repoManager, _, mapper, _, _) = MockHelper.CreatePenaltyService();

            var penaltyEntities = new List<Penalty>
            {
                new Penalty
                {
                    Id = penaltyId,
                    UserId = 100,
                    Amount = 50.00m,
                    Description = "Test penalty description that meets minimum length requirement"
                }
            }.AsQueryable().BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetByIdAsync(penaltyId))
                .Returns(penaltyEntities);

            var result = await service.GetPenaltyByIdAsync<UpdatePenaltyDto>(penaltyId);

            result.Should().NotBeNull();
            result.Should().BeOfType<UpdatePenaltyDto>();
        }

        [Fact]
        public async Task GetPenaltyByIdAsync_WithMultiplePenaltiesInDatabase_ShouldReturnCorrectOne()
        {
            var targetPenaltyId = 2L;

            var (service, repoManager, _, mapper, _, _) = MockHelper.CreatePenaltyService();

            var penaltyEntity = TestDataBuilder.CreateValidPenaltyEntity();
            penaltyEntity.Id = targetPenaltyId;

            var penaltyEntities = new List<Penalty> { penaltyEntity }.AsQueryable().BuildMock();

            repoManager.Setup(x => x.PenaltyRepository.GetByIdAsync(targetPenaltyId))
                .Returns(penaltyEntities);

            var result = await service.GetPenaltyByIdAsync<GetPenaltyDto>(targetPenaltyId);

            result.Should().NotBeNull();
            result.Id.Should().Be(targetPenaltyId);

            repoManager.Verify(
                x => x.PenaltyRepository.GetByIdAsync(targetPenaltyId),
                Times.Once,
                "Repository should be called exactly once with the correct penalty ID"
            );
        }

        #endregion

        #region AddPenaltyAsync Tests

        [Fact]
        public async Task AddPenaltyAsync_WithValidDto_ShouldCallValidationAndSave()
        {
            var addDto = TestDataBuilder.CreateValidAddPenaltyDto();

            var (service, repoManager, validationService, _, _, _) = MockHelper.CreatePenaltyService();

            repoManager.Setup(x => x.PenaltyRepository.AddAsync(It.IsAny<Penalty>()))
                .Returns(Task.CompletedTask);

            repoManager.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await service.AddPenaltyAsync(addDto);

            validationService.Verify(
                x => x.Validate(It.Is<AddPenaltyDto>(dto => dto == addDto)),
                Times.Once,
                "Validation should be called before adding penalty"
            );

            repoManager.Verify(
                x => x.PenaltyRepository.AddAsync(It.Is<Penalty>(p =>
                    p.UserId == addDto.UserId &&
                    p.Amount == addDto.Amount &&
                    p.Description == addDto.Description
                )),
                Times.Once,
                "Repository AddAsync should be called with mapped penalty"
            );

            repoManager.Verify(
                x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once,
                "SaveChanges should be called to persist data"
            );
        }

        [Fact]
        public async Task AddPenaltyAsync_WithValidDescription_ShouldMapCorrectly()
        {
            var addDto = TestDataBuilder.CreateValidAddPenaltyDto();
            var expectedDescription = "Valid penalty description for overdue book return";
            addDto.Description = expectedDescription;

            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService();

            Penalty? capturedPenalty = null;

            repoManager.Setup(x => x.PenaltyRepository.AddAsync(It.IsAny<Penalty>()))
                .Callback<Penalty>(p => capturedPenalty = p)
                .Returns(Task.CompletedTask);

            repoManager.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await service.AddPenaltyAsync(addDto);

            capturedPenalty.Should().NotBeNull();
            capturedPenalty!.Description.Should().Be(expectedDescription, "Valid description should be mapped as-is");
        }

        [Fact]
        public async Task AddPenaltyAsync_WithInvalidDto_ShouldThrowValidationException()
        {
            var invalidDto = TestDataBuilder.CreateValidAddPenaltyDto();
            invalidDto.Amount = -10; // Invalid amount

            var (service, _, validationService, _, _, _) = MockHelper.CreatePenaltyService();

            validationService.Setup(x => x.Validate(It.IsAny<AddPenaltyDto>()))
                .Throws(new FluentValidation.ValidationException("Amount must be greater than 0"));

            var act = async () => await service.AddPenaltyAsync(invalidDto);

            await act.Should().ThrowAsync<FluentValidation.ValidationException>()
                .WithMessage("*Amount must be greater than 0*");
        }

        [Fact]
        public async Task AddPenaltyAsync_ShouldNotCallSaveChanges_WhenValidationFails()
        {
            var invalidDto = TestDataBuilder.CreateValidAddPenaltyDto();

            var (service, repoManager, validationService, _, _, _) = MockHelper.CreatePenaltyService();

            validationService.Setup(x => x.Validate(It.IsAny<AddPenaltyDto>()))
                .Throws(new FluentValidation.ValidationException(""));

            try
            {
                await service.AddPenaltyAsync(invalidDto);
            }
            catch (FluentValidation.ValidationException)
            {
                // Expected exception
            }

            repoManager.Verify(
                x => x.PenaltyRepository.AddAsync(It.IsAny<Penalty>()),
                Times.Never,
                "AddAsync should not be called when validation fails"
            );

            repoManager.Verify(
                x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never,
                "SaveChanges should not be called when validation fails"
            );
        }

        [Fact]
        public async Task AddPenaltyAsync_WithAllOptionalFieldsNull_ShouldMapWithDefaults()
        {
            var addDto = TestDataBuilder.CreateValidAddPenaltyDto();
            addDto.TransectionId = null;
            addDto.Description = null;
            addDto.Amount = null;
            addDto.OverDueDays = null;

            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService();

            Penalty? capturedPenalty = null;

            repoManager.Setup(x => x.PenaltyRepository.AddAsync(It.IsAny<Penalty>()))
                .Callback<Penalty>(p => capturedPenalty = p)
                .Returns(Task.CompletedTask);

            repoManager.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await service.AddPenaltyAsync(addDto);

            capturedPenalty.Should().NotBeNull();
            capturedPenalty!.Description.Should().Be("Other reasons", "Null description should map to default value");
            capturedPenalty.Amount.Should().Be(0, "Null amount should map to zero");
            capturedPenalty.TransectionId.Should().BeNull();
            capturedPenalty.OverDueDays.Should().BeNull();
        }

        [Fact]
        public async Task AddPenaltyAsync_ShouldMapAllRequiredFields()
        {
            var addDto = TestDataBuilder.CreateValidAddPenaltyDto();

            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService();

            Penalty? capturedPenalty = null;

            repoManager.Setup(x => x.PenaltyRepository.AddAsync(It.IsAny<Penalty>()))
                .Callback<Penalty>(p => capturedPenalty = p)
                .Returns(Task.CompletedTask);

            repoManager.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await service.AddPenaltyAsync(addDto);

            capturedPenalty.Should().NotBeNull();
            capturedPenalty!.UserId.Should().Be(addDto.UserId);
            capturedPenalty.TransectionId.Should().Be(addDto.TransectionId);
            capturedPenalty.StatusId.Should().Be(addDto.StatusId);
            capturedPenalty.PenaltyTypeId.Should().Be(addDto.PenaltyTypeId);
            capturedPenalty.Description.Should().Be(addDto.Description);
            capturedPenalty.Amount.Should().Be(addDto.Amount!.Value);
            capturedPenalty.OverDueDays.Should().Be(addDto.OverDueDays);
        }

        #endregion
    }
}
