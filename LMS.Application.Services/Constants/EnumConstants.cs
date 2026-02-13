using LMS.Core.Enums;

namespace LMS.Application.Services.Constants;

internal static class StatusGroups
{
    #region Transaction Status Groups

    /// <summary>
    /// Transaction status groupings for books borrowed by users.
    /// </summary>
    public static class Transaction
    {
        /// <summary>
        /// Active transaction statuses where the book is still with the user.
        /// Used for counting books currently held by a user.
        /// </summary>
        /// <remarks>
        /// Includes: Borrowed, Renewed, Overdue
        /// </remarks>
        public static readonly long[] Active = new[]
        {
            (long)TransectionStatusEnum.Borrowed,
            (long)TransectionStatusEnum.Renewed,
            (long)TransectionStatusEnum.Overdue
        };

        /// <summary>
        /// Finalized transaction statuses where the book is no longer with the user.
        /// Used for excluding transactions from active counts and penalty calculations.
        /// </summary>
        /// <remarks>
        /// Includes: Returned, Cancelled, ClaimedLost
        /// </remarks>
        public static readonly long[] Finalized = new[]
        {
            (long)TransectionStatusEnum.Returned,
            (long)TransectionStatusEnum.Cancelled,
            (long)TransectionStatusEnum.ClaimedLost
        };

        /// <summary>
        /// Transaction statuses for books currently checked out (not overdue yet).
        /// Used when checking if user can perform actions like renewals.
        /// </summary>
        /// <remarks>
        /// Includes: Borrowed, Renewed
        /// </remarks>
        public static readonly long[] CheckedOut = new[]
        {
            (long)TransectionStatusEnum.Borrowed,
            (long)TransectionStatusEnum.Renewed
        };

        /// <summary>
        /// Transaction statuses that are relevant for penalty evaluation and updates.
        /// Used when processing penalty calculations or waiving penalties.
        /// </summary>
        /// <remarks>
        /// Includes: Renewed, Returned, ClaimedLost
        /// </remarks>
        public static readonly long[] PenaltyRelevant = new[]
        {
            (long)TransectionStatusEnum.Renewed,
            (long)TransectionStatusEnum.Returned,
            (long)TransectionStatusEnum.ClaimedLost
        };

        /// <summary>
        /// Transaction statuses that require a recorded return date.
        /// Used for validation when updating transaction status.
        /// </summary>
        /// <remarks>
        /// Includes: Returned, Cancelled
        /// </remarks>
        public static readonly long[] RequiringReturnDate = new[]
        {
            (long)TransectionStatusEnum.Returned,
            (long)TransectionStatusEnum.Cancelled
        };
    }

    #endregion

    #region Transaction Action Groups

    /// <summary>
    /// Transaction action groupings for operations performed on transactions.
    /// </summary>
    public static class TransactionAction
    {
        /// <summary>
        /// Actions that are invalid once a book has already been allocated or borrowed.
        /// Used for validation before allowing certain operations.
        /// </summary>
        /// <remarks>
        /// Includes: Cancel, Delete, Return
        /// </remarks>
        public static readonly TransectionActionEnum[] InvalidAfterAllocation = new[]
        {
            TransectionActionEnum.Cancel,
            TransectionActionEnum.Delete,
            TransectionActionEnum.Return
        };
    }

    #endregion

    #region Reservation Status Groups

    /// <summary>
    /// Reservation status groupings for book reservations.
    /// </summary>
    public static class Reservation
    {
        /// <summary>
        /// Active reservation statuses that are not yet fulfilled or cancelled.
        /// Used for counting user's active reservations and checking limits.
        /// </summary>
        /// <remarks>
        /// Includes: Reserved, Allocated
        /// </remarks>
        public static readonly long[] Active = new[]
        {
            (long)ReservationsStatusEnum.Reserved,
            (long)ReservationsStatusEnum.Allocated
        };

        /// <summary>
        /// Finalized reservation statuses that have reached terminal state.
        /// Used for excluding reservations from active processing.
        /// </summary>
        /// <remarks>
        /// Includes: Fulfilled, Cancelled
        /// </remarks>
        public static readonly long[] Finalized = new[]
        {
            (long)ReservationsStatusEnum.Fulfilled,
            (long)ReservationsStatusEnum.Cancelled
        };

        /// <summary>
        /// Reservation statuses that are no longer awaiting allocation.
        /// Used when checking if a reservation can transition to allocated state.
        /// </summary>
        /// <remarks>
        /// Includes: Fulfilled, Cancelled, Allocated
        /// </remarks>
        public static readonly long[] NonPendingAllocation = new[]
        {
            (long)ReservationsStatusEnum.Fulfilled,
            (long)ReservationsStatusEnum.Cancelled,
            (long)ReservationsStatusEnum.Allocated
        };
    }

