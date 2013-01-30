using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkItemMigrator.Migration.GraphTypes;

namespace WorkItemMigrator.Tests.GraphTests
{
    [TestClass]
    public class GraphBuilderTests
    {
        [TestMethod]
        public void Null_From_Starting_Transition_Returns_A_Node_With_No_Vertices()
        {
            var graphBuilder = new StateGraphBuilder();
            var definitionContent =
                new XElement("TRANSITION",
                    new XAttribute("from", string.Empty),
                    new XAttribute("to", "Proposed"));
            var definition = new XDocument(definitionContent);
            
            var graph = graphBuilder.BuildStateGraph(definition);

            Assert.AreEqual(0, graph.Relatives.Count);
        }

        [TestMethod]
        public void Single_Transition_Results_In_Two_Nodes()
        {
            var graphBuilder = new StateGraphBuilder();
            var definitionContent =
                new XElement("TRANSITIONS",
                     new XElement("TRANSITION",
                        new XAttribute("from", string.Empty),
                        new XAttribute("to", "Proposed")),
                    new XElement("TRANSITION",
                        new XAttribute("from", "Proposed"),
                        new XAttribute("to", "Ready for Development")));
            var definition = new XDocument(definitionContent);

            var graph = graphBuilder.BuildStateGraph(definition);

            Assert.AreEqual(1, graph.Relatives.Count, "A single transition should result in a 2 node graph with a single vertice");
        }

        [TestMethod]
        public void Two_Vertices_Between_Two_Nodes_Results_In_Two_Nodes()
        {
            var graphBuilder = new StateGraphBuilder();
            var definitionContent =
                new XElement("TRANSITIONS",
                    new XElement("TRANSITION",
                        new XAttribute("from", string.Empty),
                        new XAttribute("to", "Proposed")
                    ),
                    new XElement("TRANSITION",
                        new XAttribute("from", "Proposed"),
                        new XAttribute("to", "Ready for Development")
                    ),
                    new XElement("TRANSITION",
                        new XAttribute("from", "Ready for Development"),
                        new XAttribute("to", "Proposed")
                    )
                );

            var definition = new XDocument(definitionContent);

            var graph = graphBuilder.BuildStateGraph(definition);
            var relative = graph.Relatives.FirstOrDefault();

            Assert.IsNotNull(relative, "Could not identify a second node in the graph");
            Assert.AreEqual(1, graph.Relatives.Count, "More than one vertice on the graph root");
            Assert.AreEqual(1, relative.Relatives.Count, "More than one vertice on the second graph node");
        }

        [TestMethod]
        public void Three_Node_Tree_Returns_Two_Vertices()
        {
            var graphBuilder = new StateGraphBuilder();
            var definitionContent =
                new XElement("TRANSITIONS",
                    new XElement("TRANSITION",
                        new XAttribute("from", string.Empty),
                        new XAttribute("to", "Proposed")
                    ),
                    new XElement("TRANSITION",
                        new XAttribute("from", "Proposed"),
                        new XAttribute("to", "Ready for Development")
                    ),
                    new XElement("TRANSITION",
                        new XAttribute("from", "Proposed"),
                        new XAttribute("to", "Closed")
                    )
                );

            var definition = new XDocument(definitionContent);

            var graph = graphBuilder.BuildStateGraph(definition);
            
            Assert.AreEqual(2, graph.Relatives.Count, "Two vertices should exist with three nodes in a tree");
        }

        [TestMethod]
        public void Graph_Builder_Returns_Start_Transition_Target_As_Root()
        {
            var graphBuilder = new StateGraphBuilder();
            var definitionContent =
                 new XElement("TRANSITIONS",
                    new XElement("TRANSITION",
                        new XAttribute("from", ""),
                        new XAttribute("to", "Proposed")),
                    new XElement("TRANSITION",
                        new XAttribute("from", "Ready for Development"),
                        new XAttribute("to", "Proposed")));

            var definition = new XDocument(definitionContent);

            var graph = graphBuilder.BuildStateGraph(definition);
            
            Assert.AreEqual("Proposed", graph.Value, "Root transition was not returned as graph root");
        }

        [TestMethod]
        public void Graph_With_No_Root_Throws_Argument_Exception()
        {
            var graphBuilder = new StateGraphBuilder();
            var definitionContent =
                 new XElement("TRANSITIONS",
                    new XElement("TRANSITION",
                        new XAttribute("from", "Ready for Development"),
                        new XAttribute("to", "Proposed")));

            var definition = new XDocument(definitionContent);
            var exceptional = false;

            try
            {
                graphBuilder.BuildStateGraph(definition);
            }
            catch (ArgumentException ex)
            {
                exceptional = true;
                Assert.AreEqual("workItemDefinition", ex.ParamName);
            }

            Assert.IsTrue(exceptional,"Expected an argument exception");
        }
    }
}
