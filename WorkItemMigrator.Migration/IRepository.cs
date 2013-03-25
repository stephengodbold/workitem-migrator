using System.Collections.Generic;
using WorkItemMigrator.Migration.Models;

namespace WorkItemMigrator.Migration
{
    public interface IRepository
    {
        string FriendlyName { get; }

        WorkItem Fetch(string itemId);

        IEnumerable<WorkItem> Search(string query);

        void MigrateFrom(string itemId, string migratedId, bool close, string comment);

        string MigrateTo(WorkItem item);
    }
}