using LMS.Application.Contracts.DTOs.Penalty;
using LMS.Core.Entities;
using LMS.Core.Enums;

namespace LMS.Tests.Helpers
{
    /// <summary>
    /// Builder class for creating test data objects with valid default values
    /// </summary>
    public class TestDataBuilder
    {
        #region Penalty Test Data

        #region GetPenaltyDto

        /// <summary>
        /// Creates a valid GetPenaltyDto with all required fields populated
        /// </summary>
        public static GetPenaltyDto CreateValidGetPenaltyDto()
        {
            return new GetPenaltyDto
            {
                Id = TestConstants.ValidPenaltyId,
                UserName = TestConstants.ValidUserName,
                UserProfilePhoto = "/images/profile/default.jpg",
                TransectionId = TestConstants.ValidTransactionId,
                TransectionDueDate = DateTimeOffset.UtcNow.AddDays(-5), // Past date (valid)
                StatusLabel = TestConstants.ValidStatusLabel,
                StatusLabelColor = TestConstants.ValidStatusLabelColor,
                StatusLabelBgColor = TestConstants.ValidStatusLabelBgColor,
                PenaltyTypeName = TestConstants.ValidPenaltyTypeName,
                PenaltyTypeInfo = TestConstants.ValidPenaltyTypeInfo,
                Description = TestConstants.ValidDescription,
                Amount = TestConstants.ValidAmount,
                OverDueDays = TestConstants.ValidOverDueDays,
                IsRemoved = false
            };
        }

        #endregion

        #region AddPenaltyDto

        /// <summary>
        /// Creates a valid AddPenaltyDto with all required fields populated
        /// </summary>
        public static AddPenaltyDto CreateValidAddPenaltyDto()
        {
            return new AddPenaltyDto
            {
                UserId = TestConstants.ValidUserId,
                TransectionId = TestConstants.ValidTransactionId,
                StatusId = TestConstants.ValidStatusId,
                PenaltyTypeId = TestConstants.ValidPenaltyTypeId,
                Description = TestConstants.ValidShortDescription,
                Amount = TestConstants.ValidAmount,
                OverDueDays = TestConstants.ValidOverDueDays
            };
        }

        #endregion

        #region UpdatePenaltyDto

        /// <summary>
        /// Creates a valid UpdatePenaltyDto with all required fields populated
        /// </summary>
        public static UpdatePenaltyDto CreateValidUpdatePenaltyDto()
        {
            return new UpdatePenaltyDto
            {
                Id = TestConstants.ValidPenaltyId,
                UserId = TestConstants.ValidUserId,
                TransectionId = TestConstants.ValidTransactionId,
                StatusId = TestConstants.ValidStatusId,
                PenaltyTypeId = TestConstants.ValidPenaltyTypeId,
                TransectionStatusId = TestConstants.ValidTransactionStatusId,
                Description = TestConstants.ValidShortDescription,
                Amount = TestConstants.ValidAmount,
                OverDueDays = TestConstants.ValidOverDueDays
            };
        }

        #endregion

        #region Penalty Entity

        public static Penalty CreateValidPenaltyEntity()
        {
            return new Penalty
            {
                Id = TestConstants.ValidPenaltyId,
                UserId = TestConstants.ValidUserId,
                TransectionId = TestConstants.ValidTransactionId,
                StatusId = TestConstants.ValidStatusId,
                PenaltyTypeId = TestConstants.ValidPenaltyTypeId,
                Description = TestConstants.ValidDescription,
                Amount = TestConstants.ValidAmount,
                OverDueDays = TestConstants.ValidOverDueDays,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-5),

                // Navigation properties
                User = CreateValidUserEntity(id: TestConstants.ValidUserId),
                Transection = CreateValidTransectionEntity(),
                Status = CreateValidStatusEntity(),
                PenaltyType = CreateValidPenaltyTypeEntity()
            };
        }

