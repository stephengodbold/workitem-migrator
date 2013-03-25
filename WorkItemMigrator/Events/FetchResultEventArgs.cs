using System;

namespace WorkItemMigrator.Events
{
    public class FetchResultEventArgs : EventArgs
    {
        public string SourceRepository { get; set; }
        public string TargetRepository { get; set; }
        public string Id { get; set; }
        public string Comment { get; set; }
        public bool CloseItem { get; set; }
    }
}