using System.Collections.Generic;
using WorkItemMigrator.Migration.Locators;
using WorkItemMigrator.Migration.Models;
using WorkItemMigrator.Migration.Providers;

namespace WorkItemMigrator.StudyGlobal.Migration.Providers
{
    public class FootprintItemRepositoryProvider : IRepositoryProvider
    {
        private readonly IServiceLocator _locator;

        public string FriendlyName { get { return "Footprint eHelpdesk"; } }
        public IEnumerable<string> Targets { get; private set; }

        public FootprintItemRepositoryProvider(IServiceLocator locator)
        {
            _locator = locator;
            Targets = new[] {"Main"};
        }

        public WorkItem Get(string id)
        {
            throw new System.NotImplementedException();
        }

        public void Save(WorkItem item)
        {
            throw new System.NotImplementedException();
        }
    }
}