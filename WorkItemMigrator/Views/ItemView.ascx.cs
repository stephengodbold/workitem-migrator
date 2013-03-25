using System;
using WebFormsMvp.Web;
using WorkItemMigrator.Events;
using WorkItemMigrator.Models;

namespace WorkItemMigrator.Views
{
    public partial class ItemView : MvpUserControl<ItemModel>, IItemView
    {
        public event EventHandler<FetchResultEventArgs> Migrate;
        public void OnMigrate(FetchResultEventArgs e)
        {
            var handler = Migrate;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler Cancel;
        public void OnCancel(EventArgs e)
        {
            var handler = Cancel;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void MigrateHandler(object sender, EventArgs e)
        {
            OnMigrate(new FetchResultEventArgs
            {
                Id = WorkItemId.Value,
                SourceRepository = SearchProvider.Value,
                TargetRepository = Provider.SelectedValue,
                Comment = Message.Text,
                CloseItem = CloseItem.Checked
            });
        }

        protected void CancelHandler(object sender, EventArgs e)
        {
            OnCancel(e);   
        }
    }
}