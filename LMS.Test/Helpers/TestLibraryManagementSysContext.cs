using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Tests.Helpers;

public class TestLibraryManagementSysContext : LibraryManagementSysContext
{
    public TestLibraryManagementSysContext(DbContextOptions<LibraryManagementSysContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Remove IDENTITY only from lookup/reference tables
        var lookupTables = new[]
        {
            nameof(Status),
            nameof(StatusType),
            nameof(PenaltyType),
            nameof(Configs),
            nameof(RoleList),
            nameof(Genre),
            nameof(Membership)
        };

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();

            // Only disable identity for lookup tables
            if (lookupTables.Contains(tableName))
            {
                var properties = entity.GetProperties()
                    .Where(p => p.IsPrimaryKey() && p.ValueGenerated == ValueGenerated.OnAdd);

                foreach (var property in properties)
                {
                    property.ValueGenerated = ValueGenerated.Never;
                }
            }
        }

        // Seed data with explicit IDs
        //modelBuilder.Entity<Role>().HasData(
        //    new Role { Id = 1, Name = "Admin" },
        //    new Role { Id = 2, Name = "User" }
        //);
    }
}

