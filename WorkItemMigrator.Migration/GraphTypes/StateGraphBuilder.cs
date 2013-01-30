using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WorkItemMigrator.Migration.GraphTypes
{
    public class StateGraphBuilder
    {
        public GraphNode<string> BuildStateGraph(XDocument workItemDefinition)
        {
            var stateCache = new Dictionary<string, GraphNode<string>>();

            var graphRootKey = string.Empty;
            var transitions = workItemDefinition.Descendants().Where(node => node.Name == "TRANSITION").ToArray();

            if (!transitions.Any()) return null;

            foreach (var vertice in transitions)
            {
                var sourceStateKey = ExtractAttribute(vertice, "from");
                var targetStateKey = ExtractAttribute(vertice, "to");

                var targetState = ParseGraphNode(targetStateKey, stateCache);

                if (string.IsNullOrWhiteSpace(sourceStateKey))
                {
                    graphRootKey = targetStateKey;
                    continue;
                }

                var sourceState = ParseGraphNode(sourceStateKey, stateCache);
                sourceState.AddRelative(targetState);
            }

            if (string.IsNullOrWhiteSpace(graphRootKey))
            {
                throw new ArgumentException(
                    "No root node was found in the graph. Work Item state graphs must have a root node", 
                    "workItemDefinition");
            }

            return stateCache[graphRootKey];
        }

        private string ExtractAttribute(XElement vertice, string attribute)
        {
            return vertice.Attributes()
                           .First(attr => attr.Name.LocalName.Equals(attribute, StringComparison.InvariantCultureIgnoreCase))
                           .Value;
        }

        private GraphNode<string> ParseGraphNode(string stateKey, Dictionary<string, GraphNode<string>> stateCache)
        {
            if (!stateCache.ContainsKey(stateKey))
            {
                stateCache.Add(stateKey, new GraphNode<string> { Value = stateKey });
            }

            return stateCache[stateKey];
        }

    }
}
