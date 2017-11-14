using System.Collections.Generic;
using System.IO;
using BackupApp.Configuration;

namespace BackupApp.Backup
{
    internal static class DirectoryUtils
    { 
        public static long GetDirectorySize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            if (!d.Attributes.HasFlag(FileAttributes.Hidden))
            {
                FileInfo[] fis = d.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    size += fi.Length;
                }
                // Add subdirectory sizes.
                DirectoryInfo[] dis = d.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                    size += GetDirectorySize(di);
                }
            }

            return size;
        }
    }
}
