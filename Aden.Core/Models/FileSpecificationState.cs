namespace Aden.Core.Models
{
    public enum FileSpecificationState : byte
    {
        NotStarted,
        Generated,
        Approved,
        SubmittedWithError,
        SubmittedWithSuccess,
        Completed
    }
}