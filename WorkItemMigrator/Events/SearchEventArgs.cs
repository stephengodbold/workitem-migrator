using System;

namespace WorkItemMigrator.Events
{
    public class SearchEventArgs : EventArgs
    {
        public string Criteria { get; set; }
        public string ProviderName { get; set; }
    }
}