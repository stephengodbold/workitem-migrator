using System.Collections.Generic;

namespace WorkItemMigrator.Migration.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public string Provider { get; set; }
        public WorkItemType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string Url { get; set; }
        public bool Migrated { get; set; }
        public Dictionary<string, object> ExtendedProperties { get; set; }
    }
}