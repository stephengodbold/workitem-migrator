using System.Linq;

namespace WorkItemMigrator.Migration.GraphTypes
{
    public static class GraphExtensions
    {
        public static GraphNode<T> FindRelative<T>(this GraphNode<T> startNode, T value)
        {
            var walker = new GraphWalker<T>(startNode);
            var walkingPath = walker.WalkToNode(value);

            return walkingPath.Path.Last();
        }
    }
}
