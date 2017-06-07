using System.IO;

namespace PocDatabase.Helper
{
    /// <summary>
    /// Helper to working with files
    /// </summary>
    internal static class FileHelper
    {
        /// <summary>
        /// Create the folder if not existing for a full file name
        /// </summary>
        /// <param name="filename">full path of the file</param>
        public static void CreateFolderIfNeeded(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string folder = Path.GetDirectoryName(filename);
                if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
        }
    }
}
