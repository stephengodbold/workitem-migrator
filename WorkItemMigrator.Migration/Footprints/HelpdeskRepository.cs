using System;
using System.Linq;
using System.Xml.Linq;
using WorkItemMigrator.Migration.Models;

namespace WorkItemMigrator.Migration.Footprints
{
    //Re-add the interface when this is ready to use
    public class HelpdeskRepository // : IRepository 
    {
        public string FriendlyName { get { return "Study Group Helpdesk"; } }

        public WorkItem Fetch(string id)
        {
            var issue = GetIssueDetails("1", id);
            return ParseWorkItem(issue);
        }

        public void MigrateFrom(string itemId, string migratedId, bool close, string comment)
        {
            throw new NotImplementedException();
        }

        public string MigrateTo(WorkItem item)
        {
            throw new NotImplementedException();
        }

        private static WorkItem ParseWorkItem(string issue)
        {
            var document = XDocument.Parse(issue);

            return new WorkItem
                       {
                           Id = int.Parse(document.Elements().Descendants().Where(node => node.Name == "id").ToString())
                       };
        }

        public string GetIssueDetails(string projid, string issueid)
        {
            var proxy = new ServiceProxy();
            var issue = proxy.GetIssueDetails(
                @"staff\sgodbold",
                "",
                "RETURN_MODE => 'xml'",
                projid,
                issueid);

            return issue;
        }
    }
}
