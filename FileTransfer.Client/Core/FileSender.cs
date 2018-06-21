using FileTransfer.Client.Model;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using FileTransfer.Client.ViewModel;
using FileTransfer.Client.Event;
using FileTransfer.Base.Core;
using FileTransfer.Base.Utils;

namespace FileTransfer.Client.Core
{    
    public class FileSender : BaseFileTransfer
    {        
        private const int TIMEOUT = 10000;
        
        public ClientViewModel ClientViewModel { get; }
        public StreamWriter Writer { get; private set; }

        public int LastSecondBytes { get; private set; }

        public FileSender(ClientViewModel clientViewModel, NetworkStream networkStream)
        {
            ClientViewModel = clientViewModel ?? throw new ArgumentNullException($"{nameof(clientViewModel)} can't be null");
            NetworkStream = networkStream ?? throw new ArgumentNullException($"{nameof(networkStream)} can't be null");
          
            Writer = new StreamWriter(NetworkStream, DefaultEncoding);
            NetworkStream.WriteTimeout = TIMEOUT;
            Writer.AutoFlush = true;
        }

        public async Task<ClientTaskResult> SendFile(string filePath, string fileName, IProgress<int> progress, CancellationToken token)
        {
            System.Timers.Timer speedTimer = SetupSpeedTimer();

            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] sendingBuffer = null;

                        int fileSize = (int)fs.Length;

                        Writer.WriteLine($"{fileName}@{fileSize}");

                        int packetsToSend = Convert.ToInt32(Math.Ceiling((double)fileSize / BUFFER_SIZE));
                        
                        int currentPacketSize;

                        ExecuteOnMainThread(() => ClientViewModel.OnProgressReport(new ProgressBarUpdateEventArgs(packetsToSend)));
                        ExecuteOnMainThread(() => ClientViewModel.Sending = true);

                        speedTimer.Start();

                        for (int i = 0; i < packetsToSend; i++)
                        {
                            token.ThrowIfCancellationRequested();

                            if (fileSize > BUFFER_SIZE)
                            {
                                currentPacketSize = BUFFER_SIZE;
                                fileSize -= currentPacketSize;
                            }
                            else
                                currentPacketSize = fileSize;

                            sendingBuffer = new byte[currentPacketSize];
                            fs.Read(sendingBuffer, 0, currentPacketSize);

                            NetworkStream.Write(sendingBuffer, 0, sendingBuffer.Length);
                            LastSecondBytes += currentPacketSize;

                            progress.Report(i);

                            ClientViewModel.ProgressStatus = $"{ProgressPercentage(i + 1, packetsToSend)}%";

                            token.ThrowIfCancellationRequested();
                        }
                    }

                    return new ClientTaskResult(true);
                }
                catch (Exception e)
                {                   
                    return new ClientTaskResult(false, e.Message);
                }
                finally
                {
                    speedTimer.Stop();
                }
            }, token);
        }

        private System.Timers.Timer SetupSpeedTimer()
        {
            System.Timers.Timer timer = new System.Timers.Timer(1000);

            timer.Elapsed += (sender, args) =>
            {
                string speed = NetworkUtil.GetSpeed(LastSecondBytes);
                ClientViewModel.Speed = speed;

                LastSecondBytes = 0;
            };

            return timer;
        }
        
        private void ExecuteOnMainThread(Action action)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                action.Invoke();
            });
        }      
    }
}
