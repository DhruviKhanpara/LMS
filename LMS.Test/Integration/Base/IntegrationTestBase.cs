using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Application.Services.Services;
using LMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace LMS.Tests.Integration.Base;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected TestLibraryManagementSysContext _context;
    protected string _databaseName;

    public async Task InitializeAsync()
    {
        // Create unique database name for each test run
        _databaseName = $"LMS_Test_{Guid.NewGuid()}";

        // LocalDB connection string
        var connectionString = $@"Server=(localdb)\MSSQLLocalDB;Database={_databaseName};Trusted_Connection=true;MultipleActiveResultSets=true";

        var options = new DbContextOptionsBuilder<LibraryManagementSysContext>()
            .UseSqlServer(connectionString)
            .EnableSensitiveDataLogging() // Helpful for debugging tests
            .Options;

        _context = new TestLibraryManagementSysContext(options);
        await _context.Database.EnsureCreatedAsync();

        // Create database and schema
        _context.Database.EnsureCreated();

        // Seed test data here
        await SeedDependancyDataFromBuilder();
        _context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        // Delete test database
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    #region Helper Methods

    /// <summary>
    /// Seeds status, role info using TestDataBuilder
    /// </summary>
    protected virtual async Task SeedDependancyDataFromBuilder()
    {
        var statusTypes = TestDataBuilder.CreateStatusTypeList();

        var status = TestDataBuilder.CreateStatusList();
        var roles = TestDataBuilder.CreateRoleList();
        var genres = TestDataBuilder.CreateGenreList();
        var configs = TestDataBuilder.CreatePenaltyConfig();
        var memberships = Enumerable.Range(1, 2).Select(i => TestDataBuilder.CreateActiveMembership(id: i));
        var penaltyType = TestDataBuilder.CreatePenaltyTypeList();

        await _context.StatusType.AddRangeAsync(statusTypes);
        await _context.Status.AddRangeAsync(status);
        await _context.RoleList.AddRangeAsync(roles);
        await _context.Genre.AddRangeAsync(genres);
        await _context.Configs.AddRangeAsync(configs);
        await _context.Membership.AddRangeAsync(memberships);
        await _context.PenaltyType.AddRangeAsync(penaltyType);

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates PenaltyService using MockHelper for consistency
    /// </summary>
    protected PenaltyService CreatePenaltyServiceFromMockHelper(Mock<IHttpContextAccessor>? httpContextAccessor = null)
    {
        var httpContext = httpContextAccessor ?? MockHelper.CreateHttpContextAccessor();

        var connectionString = $@"Server=(localdb)\MSSQLLocalDB;Database={_databaseName};Trusted_Connection=true;MultipleActiveResultSets=true";
        var options = new DbContextOptionsBuilder<LibraryManagementSysContext>()
            .UseSqlServer(connectionString)
            .EnableSensitiveDataLogging()
            .Options;

        var serviceContext = new TestLibraryManagementSysContext(options);

        var assembly = Assembly.Load("LMS.Infrastructure");
        var repositoryManagerType = assembly.GetType("LMS.Infrastructure.Repositories.RepositoryManager", throwOnError: true);

        var repositoryManager = (IRepositoryManager)Activator.CreateInstance(
            repositoryManagerType!,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            new object[] { serviceContext, httpContext.Object },
            culture: null
        )!;

        var mapper = MockHelper.CreateRealMapper();
        var validationService = MockHelper.CreateValidationService();
        var logger = MockHelper.CreateLogger<PenaltyService>();

        return new PenaltyService(
            repositoryManager,
            httpContext.Object,
            mapper,
            validationService.Object,
            logger.Object
        );
    }

    #endregion
}
