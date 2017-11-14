using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using BackupApp.Configuration;

namespace BackupApp.Backup
{
    internal class BackupMaker
    {
        private string backupDirectory;
        private Action<string> logger;
        private bool cancelRequested;

        public BackupMaker(Action<string> logger)
        {
            this.logger = logger;
        }

        public void Start(string backupDir)
        {
            logger("Start backup naar " + backupDir);
            backupDirectory = backupDir;
            Task.Run(() => DoBackup());
        }

        public void Cancel()
        {
            cancelRequested = true;
        }

        private bool CheckSpace(DirectoryCollection dirs)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(backupDirectory);
            long freeSpace = -1;
            foreach (var item in DriveInfo.GetDrives())
            {
                if (item.RootDirectory.FullName == dirInfo.Root.FullName)
                {
                    freeSpace = item.TotalFreeSpace;
                }
            }

            if (freeSpace == -1)
            {
                return false;
            }

            long totalRequired = 0;
            long totalOld = 0;
            foreach (DirectoryElement item in dirs)
            {
                try
                {
                    long size = DirectoryUtils.GetDirectorySize(new DirectoryInfo(item.Path));
                    totalRequired += size;
                    float roundedSize = (float)size / (float)1024 / (float)1024;
                    logger(item.Name + " - " + roundedSize.ToString("F") + " MB");
                }
                catch (DirectoryNotFoundException ex)
                {
                    logger(ex.Message);
                }

                try
                {
                    long size = DirectoryUtils.GetDirectorySize(new DirectoryInfo(backupDirectory + "\\" + item.Name));
                    totalOld += size;
                }
                catch (DirectoryNotFoundException ex)
                {
                    logger(ex.Message);
                }
            }

            freeSpace += totalOld;
            logger("ruimte nodig: " + ((float)totalRequired / (float)1024 / (float)1024).ToString("F") + " MB - vrij: " + ((float)freeSpace / (float)1024 / (float)1024).ToString("F") + " MB");
            return (freeSpace >= totalRequired);
        }

        private void DoBackup()
        {
            DirectoryCollection dirs = ((DirectoriesSection)ConfigurationManager.GetSection("directories")).Directories;
            if (CheckSpace(dirs))
            {
                foreach (DirectoryElement item in dirs)
                {
                    if (cancelRequested)
                    {
                        return;
                    }

                    string backupPath = backupDirectory + "\\" + item.Name;
                    if (Directory.Exists(backupPath))
                    {
                        logger("Verwijderen oude backup folder " + backupPath);
                        Directory.Delete(backupPath, true);
                    }

                    CopyDirectory(new DirectoryInfo(item.Path), backupPath);
                }
            }
            else
            {
                
            }

            logger("Klaar!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }

        private void CopyDirectory(DirectoryInfo sourceDir, string destinationPath)
        {
            if (cancelRequested)
            {
                return;
            }

            logger("Aanmaken nieuwe backup folder " + destinationPath);
            Directory.CreateDirectory(destinationPath);
            foreach (DirectoryInfo subDir in sourceDir.GetDirectories())
            {
                if (!subDir.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    CopyDirectory(subDir, destinationPath + "\\" + subDir.Name);
                }
            }

            foreach (FileInfo item in sourceDir.GetFiles("*", SearchOption.TopDirectoryOnly))
            {
                if (cancelRequested)
                {
                    return;
                }

                if (!(item.Attributes.HasFlag(FileAttributes.Hidden) || item.Attributes.HasFlag(FileAttributes.Hidden)))
                {
                    string destinationFile = destinationPath + "\\" + item.Name;
                    logger("Kopiëren bestand " + destinationFile);
                    item.CopyTo(destinationFile);
                }
            }
        }
    }
}
