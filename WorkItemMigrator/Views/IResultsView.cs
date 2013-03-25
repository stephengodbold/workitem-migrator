using System;
using WebFormsMvp;
using WorkItemMigrator.Events;
using WorkItemMigrator.Models;

namespace WorkItemMigrator.Views
{
    public interface IResultsView : IView<ResultsModel>
    {
        event EventHandler<SearchResultEventArgs> Migrate;
        event EventHandler Cancel;
    }
}