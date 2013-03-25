using System;

namespace WorkItemMigrator.Events
{
    public class FetchEventArgs : EventArgs
    {
            public string ItemId { get; set; }
            public string ProviderName { get; set; }
    }
}