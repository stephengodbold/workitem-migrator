using System;
using System.Collections.Generic;
using Elmah;
using WebFormsMvp;
using WorkItemMigrator.Events;
using WorkItemMigrator.Messages;
using WorkItemMigrator.Migration.Models;
using WorkItemMigrator.Migration.Providers;
using WorkItemMigrator.Models;
using WorkItemMigrator.Views;

namespace WorkItemMigrator.Presenters
{
    public class ResultPresenter : Presenter<IResultView>
    {
        private readonly IExtensionManager _extensions;
        private readonly ISearchProviderFactory _searchProviderFactory;
        private readonly IRepositoryProviderFactory _repositoryProviderFactory;
        
        public ResultPresenter(
            IResultView view,
            IExtensionManager extensions,
            ISearchProviderFactory searchProviderFactory,
            IRepositoryProviderFactory repositoryProviderFactory) 
            : base(view)
        {
            _extensions = extensions;
            _searchProviderFactory = searchProviderFactory;
            _repositoryProviderFactory = repositoryProviderFactory;
            
            View.Model = new ResultModel { Item = new WorkItem(), Repositories = new Dictionary<string, string>() };

            view.Load += delegate
                             {
                                 Messages.Subscribe<ResultMessage>(ShowResult);
                             };

            view.Migrate += Migrate;
            view.Cancel += Cancel;
        }

        private void ShowResult(ResultMessage message)
        {
            View.Model = new ResultModel
                             {
                                 Item = message.Item,
                                 Repositories = _extensions.Repositories,
                                 SearchProvider = message.Provider
                             };
        }

        private void Cancel(object sender, EventArgs e)
        {
            Messages.Publish(new CancelMessage());
        }

        private void Migrate(object sender, SearchResultEventArgs e)
        {

            var repository = _repositoryProviderFactory.GetByName(e.RepositoryName);

            try
            {
                var searchProvider = _searchProviderFactory.GetByName(e.SearchProvider);
                var workItem = searchProvider.Get(e.Id);
                
                repository.Migrate(workItem);
            } catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                Messages.Publish(new StatusMessage
                                                    {
                                                        Text =
                                                            string.Format(
                                                                "Failed to save item {0}. The exception has been logged.",
                                                                e.Id)
                                                    });

                return;
            }

            try
            {
                if (e.CloseItem)
                {
                    repository.Close(e.Id, e.Comment);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                Messages.Publish(new StatusMessage
                {
                    Text =
                        string.Format(
                            "Failed to close original item {0}. The exception has been logged.",
                            e.Id)
                });

                return;
            }

            Messages.Publish(new StatusMessage { Text = string.Format("Successfully migrated item {0}", e.Id)});
        }
    }
}