using FluentAssertions;
using LMS.Core.Enums;
using LMS.Tests.Integration.Base;

namespace LMS.Tests.Integration.Services;

/// <summary>
/// Integration tests for CalculatePenaltyForHoldingBooks method.
/// These tests use an LocalDB database to test the complete penalty calculation flow
/// including complex LINQ queries and entity relationships.
/// 
/// NOTE: These tests are more comprehensive than unit tests but slower to run.
/// They validate the entire penalty calculation workflow end-to-end.
/// </summary>
public class PenaltyCalculationIntegrationTests : IntegrationTestBase
{
    #region Overdue Transaction Penalty Tests

    [Fact]
    public async Task CalculatePenaltyForHoldingBooks_WithOverdueTransaction_ShouldCreatePenalty()
    {
        var user = TestDataBuilder.CreateValidUserEntity();
        user.RoleId = (long)RoleListEnum.Admin;

        var book = TestDataBuilder.CreateBook();
        book.GenreId = 1;

        var transection = TestDataBuilder.CreateOverdueTransaction(daysOverdue: 10);
        transection.User = user;
        transection.Book = book;

        var userMembership = TestDataBuilder.CreateActiveUserMembership(borrowLimit: 3);
        userMembership.User = user;

        await _context.AddRangeAsync(user, book, transection, userMembership);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: user.Id.ToString(), role: "Admin");
        var service = CreatePenaltyServiceFromMockHelper(httpContextAccessor: httpContextAccessor);

        await service.CalculatePenaltyForHoldingBooks(forLogin: false);

        var penalties = await _context.Penalty
            .Where(p => p.UserId == user.Id)
            .ToListAsync();

        var latestTransection = await _context.Transection
            .Where(t => t.Id == transection.Id)
            .FirstOrDefaultAsync();

        penalties.Should().HaveCount(1);
        penalties.First().PenaltyTypeId.Should().Be((long)PenaltyTypeEnum.LateReturnRenew);
        penalties.First().Amount.Should().BeGreaterThan(0);
        penalties.First().OverDueDays.Should().Be(10);
        penalties.First().TransectionId.Should().Be(transection.Id);

