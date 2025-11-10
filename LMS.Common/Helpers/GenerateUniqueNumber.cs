namespace LMS.Common.Helpers;

public static class GenerateUniqueNumber
{
    public static string GenerateLibraryCardNumber(string latestCardNumber)
    {
        if (!string.IsNullOrWhiteSpace(latestCardNumber) && latestCardNumber.Length >= 7)
        {
            var isGetNumber = long.TryParse(latestCardNumber.Trim().Substring(7), out long uniqueNumber);
            if (!isGetNumber)
                throw new Exception("Something wants wrong while parsing last libraryCardNumber");

            var newUniqueNumber = uniqueNumber + 1;
            return $"USR{DateTimeOffset.Now.Year}{(newUniqueNumber):D5}";
        }
        else
            throw new Exception("Last libraryCard number not valid");
    }
}
