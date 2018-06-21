using FileTransfer.Base.Utils;

namespace FileTransfer.Server.Model
{
    public class FileData
    {
        public string FileName { get; }
        public int ByteSize { get; }
        public string FileSize { get; }

        public FileData(string fileName, int byteSize)
        {
            FileName = fileName;
            ByteSize = byteSize;

            FileSize = NetworkUtil.GetFileSize(ByteSize);
        }
    }
}
