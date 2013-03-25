namespace WorkItemMigrator.Migration
{
    public interface IRepositorySelector
    {
        IRepository GetByName(string name);
    }
}

