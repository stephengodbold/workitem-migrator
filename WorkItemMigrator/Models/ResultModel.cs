using System.Collections.Generic;
using WorkItemMigrator.Migration.Models;

namespace WorkItemMigrator.Models
{
    public class ResultModel
    {
        public WorkItem Item { get; set; }
        public IDictionary<string, string> Repositories { get; set; }
        public string SearchProvider { get; set; }
        public string Message { get; set; }
    }
}