using FileTransfer.Server.Utils;
using System;
using System.IO;

namespace FileTransfer.Server.Log
{
    public class Logger
    {
        public static void Log( string logMessage)
        {
            DateTime date = DateTime.Now;

            string fileName = $"log {date.Day}-{date.Month}-{date.Year}";

            using(var writer = File.AppendText($"{FolderUtil.LOG_FOLDER}/{fileName}.txt"))            
                writer.WriteLine(logMessage);
                        
        }
    }
}
