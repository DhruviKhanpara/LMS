namespace LMS.Common.Helpers;

public static class CornGeneratorForJob
{
    /// <summary>
    /// Generates a CRON expression for Hangfire based on the given frequency type and value.
    /// </summary>
    /// <param name="time">Time of day in HH:mm format (used for daily, weekly, monthly, everyndays).</param>
    /// <param name="frequencyType">Type of frequency (e.g., everynminutes, daily, weekly, monthly, everyndays).</param>
    /// <param name="frequencyValue">Interval value (e.g., every 5 minutes).</param>
    /// <returns>CRON expression string.</returns>
    public static string GenerateCron(string frequencyType, int frequencyValue = 1, string? time = null)
    {
        frequencyType = frequencyType?.Trim().ToLowerInvariant() ?? string.Empty;

        switch (frequencyType)
        {
            case "everynminutes":
                return $"*/{frequencyValue} * * * *";

            case "daily":
            case "weekly":
            case "monthly":
            case "everyndays":
                if (!TimeSpan.TryParse(time, out TimeSpan istTime))
                    throw new ArgumentException("Invalid time format. Expected HH:mm.");

                // Convert IST time to UTC time
                var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                var utcZone = TimeZoneInfo.Utc;

                // Create a DateTime from IST time
                var istDateTime = new DateTime(2000, 1, 1, istTime.Hours, istTime.Minutes, 0);
                var utcDateTime = TimeZoneInfo.ConvertTime(istDateTime, istZone, utcZone);

                int hour = utcDateTime.Hour;
                int minute = utcDateTime.Minute;

                switch (frequencyType)
                {
                    case "daily":
                        return $"{minute} {hour} * * *";
                    case "weekly":
                        return $"{minute} {hour} * * 0"; // Sunday
                    case "monthly":
                        return $"{minute} {hour} 1 * *";
                    case "everyndays":
                        return $"{minute} {hour} */{frequencyValue} * *";
                }
                break;

            default:
                throw new ArgumentException($"Unsupported frequency type: {frequencyType}");
        }

        throw new InvalidOperationException("Unhandled frequency type.");
    }
}
