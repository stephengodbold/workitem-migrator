using System;
using System.Configuration;

namespace WorkItemMigrator.Migration.Locators
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