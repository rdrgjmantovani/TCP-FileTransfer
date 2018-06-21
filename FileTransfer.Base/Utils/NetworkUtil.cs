using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace FileTransfer.Base.Utils
{
    public class NetworkUtil
    {
        private const string PUBLIC_IP_URL = @"http://bot.whatismyipaddress.com/";

        public const string IP_PATTERN = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";

        private const int MAX_PORT = 65535;

        public const string NUMERIC_INPUT_PATTERN = @"^[^0-9]+";
        private const string PORT_PATTERN = @"^\d{1,5}$";

        private const int ONE_KILOBYTE = 1024;
        private const int ONE_MEGABYTE = 1048576;
        private const int ONE_GIGABYTE = 1073741824;

        public static string GetSpeed(int bytesReceived)
        {
            if (bytesReceived < ONE_KILOBYTE)
                return $"{bytesReceived} B/s";

            else if (bytesReceived >= ONE_KILOBYTE && bytesReceived < ONE_MEGABYTE)
                return $"{bytesReceived / ONE_KILOBYTE}KB/s";

            else
                return $"{bytesReceived / ONE_MEGABYTE}MB/s";
        }

        public static string GetFileSize(int bytes)
        {
            float fBytes = bytes;

            if (bytes < ONE_KILOBYTE)
                return $"{fBytes} Bytes";

            else if (bytes >= ONE_KILOBYTE && bytes < ONE_MEGABYTE)
                return $"{fBytes / ONE_KILOBYTE} KB";

            else if (bytes >= ONE_MEGABYTE && bytes < ONE_GIGABYTE)
                return $"{fBytes / ONE_MEGABYTE} MB";
            else
                return $"{fBytes / ONE_GIGABYTE} GB";
        }

        public static string GetPublicAddress()
        {
            if (!Connected())
                return "Could not get public address. Check your internet connection";

            WebRequest request = WebRequest.Create(PUBLIC_IP_URL);
            WebResponse response = request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string address = reader.ReadToEnd();
                return $"Public Address: {address}";
            }

        }

        private static bool Connected()
        {
            Ping ping = new Ping();

            try
            {
                PingReply reply = ping.Send("www.google.com");
                return reply.Status == IPStatus.Success;
            }
            catch (PingException e)
            {
                return false;
            }

        }

        public static bool ValidPort(int port) =>
            port > 0 && port <= MAX_PORT && Regex.IsMatch(port.ToString(), PORT_PATTERN);
    }
}
