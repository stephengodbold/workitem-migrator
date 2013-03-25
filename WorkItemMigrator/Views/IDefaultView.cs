using System;
using WebFormsMvp;
using WorkItemMigrator.Models;

namespace WorkItemMigrator.Views
{
    public interface IDefaultView : IView<DefaultModel>
    {
        event EventHandler Cancel;
    }
}
