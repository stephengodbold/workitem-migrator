using System;
using WebFormsMvp;
using WorkItemMigrator.Messages;
using WorkItemMigrator.Models;
using WorkItemMigrator.Views;
using Action = WorkItemMigrator.Models.Action;

namespace WorkItemMigrator.Presenters
{
    public class DefaultPresenter : Presenter<IDefaultView>
    {
        public DefaultPresenter(IDefaultView view)
            : base(view)
        {
            View.Model = new DefaultModel();

            view.Load += delegate { Messages.Subscribe<StatusMessage>(SetStatusMessage); };
            view.Cancel += Cancel;
        }

        private void SetStatusMessage(StatusMessage message)
        {
            View.Model.Message = message.Text;
        }

        private void Cancel(object sender, EventArgs eventArgs)
        {
            View.Model.Message = "Migration cancelled";
            View.Model.Mode = Action.Search;
        }
    }
}