        public static Penalty CreateValidPenaltyEntityWithoutNavigation(long? userId = null, long? transectionId = null, int? overDueDays = null, long id = 0)
        {
            return new Penalty
            {
                Id = id,
                UserId = userId ?? TestConstants.ValidUserId,
                TransectionId = transectionId ?? TestConstants.ValidTransactionId,
                StatusId = TestConstants.ValidStatusId,
                PenaltyTypeId = TestConstants.ValidPenaltyTypeId,
                Description = TestConstants.ValidDescription,
                Amount = TestConstants.ValidAmount,
                OverDueDays = overDueDays ?? TestConstants.ValidOverDueDays,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-5)
            };
        }

        /// <summary>
        /// Creates a Penalty entity with custom Id
        /// </summary>
        public static Penalty CreatePenaltyEntityWithId(long id)
        {
            var penalty = CreateValidPenaltyEntity();
            penalty.Id = id;
            return penalty;
        }

        /// <summary>
        /// Creates a Penalty entity with custom Id
        /// </summary>
        public static Penalty CreatePenaltyEntityWithoutNavigationWithId(long id)
        {
            var penalty = CreateValidPenaltyEntityWithoutNavigation();
            penalty.Id = id;
            return penalty;
        }

        /// <summary>
        /// Creates a removed/inactive Penalty entity
        /// </summary>
        public static Penalty CreateRemovedPenaltyEntity()
        {
            var penalty = CreateValidPenaltyEntity();
            penalty.IsActive = false;
            return penalty;
        }

        #endregion

        #endregion

        #region User Test Data

        #region User Entity

        public static User CreateValidUserEntity(long id = 0)
        {
            return new User
            {
                Id = id,
                FirstName = "John",
                MiddleName = "Michael",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Address = "test address",
                Gender = "Male",
                MobileNo = "6598784587",
                Username = "JMichael",
                PasswordHash = Array.Empty<byte>(),
                PasswordSolt = Array.Empty<byte>(),
                ProfilePhoto = "/uploads/profiles/user100.jpg"
            };
        }

        public static User CreateUserWithoutOptionalFields()
        {
            return new User
            {
                Id = TestConstants.ValidUserId,
                FirstName = "Jane",
                MiddleName = null,
                LastName = "Smith",
                Email = "jane.smith@example.com",
                ProfilePhoto = null,
                RoleId = 3 // Role - User

            };
        }

        public static User CreateUserWithoutOptionalFieldsWithId(long id)
        {
            var user = CreateUserWithoutOptionalFields();
            user.Id = id;

            return user;
        }

        #endregion

        #endregion

        #region Transection Test Data

        #region Transection Entity

        public static Transection CreateValidTransectionEntity()
        {
            return new Transection
            {
                Id = TestConstants.ValidTransactionId,
                UserId = TestConstants.ValidUserId,
                BookId = 1,
                BorrowDate = DateTimeOffset.UtcNow.AddDays(-20),
                DueDate = DateTimeOffset.UtcNow.AddDays(-5), // Overdue
                ReturnDate = null
            };
        }

        /// <summary>
        /// Creates a valid overdue transection for penalty calculation tests
        /// </summary>
        public static Transection CreateOverdueTransaction(long userId = 0, int daysOverdue = 5, long id = 0)
        {
            return new Transection
            {
                Id = id,
                UserId = userId,
                BookId = 1,
                BorrowDate = DateTimeOffset.UtcNow.AddDays(-(15 + daysOverdue)),
                DueDate = DateTimeOffset.UtcNow.AddDays(-daysOverdue),
                ReturnDate = null,
                StatusId = (long)TransectionStatusEnum.Borrowed,
                IsActive = true
            };
        }

        /// <summary>
        /// Creates a transection that's not overdue yet
        /// </summary>
        public static Transection CreateActiveTransaction(long userId = 0, long id = 0)
        {
            return new Transection
            {
                Id = id,
                UserId = userId,
                BookId = 2,
                BorrowDate = DateTimeOffset.UtcNow.AddDays(-5),
                DueDate = DateTimeOffset.UtcNow.AddDays(10), // Due in future
                ReturnDate = null,
                StatusId = (long)TransectionStatusEnum.Borrowed,
                IsActive = true
            };
        }

        /// <summary>
        /// Creates a returned transection (finalized)
        /// </summary>
        public static Transection CreateReturnedTransaction(long userId = 0, long bookId = 0, long id = 0)
        {
            return new Transection
            {
                Id = id,
                UserId = userId,
                BookId = bookId,
                BorrowDate = DateTimeOffset.UtcNow.AddDays(-20),
                DueDate = DateTimeOffset.UtcNow.AddDays(-10),
                ReturnDate = DateTimeOffset.UtcNow.AddDays(-8),
                StatusId = (long)TransectionStatusEnum.Returned,
                IsActive = true
            };
        }

        #endregion

        #endregion

        #region PenaltyType Test Data

        #region PenaltyType Entity

        public static PenaltyType CreateValidPenaltyTypeEntity()
        {
            return new PenaltyType
            {
                Id = TestConstants.ValidPenaltyTypeId,
                Label = TestConstants.ValidPenaltyTypeName,
                Description = TestConstants.ValidPenaltyTypeInfo,
                IsActive = true
            };
        }

        public static PenaltyType CreateValidPenaltyTypeEntityWithId(long id)
        {
            var penaltyType = CreateValidPenaltyTypeEntity();
            penaltyType.Id = id;

            return penaltyType;
        }

        public static List<PenaltyType> CreatePenaltyTypeList()
        {
            return new List<PenaltyType>
            {
                new PenaltyType { Id = (long)PenaltyTypeEnum.LateReturnRenew, Label = "LateReturnRenew", Description = "Late Return Renew" },
                new PenaltyType { Id = (long)PenaltyTypeEnum.BookDamage, Label = "BookDamage", Description = "Book Damage" },
                new PenaltyType { Id = (long)PenaltyTypeEnum.LostBook, Label = "LostBook", Description = "Lost Book" },
                new PenaltyType { Id = (long)PenaltyTypeEnum.ExtraHoldings, Label = "ExtraHoldings", Description = "Extra Holdings" },
                new PenaltyType { Id = (long)PenaltyTypeEnum.BooksHeldUnderExpiredMembership, Label = "BooksHeldUnderExpiredMembership", Description = "Books Held Under Expired Membership" },
                new PenaltyType { Id = (long)PenaltyTypeEnum.Other, Label = "Other", Description = "Other" },
            };
        }

        #endregion

        #endregion

        #region Status Test Data

        #region Status Entity

        public static Status CreateValidStatusEntity()
        {
            return new Status
            {
                Id = TestConstants.ValidStatusId,
                Label = TestConstants.ValidStatusLabel,
                Color = TestConstants.ValidStatusLabelColor,
                StatusTypeId = TestConstants.ValidStatusTypeId,
                StatusType = CreateValidStatusTypeEntity()
            };
        }

        public static List<Status> CreateStatusList()
        {
            return new List<Status>
            {
                new Status { Id = (long)BookStatusEnum.Available, Label = "Available", Description = "Available", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 1 },
                new Status { Id = (long)BookStatusEnum.CheckedOut, Label = "CheckedOut", Description = "CheckedOut", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 1 },
                new Status { Id = (long)BookStatusEnum.Reserved, Label = "Reserved", Description = "Reserved", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 1 },
                new Status { Id = (long)BookStatusEnum.Lost_Damaged, Label = "Lost_Damaged", Description = "Lost/Damaged", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 1 },
                new Status { Id = (long)TransectionStatusEnum.Returned, Label = "Returned", Description = "Returned", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 2 },
                new Status { Id = (long)TransectionStatusEnum.Overdue, Label = "Overdue", Description = "Overdue", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 2 },
                new Status { Id = (long)TransectionStatusEnum.Renewed, Label = "Renewed", Description = "Renewed", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 2 },
                new Status { Id = (long)TransectionStatusEnum.Cancelled, Label = "Cancelled", Description = "Cancelled", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 2 },
                new Status { Id = (long)ReservationsStatusEnum.Fulfilled, Label = "Fulfilled", Description = "Fulfilled", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 4 },
                new Status { Id = (long)FineStatusEnum.Paid, Label = "Paid", Description = "Paid", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 3 },
                new Status { Id = (long)FineStatusEnum.UnPaid, Label = "UnPaid", Description = "UnPaid", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 3 },
                new Status { Id = (long)BookStatusEnum.OnHold, Label = "OnHold", Description = "OnHold", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 1 },
                new Status { Id = (long)TransectionStatusEnum.Borrowed, Label = "Borrowed", Description = "Borrowed", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 2 },
                new Status { Id = (long)ReservationsStatusEnum.Reserved, Label = "Reserved", Description = "Reserved", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 4 },
                new Status { Id = (long)ReservationsStatusEnum.Cancelled, Label = "Cancelled", Description = "Cancelled", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 4 },
                new Status { Id = (long)BookStatusEnum.Removed, Label = "Removed", Description = "Removed", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 1 },
                new Status { Id = (long)ReservationsStatusEnum.Allocated, Label = "Allocated", Description = "Allocated", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 4 },
                new Status { Id = (long)FineStatusEnum.Waived, Label = "Waived", Description = "Waived", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 3 },
                new Status { Id = (long)TransectionStatusEnum.ClaimedLost, Label = "ClaimedLost", Description = "ClaimedLost", Color = TestConstants.ValidStatusLabelColor, StatusTypeId = 2 },
            };
        }

        #endregion

        #endregion

        #region StatusType Test Data

        #region StatusType Entity

        public static StatusType CreateValidStatusTypeEntity()
        {
            return new StatusType
            {
                Id = TestConstants.ValidStatusTypeId,
                Description = "Penalty"
            };
        }

        public static List<StatusType> CreateStatusTypeList()
        {
            return new List<StatusType>
            {
                new StatusType { Id = 1, Label = "Books", Description = "Books" },
                new StatusType { Id = 2, Label = "Transection", Description = "Transection" },
                new StatusType { Id = 3, Label = "Fine", Description = "Penalty" },
                new StatusType { Id = 4, Label = "Reservation", Description = "Reservation" },
            };
        }

        #endregion

        #endregion

        #region Membership Test Data

        #region Membership Entity

        /// <summary>
        /// Creates an active membership
        /// </summary>
        public static Membership CreateActiveMembership(int id = 0, int borrowLimit = 3)
        {
            return new Membership
            {
                Id = id,
                Type = "Test membership",
                Description = "Test membership",
                BorrowLimit = borrowLimit,
                ReservationLimit = 2,
                Duration = 15,
                Cost = 1000,
                Discount = 0
            };
        }

        #endregion

        #endregion

        #region User Membership Test Data

        #region User Membership Entity

        /// <summary>
        /// Creates an active user membership
        /// </summary>
        public static UserMembershipMapping CreateActiveUserMembership(long userId = 0, int borrowLimit = 3, long id = 0)
        {
            return new UserMembershipMapping
            {
                Id = id,
                UserId = userId,
                MembershipId = 1,
                EffectiveStartDate = DateTimeOffset.UtcNow.AddDays(-30),
                ExpirationDate = DateTimeOffset.UtcNow.AddDays(30),
                BorrowLimit = borrowLimit,
                ReservationLimit = 2,
                IsActive = true
            };
        }

        /// <summary>
        /// Creates an expired user membership
        /// </summary>
        public static UserMembershipMapping CreateExpiredUserMembership(long userId = 0, int daysExpired = 10, long id = 0)
        {
            return new UserMembershipMapping
            {
                Id = id,
                UserId = userId,
                MembershipId = 1,
                EffectiveStartDate = DateTimeOffset.UtcNow.AddDays(-90),
                ExpirationDate = DateTimeOffset.UtcNow.AddDays(-daysExpired),
                BorrowLimit = 5,
                ReservationLimit = 2,
                IsActive = false
            };
        }

        #endregion

        #endregion

        #region User Test Data

        #region User Entity

        public static Books CreateBook(long id = 0)
        {
            return new Books
            {
                Id = id,
                Title = $"Book {id}",
                Isbn = "ISBN-" + Guid.NewGuid().ToString("N").Substring(0, 13),
                Author = "John Michael",
                AuthorDescription = "Lorem ipsum is placeholder text used in design to show how a layout will look before the real content is added.",
                BookDescription = "Lorem ipsum is placeholder text used in design to show how a layout will look before the real content is added.",
                Publisher = "John Michael",
                TotalCopies = 5,
                AvailableCopies = 3,
                StatusId = (long)BookStatusEnum.Available,
                IsActive = true
            };
        }

        #endregion

        #endregion

        #region Config Test Data

        #region Config Entity

        /// <summary>
        /// Creates standard penalty configuration
        /// </summary>
        public static List<Configs> CreatePenaltyConfig()
        {
            return new List<Configs>
            {
                new Configs { Id = 1, KeyName = "BasePenaltyPerDay", KeyValue = "5", Description = "test" },
                new Configs { Id = 2, KeyName = "PenaltyIncreaseType", KeyValue = "ADD", Description = "test" },
                new Configs { Id = 3, KeyName = "PenaltyIncreaseValue", KeyValue = "5", Description = "test" },
                new Configs { Id = 4, KeyName = "PenaltyIncreaseDurationInDays", KeyValue = "5", Description = "test" },
                new Configs { Id = 5, KeyName = "PreviousLimitCarryoverDays", KeyValue = "7", Description = "test" },
                new Configs { Id = 6, KeyName = "MembershipExpiryBufferDays", KeyValue = "5", Description = "test" }
            };
        }

        #endregion

        #endregion

        #region RoleList Test Data

        #region RoleList Entity

        public static List<RoleList> CreateRoleList()
        {
            return new List<RoleList>
            {
                new RoleList { Id = (long)RoleListEnum.Admin, Label = "Admin", Description = "admin" },
                new RoleList { Id = (long)RoleListEnum.Librarian, Label = "Librarian", Description = "librarian" },
                new RoleList { Id = (long)RoleListEnum.User, Label = "User", Description = "user" },
            };
        }

        #endregion

        #endregion

        #region Genre Test Data

        #region Genre Entity

        public static List<Genre> CreateGenreList()
        {
            return new List<Genre>
            {
                new Genre { Id = 1, Name = "Type1", Description = "Type1" },
                new Genre { Id = 2, Name = "Type2", Description = "Type2" },
                new Genre { Id = 3, Name = "Type3", Description = "Type3" }
            };
        }

        #endregion

        #endregion
    }
}