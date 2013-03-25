namespace WorkItemMigrator.Migration.Locators
{
    public interface IServiceLocatorSelector
    {
        IServiceLocator GetByName(string name);
    }
}
