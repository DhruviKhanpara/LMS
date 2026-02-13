using FluentAssertions;
using LMS.Application.Services.Services;
using LMS.Core.Entities;
using MockQueryable;

namespace LMS.Tests.Unit.Services
{
    /// <summary>
    /// Unit tests for penalty calculation helper methods in PenaltyService.
    /// These tests focus on pure calculation logic and config retrieval.
    /// Complex LINQ queries are tested in PenaltyCalculationIntegrationTests.
    /// </summary>
    public class PenaltyCalculationTests
    {
        #region CalculateDynamicPenalty Tests - Pure Function

        /// <summary>
        /// Tests basic penalty calculation with no increase (interval not reached)
        /// </summary>
        [Fact]
        public void CalculateDynamicPenalty_WithinFirstInterval_ShouldReturnBasePenaltyOnly()
        {
            var (service, _, _, _, _, _) = MockHelper.CreatePenaltyService();
            int overdueDays = 3;
            int lastCalculatedDay = 0;
            int basePenalty = 5;
            string increaseType = "ADD";
            int increaseValue = 5;
            int intervalDays = 5;
            int itemCount = 1;

            var result = CallCalculateDynamicPenalty(service, overdueDays, lastCalculatedDay, basePenalty, increaseType, increaseValue, intervalDays, itemCount);

            // Assert - 3 days × 5 Rs = 15 Rs (no increase yet)
            result.Should().Be(15m);
        }

        [Fact]
        public void CalculateDynamicPenalty_ExactlyAtIntervalBoundary_ShouldApplyIncrease()
        {
            var (service, _, _, _, _, _) = MockHelper.CreatePenaltyService();
            int overdueDays = 5;
            int lastCalculatedDay = 0;
            int basePenalty = 5;
            string increaseType = "ADD";
            int increaseValue = 5;
            int intervalDays = 5;
            int itemCount = 1;

            // Act
            var result = CallCalculateDynamicPenalty(service, overdueDays, lastCalculatedDay, basePenalty, increaseType, increaseValue, intervalDays, itemCount);

            // Assert
            // Day 1-5: Base penalty = 5 Rs/day
            // Day 5 is in interval 1 (5/5 = 1), so penalty = 5 + (1 × 5) = 10 Rs
            // Total: (5 + 5 + 5 + 5 + 10) = 30 Rs
            result.Should().Be(30m);
        }

        [Theory]
        [InlineData(1, 0, 5, "ADD", 5, 5, 1, 5)]      // Day 1: 5 Rs
        [InlineData(5, 0, 5, "ADD", 5, 5, 1, 30)]     // Days 1-5: interval changes at day 5
        [InlineData(10, 0, 5, "ADD", 5, 5, 1, 85)]    // Days 1-10: two intervals
        [InlineData(15, 0, 5, "ADD", 5, 5, 1, 165)]   // Days 1-15: three intervals
        public void CalculateDynamicPenalty_WithAdditiveIncrease_ShouldCalculateCorrectly(int overdueDays, int lastCalculatedDay, int basePenalty, string increaseType, int increaseValue, int intervalDays, int itemCount, decimal expected)
        {
            var (service, _, _, _, _, _) = MockHelper.CreatePenaltyService();

            var result = CallCalculateDynamicPenalty(service, overdueDays, lastCalculatedDay, basePenalty, increaseType, increaseValue, intervalDays, itemCount);

            result.Should().Be(expected);
        }

        [Fact]
        public void CalculateDynamicPenalty_WithMultiplicativeIncrease_ShouldCalculateCorrectly()
        {
            var (service, _, _, _, _, _) = MockHelper.CreatePenaltyService();
            int overdueDays = 10;
            int lastCalculatedDay = 0;
            int basePenalty = 5;
            string increaseType = "MULTIPLY";
            int increaseValue = 2;
            int intervalDays = 5;
            int itemCount = 1;

            var result = CallCalculateDynamicPenalty(service, overdueDays, lastCalculatedDay, basePenalty, increaseType, increaseValue, intervalDays, itemCount);

            // Assert
            // Days 1-4: 5 Rs/day (interval 0)
            // Day 5: 5 × 2^1 = 10 Rs (interval 1)
            // Days 6-9: 5 × 2^1 = 10 Rs/day (interval 1)
            // Day 10: 5 × 2^2 = 20 Rs (interval 2)
            // Total: (5×4) + 10 + (10×4) + 20 = 20 + 10 + 40 + 20 = 90 Rs
            result.Should().Be(90m);
        }

        [Fact]
        public void CalculateDynamicPenalty_WithShorthandAddSymbol_ShouldWorkSameAsADD()
        {
            var (service, _, _, _, _, _) = MockHelper.CreatePenaltyService();
            int overdueDays = 10;
            int lastCalculatedDay = 0;
            int basePenalty = 5;
            string increaseType = "+";
            int increaseValue = 5;
            int intervalDays = 5;
            int itemCount = 1;

            var result = CallCalculateDynamicPenalty(service, overdueDays, lastCalculatedDay, basePenalty, increaseType, increaseValue, intervalDays, itemCount);

            result.Should().Be(85m);
        }

