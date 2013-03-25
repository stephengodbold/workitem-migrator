using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WorkItemMigrator.Migration.GraphTypes;
using WorkItemMigrator.Migration.Locators;
using WorkItem = WorkItemMigrator.Migration.Models.WorkItem;
using WorkItemType = WorkItemMigrator.Migration.Models.WorkItemType;

namespace WorkItemMigrator.Migration.TeamFoundation
{
    public class StudyGlobalRepository : IRepository
    {
        private const string TitleFormat = "[{0}] {1}";
        private const string ProjectName = "StudyGlobal";

        private readonly IServiceLocatorSelector serviceLocatorSelector;

        public string FriendlyName { get { return "Study Global"; } }

        public StudyGlobalRepository(IServiceLocatorSelector serviceLocatorSelector)
        {
            this.serviceLocatorSelector = serviceLocatorSelector;
        }

        public void MigrateFrom(string itemId, string migratedId, bool close, string comment)
        {
            var locator = serviceLocatorSelector.GetByName("TfsServiceLocator");
            using (var collection = new TfsTeamProjectCollection(locator.Location))
            {
                var workItemId = int.Parse(itemId);
                var workItemStore = new WorkItemStore(collection, WorkItemStoreFlags.BypassRules);
                var workItem = workItemStore.GetWorkItem(workItemId);
                var workItemDefinition = workItem.Store.Projects[workItem.Project.Name].WorkItemTypes[workItem.Type.Name];

                if (workItemDefinition == null)
                {
                    throw new ArgumentException("Could not obtain work item definition to close work item");
                }

                var definitionDocument = workItemDefinition.Export(false).InnerXml;
                var xDocument = XDocument.Parse(definitionDocument);
                var graphBuilder = new StateGraphBuilder();
                var stateGraph = graphBuilder.BuildStateGraph(xDocument);
                var currentStateNode = stateGraph.FindRelative(workItem.State);

                var graphWalker = new GraphWalker<string>(currentStateNode);
                var shortestWalk = graphWalker.WalkToNode("Closed");

                foreach (var step in shortestWalk.Path)
                {
                    workItem.State = step.Value;
                    workItem.Save();
                }

                var title = workItem.Fields[CoreField.Title].Value;

                workItem.Fields[CoreField.Title].Value = string.Format(TitleFormat, migratedId, title);
                workItem.Fields[CoreField.Description].Value = comment + "<br /><br />" + workItem.Fields[CoreField.Description].Value;

                workItem.Save();
            }
        }

        public string MigrateTo(WorkItem item)
        {
            throw new NotImplementedException("This repository is not valid for the migrate action");
        }

        public WorkItem Fetch(string itemId)
        {
            var tfsWorkItemProvider = new WorkItemProvider(serviceLocatorSelector);
            var workItem = tfsWorkItemProvider.Fetch(itemId);

            return workItem.Project.Name.Equals("StudyGlobal", StringComparison.InvariantCultureIgnoreCase) ? MapWorkItem(workItem) : null;
        }

        public IEnumerable<WorkItem> Search(string queryName)
        {
            var locator = serviceLocatorSelector.GetByName("TfsServiceLocator");
            using (var collection = new TfsTeamProjectCollection(locator.Location))
            {
                var workItemStore = collection.GetService<WorkItemStore>();
                var queryDefinition = (QueryDefinition)workItemStore.Projects[ProjectName].QueryHierarchy[queryName];

                if (queryDefinition.QueryType != QueryType.List)
                    throw new ArgumentOutOfRangeException("queryName", "Only list type queries are supported");

                var query = new Query(workItemStore,
                                      queryDefinition.QueryText,
                                      new Dictionary<string, string> { { "Project", ProjectName } });

                var results = query.RunQuery();

                foreach (var result in results.OfType<Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem>())
                {
                    yield return MapWorkItem(result);
                }
            }
        }

        private WorkItem MapWorkItem(Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem workItem)
        {
            var mappedItem = new WorkItem
            {
                Id = workItem.Id,
                Provider = FriendlyName,
                AssignedTo = workItem.Fields[CoreField.AssignedTo].Value.ToString(),
                Description = workItem.Fields[CoreField.Description].Value.ToString(),
                Title = workItem.Fields[CoreField.Title].Value.ToString(),
                Type =
                    (workItem.Type.Name == "Bug" ? WorkItemType.Bug : WorkItemType.Requirement),
                Url = workItem.Uri.ToString(),
                ExtendedProperties = new Dictionary<string, object>(),
                Migrated = false
            };

            mappedItem.ExtendedProperties.Add("Attachments", workItem.Attachments);
            mappedItem.ExtendedProperties.Add("Links", workItem.Links);

            if (workItem.Fields.Contains("StudyGlobal.Division"))
            {
                mappedItem.ExtendedProperties.Add("Division", workItem.Fields["StudyGlobal.Division"].Value);
            }

            if (workItem.Fields.Contains("Microsoft.VSTS.Scheduling.Size"))
            {
                mappedItem.ExtendedProperties.Add("Size", workItem.Fields["Microsoft.VSTS.Scheduling.Size"].Value);
            }

            if (workItem.Fields.Contains("StudyGlobal.PageCode"))
            {
                mappedItem.ExtendedProperties.Add("PageCode", workItem.Fields["StudyGlobal.PageCode"].Value);
            }

            return mappedItem;
        }

    }
}
