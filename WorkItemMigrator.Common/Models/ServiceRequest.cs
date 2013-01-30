using System;

namespace WorkItemMigrator.Common.Models
{
    public class ServiceRequest
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}