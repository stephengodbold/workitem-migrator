using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WorkItemMigrator.Migration.Locators;
using WorkItem = WorkItemMigrator.Migration.Models.WorkItem;
using WorkItemType = WorkItemMigrator.Migration.Models.WorkItemType;

namespace WorkItemMigrator.Migration.TeamFoundation
{
    public class StudyGlobalDevRepository : IRepository
    {
        private readonly IServiceLocatorSelector serviceLocatorSelector;
     
        private const string WorkItemTitleFormat = "[{0}] {1}";
        private const string ProjectName = "StudyGlobalDev";
        private const string RequirementType = "Requirement";
        private const string BugType = "Bug";

        public string FriendlyName { get { return "Study Global Dev"; } }

        public StudyGlobalDevRepository(IServiceLocatorSelector serviceLocatorSelector)
        {
            this.serviceLocatorSelector = serviceLocatorSelector;
        }

        public IEnumerable<WorkItem> Search(string queryName)
        {
            var locator = serviceLocatorSelector.GetByName("TfsServiceLocator");
            using (var collection = new TfsTeamProjectCollection(locator.Location))
            {
                var workItemStore = collection.GetService<WorkItemStore>();
                                var queryDefinition = (QueryDefinition) workItemStore.Projects[ProjectName].QueryHierarchy[queryName];

                if (queryDefinition.QueryType != QueryType.List)
                    throw new ArgumentOutOfRangeException("queryName", "Only list type queries are supported");

                var query = new Query(workItemStore,
                                      queryDefinition.QueryText,
                                      new Dictionary<string, string> {{"Project", ProjectName}});

                var results = query.RunQuery();

                foreach (var result in results.OfType<Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem>())
                {
                    yield return MapWorkItem(result);
                }
            }
        }

        public string MigrateTo(WorkItem item)
        {
            var locator = serviceLocatorSelector.GetByName("TfsServiceLocator");
            using (var collection = new TfsTeamProjectCollection(locator.Location))
            {
                var workItemStore = collection.GetService<WorkItemStore>();
                var workItemType = workItemStore.Projects[ProjectName].WorkItemTypes[RequirementType];

                if (item.Type == WorkItemType.Bug)
                {
                    workItemType = workItemStore.Projects[ProjectName].WorkItemTypes[BugType];
                }

                var targetWorkItem = new Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem(workItemType);
                targetWorkItem.Fields[CoreField.AssignedTo].Value = item.AssignedTo;
                targetWorkItem.Fields[CoreField.Description].Value = item.Description;
                targetWorkItem.Fields[CoreField.Title].Value = String.Format(WorkItemTitleFormat, item.Id,
                                                                             item.Title);

                if (item.ExtendedProperties.ContainsKey("Division"))
                {
                    targetWorkItem.Fields["StudyGlobal.Division"].Value = item.ExtendedProperties["Division"].ToString();
                }

                if (item.ExtendedProperties.ContainsKey("Size"))
                {
                    targetWorkItem.Fields["Microsoft.VSTS.Scheduling.Size"].Value =
                        item.ExtendedProperties["Size"].ToString();
                }

                if (item.Type == WorkItemType.Bug)
                {
                    targetWorkItem.Fields["Microsoft.VSTS.TCM.ReproSteps"].Value = item.Description;
                    targetWorkItem.Fields["StudyGlobal.PageCode"].Value = item.ExtendedProperties["PageCode"] == null
                                                                                ? string.Empty
                                                                                : item.ExtendedProperties["PageCode"].ToString();
                }

                var attachments = item.ExtendedProperties["Attachments"] as AttachmentCollection;
                var attachmentsToClean = new List<string>();
                if (attachments != null)
                {
                    ShallowCloneAttachments(item, attachments, targetWorkItem, attachmentsToClean);
                }

                if (targetWorkItem.IsValid())
                {
                    targetWorkItem.Save();

                    foreach (var filePath in attachmentsToClean)
                    {
                        File.Delete(filePath);
                    }

                    return targetWorkItem.Id.ToString(CultureInfo.InvariantCulture);
                }

                throw new ArgumentException(
                    string.Format(
                        "The work item provided was not valid for migration. The following fields have issues: {0}",
                        targetWorkItem.Validate().Cast<string>()));
            }
        }

        private static void ShallowCloneAttachments(WorkItem item, AttachmentCollection attachments, Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem targetWorkItem,
                                                    ICollection<string> attachmentsToClean)
        {
            var downloadClient = new WebClient {UseDefaultCredentials = true};
            var tempDownloadPath = Path.GetTempPath();

            foreach (var existingAttachment in attachments.Cast<Attachment>())
            {
                var tempFile = Path.Combine(tempDownloadPath, existingAttachment.Name);
                downloadClient.DownloadFile(existingAttachment.Uri, tempFile);

                var attachmentComment = string.IsNullOrWhiteSpace(existingAttachment.Comment)
                                            ? existingAttachment.Comment
                                            : string.Format("Migrated from work item {0}", item.Id);
                var clonedAttachment = new Attachment(tempFile, attachmentComment);
                targetWorkItem.Attachments.Add(clonedAttachment);
                attachmentsToClean.Add(tempFile);
            }
        }

        public void MigrateFrom(string itemId, string migratedId, bool close, string comment)
        {
            var tfsWorkItemProvider = new WorkItemProvider(serviceLocatorSelector);
            var workItem = tfsWorkItemProvider.Fetch(itemId);

            var title = workItem.Fields[CoreField.Title].Value;

            workItem.Fields[CoreField.Title].Value = string.Format(WorkItemTitleFormat, migratedId, title);
            workItem.Fields[CoreField.Description].Value = comment + "<br /><br />" + workItem.Fields[CoreField.Description].Value;

            workItem.Save();
        }

        public WorkItem Fetch(string itemId)
        {
            var tfsWorkItemProvider = new WorkItemProvider(serviceLocatorSelector);
            var workItem = tfsWorkItemProvider.Fetch(itemId);

            return workItem.Project.Name.Equals(ProjectName, StringComparison.InvariantCultureIgnoreCase) ? MapWorkItem(workItem) : null;
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