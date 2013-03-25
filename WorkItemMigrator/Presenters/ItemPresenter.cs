using System;
using System.Collections.Generic;
using Elmah;
using WebFormsMvp;
using WorkItemMigrator.Events;
using WorkItemMigrator.Messages;
using WorkItemMigrator.Migration;
using WorkItemMigrator.Migration.Models;
using WorkItemMigrator.Models;
using WorkItemMigrator.Services;
using WorkItemMigrator.Views;

namespace WorkItemMigrator.Presenters
{
    public class ItemPresenter : Presenter<IItemView>
    {
        private readonly IExtensionManager extensions;
        private readonly IRepositorySelector repositoryProvider;
        
        public ItemPresenter(
            IItemView view,
            IExtensionManager extensions,
            IRepositorySelector repositoryProvider)
            : base(view)
        {
            this.extensions = extensions;
            this.repositoryProvider = repositoryProvider;
            
            View.Model = new ItemModel { Item = new WorkItem(), Repositories = new Dictionary<string, string>() };

            view.Load += delegate
                             {
                                 Messages.Subscribe<FetchResultMessage>(ShowResult);
                             };

            view.Migrate += Migrate;
            view.Cancel += Cancel;
        }

        private void ShowResult(FetchResultMessage message)
        {
            View.Model = new ItemModel
                             {
                                 Item = message.Item,
                                 Repositories = extensions.Repositories,
                                 SearchProvider = message.Provider
                             };
        }

        private void Cancel(object sender, EventArgs e)
        {
            Messages.Publish(new CancelMessage());
        }

        private void Migrate(object sender, FetchResultEventArgs e)
        {
            var identifier = string.Empty;
            var sourceRepository = repositoryProvider.GetByName(e.SourceRepository);
            var targetRepository = repositoryProvider.GetByName(e.TargetRepository);

            try
            {
                var workItem = sourceRepository.Fetch(e.Id);
                identifier = targetRepository.MigrateTo(workItem);
            }
            catch (ArgumentOutOfRangeException argEx)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(argEx));
                Messages.Publish(new StatusMessage
                    {
                        Text = argEx.Message
                    });
            }
            catch (Exception ex)
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
                sourceRepository.MigrateFrom(e.Id, identifier, e.CloseItem, e.Comment);
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                Messages.Publish(new StatusMessage
                {
                    Text =
                        string.Format(
                            "Failed to close original item {0}. The item has been migrated as {1} and the exception has been logged.",
                            e.Id,
                            identifier)
                });

                return;
            }

            Messages.Publish(new StatusMessage { Text = string.Format("Successfully migrated item {0} as {1}", e.Id, identifier) });
        }
    }
}