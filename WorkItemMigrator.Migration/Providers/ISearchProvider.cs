using WorkItemMigrator.Migration.Models;

namespace WorkItemMigrator.Migration.Providers
{
    public interface ISearchProvider
    {
        string FriendlyName { get; }
        WorkItem Get(string id);
    }
}
