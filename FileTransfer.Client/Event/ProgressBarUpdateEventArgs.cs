using System;

namespace FileTransfer.Client.Event
{
    public class ProgressBarUpdateEventArgs : EventArgs
    {      
        public int? MaxValue { get; }
        public bool ShouldReset { get; }


        public ProgressBarUpdateEventArgs(int maxValue) =>        
            MaxValue = maxValue;


        public ProgressBarUpdateEventArgs(bool shouldReset) =>
            ShouldReset = shouldReset;      
    }
}
