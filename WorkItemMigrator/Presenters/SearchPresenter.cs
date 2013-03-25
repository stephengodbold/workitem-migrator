using System;
using Elmah;
using WebFormsMvp;
using WorkItemMigrator.Events;
using WorkItemMigrator.Messages;
using WorkItemMigrator.Migration;
using WorkItemMigrator.Models;
using WorkItemMigrator.Views;

namespace WorkItemMigrator.Presenters
{

    public class SearchPresenter : Presenter<ISearchView>
    {
        private readonly SearchHub searchHub;

        public SearchPresenter(
            ISearchView view,
            SearchHub searchHub)
            : base(view)
        {
            this.searchHub = searchHub;
            view.Search += Search;
            view.Load += delegate
                             {
                                 Messages.Subscribe<CancelMessage>(message => View.Model = new SearchModel());
                             };
        }

        private void Search(object sender, SearchEventArgs searchEventArgs)
        {
            try
            {
                var results = searchHub.Search(searchEventArgs.Criteria);

                Messages.Publish(new SearchResultMessage
                {
                    Items = results
                });
            }
            catch (ArgumentOutOfRangeException argEx)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(argEx));
                Messages.Publish(new StatusMessage
                {
                    Text = argEx.Message
                });
            }
        }
    }
}