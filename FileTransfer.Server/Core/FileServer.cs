using FileTransfer.Server.Event;
using FileTransfer.Server.Model;
using FileTransfer.Server.Utils;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public enum MessageType
{
    OK,
    ERROR,
    ALERT,
}

namespace FileTransfer.Server.Core
{
    public class FileServer
    {
        public delegate void ServerMessageEventHandler(object sender, ServerMessageEventArgs e);
        public event ServerMessageEventHandler ServerMessageEvent;


        public delegate void FileStatusEventHandler(object sender, ServerMessageEventArgs e);
        public event FileStatusEventHandler FileStatusEvent;

        private const char SEPERATOR = '@';

        public const int DEFAULT_PORT = 4999;

        public TcpListener Listener { get; private set; }
        public TcpClient Client { get; private set; }

        public FileReceiver FileReceiver { get; private set; }

        public FileServer()
        {
            FolderUtil.SetupMainFolder();
            FolderUtil.SetupLogFolder();
        }

        public void StartListening()
        {
            Task.Run(async () =>
           {
               Listener = new TcpListener(IPAddress.Any, DEFAULT_PORT);
               Listener.Start(1);

               OnServerMessage($"Server Listening on Port {DEFAULT_PORT} ({DateTime.Now})", MessageType.ALERT);

               Client = await Listener.AcceptTcpClientAsync();

               FileReceiver = new FileReceiver(this, Client.GetStream());

               OnServerMessage($"Client Connected {DateTime.Now}", MessageType.OK);

               Listener.Stop();

               StartReceiving();
           });
        }

        public async void StartReceiving()
        {
            ServerTaskResult result = await FileReceiver.ReceiveFile();
           
            OnServerMessage(result.Message, result.MessageType);
            
            Client.Close();
            
            OnServerMessage($"Client disconnected ({DateTime.Now})", MessageType.ERROR);

            if (!result.Success)
                FileReceiver.DeleteFile();

            StartListening();
        }

        public void OnServerMessage(string message, MessageType type)
        {
            if (!string.IsNullOrEmpty(message))
                ServerMessageEvent?.Invoke(message, new ServerMessageEventArgs(message, type));
        }

        public void OnUpdateFileStatus(string message)
        {
            if (!string.IsNullOrEmpty(message))
                FileStatusEvent?.Invoke(this, new ServerMessageEventArgs(message));
        }


    }
}
