using System;

namespace WorkItemMigrator.Events
{
    public class SearchResultEventArgs : EventArgs
    {
        public string SearchProvider { get; set; }
        public string RepositoryName { get; set; }        
        public string Id { get; set; }
        public string Comment { get; set; }
        public bool CloseItem { get; set; }
    }
}