    #endregion

    #region Reservation Action Groups

    /// <summary>
    /// Reservation action groupings for operations performed on reservations.
    /// </summary>
    public static class ReservationAction
    {
        /// <summary>
        /// Actions that require freeing allocated book resources.
        /// Used when a reservation with allocated status is being cancelled/deleted/transferred.
        /// </summary>
        /// <remarks>
        /// Includes: Cancel, Delete, Transfer
        /// </remarks>
        public static readonly ReservationActionEnum[] RequiringResourceRelease = new[]
        {
            ReservationActionEnum.Cancel,
            ReservationActionEnum.Delete,
            ReservationActionEnum.Transfer
        };
    }

    #endregion

    #region Book Status Groups

    /// <summary>
    /// Book status groupings for book availability and circulation.
    /// </summary>
    public static class Book
    {
        /// <summary>
        /// Book statuses where books are available for borrowing or reservation.
        /// Used for allowing users to reserve or borrow books.
        /// </summary>
        /// <remarks>
        /// Includes: Available, Reserved
        /// </remarks>
        public static readonly long[] AvailableForAction = new[]
        {
            (long)BookStatusEnum.Available,
            (long)BookStatusEnum.Reserved
        };

        /// <summary>
        /// Book statuses representing books currently in library circulation.
        /// Used when freeing resources after reservation cancellation/transfer.
        /// </summary>
        /// <remarks>
        /// Includes: Available, Reserved, CheckedOut
        /// </remarks>
        public static readonly long[] InCirculation = new[]
        {
            (long)BookStatusEnum.Available,
            (long)BookStatusEnum.Reserved,
            (long)BookStatusEnum.CheckedOut
        };

        /// <summary>
        /// Book statuses where books are unavailable for any action.
        /// Used for filtering out books that shouldn't appear in search results.
        /// </summary>
        /// <remarks>
        /// Includes: LostDamaged, Removed, OnHold
        /// </remarks>
        public static readonly long[] Unavailable = new[]
        {
            (long)BookStatusEnum.Lost_Damaged,
            (long)BookStatusEnum.Removed,
            (long)BookStatusEnum.OnHold
        };
    }

    #endregion

    #region Penalty Type Groups

    /// <summary>
    /// Penalty type groupings for different penalty scenarios.
    /// </summary>
    public static class PenaltyType
    {
        /// <summary>
        /// Penalty types related to holding books beyond allowed limits.
        /// Used when calculating penalties for over-holding scenarios.
        /// </summary>
        /// <remarks>
        /// Includes: BooksHeldUnderExpiredMembership, ExtraHoldings
        /// </remarks>
        public static readonly long[] HoldingRelated = new[]
        {
            (long)PenaltyTypeEnum.BooksHeldUnderExpiredMembership,
            (long)PenaltyTypeEnum.ExtraHoldings
        };

        /// <summary>
        /// Penalty types related to time-based violations (overdue, delays).
        /// Used when calculating time-sensitive penalties.
        /// </summary>
        /// <remarks>
        /// Includes: LateReturnRenew
        /// </remarks>
        public static readonly long[] TimeBasedViolations = new[]
        {
            (long)PenaltyTypeEnum.LateReturnRenew
        };

        /// <summary>
        /// Penalty types related to lost or damaged books.
        /// Used when handling book loss claims.
        /// </summary>
        /// <remarks>
        /// Includes: LostBook
        /// </remarks>
        public static readonly long[] AssetRelated = new[]
        {
            (long)PenaltyTypeEnum.LostBook
        };

        /// <summary>
        /// All penalty types that can be automatically calculated by the system.
        /// Used in scheduled penalty calculation jobs.
        /// </summary>
        /// <remarks>
        /// Includes: LateReturnRenew, BooksHeldUnderExpiredMembership, ExtraHoldings, LostBook
        /// </remarks>
        public static readonly long[] AutoCalculated = new[]
        {
            (long)PenaltyTypeEnum.LateReturnRenew,
            (long)PenaltyTypeEnum.BooksHeldUnderExpiredMembership,
            (long)PenaltyTypeEnum.ExtraHoldings,
            (long)PenaltyTypeEnum.LostBook
        };
    }

    #endregion

    #region Fine Status Groups

    /// <summary>
    /// Fine/Penalty status groupings.
    /// </summary>
    public static class FineStatus
    {

        /// <summary>
        /// Fine statuses that are settled and don't require further action.
        /// Used for filtering out resolved penalties.
        /// </summary>
        /// <remarks>
        /// Includes: Paid, Waived
        /// </remarks>
        public static readonly long[] Settled = new[]
        {
            (long)FineStatusEnum.Paid,
            (long)FineStatusEnum.Waived
        };
    }

    #endregion
}