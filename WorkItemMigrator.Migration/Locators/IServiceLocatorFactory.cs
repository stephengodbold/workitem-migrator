namespace WorkItemMigrator.Migration.Locators
{
    public interface IServiceLocatorFactory
    {
        IServiceLocator GetByName(string name);
    }
}
