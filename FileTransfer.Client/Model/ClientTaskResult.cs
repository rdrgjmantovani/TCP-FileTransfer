namespace FileTransfer.Client.Model
{
    public class ClientTaskResult
    {
        public bool Success { get; }
        public string Message { get; }

        public ClientTaskResult(bool success) =>
            Success = success;

        public ClientTaskResult(bool success, string message) : this(success) =>
            Message = message;
    }
}