        [Fact]
        public void CalculateDynamicPenalty_WithShorthandMultiplySymbol_ShouldWorkSameAsMULTIPLY()
        {
            var (service, _, _, _, _, _) = MockHelper.CreatePenaltyService();
            int overdueDays = 10;
            int lastCalculatedDay = 0;
            int basePenalty = 5;
            string increaseType = "*";
            int increaseValue = 2;
            int intervalDays = 5;
            int itemCount = 1;

            var result = CallCalculateDynamicPenalty(service, overdueDays, lastCalculatedDay, basePenalty, increaseType, increaseValue, intervalDays, itemCount);

            result.Should().Be(90m);
        }

        [Fact]
        public void CalculateDynamicPenalty_WithIncrementalCalculation_ShouldOnlyCalculateNewDays()
        {
            var (service, _, _, _, _, _) = MockHelper.CreatePenaltyService();
            int overdueDays = 10;
            int lastCalculatedDay = 5;
            int basePenalty = 5;
            string increaseType = "ADD";
            int increaseValue = 5;
            int intervalDays = 5;
            int itemCount = 1;

            var result = CallCalculateDynamicPenalty(service, overdueDays, lastCalculatedDay, basePenalty, increaseType, increaseValue, intervalDays, itemCount);

            // Assert
            // Days 6-10 are in interval 1 (6/5=1, 7/5=1, 8/5=1, 9/5=1, 10/5=2)
            // Day 6-9: 5 + (1 × 5) = 10 Rs/day → 40 Rs
            // Day 10: 5 + (2 × 5) = 15 Rs → 15 Rs
            // Total: 40 + 15 = 55 Rs
            result.Should().Be(55m);
        }

        [Fact]
        public void CalculateDynamicPenalty_WithMultipleItems_ShouldMultiplyByItemCount()
        {
            var (service, _, _, _, _, _) = MockHelper.CreatePenaltyService();
            int overdueDays = 5;
            int lastCalculatedDay = 0;
            int basePenalty = 5;
            string increaseType = "ADD";
            int increaseValue = 5;
            int intervalDays = 5;
            int itemCount = 3;

            var result = CallCalculateDynamicPenalty(service, overdueDays, lastCalculatedDay, basePenalty, increaseType, increaseValue, intervalDays, itemCount);

            // Assert
            // Single item would be 30 Rs (from previous test)
            // With 3 items: 30 × 3 = 90 Rs
            result.Should().Be(90m);
        }

        [Fact]
        public void CalculateDynamicPenalty_WithZeroOverdueDays_ShouldReturnZero()
        {
            var (service, _, _, _, _, _) = MockHelper.CreatePenaltyService();
            int overdueDays = 0;
            int lastCalculatedDay = 0;
            int basePenalty = 5;
            string increaseType = "ADD";
            int increaseValue = 5;
            int intervalDays = 5;
            int itemCount = 1;

            var result = CallCalculateDynamicPenalty(service, overdueDays, lastCalculatedDay, basePenalty, increaseType, increaseValue, intervalDays, itemCount);

            result.Should().Be(0m);
        }

        [Fact]
        public void CalculateDynamicPenalty_WithUnknownIncreaseType_ShouldUseBasePenaltyOnly()
        {
            var (service, _, _, _, _, _) = MockHelper.CreatePenaltyService();
            int overdueDays = 10;
            int lastCalculatedDay = 0;
            int basePenalty = 5;
            string increaseType = "INVALID";
            int increaseValue = 5;
            int intervalDays = 5;
            int itemCount = 1;

            var result = CallCalculateDynamicPenalty(service, overdueDays, lastCalculatedDay, basePenalty, increaseType, increaseValue, intervalDays, itemCount);

            // Assert - Should use base penalty only (no increase applied)
            // 10 days × 5 Rs = 50 Rs
            result.Should().Be(50m);
        }

        [Fact]
        public void CalculateDynamicPenalty_WithCaseInsensitiveIncreaseType_ShouldWork()
        {
            var (service, _, _, _, _, _) = MockHelper.CreatePenaltyService();
            int overdueDays = 10;
            int lastCalculatedDay = 0;
            int basePenalty = 5;
            string increaseType = "add";
            int increaseValue = 5;
            int intervalDays = 5;
            int itemCount = 1;

            var result = CallCalculateDynamicPenalty(service, overdueDays, lastCalculatedDay, basePenalty, increaseType, increaseValue, intervalDays, itemCount);

            result.Should().Be(85m);
        }

        #endregion

        #region GetBufferTimeFromConfig Tests

        [Fact]
        public async Task GetBufferTimeFromConfig_WithValidConfig_ShouldReturnCorrectValues()
        {
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService();

            var configData = new List<Configs>
            {
                new Configs { KeyName = "PreviousLimitCarryoverDays", KeyValue = "7" },
                new Configs { KeyName = "MembershipExpiryBufferDays", KeyValue = "5" }
            }.AsQueryable().BuildMock();

            repoManager.Setup(x => x.ConfigRepository.GetByKeyNameListAsync(It.IsAny<List<string>>()))
                .Returns(configData);

            var (carryoverDays, bufferDays) = await CallGetBufferTimeFromConfig(service);

            carryoverDays.Should().Be(7);
            bufferDays.Should().Be(5);
        }

