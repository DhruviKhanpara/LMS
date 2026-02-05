namespace LMS.Application.Services.Constants;

public static class ConfigKeysConstants
{
    /// <summary>
    /// Not stored in table - custome use in the code
    /// </summary>
    public const string BorrowLimit = "BorrowLimit";
    public const string ReservationLimit = "ReservationLimit";

    /// <summary> 
    /// Configuration keys related to user's membership activation rule.
    /// </summary>
    public const string MaxActiveMembership = "MaxActiveMembership";
    public const string NextPlanActivationTimeInMinutes = "NextPlanActivationTimeInMinutes";

    /// <summary> 
    /// Configuration keys related to file storing location.
    /// </summary> 
    public const string ProfilePhotoDirectoryPath = "ProfilePhotoDirectoryPath";
    public const string ProfilePhotoArchiveDirectoryPath = "ProfilePhotoArchiveDirectoryPath";
    public const string BookCoverPageDirectoryPath = "BookCoverPageDirectoryPath";
    public const string BookCoverPageArchiveDirectoryPath = "BookCoverPageArchiveDirectoryPath";
    public const string BookPreviewDirectoryPath = "BookPreviewDirectoryPath";
    public const string BookPreviewArchiveDirectoryPath = "BookPreviewArchiveDirectoryPath";
    public const string ImageFileExtensions = "ImageFileExtensions";

    /// <summary> 
    /// Configuration keys related to Due period rules.
    /// </summary> 
    public const string BorrowDueDays = "BorrowDueDays";
    public const string AllocationDueDays = "AllocationDueDays";

    /// <summary> 
    /// Configuration keys related to Penalty calculations.
    /// </summary> 
    public const string BasePenaltyPerDay = "BasePenaltyPerDay";
    public const string PenaltyIncreaseType = "PenaltyIncreaseType";
    public const string PenaltyIncreaseValue = "PenaltyIncreaseValue";
    public const string PenaltyIncreaseDurationInDays = "PenaltyIncreaseDurationInDays";

    /// <summary> 
    /// Configuration keys related to membership allocation, expiry, and carryover rules.
    /// </summary> 
    public const string DefaultAllocationDelayInDays = "DefaultAllocationDelayInDays";
    public const string MembershipExpiryBufferDays = "MembershipExpiryBufferDays";
    public const string PreviousLimitCarryoverDays = "PreviousLimitCarryoverDays";

    /// <summary> 
    /// Configuration keys related to transaction rule.
    /// </summary> 
    public const string MaxTransferAllocationCount = "MaxTransferAllocationCount";
    public const string MaxRenewCount = "MaxRenewCount";

    /// <summary> 
    /// Configuration keys related to outbox notification push rule.
    /// </summary>
    public const string OutboxMaxRetryCount = "OutboxMaxRetryCount";
    public const string OutboxMaxRetryDelayMinutes = "OutboxMaxRetryDelayMinutes";

    /// <summary> 
    /// Configuration keys related to ProcessGenericOutbox Job.
    /// </summary>
    public const string ProcessGenericOutbox_Frequency = "ProcessGenericOutbox_Frequency";
    public const string ProcessGenericOutbox_Interval = "ProcessGenericOutbox_Interval";
    public const string ProcessGenericOutbox_Time = "ProcessGenericOutbox_Time";

    /// <summary> 
    /// Configuration keys related to MembershipDueReminder Job.
    /// </summary>
    public const string MembershipDueReminder_Frequency = "MembershipDueReminder_Frequency";
    public const string MembershipDueReminder_Interval = "MembershipDueReminder_Interval";
    public const string MembershipDueReminder_Time = "MembershipDueReminder_Time";

    /// <summary> 
    /// Configuration keys related to PenaltyCalculation Job.
    /// </summary>
    public const string PenaltyCalculation_Frequency = "PenaltyCalculation_Frequency";
    public const string PenaltyCalculation_Interval = "PenaltyCalculation_Interval";
    public const string PenaltyCalculation_Time = "PenaltyCalculation_Time";

