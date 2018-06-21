namespace FileTransfer.Server.Model
{
    public class ServerTaskResult
    {
        public string Message { get; private set; }        
        public bool Success { get; private set; }        
        public MessageType MessageType { get; private set; }

        public ServerTaskResult(bool success) =>
            Success = success;
                  
        public ServerTaskResult(bool success, string message) : this(success) =>        
            Message = message;
                
        
        public ServerTaskResult(bool success, string message, MessageType messageType) : this(success, message) =>
              MessageType = messageType;

    }
}
