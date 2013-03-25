using System.Collections.Generic;
using WorkItemMigrator.Migration.Models;

namespace WorkItemMigrator.Messages
{
    public class SearchResultMessage
    {
        public IEnumerable<WorkItem> Items { get; set; }
        public string Provider { get; set; }
    }
}