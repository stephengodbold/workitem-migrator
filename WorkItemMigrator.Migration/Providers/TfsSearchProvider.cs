using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WorkItemMigrator.Migration.Locators;
using WorkItem = WorkItemMigrator.Migration.Models.WorkItem;
using WorkItemType = WorkItemMigrator.Migration.Models.WorkItemType;

namespace WorkItemMigrator.Migration.Providers
{
    public class TfsSearchProvider : ISearchProvider
    {
        private readonly TfsTeamProjectCollection _collection;

        public string FriendlyName { get { return "Team Foundation Server"; } }

        public TfsSearchProvider(IServiceLocatorFactory locatorFactory)
        {
            var collectionUri = locatorFactory.GetByName("TfsServiceLocator").Location;
            _collection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(collectionUri);
        }

        public WorkItem Get(string id)
        {
            int itemId;
            if (Int32.TryParse(id, out itemId))
            {
                var workItemStore = _collection.GetService<WorkItemStore>();
                return MapWorkItem(workItemStore.GetWorkItem(itemId));
            }

            throw new ArgumentException("The value provided for id was not a valid TFS work item Id");
        }

        private WorkItem MapWorkItem(Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem workItem)
        {
            var mappedItem = new WorkItem
            {
                Id = workItem.Id,
                AssignedTo = workItem.Fields[CoreField.AssignedTo].Value.ToString(),
                Description = workItem.Fields[CoreField.Description].Value.ToString(),
                Title = workItem.Fields[CoreField.Title].Value.ToString(),
                Type =
                    (workItem.Type.Name == "Bug" ? WorkItemType.Bug : WorkItemType.Requirement),
                Url = workItem.Uri.ToString(),
                ExtendedProperties =  new Dictionary<string, object>(),
                Migrated = false
            };

            mappedItem.ExtendedProperties.Add("Attachments", workItem.Attachments);
            mappedItem.ExtendedProperties.Add("Links", workItem.Links);
            mappedItem.ExtendedProperties.Add("Example.CustomField", workItem.Fields["Example.CustomField"].Value);
            
            return mappedItem;
        }
    }
}
