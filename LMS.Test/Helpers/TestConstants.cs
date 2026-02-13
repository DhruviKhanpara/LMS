namespace LMS.Tests.Helpers
{
    /// <summary>
    /// Contains constant values used across multiple test classes
    /// </summary>
    public class TestConstants
    {
        // Penalty Test Constants
        public const long ValidPenaltyId = 1;
        public const long ValidUserId = 100;
        public const long ValidTransactionId = 500;
        public const string ValidUserName = "John Doe";
        public const string ValidStatusLabel = "Unpaid";
        public const string ValidStatusLabelColor = "#FF0000";
        public const string ValidStatusLabelBgColor = "#FFEEEE";
        public const string ValidPenaltyTypeName = "Overdue";
        public const string ValidPenaltyTypeInfo = "Penalty for overdue books";
        public const string ValidDescription = "This is a valid penalty description that meets the minimum length requirement of 50 characters.";
        public const decimal ValidAmount = 50.00m;
        public const int ValidOverDueDays = 5;

        // Invalid Test Data
        public const string TooShortDescription = "Too short"; // Less than 50 characters
        public const string TooLongDescription = "This description is way too long and exceeds the maximum allowed length of 500 characters. " +
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
            "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. " +
            "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
            "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum consectetur adipiscing elit sed do eiusmod.";

        public const string TooLongStatusLabel = "This status label is way too long and exceeds 50 characters limit";

        // AddPenaltyDto & UpdatePenaltyDto Constants
        public const long ValidStatusId = 11;
        public const long ValidStatusTypeId = 1;
        public const long ValidPenaltyTypeId = 1;
        public const long ValidTransactionStatusId = 3;
        public const string ValidShortDescription = "Valid penalty description for testing purposes."; // Exactly 50 chars for GetPenalty, valid for Add/Update (10+ chars)
        public const string TooShortAddUpdateDescription = "Short"; // Less than 10 characters for Add/Update
    }
}
