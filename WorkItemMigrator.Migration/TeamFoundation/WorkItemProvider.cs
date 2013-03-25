using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WorkItemMigrator.Migration.Locators;

namespace WorkItemMigrator.Migration.TeamFoundation
{
    public class WorkItemProvider 
    {
        private readonly TfsTeamProjectCollection collection;

        public string FriendlyName { get { return "Team Foundation Server"; } }

        public WorkItemProvider(IServiceLocatorSelector locatorSelector)
        {
            var collectionUri = locatorSelector.GetByName("TfsServiceLocator").Location;
            collection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(collectionUri);
        }

        public WorkItem Fetch(string id)
        {
            int itemId;
            if (Int32.TryParse(id, out itemId))
            {
                var workItemStore = collection.GetService<WorkItemStore>();
                return workItemStore.GetWorkItem(itemId);
            }

            throw new ArgumentException("The value provided for id was not a valid TFS work item Id");
        }

    }
}
