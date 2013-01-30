using WebFormsMvp;
using WorkItemMigrator.Events;
using WorkItemMigrator.Messages;
using WorkItemMigrator.Migration.Providers;
using WorkItemMigrator.Models;
using WorkItemMigrator.Views;

namespace WorkItemMigrator.Presenters
{

    public class SearchPresenter : Presenter<ISearchView>
    {
        private readonly ISearchProviderFactory _searchProvider;
        private readonly IExtensionManager _extensionManager;

        public SearchPresenter(
            ISearchView view, 
            ISearchProviderFactory searchProvider,
            IExtensionManager extensionManager)
            : base(view)
        {
            _searchProvider = searchProvider;
            _extensionManager = extensionManager;

            View.Model.Providers = _extensionManager.SearchProviders;

            view.Search += Search;
            view.Load += delegate
                             {
                                 Messages.Subscribe<CancelMessage>(message => View.Model = new SearchModel());
                             };
        }

        private void Search(object sender, SearchEventArgs searchEventArgs)
        {
            var search = _searchProvider.GetByName(searchEventArgs.ProviderName);
            var result = search.Get(searchEventArgs.ItemId);

            Messages.Publish(new ResultMessage
                                 {
                                     Item = result,
                                     Provider = searchEventArgs.ProviderName
                                 });
        }
    }
}