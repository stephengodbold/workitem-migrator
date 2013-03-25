using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace WorkItemMigrator.Migration.GraphTypes
{
    public class GraphWalker<T>
    {
        private readonly WalkingPath<T> _defaultPath;

        public GraphWalker(GraphNode<T> startNode)
        {
            _defaultPath = new WalkingPath<T>();
            _defaultPath.Step(startNode);
        }
        
        public WalkingPath<T> WalkToNode(T value)
        {
            var paths = new Collection<WalkingPath<T>> { _defaultPath };
            var startNode = _defaultPath.Path[0];

            if (startNode.Relatives.Count == 0) return _defaultPath;

            var currentPaths = new Collection<WalkingPath<T>>(paths.ToArray());

            while (currentPaths.Count(walk => walk.Path.Count(path => path.Relatives != null) > 0) > 0)
            {
                paths = new Collection<WalkingPath<T>>();

                foreach (var walkingPath in currentPaths)
                {
                    var currentNode = walkingPath.Path.Last();

                    foreach (var step in currentNode.Relatives)
                    {
                        var branch = new WalkingPath<T>(walkingPath);
                        branch.Step(step);

                        if (step.Value.Equals(value)) return branch;
                        if (branch.IsValid) { paths.Add(branch); } 
                    }
                }

                currentPaths = new Collection<WalkingPath<T>>(paths.ToArray());
            }

            throw new ArgumentOutOfRangeException("value", value, "The value was not found in the graph");
        }
    }
}
