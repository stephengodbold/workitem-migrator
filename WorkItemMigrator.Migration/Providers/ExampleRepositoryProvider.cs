using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WorkItemMigrator.Migration.GraphTypes;
using WorkItemMigrator.Migration.Locators;
using WorkItem = WorkItemMigrator.Migration.Models.WorkItem;
using WorkItemType = WorkItemMigrator.Migration.Models.WorkItemType;

namespace WorkItemMigrator.Migration.Providers
{
    public class ExampleRepositoryProvider : IRepositoryProvider
    {
        private const string WorkItemTitleFormat = "[{0}] {1}";
        private const string ProjectName = "ProjectName";
        private const string RequirementType = "Requirement";
        private const string BugType = "Bug";

        private readonly IServiceLocator locator;

        public ExampleRepositoryProvider()
        {
            locator = new TfsServiceLocator();
        }

        public string FriendlyName { get { return "Example Provider"; } }

        public void Migrate(WorkItem item)
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
                    targetWorkItem.Fields["Example.CustomField"].Value = item.ExtendedProperties["Example.CustomField"] == null ? string.Empty : item.ExtendedProperties["PageCode"].ToString();
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

                        var attachmentComment = string.IsNullOrWhiteSpace(existingAttachment.Comment)
                                          ? existingAttachment.Comment
                                          : string.Format("Migrated from work item {0}", item.Id);
                        var clonedAttachment = new Attachment(tempFile, attachmentComment);
                        targetWorkItem.Attachments.Add(clonedAttachment);
                        attachmentsToClean.Add(tempFile);
                    }
                }

                var links = item.ExtendedProperties["Links"] as LinkCollection;
                if (links != null)
                {
                    foreach (var linkCopy in links.Cast<Link>()
                                        .Select(link => CreateShallowCopy(link, workItemStore))
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

        public void Close(string itemId, string comment)
        {
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

                workItem.Fields[CoreField.Description].Value = comment + "<br /><br/>" + workItem.Fields[CoreField.Description].Value;
                workItem.Save();
            }
        }

        private Link CreateShallowCopy(Link link, WorkItemStore store)
        {
            if (link is Hyperlink)
            {
                var hyperLink = link as Hyperlink;
                return new Hyperlink(hyperLink.Location) { Comment = hyperLink.Comment };
            }

            var linkType = store.WorkItemLinkTypes["Microsoft.VSTS.Common.Affects"];
            var linkTypeEnd = store.WorkItemLinkTypes.LinkTypeEnds[linkType.ReverseEnd.Name];

            if (link is RelatedLink)
            {
                var relatedLink = link as RelatedLink;
                return new RelatedLink(
                       linkTypeEnd,
                       relatedLink.RelatedWorkItemId
                    ) { Comment = relatedLink.Comment };
            }

            if (link is ExternalLink)
            {
                var externalLink = link as ExternalLink;
                return new ExternalLink(
                    externalLink.ArtifactLinkType,
                    externalLink.LinkedArtifactUri
                    ) { Comment = externalLink.Comment };
            }

            return null;
        }

    }
}