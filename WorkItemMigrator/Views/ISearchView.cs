using System;
using WebFormsMvp;
using WorkItemMigrator.Events;
using WorkItemMigrator.Models;

namespace WorkItemMigrator.Views
{
    public interface ISearchView : IView<SearchModel>
    {
        event EventHandler<SearchEventArgs> Search;
    }
}