using FileTransfer.Client.Model;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FileTransfer.Client
{   
    public class Client
    {      
        public string Host { get; }
        public int Port { get; }

        public TcpClient TcpClient { get; private set; }      
                          
        public Client( string host, int port) 
        {          
            Host = host;
            Port = port;
        }
                
        public async Task<ClientTaskResult> Connect()
        {
            try
            {
                TcpClient = new TcpClient();
                await TcpClient.ConnectAsync(Host, Port);
                                
                return new ClientTaskResult(true);
            }
            catch (Exception e)
            {
                return new ClientTaskResult(false, e.Message);
            }        
        }            
    }
}
