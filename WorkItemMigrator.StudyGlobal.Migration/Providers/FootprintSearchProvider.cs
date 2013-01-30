using WorkItemMigrator.Migration.Models;
using WorkItemMigrator.Migration.Providers;

namespace WorkItemMigrator.StudyGlobal.Migration.Providers
{
    public class FootprintSearchProvider : ISearchProvider
    {
        public string FriendlyName { get { return "Footprint"; } }
        public WorkItem Get(string id)
        {
            return new WorkItem();
        }
    }
}
