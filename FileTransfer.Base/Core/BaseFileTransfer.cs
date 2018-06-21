using System;
using System.Net.Sockets;
using System.Text;

namespace FileTransfer.Base.Core
{
    public abstract class BaseFileTransfer
    {
        protected const int BUFFER_SIZE = 1024;

        public Encoding DefaultEncoding { get; } = Encoding.UTF8;

        public NetworkStream NetworkStream { get; protected set; }        
        
        protected sbyte ProgressPercentage(int value, int maxValue) => Convert.ToSByte(((float)value / maxValue) * 100);

    }
}
