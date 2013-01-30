using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WorkItemMigrator.Migration.Locators;
using WorkItemMigrator.Migration.Providers;
using WorkItemMigrator.StudyGlobal.Migration.Locators;
using WorkItem = WorkItemMigrator.Migration.Models.WorkItem;
using WorkItemType = WorkItemMigrator.Migration.Models.WorkItemType;

namespace WorkItemMigrator.StudyGlobal.Migration.Providers
{
    public class StudyGlobalDevRepositoryProvider : IRepositoryProvider
    {
        private const string WorkItemTitleFormat = "[{0}] {1}";
        private const string ProjectName = "StudyGlobalDev";
        private const string RequirementType = "Requirement";
        private const string BugType = "Bug";

        private readonly IServiceLocator locator;

        public StudyGlobalDevRepositoryProvider()
        {
            locator = new TfsServiceLocator();
        }

        public string FriendlyName { get { return "StudyGlobalDev"; } }

        public IEnumerable<string> Targets { get; private set; }

        public void Save(WorkItem item)
        {
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

                if (item.Type == WorkItemType.Bug)
                {
                    targetWorkItem.Fields["Microsoft.VSTS.TCM.ReproSteps"].Value = item.Description;
                    targetWorkItem.Fields["StudyGlobal.PageCode"].Value = item.ExtendedProperties["PageCode"] == null ? string.Empty : item.ExtendedProperties["PageCode"].ToString();
                }

                var attachments = item.ExtendedProperties["Attachments"] as AttachmentCollection;
                var attachmentsToClean = new List<string>();
                if (attachments != null)
                {
                    var downloadClient = new WebClient { UseDefaultCredentials = true };
                    var tempDownloadPath = Path.GetTempPath();

                    foreach (var existingAttachment in attachments.Cast<Attachment>())
                    {
                        var tempFile = Path.Combine(tempDownloadPath, existingAttachment.Name);
                        downloadClient.DownloadFile(existingAttachment.Uri, tempFile);

                        var comment = string.IsNullOrWhiteSpace(existingAttachment.Comment)
                                          ? existingAttachment.Comment
                                          : string.Format("Migrated from work item {0}", item.Id);
                        var clonedAttachment = new Attachment(tempFile, comment);
                        targetWorkItem.Attachments.Add(clonedAttachment);
                        attachmentsToClean.Add(tempFile);
                    }
                }

                var links = item.ExtendedProperties["Links"] as LinkCollection;
                if (links != null)
                {
                    foreach (var linkCopy in links.Cast<Link>()
                                        .Select(link => CreateShallowCopy(link, item.Id, workItemStore))
                                        .Where(link => link != null))
                    {
                        targetWorkItem.Links.Add(linkCopy);
                        targetWorkItem.Links.Add(new RelatedLink(item.Id));
                    }
                }

                if (targetWorkItem.IsValid())
                {
                    targetWorkItem.Save();

                    foreach (var filePath in attachmentsToClean)
                    {
                        File.Delete(filePath);
                    }

                    return;
                }

                throw new ArgumentException(
                    string.Format(
                        "The work item provided was not valid for migration. The following fields have issues: {0}",
                        targetWorkItem.Validate().Cast<string>()));
            }
        }

        private Link CreateShallowCopy(Link link, int sourceWorkItemId, WorkItemStore store)
        {
            if (link is Hyperlink)
            {
                var hyperLink = link as Hyperlink;
                return new Hyperlink(hyperLink.Location)
                               { Comment = hyperLink.Comment };
            }

            var linkType = store.WorkItemLinkTypes["Microsoft.VSTS.Common.Affects"];
            var linkTypeEnd = store.WorkItemLinkTypes.LinkTypeEnds[linkType.ReverseEnd.Name];

            if (link is RelatedLink)
            {
                var relatedLink = link as RelatedLink;
                return new RelatedLink(
                       linkTypeEnd,
                       relatedLink.RelatedWorkItemId
                    ) {Comment = relatedLink.Comment};
            }

            if (link is ExternalLink)
            {
                var externalLink = link as ExternalLink;
                return new ExternalLink(
                    externalLink.ArtifactLinkType,
                    externalLink.LinkedArtifactUri
                    ) {Comment = externalLink.Comment};
            }

            return null;
        }
    }
}