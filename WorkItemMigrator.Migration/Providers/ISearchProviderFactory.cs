namespace WorkItemMigrator.Migration.Providers
{
    public interface ISearchProviderFactory
    {
        ISearchProvider GetByName(string name);
    }
}