using System;
using WebFormsMvp.Web;
using WorkItemMigrator.Events;
using WorkItemMigrator.Models;

namespace WorkItemMigrator.Views
{
    public partial class SearchView : MvpUserControl<SearchModel>, ISearchView
    {
        public event EventHandler<SearchEventArgs> Search;

        public void OnSearch(SearchEventArgs e)
        {
            var handler = Search;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void SearchHandler(object sender, EventArgs e)
        {
            OnSearch(new SearchEventArgs
                         {
                             Criteria = Criteria.Text
                         });
        }
    }
}