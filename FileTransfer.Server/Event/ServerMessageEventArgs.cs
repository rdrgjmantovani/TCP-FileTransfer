using System;

namespace FileTransfer.Server.Event
{
    public class ServerMessageEventArgs : EventArgs
    {
        public string Message { get; }
        public MessageType MessageType { get; }

        public ServerMessageEventArgs(string message) =>
            Message = message;

        public ServerMessageEventArgs(string message, MessageType messageType) : this(message) =>
            MessageType = messageType;
    }
}
