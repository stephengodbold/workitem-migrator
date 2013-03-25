using System.Collections.Generic;

namespace WorkItemMigrator.Models
{
    public class SearchModel
    {
        public string Id { get; set; }
        public IDictionary<string, string> Providers { get; set; }
    }
}