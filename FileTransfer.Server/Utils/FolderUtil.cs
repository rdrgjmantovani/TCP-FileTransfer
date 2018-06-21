using System;
using System.Globalization;
using System.IO;

namespace FileTransfer.Server.Utils
{
    public class FolderUtil
    {
        private const string MAIN_FOLDER = "Files";        
        private const string MONTH_PATTERN = "MMM";

        public const string LOG_FOLDER = "Log";

        public static void SetupMainFolder()
        {
            if (!Directory.Exists(MAIN_FOLDER))
                Directory.CreateDirectory(MAIN_FOLDER);
        }

        public static void SetupLogFolder()
        {
            if (!Directory.Exists(LOG_FOLDER))
                Directory.CreateDirectory(LOG_FOLDER);
        }

        public static string GetFolderPath(string fileName)
        {
            string currentMonth = DateTime.Now.ToString(MONTH_PATTERN, CultureInfo.CurrentCulture);
            int currentYear = DateTime.Now.Year;

            string folderName = $"{currentMonth} {currentYear}";

            string folderPath = $"{MAIN_FOLDER}/{folderName}";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return $"{folderPath}/{fileName}";
        }
    }
}
