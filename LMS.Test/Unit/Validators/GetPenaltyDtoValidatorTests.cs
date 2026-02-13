using FluentAssertions;
using FluentValidation.TestHelper;
using LMS.Application.Contracts.DTOs.Penalty;
using LMS.Application.Services.Validations.Penalty;

namespace LMS.Tests.Unit.Validators
{
    /// <summary>
    /// Tests follow the AAA pattern (Arrange, Act, Assert)
    /// Test naming convention: MethodName_Scenario_ExpectedResult
    /// </summary>
    public class GetPenaltyDtoValidatorTests
    {
        private readonly GetPenaltyDtoValidator _validator;

        public GetPenaltyDtoValidatorTests()
        {
            _validator = new GetPenaltyDtoValidator();
        }

        #region Valid Scenarios - Should Pass Validation

        [Fact]
        public void Validate_WithAllValidData_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WithNullTransectionId_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.TransectionId = null;
            dto.TransectionDueDate = null;
            dto.OverDueDays = null;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.TransectionId);
        }

        [Fact]
        public void Validate_WithNullOverDueDays_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.OverDueDays = null;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.OverDueDays);
        }

        [Fact]
        public void Validate_WithNullTransectionDueDate_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.TransectionDueDate = null;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.TransectionDueDate);
        }

        [Fact]
        public void Validate_WithMinimumLengthDescription_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.Description = "This is exactly fifty characters in this sentence!!";

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Validate_WithMaximumLengthDescription_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.Description = new string('A', 500);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Validate_WithValidDecimalAmount_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.Amount = 25.75m;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Amount);
        }

        #endregion

        #region Id Validation Tests

        [Fact]
        public void Validate_WithZeroId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.Id = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Penalty Id is required");
        }

        [Fact]
        public void Validate_WithNegativeId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.Id = -1;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        #endregion

        #region Status Label Validation Tests

        [Fact]
        public void Validate_WithEmptyStatusLabel_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.StatusLabel = string.Empty;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StatusLabel)
                .WithErrorMessage("Status Label is required");
        }

        [Fact]
        public void Validate_WithTooLongStatusLabel_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.StatusLabel = TestConstants.TooLongStatusLabel;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StatusLabel)
                .WithErrorMessage("Status Label must be of length 50 characters");
        }

        [Fact]
        public void Validate_WithEmptyStatusLabelColor_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.StatusLabelColor = string.Empty;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StatusLabelColor)
                .WithErrorMessage("Status Label is required");
        }

        [Fact]
        public void Validate_WithTooLongStatusLabelColor_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.StatusLabelColor = new string('A', 51); // 51 characters

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StatusLabelColor)
                .WithErrorMessage("Status Label color must be of length 50 characters");
        }

        [Fact]
        public void Validate_WithEmptyStatusLabelBgColor_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.StatusLabelBgColor = string.Empty;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StatusLabelBgColor)
                .WithErrorMessage("Status Label is required");
        }

        #endregion

        #region TransectionId Validation Tests

        [Fact]
        public void Validate_WithZeroTransectionId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.TransectionId = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TransectionId)
                .WithErrorMessage("Transection Id must be null or greater than 0.");
        }

        [Fact]
        public void Validate_WithNegativeTransectionId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.TransectionId = -1;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TransectionId)
                .WithErrorMessage("Transection Id must be null or greater than 0.");
        }

        #endregion

        #region Description Validation Tests

        [Fact]
        public void Validate_WithEmptyDescription_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.Description = string.Empty;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description is required");
        }

        [Fact]
        public void Validate_WithTooShortDescription_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.Description = TestConstants.TooShortDescription;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Penalty Description must be at least 50 characters long.");
        }

        [Fact]
        public void Validate_WithTooLongDescription_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.Description = TestConstants.TooLongDescription;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Penalty Description must be at most 500 characters long.");
        }

        #endregion

        #region Amount Validation Tests

        [Fact]
        public void Validate_WithZeroAmount_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.Amount = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Penalty must more than 0");
        }

        [Fact]
        public void Validate_WithNegativeAmount_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.Amount = -10.50m;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Penalty must more than 0");
        }

        #endregion

        #region OverDueDays Validation Tests

        [Fact]
        public void Validate_WithZeroOverDueDays_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.OverDueDays = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.OverDueDays)
                .WithErrorMessage("OverDueDays must be null or greater than 0");
        }

        [Fact]
        public void Validate_WithNegativeOverDueDays_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.OverDueDays = -5;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.OverDueDays)
                .WithErrorMessage("OverDueDays must be null or greater than 0");
        }

        #endregion

        #region TransectionDueDate Validation Tests

        [Fact]
        public void Validate_WithFutureTransectionDueDate_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.TransectionDueDate = DateTimeOffset.UtcNow.AddDays(5);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TransectionDueDate)
                .WithErrorMessage("Transection Due Date cannot be in the future.");
        }

        [Fact]
        public void Validate_WithPastTransectionDueDate_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.TransectionDueDate = DateTimeOffset.UtcNow.AddDays(-10);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.TransectionDueDate);
        }

        [Fact]
        public void Validate_WithTodayTransectionDueDate_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.TransectionDueDate = DateTimeOffset.UtcNow;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.TransectionDueDate);
        }

        #endregion

        #region IsRemoved Validation Tests

        [Fact]
        public void Validate_WithIsRemovedTrue_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.IsRemoved = true;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.IsRemoved);
        }

        [Fact]
        public void Validate_WithIsRemovedFalse_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidGetPenaltyDto();
            dto.IsRemoved = false;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.IsRemoved);
        }

        #endregion

        #region Multiple Validation Errors

        [Fact]
        public void Validate_WithMultipleErrors_ShouldReturnAllValidationErrors()
        {
            var dto = new GetPenaltyDto
            {
                Id = 0, // Invalid
                StatusLabel = "", // Invalid
                StatusLabelColor = "", // Invalid
                StatusLabelBgColor = "", // Invalid
                Description = "Short", // Invalid (less than 50 chars)
                Amount = -10, // Invalid
                TransectionId = -1, // Invalid
                OverDueDays = -5, // Invalid
                TransectionDueDate = DateTimeOffset.UtcNow.AddDays(5), // Invalid (future)
                IsRemoved = false
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id);
            result.ShouldHaveValidationErrorFor(x => x.StatusLabel);
            result.ShouldHaveValidationErrorFor(x => x.StatusLabelColor);
            result.ShouldHaveValidationErrorFor(x => x.StatusLabelBgColor);
            result.ShouldHaveValidationErrorFor(x => x.Description);
            result.ShouldHaveValidationErrorFor(x => x.Amount);
            result.ShouldHaveValidationErrorFor(x => x.TransectionId);
            result.ShouldHaveValidationErrorFor(x => x.OverDueDays);
            result.ShouldHaveValidationErrorFor(x => x.TransectionDueDate);

            // Verify total error count
            result.Errors.Count.Should().BeGreaterThan(5);
        }

        #endregion
    }
}
