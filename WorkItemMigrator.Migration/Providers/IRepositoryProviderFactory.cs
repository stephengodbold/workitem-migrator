namespace WorkItemMigrator.Migration.Providers
{
    public interface IRepositoryProviderFactory
    {
        IRepositoryProvider GetByName(string name);
    }
}