    /// <summary> 
    /// Configuration keys related to ReallocateExpiredReservations Job.
    /// </summary>
    public const string ReallocateExpiredReservations_Frequency = "ReallocateExpiredReservations_Frequency";
    public const string ReallocateExpiredReservations_Interval = "ReallocateExpiredReservations_Interval";
    public const string ReallocateExpiredReservations_Time = "ReallocateExpiredReservations_Time";

    /// <summary> 
    /// Configuration keys related to AllocateReservedBooks Job.
    /// </summary>
    public const string AllocateReservedBooks_Frequency = "AllocateReservedBooks_Frequency";
    public const string AllocateReservedBooks_Interval = "AllocateReservedBooks_Interval";
    public const string AllocateReservedBooks_Time = "AllocateReservedBooks_Time";

    /// <summary> 
    /// Configuration keys related to DueDateReminder Job.
    /// </summary>
    public const string DueDateReminder_Frequency = "DueDateReminder_Frequency";
    public const string DueDateReminder_Interval = "DueDateReminder_Interval";
    public const string DueDateReminder_Time = "DueDateReminder_Time";

    /// <summary> 
    /// Configuration keys related to NotifyReservationAllocation Job.
    /// </summary>
    public const string NotifyReservationAllocation_Frequency = "NotifyReservationAllocation_Frequency";
    public const string NotifyReservationAllocation_Interval = "NotifyReservationAllocation_Interval";
    public const string NotifyReservationAllocation_Time = "NotifyReservationAllocation_Time";


    /// <summary> 
    /// Collection of all penalty-related configuration keys. 
    /// </summary> 
    public static readonly List<string> PenaltyConfigKeys = new List<string> { BasePenaltyPerDay, PenaltyIncreaseType, PenaltyIncreaseValue, PenaltyIncreaseDurationInDays };

    /// <summary> 
    /// Collection of all buffer time-related configuration keys. 
    /// </summary> 
    public static readonly List<string> BufferTimeConfigKeys = new List<string> { PreviousLimitCarryoverDays, MembershipExpiryBufferDays };

    /// <summary> 
    /// Collection of all BookCover-related configuration keys. 
    /// </summary> 
    public static readonly List<string> BookCoverConfigKeys = new List<string> { BookCoverPageDirectoryPath, BookCoverPageArchiveDirectoryPath, ImageFileExtensions };

    /// <summary> 
    /// Collection of all UserProfile-related configuration keys. 
    /// </summary> 
    public static readonly List<string> UserProfileConfigKeys = new List<string> { ProfilePhotoDirectoryPath, ProfilePhotoArchiveDirectoryPath, ImageFileExtensions };

    /// <summary> 
    /// Collection of all BookPreview-related configuration keys. 
    /// </summary> 
    public static readonly List<string> BookPreviewConfigKeys = new List<string> { BookPreviewDirectoryPath, BookPreviewArchiveDirectoryPath };

    /// <summary> 
    /// Collection of all Job-specific configuration keys. 
    /// </summary> 
    public static readonly List<string> JobsConfigKeys = new List<string> 
    {
        ProcessGenericOutbox_Frequency, ProcessGenericOutbox_Interval, ProcessGenericOutbox_Time,
        MembershipDueReminder_Frequency, MembershipDueReminder_Interval, MembershipDueReminder_Time,
        PenaltyCalculation_Frequency, PenaltyCalculation_Interval, PenaltyCalculation_Time,
        ReallocateExpiredReservations_Frequency, ReallocateExpiredReservations_Interval, ReallocateExpiredReservations_Time,
        AllocateReservedBooks_Frequency, AllocateReservedBooks_Interval, AllocateReservedBooks_Time,
        DueDateReminder_Frequency, DueDateReminder_Interval, DueDateReminder_Time,
        NotifyReservationAllocation_Frequency, NotifyReservationAllocation_Interval, NotifyReservationAllocation_Time
    };
}