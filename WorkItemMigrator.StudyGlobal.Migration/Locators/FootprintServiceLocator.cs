using System;
using System.Configuration;
using WorkItemMigrator.Migration.Locators;

namespace WorkItemMigrator.StudyGlobal.Migration.Locators
{
    public class FootprintServiceLocator : IServiceLocator
    {
        public Uri Location { get; private set; }

        public FootprintServiceLocator()
        {
            ReadSettings();
        }

        private void ReadSettings()
        {
            var appSettingsReader = new AppSettingsReader();

            Uri uri;
            //Uri.TryCreate(((string)appSettingsReader.GetValue("HelpdeskUri", typeof(string))),
            //              UriKind.RelativeOrAbsolute,
            //              out uri);

            Location = new Uri("http://www.google.com");
        }
    }
}