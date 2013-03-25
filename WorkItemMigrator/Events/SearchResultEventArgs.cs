using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkItemMigrator.Migration.Models;

namespace WorkItemMigrator.Events
{
    public class SearchResultEventArgs : EventArgs
    {
        public string SearchProvider { get; set; }
        public string RepositoryName { get; set; }
        public string QueryName  { get; set; }
        public string Comment { get; set; }
        public bool CloseItem { get; set; }
    }
}