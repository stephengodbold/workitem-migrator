using System;
using WebFormsMvp;
using WorkItemMigrator.Events;
using WorkItemMigrator.Models;

namespace WorkItemMigrator.Views
{
    public interface IItemView : IView<ItemModel>
    {
        event EventHandler<FetchResultEventArgs> Migrate;
        event EventHandler Cancel;
    }
}