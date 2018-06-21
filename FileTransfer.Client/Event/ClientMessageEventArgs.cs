using System;

namespace FileTransfer.Client.Event
{
    public class ClientMessageEventArgs : EventArgs
   {
        public string Message { get; }

        public ClientMessageEventArgs(string message) =>
            Message = message;        
    }
}
