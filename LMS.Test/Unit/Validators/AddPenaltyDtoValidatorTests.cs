using FluentAssertions;
using FluentValidation.TestHelper;
using LMS.Application.Contracts.DTOs.Penalty;
using LMS.Application.Services.Validations.Penalty;

namespace LMS.Tests.Unit.Validators
{
    /// <summary>
    /// Unit tests for AddPenaltyDtoValidator
    /// Tests validation rules for adding new penalties
    /// </summary>
    public class AddPenaltyDtoValidatorTests
    {
        private readonly AddPenaltyDtoValidator _validator;

        public AddPenaltyDtoValidatorTests()
        {
            _validator = new AddPenaltyDtoValidator();
        }

        #region Valid Scenarios - Should Pass Validation

        [Fact]
        public void Validate_WithAllValidData_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WithNullTransectionId_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.TransectionId = null;
            dto.OverDueDays = null;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.TransectionId);
        }

        [Fact]
        public void Validate_WithNullOverDueDays_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.OverDueDays = null;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.OverDueDays);
        }

        #endregion

        #region UserId Validation Tests

        [Fact]
        public void Validate_WithZeroUserId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.UserId = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.UserId)
                .WithErrorMessage("User is required");
        }

        [Fact]
        public void Validate_WithNegativeUserId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.UserId = -1;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.UserId)
                .WithErrorMessage("User Id must be greater than 0.");
        }

        [Fact]
        public void Validate_WithValidUserId_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.UserId = 100;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.UserId);
        }

        #endregion

        #region StatusId Validation Tests

        [Fact]
        public void Validate_WithZeroStatusId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.StatusId = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StatusId)
                .WithErrorMessage("Status Label is required");
        }

        [Fact]
        public void Validate_WithNegativeStatusId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.StatusId = -1;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StatusId)
                .WithErrorMessage("Status Label Id must be greater than 0.");
        }

        #endregion

        #region PenaltyTypeId Validation Tests

        [Fact]
        public void Validate_WithZeroPenaltyTypeId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.PenaltyTypeId = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.PenaltyTypeId)
                .WithErrorMessage("Penalty type is required");
        }

        [Fact]
        public void Validate_WithNegativePenaltyTypeId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.PenaltyTypeId = -1;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.PenaltyTypeId)
                .WithErrorMessage("Penalty type Id must be greater than 0.");
        }

        #endregion

        #region TransectionId Validation Tests

        [Fact]
        public void Validate_WithZeroTransectionId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.TransectionId = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TransectionId)
                .WithErrorMessage("Transection Id must be null or greater than 0.");
        }

        [Fact]
        public void Validate_WithNegativeTransectionId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.TransectionId = -1;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TransectionId)
                .WithErrorMessage("Transection Id must be null or greater than 0.");
        }

        [Fact]
        public void Validate_WithValidTransectionId_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.TransectionId = 500;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.TransectionId);
        }

        #endregion

        #region Description Validation Tests

        [Fact]
        public void Validate_WithEmptyDescription_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.Description = string.Empty;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description is required");
        }

        [Fact]
        public void Validate_WithNullDescription_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.Description = null;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description is required");
        }

        [Fact]
        public void Validate_WithTooShortDescription_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.Description = TestConstants.TooShortAddUpdateDescription;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Penalty Description must be at least 10 characters long.");
        }

        [Fact]
        public void Validate_WithTooLongDescription_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.Description = TestConstants.TooLongDescription;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Penalty Description must be at most 500 characters long.");
        }

        [Fact]
        public void Validate_WithMinimumLengthDescription_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.Description = "1234567890";

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Validate_WithMaximumLengthDescription_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.Description = new string('A', 500);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        #endregion

        #region Amount Validation Tests

        [Fact]
        public void Validate_WithNullAmount_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.Amount = null;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Amount can't be null");
        }

        [Fact]
        public void Validate_WithZeroAmount_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.Amount = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Penalty must more than 0");
        }

        [Fact]
        public void Validate_WithNegativeAmount_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.Amount = -10.50m;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Penalty must more than 0");
        }

        [Fact]
        public void Validate_WithValidDecimalAmount_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.Amount = 25.75m;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Amount);
        }

        [Fact]
        public void Validate_WithVerySmallAmount_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.Amount = 0.01m;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Amount);
        }

        #endregion

        #region OverDueDays Validation Tests

        [Fact]
        public void Validate_WithZeroOverDueDays_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.OverDueDays = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.OverDueDays)
                .WithErrorMessage("OverDueDays must be null or greater than 0");
        }

        [Fact]
        public void Validate_WithNegativeOverDueDays_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.OverDueDays = -5;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.OverDueDays)
                .WithErrorMessage("OverDueDays must be null or greater than 0");
        }

        [Fact]
        public void Validate_WithValidOverDueDays_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidAddPenaltyDto();
            dto.OverDueDays = 10;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.OverDueDays);
        }

        #endregion

        #region Multiple Validation Errors

        [Fact]
        public void Validate_WithMultipleErrors_ShouldReturnAllValidationErrors()
        {
            var dto = new AddPenaltyDto
            {
                UserId = 0, // Invalid
                StatusId = -1, // Invalid
                PenaltyTypeId = 0, // Invalid
                TransectionId = -5, // Invalid
                Description = "Short", // Invalid (less than 10 chars)
                Amount = null, // Invalid
                OverDueDays = -10 // Invalid
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.UserId);
            result.ShouldHaveValidationErrorFor(x => x.StatusId);
            result.ShouldHaveValidationErrorFor(x => x.PenaltyTypeId);
            result.ShouldHaveValidationErrorFor(x => x.TransectionId);
            result.ShouldHaveValidationErrorFor(x => x.Description);
            result.ShouldHaveValidationErrorFor(x => x.Amount);
            result.ShouldHaveValidationErrorFor(x => x.OverDueDays);

            result.Errors.Count.Should().BeGreaterThan(5);
        }

        #endregion
    }
}
