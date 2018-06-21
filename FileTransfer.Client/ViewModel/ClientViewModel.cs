using FileTransfer.Base.Utils;
using FileTransfer.Client.Command;
using FileTransfer.Client.Core;
using FileTransfer.Client.Event;
using FileTransfer.Client.Model;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileTransfer.Client.ViewModel
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public delegate void ClientMessageEventHandler(object sender, ClientMessageEventArgs e);
        public event ClientMessageEventHandler ClientMessage;

        private void OnClientMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
                ClientMessage?.Invoke(this, new ClientMessageEventArgs(message));
        }

        public delegate void ProgressBarUpdateEventHandler(object sender, ProgressBarUpdateEventArgs e);
        public event ProgressBarUpdateEventHandler ProgressBarUpdate;

        public void OnProgressReport(ProgressBarUpdateEventArgs e)
        {
            if (e != null)
                ProgressBarUpdate?.Invoke(this, e);
        }

        private string _host;
        public string Host
        {
            get { return _host; }
            set
            {
                _host = value;
                OnPropertyChanged();
            }
        }

        private int _port;
        public int Port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged();
            }
        }


        public string FileName { get; private set; }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }

        private string _progressStatus;
        public string ProgressStatus
        {
            get { return _progressStatus; }
            set
            {
                _progressStatus = value;
                OnPropertyChanged();
            }
        }

        private string _speed;
        public string Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                OnPropertyChanged();
            }
        }

        private bool _sending;
        public bool Sending
        {
            get { return _sending; }
            set
            {
                _sending = value;
                CancelCommand.RaiseRequerySuggested();
                OnPropertyChanged();
            }
        }

        private const int DEFAULT_PORT = 4999;
        private const string DEFAULT_HOST = "127.0.0.1";

        private const string DEFAULT_MESSAGE = "No File Selected";
        private const int SEND_DELAY = 1500;

        public Client Client { get; private set; }
     
        public ICommand SetFilePathCommand { get; }
        public ICommand SendCommand { get; }
        public ICommand CancelCommand { get; }

        public MainWindow MainWindow { get; }
        public IProgress<int> Progress { get; }

        private CancellationTokenSource tokenSource;

        public ClientViewModel(IProgress<int> progress)
        {
            Progress = progress;

            Host = DEFAULT_HOST;
            Port = DEFAULT_PORT;

            FilePath = DEFAULT_MESSAGE;

            SetFilePathCommand = new RelayCommand(SetFilePath);
            SendCommand = new RelayCommand(Send, CanSend);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        private void SetFilePath(object parameter)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                FilePath = dialog.FileName;
                FileName = dialog.SafeFileName;
            }
        }

        private async void Send(object parameter)
        {
            Client = new Client(Host, Port);

            ClientTaskResult connectionResult = await Client.Connect();

            if (connectionResult.Success)
            {
                FileSender fileManager = new FileSender(this, Client.TcpClient.GetStream());

                tokenSource = new CancellationTokenSource();

                await Task.Delay(SEND_DELAY);

                ClientTaskResult result = await fileManager.SendFile(FilePath, FileName, Progress, tokenSource.Token);

                if (!result.Success)
                    OnClientMessage(result.Message);
            }
            else
                OnClientMessage(connectionResult.Message);
            
            Sending = false;
            ResetStatus();

            OnProgressReport(new ProgressBarUpdateEventArgs(true));
        }

        private void ResetStatus()
        {
            Speed = string.Empty;
            ProgressStatus = string.Empty;
        }
     
        private bool CanSend(object parameter)
        {
            return Regex.IsMatch(Host, NetworkUtil.IP_PATTERN) &&
                   HasFilePath() &&
                   File.Exists(FilePath) &&
                   !Sending;
        }

        private void Cancel(object parameter) =>
            tokenSource?.Cancel();

        private bool CanCancel(object parameter) => Sending;

        private bool HasFilePath() => FilePath != null && File.Exists(FilePath);

     

    }
}
