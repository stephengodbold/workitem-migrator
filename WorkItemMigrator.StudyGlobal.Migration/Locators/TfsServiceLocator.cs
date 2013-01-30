using System;
using System.Configuration;
using WorkItemMigrator.Migration.Locators;

namespace WorkItemMigrator.StudyGlobal.Migration.Locators
{
    public class TfsServiceLocator : IServiceLocator
    {
        public Uri Location { get; private set; }

        public TfsServiceLocator()
        {
            ReadSettings();
        }

        private void ReadSettings()
        {
            var appSettingsReader = new AppSettingsReader();
            Uri uri;
            Uri.TryCreate(((string)appSettingsReader.GetValue("ProjectCollectionUri", typeof (string))),
                          UriKind.RelativeOrAbsolute, 
                          out uri);

            Location = uri;
        }
    }
}