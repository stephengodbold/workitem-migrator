using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WorkItemMigrator.Migration.GraphTypes
{
    public class GraphNode<T> : IEquatable<GraphNode<T>>
    {
        public T Value { get; set; }
        public ICollection<GraphNode<T>> Relatives { get; private set; }

        public GraphNode()
        {
            Relatives = new Collection<GraphNode<T>>();
        }

        public void AddRelative(GraphNode<T> relative)
        {
            Relatives.Add(relative);
        }

        public bool Equals(GraphNode<T> other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return ReferenceEquals(Value, null) ? 0 : Value.GetHashCode();
        }
    }
}