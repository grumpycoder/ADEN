namespace ADEN.Web.Core
{
    public class OperationResult
    {
        public string Message { get; set; }
        public bool Success { get; set; }

        public OperationResult(string message, bool status = true)
        {
            Message = message;
            Success = status;
        }
    }
}