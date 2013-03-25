using System;
using WebFormsMvp.Web;
using WorkItemMigrator.Events;
using WorkItemMigrator.Models;

namespace WorkItemMigrator.Views
{
    public partial class ResultsView : MvpUserControl<ResultsModel>, IResultsView
    {
        public event EventHandler<SearchResultEventArgs> Migrate;
        public void OnMigrate(SearchResultEventArgs e)
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
            OnMigrate(new SearchResultEventArgs
            {
                QueryName = Query.Value,
                SearchProvider = SearchProvider.Value,
                RepositoryName = Repository.SelectedValue,
                Comment = Message.Text,
                CloseItem = CloseItems.Checked
            });
        }

        protected void CancelHandler(object sender, EventArgs e)
        {
            OnCancel(e);
        }
    }
}