        latestTransection?.StatusId.Should().Be((long)TransectionStatusEnum.Overdue);
    }

    [Fact]
    public async Task CalculatePenaltyForHoldingBooks_WithExistingPenalty_ShouldUpdateIncrementally()
    {
        // Arrange - Transaction has existing penalty for 5 days, now 10 days overdue
        var user = TestDataBuilder.CreateValidUserEntity();
        user.RoleId = (long)RoleListEnum.Admin;

        var book = TestDataBuilder.CreateBook();
        book.GenreId = 1;

        var transection = TestDataBuilder.CreateOverdueTransaction(daysOverdue: 10);
        transection.User = user;
        transection.Book = book;

        var userMembership = TestDataBuilder.CreateActiveUserMembership(borrowLimit: 3);
        userMembership.User = user;

        var existingPenalty = TestDataBuilder.CreateValidPenaltyEntityWithoutNavigation(userId: 0, transectionId: 0, overDueDays: 5);
        existingPenalty.User = user;
        existingPenalty.Transection = transection;
        existingPenalty.Amount = 25m; // 5 days × 5 Rs

        await _context.AddRangeAsync(user, book, transection, existingPenalty, userMembership);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        var initialAmount = existingPenalty.Amount;

        var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: user.Id.ToString(), role: "Admin");
        var service = CreatePenaltyServiceFromMockHelper(httpContextAccessor: httpContextAccessor);

        await service.CalculatePenaltyForHoldingBooks(forLogin: false);

        var updatedPenalty = await _context.Penalty
            .FirstAsync(p => p.Id == existingPenalty.Id);

        updatedPenalty.OverDueDays.Should().Be(10);
        updatedPenalty.Amount.Should().BeGreaterThan(initialAmount);
        // New amount should be initial + incremental (days 6-10)
        // Days 6-10 in interval 1: 10 Rs/day × 4 + 15 Rs/day × 1 = 55 Rs
        updatedPenalty.Amount.Should().Be(initialAmount + 55m);
    }

    [Fact]
    public async Task CalculatePenaltyForHoldingBooks_WithMultipleOverdueTransactions_ShouldCreateMultiplePenalties()
    {
        var user = TestDataBuilder.CreateValidUserEntity();
        user.RoleId = (long)RoleListEnum.Admin;

        var book1 = TestDataBuilder.CreateBook();
        var book2 = TestDataBuilder.CreateBook();
        book1.GenreId = book2.GenreId = 1;

        var transection1 = TestDataBuilder.CreateOverdueTransaction(daysOverdue: 5);
        transection1.User = user;
        transection1.Book = book1;

        var transection2 = TestDataBuilder.CreateOverdueTransaction(daysOverdue: 10);
        transection2.User = user;
        transection2.Book = book2;

        var userMembership = TestDataBuilder.CreateActiveUserMembership(borrowLimit: 3);
        userMembership.User = user;

        await _context.AddRangeAsync(user, book1, book2, transection1, transection2, userMembership);
        await _context.SaveChangesAsync();

        var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: user.Id.ToString(), role: "Admin");
        var service = CreatePenaltyServiceFromMockHelper(httpContextAccessor: httpContextAccessor);

        await service.CalculatePenaltyForHoldingBooks(forLogin: false);

        var penalties = await _context.Penalty
            .Where(p => p.UserId == user.Id)
            .ToListAsync();

        penalties.Should().HaveCount(2);
        penalties.Should().AllSatisfy(p => p.PenaltyTypeId.Should().Be((long)PenaltyTypeEnum.LateReturnRenew));
    }

    [Fact]
    public async Task CalculatePenaltyForHoldingBooks_WithNonOverdueTransaction_ShouldNotCreatePenalty()
    {
        var user = TestDataBuilder.CreateValidUserEntity();
        user.RoleId = (long)RoleListEnum.Admin;

        var book = TestDataBuilder.CreateBook();
        book.GenreId = 1;

        var transection = TestDataBuilder.CreateActiveTransaction();
        transection.User = user;
        transection.Book = book;

        var userMembership = TestDataBuilder.CreateActiveUserMembership(borrowLimit: 3);
        userMembership.User = user;

        await _context.AddRangeAsync(user, book, transection, userMembership);
        await _context.SaveChangesAsync();

        var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: user.Id.ToString(), role: "Admin");
        var service = CreatePenaltyServiceFromMockHelper(httpContextAccessor: httpContextAccessor);

        await service.CalculatePenaltyForHoldingBooks(forLogin: false);

        var penalties = await _context.Penalty.ToListAsync();
        penalties.Should().BeEmpty();
    }

    [Fact]
    public async Task CalculatePenaltyForHoldingBooks_WithReturnedTransaction_ShouldNotCreatePenalty()
    {
        var user = TestDataBuilder.CreateValidUserEntity();
        user.RoleId = (long)RoleListEnum.Admin;

        var book = TestDataBuilder.CreateBook();
        book.GenreId = 1;

        var transection = TestDataBuilder.CreateReturnedTransaction();
        transection.User = user;
        transection.Book = book;

        var userMembership = TestDataBuilder.CreateActiveUserMembership(borrowLimit: 3);
        userMembership.User = user;

        await _context.AddRangeAsync(user, book, transection, userMembership);
        await _context.SaveChangesAsync();

        var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: user.Id.ToString(), role: "Admin");
        var service = CreatePenaltyServiceFromMockHelper(httpContextAccessor: httpContextAccessor);

        await service.CalculatePenaltyForHoldingBooks(forLogin: false);

        var penalties = await _context.Penalty.ToListAsync();
        penalties.Should().BeEmpty();
    }

    #endregion

    #region Extra Holdings Penalty Tests

    [Fact]
    public async Task CalculatePenaltyForHoldingBooks_UserWithActiveMembershipOverHolding_ShouldCreatePenalty()
    {
        // Arrange - User has 5 books but membership allows only 3
        var user = TestDataBuilder.CreateValidUserEntity();
        user.RoleId = (long)RoleListEnum.Admin;

        var userMembership = TestDataBuilder.CreateActiveUserMembership(borrowLimit: 3);
        userMembership.User = user;
        userMembership.EffectiveStartDate = DateTimeOffset.UtcNow.AddDays(-20); // > 7 day carryover

        var books = Enumerable.Range(1, 5).Select(i =>
        {
            var book = TestDataBuilder.CreateBook();
            book.GenreId = 1;
            return book;
        })
        .ToList();

        var transactions = books.Select(b =>
        {
            var t = TestDataBuilder.CreateActiveTransaction();
            t.User = user;
            t.Book = b;
            return t;
        }).ToList();

        await _context.AddRangeAsync(user, userMembership);
        await _context.Books.AddRangeAsync(books);
        await _context.Transection.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();

        var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: user.Id.ToString(), role: "Admin");
        var service = CreatePenaltyServiceFromMockHelper(httpContextAccessor: httpContextAccessor);

        await service.CalculatePenaltyForHoldingBooks(forLogin: false);

        var penalties = await _context.Penalty
            .Where(p => p.UserId == user.Id)
            .ToListAsync();

        penalties.Should().HaveCount(1);
        penalties.First().PenaltyTypeId.Should().Be((long)PenaltyTypeEnum.ExtraHoldings);
        penalties.First().TransectionId.Should().BeNull(); // User-level penalty
        penalties.First().Amount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CalculatePenaltyForHoldingBooks_UserWithinCarryoverPeriod_ShouldNotCreatePenalty()
    {
        // Arrange - User has extra books but within 7-day carryover period
        var user = TestDataBuilder.CreateValidUserEntity();
        user.RoleId = (long)RoleListEnum.Admin;

        var userMembership = TestDataBuilder.CreateActiveUserMembership(borrowLimit: 3);
        userMembership.User = user;
        userMembership.EffectiveStartDate = DateTimeOffset.UtcNow.AddDays(-3); // Only 3 days < 7        

        var books = Enumerable.Range(1, 5).Select(i =>
        {
            var book = TestDataBuilder.CreateBook();
            book.GenreId = 1;
            return book;
        })
        .ToList();

        var transactions = books.Select(b =>
        {
            var t = TestDataBuilder.CreateActiveTransaction();
            t.User = user;
            t.Book = b;
            return t;
        }).ToList();

        await _context.AddRangeAsync(user, userMembership);
        await _context.Books.AddRangeAsync(books);
        await _context.Transection.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();

        var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: user.Id.ToString(), role: "Admin");
        var service = CreatePenaltyServiceFromMockHelper(httpContextAccessor: httpContextAccessor);

        await service.CalculatePenaltyForHoldingBooks(forLogin: false);

        var penalties = await _context.Penalty.ToListAsync();
        penalties.Should().BeEmpty();
    }

    #endregion

    #region Expired Membership Penalty Tests

    [Fact]
    public async Task CalculatePenaltyForHoldingBooks_UserWithExpiredMembership_ShouldCreatePenalty()
    {
        // Arrange - User's membership expired but still holding books
        var user = TestDataBuilder.CreateValidUserEntity();
        user.RoleId = (long)RoleListEnum.Admin;

        var expiredUserMembership = TestDataBuilder.CreateExpiredUserMembership(daysExpired: 10);  // > 5 day buffer
        expiredUserMembership.User = user;

        var book = TestDataBuilder.CreateBook();
        book.GenreId = 1;

        var transection = TestDataBuilder.CreateActiveTransaction();
        transection.User = user;
        transection.Book = book;

        await _context.AddRangeAsync(user, book, transection, expiredUserMembership);
        await _context.SaveChangesAsync();

        var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: user.Id.ToString(), role: "Admin");
        var service = CreatePenaltyServiceFromMockHelper(httpContextAccessor: httpContextAccessor);

        await service.CalculatePenaltyForHoldingBooks(forLogin: false);

        var penalties = await _context.Penalty
            .Where(p => p.UserId == user.Id)
            .ToListAsync();

        penalties.Should().HaveCount(1);
        penalties.First().PenaltyTypeId.Should().Be(
            (long)PenaltyTypeEnum.BooksHeldUnderExpiredMembership
        );
    }

    [Fact]
    public async Task CalculatePenaltyForHoldingBooks_UserWithinBufferPeriod_ShouldNotCreatePenalty()
    {
        // Arrange - Membership expired but within 5-day buffer
        var user = TestDataBuilder.CreateValidUserEntity();
        user.RoleId = (long)RoleListEnum.Admin;

        var expiredUserMembership = TestDataBuilder.CreateExpiredUserMembership(daysExpired: 3);  // < 5 day buffer
        expiredUserMembership.User = user;

        var book = TestDataBuilder.CreateBook();
        book.GenreId = 1;

        var transection = TestDataBuilder.CreateActiveTransaction();
        transection.User = user;
        transection.Book = book;

        await _context.AddRangeAsync(user, book, transection, expiredUserMembership);
        await _context.SaveChangesAsync();

        var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: user.Id.ToString(), role: "Admin");
        var service = CreatePenaltyServiceFromMockHelper(httpContextAccessor: httpContextAccessor);

        await service.CalculatePenaltyForHoldingBooks(forLogin: false);

        var penalties = await _context.Penalty.ToListAsync();
        penalties.Should().BeEmpty();
    }

    #endregion

    #region ForLogin Parameter Tests

    [Fact]
    public async Task CalculatePenaltyForHoldingBooks_WithForLoginTrue_ShouldOnlyCalculateForAuthenticatedUser()
    {
        // Arrange - Multiple users with overdue transactions
        var user1 = TestDataBuilder.CreateValidUserEntity();
        user1.RoleId = (long)RoleListEnum.User;

        var user2 = TestDataBuilder.CreateValidUserEntity();
        user2.RoleId = (long)RoleListEnum.User;
        user2.Email = "john.doe@example1.com";

        var book1 = TestDataBuilder.CreateBook();
        var book2 = TestDataBuilder.CreateBook();
        book1.GenreId = book2.GenreId = 1;

        var transaction1 = TestDataBuilder.CreateOverdueTransaction(daysOverdue: 5);
        transaction1.User = user1;
        transaction1.Book = book1;

        var transaction2 = TestDataBuilder.CreateOverdueTransaction(daysOverdue: 5);
        transaction2.User = user2;
        transaction2.Book = book2;

        var userMembership1 = TestDataBuilder.CreateActiveUserMembership(borrowLimit: 3);
        userMembership1.User = user1;

        var userMembership2 = TestDataBuilder.CreateActiveUserMembership(borrowLimit: 3);
        userMembership2.User = user2;

        await _context.AddRangeAsync(user1, user2, book1, book2, transaction1, transaction2, userMembership1, userMembership2);
        await _context.SaveChangesAsync();

        var httpContextAccessor = MockHelper.CreateHttpContextAccessor(userId: user1.Id.ToString(), role: "User");
        var service = CreatePenaltyServiceFromMockHelper(httpContextAccessor: httpContextAccessor);

        await service.CalculatePenaltyForHoldingBooks(forLogin: true);

        var penalties = await _context.Penalty.ToListAsync();
        penalties.Should().HaveCount(1);
        penalties.First().UserId.Should().Be(user1.Id);
    }

    #endregion
}
