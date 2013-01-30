using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkItemMigrator.Migration.GraphTypes;

namespace WorkItemMigrator.Tests.GraphTests
{
    [TestClass]
    public class GraphWalkerTests
    {

        [TestMethod]
        public void One_Node_In_Graph_Returns_Valid_Path_Of_Length_One()
        {
            var graph = new GraphNode<int> { Value = 1 };
            var walker = new GraphWalker<int>(graph);

            var walkingPath = walker.WalkToNode(1);

            Assert.AreEqual(true, walkingPath.IsValid);
            Assert.AreEqual(1, walkingPath.Path.Count, "Path length should be one (node) for single node graph");
        }

        [TestMethod]
        public void Three_Nodes_In_Linear_Graph_Returns_Valid_Path_With_Length_Of_Three()
        {
            var graphRoot = new GraphNode<int> {Value = 1};
            var secondNode = new GraphNode<int> {Value = 2};
            var thirdNode = new GraphNode<int> { Value = 3 };
        
            secondNode.AddRelative(thirdNode);
            graphRoot.AddRelative(secondNode);
            
            var walker = new GraphWalker<int>(graphRoot);

            var walkingPath = walker.WalkToNode(3);

            Assert.AreEqual(true, walkingPath.IsValid);
            Assert.AreEqual(3, walkingPath.Path.Count, "Path length should be three (nodes) for three node linear graph");
        }

        [TestMethod]
        public void Three_Nodes_In_Tree_Graph_Returns_Valid_Path_With_Length_Of_Two()
        {
            var graphRoot = new GraphNode<int> { Value = 1 };
            var secondNode = new GraphNode<int> { Value = 2 };
            var thirdNode = new GraphNode<int> { Value = 3 };

            graphRoot.AddRelative(secondNode);
            graphRoot.AddRelative(thirdNode);

            var walker = new GraphWalker<int>(graphRoot);

            var walkingPath = walker.WalkToNode(3);

            Assert.AreEqual(true, walkingPath.IsValid);
            Assert.AreEqual(2, walkingPath.Path.Count, "Path length should be two (nodes) for three node tree graph");
        }

        [TestMethod]
        public void Two_Paths_To_Target_Returns_Shortest_Valid_Path()
        {
            var graphRoot = new GraphNode<int> { Value = 1 };
            var secondNode = new GraphNode<int> { Value = 2 };
            var thirdNode = new GraphNode<int> { Value = 3 };
            var fourthNode = new GraphNode<int> { Value = 4 };
            var fifthNode = new GraphNode<int> { Value = 5 };
            
            graphRoot.AddRelative(secondNode);
            graphRoot.AddRelative(thirdNode);
            secondNode.AddRelative(fourthNode);
            thirdNode.AddRelative(fifthNode);
            fourthNode.AddRelative(fifthNode);

            var walker = new GraphWalker<int>(graphRoot);
            var walkingPath = walker.WalkToNode(5);

            Assert.AreEqual(true, walkingPath.IsValid);
            Assert.AreEqual(3, walkingPath.Path.Count, "Path length should be three (nodes) for five node graph");
        }

        [TestMethod]
        public void Five_Nodes_In_A_Graph_Returns_Valid_Path_With_Length_Of_Three()
        {
            var graphRoot = new GraphNode<int> { Value = 1 };
            var secondNode = new GraphNode<int> { Value = 2 };
            var thirdNode = new GraphNode<int> { Value = 3 };
            var fourthNode = new GraphNode<int> { Value = 4 };
            var fifthNode = new GraphNode<int> { Value = 5 };


            //first level
            graphRoot.AddRelative(secondNode);
            graphRoot.AddRelative(thirdNode);

            //second level
            thirdNode.AddRelative(fourthNode);
            secondNode.AddRelative(fifthNode);

            fourthNode.AddRelative(graphRoot);

            var walker = new GraphWalker<int>(graphRoot);
            var walkingPath = walker.WalkToNode(5);

            Assert.AreEqual(true, walkingPath.IsValid);
            Assert.AreEqual(3, walkingPath.Path.Count, "Path length should be three (nodes) for six node graph");
        }

        [TestMethod]
        public void Value_Not_In_Graph_Throws_Arg_Out_Of_Range()
        {
            var graphRoot = new GraphNode<int> { Value = 1 };
            var secondNode = new GraphNode<int> { Value = 2 };
            var thirdNode = new GraphNode<int> { Value = 3 };

            graphRoot.AddRelative(secondNode);
            graphRoot.AddRelative(thirdNode);

            var walker = new GraphWalker<int>(graphRoot);
            var exceptional = false;

            try
            {
                walker.WalkToNode(5);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                exceptional = true;
                Assert.AreEqual(5, ex.ActualValue);
                Assert.AreEqual("value", ex.ParamName);
            }

            Assert.IsTrue(exceptional, "Expected to catch an argument out of range exception");
        }
    }
}
