using WorkItemMigrator.Migration.Models;
using WorkItemMigrator.Models;

namespace WorkItemMigrator.Messages
{
    public class ResultMessage
    {
        public WorkItem Item { get; set; }
        public string Provider { get; set; }
    }
}