        [Fact]
        public async Task GetBufferTimeFromConfig_WithMissingConfig_ShouldReturnZeroDefaults()
        {
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService();

            var emptyConfigData = new List<Configs>()
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.ConfigRepository.GetByKeyNameListAsync(It.IsAny<List<string>>()))
                .Returns(emptyConfigData);

            var (carryoverDays, bufferDays) = await CallGetBufferTimeFromConfig(service);

            carryoverDays.Should().Be(0);
            bufferDays.Should().Be(0);
        }

        [Fact]
        public async Task GetBufferTimeFromConfig_WithInvalidConfigValues_ShouldReturnZeroDefaults()
        {
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService();

            var invalidConfigData = new List<Configs>
            {
                new Configs { KeyName = "PreviousLimitCarryoverDays", KeyValue = "invalid" },
                new Configs { KeyName = "MembershipExpiryBufferDays", KeyValue = "not_a_number" }
            }.AsQueryable().BuildMock();

            repoManager.Setup(x => x.ConfigRepository.GetByKeyNameListAsync(It.IsAny<List<string>>()))
                .Returns(invalidConfigData);

            var (carryoverDays, bufferDays) = await CallGetBufferTimeFromConfig(service);

            carryoverDays.Should().Be(0);
            bufferDays.Should().Be(0);
        }

        #endregion

        #region GetPenaltyInfoFromConfig Tests

        [Fact]
        public async Task GetPenaltyInfoFromConfig_WithValidConfig_ShouldReturnCorrectValues()
        {
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService();

            var configData = new List<Configs>
            {
                new Configs { KeyName = "BasePenaltyPerDay", KeyValue = "5" },
                new Configs { KeyName = "PenaltyIncreaseType", KeyValue = "ADD" },
                new Configs { KeyName = "PenaltyIncreaseValue", KeyValue = "5" },
                new Configs { KeyName = "PenaltyIncreaseDurationInDays", KeyValue = "5" }
            }.AsQueryable().BuildMock();

            repoManager.Setup(x => x.ConfigRepository.GetByKeyNameListAsync(It.IsAny<List<string>>()))
                .Returns(configData);

            var (basePenalty, increaseValue, intervalDays, increaseType) = await CallGetPenaltyInfoFromConfig(service);

            basePenalty.Should().Be(5);
            increaseValue.Should().Be(5);
            intervalDays.Should().Be(5);
            increaseType.Should().Be("ADD");
        }

        [Fact]
        public async Task GetPenaltyInfoFromConfig_WithMissingConfig_ShouldReturnDefaults()
        {
            var (service, repoManager, _, _, _, _) = MockHelper.CreatePenaltyService();

            var emptyConfigData = new List<Configs>()
                .AsQueryable()
                .BuildMock();

            repoManager.Setup(x => x.ConfigRepository.GetByKeyNameListAsync(It.IsAny<List<string>>()))
                .Returns(emptyConfigData);

            var (basePenalty, increaseValue, intervalDays, increaseType) = await CallGetPenaltyInfoFromConfig(service);

            basePenalty.Should().Be(0);
            increaseValue.Should().Be(0);
            intervalDays.Should().Be(0);
            increaseType.Should().Be("+");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Helper method to call private GetPenaltyInfoFromConfig method using reflection
        /// </summary>
        private async Task<(int basePenalty, int increaseValue, int intervalDays, string increaseType)> CallGetPenaltyInfoFromConfig(PenaltyService service)
        {
            var method = typeof(PenaltyService).GetMethod(
                "GetPenaltyInfoFromConfig",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

            var result = (Task<(int, int, int, string)>)method!.Invoke(service, null)!;

            return await result!;
        }

        /// <summary>
        /// Helper method to call private GetBufferTimeFromConfig method using reflection
        /// </summary>
        private async Task<(int carryoverDays, int bufferDays)> CallGetBufferTimeFromConfig(PenaltyService service)
        {
            var method = typeof(PenaltyService).GetMethod(
                "GetBufferTimeFromConfig",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

            var result = (Task<(int, int)>)method!.Invoke(service, null)!;

            return await result!;
        }

        /// <summary>
        /// Helper method to call private CalculateDynamicPenalty method using reflection
        /// </summary>
        private decimal CallCalculateDynamicPenalty(
            PenaltyService service,
            int overdueDays,
            int lastCalculatedDay,
            int basePenalty,
            string increaseType,
            int increaseValue,
            int intervalDays,
            int penalizedItemCount)
        {
            var method = typeof(PenaltyService).GetMethod(
                "CalculateDynamicPenalty",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

            var result = method!.Invoke(service, new object[]
            {
                overdueDays,
                lastCalculatedDay,
                basePenalty,
                increaseType,
                increaseValue,
                intervalDays,
                penalizedItemCount
            });

            return (decimal)result!;
        }

        #endregion
    }
}
