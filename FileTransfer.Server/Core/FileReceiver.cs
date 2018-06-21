using FileTransfer.Base.Core;
using FileTransfer.Base.Utils;
using FileTransfer.Server.Model;
using FileTransfer.Server.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FileTransfer.Server.Core
{
    public class FileReceiver : BaseFileTransfer
    {
        private const char SEPERATOR = '@';

        private const int TIMEOUT = 3000;

        public FileServer Server { get; }
        public StreamReader Reader { get; private set; }

        public string FilePath { get; private set; }

        public FileReceiver(FileServer server, NetworkStream networkStream)
        {
            Server = server ?? throw new ArgumentException($"{nameof(server)} can't be null");
            NetworkStream = networkStream ?? throw new ArgumentNullException($"{nameof(networkStream)} can't be null");
            NetworkStream.ReadTimeout = TIMEOUT;

            Reader = new StreamReader(NetworkStream, DefaultEncoding);
        }

        private FileData GetFileInfo()
        {
            string fileInfo = Reader.ReadLine();
            string[] info = fileInfo.Split(SEPERATOR);

            FileData fileData = new FileData(info[0], Convert.ToInt32(info[1]));

            Server.OnServerMessage($"Client is Sending {fileData.FileName} ({fileData.FileSize})", MessageType.ALERT);

            return fileData;
        }

        public async Task<ServerTaskResult> ReceiveFile()
        {
            return await Task.Factory.StartNew(() =>
            {              
                try
                {
                    FileData fileData = GetFileInfo();

                    byte[] receivedData = new byte[BUFFER_SIZE];

                    int receivedBytes = 0;
                    int totalBytes = 0;

                    FilePath = FolderUtil.GetFolderPath(fileData.FileName);

                    DeleteFile();

                    Stopwatch watch = Stopwatch.StartNew();
                    
                    using (var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
                    {
                        while (totalBytes != fileData.ByteSize)
                        {
                            receivedBytes = NetworkStream.Read(receivedData, 0, receivedData.Length);

                            fs.Write(receivedData, 0, receivedBytes);

                            totalBytes += receivedBytes;

                            string fileStatus = $"({NetworkUtil.GetFileSize(totalBytes)} / {fileData.FileSize}) {ProgressPercentage(totalBytes, fileData.ByteSize)}%";
                            Server.OnUpdateFileStatus(fileStatus);
                        }
                    }

                    watch.Stop();

                    int elapsedSeconds = (int)watch.ElapsedMilliseconds / 1000;

                    return new ServerTaskResult(true, $"\n{FilePath} [{elapsedSeconds}s] ({DateTime.Now})", MessageType.OK);
                }
                catch (Exception e)
                {
                    return new ServerTaskResult(false, e.Message, MessageType.ERROR);
                }
            });

        }

        public void DeleteFile()
        {
            if (!String.IsNullOrEmpty(FilePath) && File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
}
