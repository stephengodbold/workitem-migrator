using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkItemMigrator.Migration.GraphTypes;

namespace WorkItemMigrator.Tests.GraphTests
{
    [TestClass]
    public class WalkingPathTests
    {
        [TestMethod]
        public void Path_With_Unique_Node_Values_Is_Valid()
        {
            var path = new WalkingPath<int>();
            path.Step(new GraphNode<int> { Value = 1 });
            path.Step(new GraphNode<int> { Value = 2 });
            path.Step(new GraphNode<int> { Value = 3 });

            Assert.IsTrue(path.IsValid, "Path with unique node values should be valid");
        }

        [TestMethod]
        public void Path_With_Non_Unique_Node_Value_Is_Invalid()
        {
            var path = new WalkingPath<int>();

            path.Step(new GraphNode<int> { Value = 0});
            path.Step(new GraphNode<int> { Value = 1 });
            path.Step(new GraphNode<int> { Value = 2 });
            path.Step(new GraphNode<int> { Value = 1 });

            Assert.IsFalse(path.IsValid, "Path with non-unique node values should be invalid");
        }
    }
}
