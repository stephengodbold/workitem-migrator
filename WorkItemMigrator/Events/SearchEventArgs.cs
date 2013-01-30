using System;

namespace WorkItemMigrator.Events
{
    public class SearchEventArgs : EventArgs
    {
        public string ItemId { get; set; }
        public string ProviderName { get; set; }
    }
}