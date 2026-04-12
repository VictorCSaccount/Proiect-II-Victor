namespace ProiectII.Models
{
    public enum AdoptionStatus : uint
    {
        Pending = 1,
        UnderReview = 2,
        InterviewScheduled = 3,
        Approved = 4,
        Completed = 5,
        Rejected = 6,
        CanceledByUser = 7,
        Archived = 8

    }
}
