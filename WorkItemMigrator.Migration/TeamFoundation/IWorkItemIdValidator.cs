namespace WorkItemMigrator.Migration.TeamFoundation
{
    public interface IWorkItemIdValidator
    {
        bool IsValid(string id);
    }
}