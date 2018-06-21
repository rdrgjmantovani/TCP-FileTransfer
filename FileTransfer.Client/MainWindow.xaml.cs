using FileTransfer.Base.Utils;
using FileTransfer.Client.Event;
using FileTransfer.Client.ViewModel;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace FileTransfer.Client
{
    public partial class MainWindow : Window
    {        
        public IProgress<int> Progress { get; }

        public MainWindow()
        {
            InitializeComponent();

            Progress = new Progress<int>(value => pgsBar.Value++);

            ClientViewModel clientViewModel = new ClientViewModel(Progress);
            clientViewModel.ClientMessage += ShowErrorMessage;

            clientViewModel.ProgressBarUpdate += UpdateProgressBarMaximum;
            clientViewModel.ProgressBarUpdate += ResetProgressBar;

            this.DataContext = clientViewModel;
        }

        private void UpdateProgressBarMaximum(object sender, ProgressBarUpdateEventArgs e)
        {
            if (e.MaxValue.HasValue)
                pgsBar.Maximum = e.MaxValue.Value;
        }

        private void ResetProgressBar(object sener, ProgressBarUpdateEventArgs e)
        {
            if (e.ShouldReset)
                pgsBar.Value = 0;
        }
               
        private void ShowErrorMessage(object sender, ClientMessageEventArgs e) => MessageBox.Show(e.Message, "Client Error", MessageBoxButton.OK, MessageBoxImage.Error);

        private void NumericInputOnly_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, NetworkUtil.NUMERIC_INPUT_PATTERN);
        }
    }
}
