using System;
using FileTransfer.Base.Utils;
using FileTransfer.Server.Core;
using FileTransfer.Server.Event;
using FileTransfer.Server.Log;

namespace FileTransfer.Server
{    
    class Program
    {
        static void Main(string[] args)
        {
            FileServer server = new FileServer();

            Console.WriteLine(NetworkUtil.GetPublicAddress());

            server.ServerMessageEvent += ShowServerMessage;
            server.FileStatusEvent += UpdateFileStatus;
        
            server.StartListening();

            string command = string.Empty;

            while (!command.Equals("exit"))
                command = Console.ReadLine();

        }

        private static void ShowServerMessage(object sender, ServerMessageEventArgs e)
        {
            switch (e.MessageType)
            {
                case MessageType.OK:
                    Console.ForegroundColor = ConsoleColor.Green;                    
                    Console.WriteLine(e.Message);                    
                    break;

                case MessageType.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine(e.Message);                    
                    break;

                case MessageType.ALERT:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(e.Message);
                    break;
            }

            Logger.Log(e.Message);
        }

        private static void UpdateFileStatus(object sender, ServerMessageEventArgs e)
        {
            int cursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, cursor);

            Console.ForegroundColor = ConsoleColor.DarkCyan;

            Console.Write(e.Message);
        }
    }
}
