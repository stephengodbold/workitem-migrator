using System.Collections.Generic;

namespace WorkItemMigrator.Services
{
    public class ExtensionManager : IExtensionManager
    {
        public IDictionary<string, string> Repositories { get; set; }
    }

    public interface IExtensionManager
    {
        IDictionary<string, string> Repositories { get; set; }
    }
}