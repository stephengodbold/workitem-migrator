using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WorkItemMigrator.Migration.GraphTypes
{
    public class WalkingPath<T>
    {
        private readonly Collection<GraphNode<T>> _path;

        public Collection<GraphNode<T>> Path { get { return _path; } }
        public bool IsValid { get; private set; }

        public WalkingPath()
        {
            _path = new Collection<GraphNode<T>>();
            IsValid = true;
        }

        public WalkingPath(WalkingPath<T> walkingPath)
        {
            _path = new Collection<GraphNode<T>>();
            foreach (var step in walkingPath.Path)
            {
                _path.Add(step);
            }

            CheckValidity();
        }

        public void Step(GraphNode<T> nextStep)
        {
            _path.Add(nextStep);
            CheckValidity();
        }

        private void CheckValidity()
        {
            IsValid = _path.Distinct().Count() == Path.Count;
        }
    }
}