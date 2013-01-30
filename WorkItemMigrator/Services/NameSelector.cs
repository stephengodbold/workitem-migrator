using Castle.Facilities.TypedFactory;

namespace WorkItemMigrator.Services
{
    internal class NameSelector : DefaultTypedFactoryComponentSelector
    {
        protected override string GetComponentName(System.Reflection.MethodInfo method, object[] arguments)
        {
            return arguments[0].ToString();
        }
    }
}