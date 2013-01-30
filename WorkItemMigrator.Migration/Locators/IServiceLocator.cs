using System;

namespace WorkItemMigrator.Migration.Locators
{
    public interface IServiceLocator
    {
        Uri Location { get; }
    }
}
