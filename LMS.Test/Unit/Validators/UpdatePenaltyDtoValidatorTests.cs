using FluentAssertions;
using FluentValidation.TestHelper;
using LMS.Application.Contracts.DTOs.Penalty;
using LMS.Application.Services.Validations.Penalty;

namespace LMS.Tests.Unit.Validators
{
    /// <summary>
    /// Unit tests for UpdatePenaltyDtoValidator
    /// Tests validation rules for updating existing penalties
    /// </summary>
    public class UpdatePenaltyDtoValidatorTests
    {
        private readonly UpdatePenaltyDtoValidator _validator;

        public UpdatePenaltyDtoValidatorTests()
        {
            _validator = new UpdatePenaltyDtoValidator();
        }

        #region Valid Scenarios - Should Pass Validation

        [Fact]
        public void Validate_WithAllValidData_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WithNullTransectionId_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.TransectionId = null;
            dto.TransectionStatusId = null; // Not required when no transaction
            dto.OverDueDays = null;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.TransectionId);
            result.ShouldNotHaveValidationErrorFor(x => x.TransectionStatusId);
        }

        [Fact]
        public void Validate_WithTransectionIdAndStatusId_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.TransectionId = 100;
            dto.TransectionStatusId = 5;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.TransectionStatusId);
        }

        [Fact]
        public void Validate_WithNullOverDueDays_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.OverDueDays = null;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.OverDueDays);
        }

        [Fact]
        public void Validate_WithMinimumLengthDescription_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Description = "1234567890";

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Validate_WithMaximumLengthDescription_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Description = new string('A', 500);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        #endregion

        #region Id Validation Tests

        [Fact]
        public void Validate_WithZeroId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Id = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Penalty Id is required");
        }

        [Fact]
        public void Validate_WithNegativeId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Id = -1;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Penalty Id must be greater than 0.");
        }

        [Fact]
        public void Validate_WithValidId_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Id = 100;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }

        #endregion

        #region UserId Validation Tests

        [Fact]
        public void Validate_WithZeroUserId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.UserId = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.UserId)
                .WithErrorMessage("User is required");
        }

        [Fact]
        public void Validate_WithNegativeUserId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.UserId = -1;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.UserId)
                .WithErrorMessage("User Id must be greater than 0.");
        }

        #endregion

        #region StatusId Validation Tests

        [Fact]
        public void Validate_WithZeroStatusId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.StatusId = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StatusId)
                .WithErrorMessage("Status Label is required");
        }

        [Fact]
        public void Validate_WithNegativeStatusId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
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
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.PenaltyTypeId = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.PenaltyTypeId)
                .WithErrorMessage("Penalty type is required");
        }

        [Fact]
        public void Validate_WithNegativePenaltyTypeId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
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
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.TransectionId = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TransectionId)
                .WithErrorMessage("Transection Id must be null or greater than 0.");
        }

        [Fact]
        public void Validate_WithNegativeTransectionId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.TransectionId = -1;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TransectionId)
                .WithErrorMessage("Transection Id must be null or greater than 0.");
        }

        #endregion

        #region TransectionStatusId Conditional Validation Tests

        [Fact]
        public void Validate_WithTransectionIdButNullTransectionStatusId_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.TransectionId = TestConstants.ValidTransactionId;
            dto.TransectionStatusId = null; // Invalid - should fail validation

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TransectionStatusId)
                .WithErrorMessage("Transection status is required when you change penalty status from Un-paid to other.");
        }

        [Fact]
        public void Validate_WithBothTransectionIdAndTransectionStatusId_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.TransectionId = 200;
            dto.TransectionStatusId = 3;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.TransectionStatusId);
        }

        #endregion

        #region Description Validation Tests

        [Fact]
        public void Validate_WithEmptyDescription_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Description = string.Empty;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description is required");
        }

        [Fact]
        public void Validate_WithNullDescription_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Description = null;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description is required");
        }

        [Fact]
        public void Validate_WithTooShortDescription_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Description = TestConstants.TooShortAddUpdateDescription;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Penalty Description must be at least 10 characters long.");
        }

        [Fact]
        public void Validate_WithTooLongDescription_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Description = TestConstants.TooLongDescription;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Penalty Description must be at most 500 characters long.");
        }

        #endregion

        #region Amount Validation Tests

        [Fact]
        public void Validate_WithNullAmount_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Amount = null;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Amount can't be null");
        }

        [Fact]
        public void Validate_WithZeroAmount_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Amount = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Penalty must more than 0");
        }

        [Fact]
        public void Validate_WithNegativeAmount_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Amount = -10.50m;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Penalty must more than 0");
        }

        [Fact]
        public void Validate_WithValidDecimalAmount_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Amount = 25.75m;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Amount);
        }

        [Fact]
        public void Validate_WithVerySmallAmount_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.Amount = 0.01m;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Amount);
        }

        #endregion

        #region OverDueDays Validation Tests

        [Fact]
        public void Validate_WithZeroOverDueDays_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.OverDueDays = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.OverDueDays)
                .WithErrorMessage("OverDueDays must be null or greater than 0");
        }

        [Fact]
        public void Validate_WithNegativeOverDueDays_ShouldFailValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.OverDueDays = -5;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.OverDueDays)
                .WithErrorMessage("OverDueDays must be null or greater than 0");
        }

        [Fact]
        public void Validate_WithValidOverDueDays_ShouldPassValidation()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.OverDueDays = 10;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.OverDueDays);
        }

        #endregion

        #region Multiple Validation Errors

        [Fact]
        public void Validate_WithMultipleErrors_ShouldReturnAllValidationErrors()
        {
            var dto = new UpdatePenaltyDto
            {
                Id = 0, // Invalid
                UserId = -1, // Invalid
                StatusId = 0, // Invalid
                PenaltyTypeId = -1, // Invalid
                TransectionId = 100, // Valid but...
                TransectionStatusId = null, // Invalid (required when TransectionId exists)
                Description = "Bad", // Invalid (less than 10 chars)
                Amount = -5, // Invalid
                OverDueDays = -10 // Invalid
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id);
            result.ShouldHaveValidationErrorFor(x => x.UserId);
            result.ShouldHaveValidationErrorFor(x => x.StatusId);
            result.ShouldHaveValidationErrorFor(x => x.PenaltyTypeId);
            result.ShouldHaveValidationErrorFor(x => x.TransectionStatusId);
            result.ShouldHaveValidationErrorFor(x => x.Description);
            result.ShouldHaveValidationErrorFor(x => x.Amount);
            result.ShouldHaveValidationErrorFor(x => x.OverDueDays);

            result.Errors.Count.Should().BeGreaterThan(6);
        }

        #endregion

        #region Edge Cases for Conditional Validation

        [Fact]
        public void Validate_ConditionalValidation_Scenario1_NoTransactionNoStatus_ShouldPass()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.TransectionId = null;
            dto.TransectionStatusId = null;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.TransectionStatusId);
        }

        [Fact]
        public void Validate_ConditionalValidation_Scenario2_HasTransactionHasStatus_ShouldPass()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.TransectionId = 123;
            dto.TransectionStatusId = 456;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.TransectionStatusId);
        }

        [Fact]
        public void Validate_ConditionalValidation_Scenario3_HasTransactionNoStatus_ShouldFail()
        {
            var dto = TestDataBuilder.CreateValidUpdatePenaltyDto();
            dto.TransectionId = 789;
            dto.TransectionStatusId = null;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TransectionStatusId);
        }

        #endregion
    }
}
