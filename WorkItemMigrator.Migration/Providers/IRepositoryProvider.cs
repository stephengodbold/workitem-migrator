using WorkItemMigrator.Migration.Models;

namespace WorkItemMigrator.Migration.Providers
{
    public interface IRepositoryProvider
    {
        string FriendlyName { get; }

        void Migrate(WorkItem item);

        void Close(string itemId, string comment);
    }
}