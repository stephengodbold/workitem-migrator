using System;
using WebFormsMvp;
using WorkItemMigrator.Events;
using WorkItemMigrator.Models;

namespace WorkItemMigrator.Views
{
    public interface IResultView : IView<ResultModel>
    {
        event EventHandler<SearchResultEventArgs> Migrate;
        event EventHandler Cancel;
    }
}