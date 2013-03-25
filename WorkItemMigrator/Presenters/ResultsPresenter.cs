using System;
using System.Collections.Generic;
using WebFormsMvp;
using WorkItemMigrator.Events;
using WorkItemMigrator.Messages;
using WorkItemMigrator.Migration;
using WorkItemMigrator.Migration.Models;
using WorkItemMigrator.Models;
using WorkItemMigrator.Views;

namespace WorkItemMigrator.Presenters
{
    public class ResultsPresenter : Presenter<IResultsView>
    {
        private readonly IRepositorySelector repositorySelector;

        public ResultsPresenter(
            IResultsView view,
            IRepositorySelector repositorySelector)
            : base(view)
        {
            this.repositorySelector = repositorySelector;

            View.Model = new ResultsModel
                             {
                                 Items = new[] {new WorkItem()},
                                 Repositories = new Dictionary<string, string>(),
                             };

            view.Load += delegate {
                                 Messages.Subscribe<SearchResultMessage>(ShowResults);
                             };

            view.Migrate += Migrate;
            view.Cancel += Cancel;
        }

        private void ShowResults(SearchResultMessage message)
        {
        }

        private void Cancel(object sender, EventArgs e)
        {
            Messages.Publish(new CancelMessage());
        }

        private void Migrate(object sender, SearchResultEventArgs e)
        {

        }
    }
}