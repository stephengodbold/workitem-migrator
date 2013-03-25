using WorkItemMigrator.Migration.Models;

namespace WorkItemMigrator.Messages
{
    public class FetchResultMessage
    {
        public WorkItem Item { get; set; }
        public string Provider { get; set; }
    }
}