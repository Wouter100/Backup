using System.Configuration;

namespace BackupApp.Configuration
{
    internal class DirectoriesSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public DirectoryCollection Directories
        {
            get { return (DirectoryCollection)this[""]; }
            set { this[""] = value; }
        }
    }
}
