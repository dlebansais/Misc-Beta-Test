using BaseNode;
using BaseNodeHelper;
using EaslyController.ReadOnly;
using NUnit.Framework;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using PolySerializer;
using System.Collections.Generic;
using System;
using EaslyController;

namespace Test
{
    [TestFixture]
    public class TestSet
    {
        #region Setup
        [OneTimeSetUp]
        public static void InitTestSession()
        {
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.DefaultThreadCurrentCulture = enUS;
            CultureInfo.DefaultThreadCurrentUICulture = enUS;
            Thread.CurrentThread.CurrentCulture = enUS;
            Thread.CurrentThread.CurrentUICulture = enUS;

            Assembly EaslyControllerAssembly;

            try
            {
                EaslyControllerAssembly = Assembly.Load("Easly-Controller");
            }
            catch
            {
                EaslyControllerAssembly = null;
            }
            Assume.That(EaslyControllerAssembly != null);

            if (File.Exists("test.easly"))
            {
                using (FileStream fs = new FileStream("test.easly", FileMode.Open, FileAccess.Read))
                {
                    Serializer Serializer = new Serializer();
                    RootNode = Serializer.Deserialize(fs) as INode;
                }
            }
            else if (File.Exists("./Easly-Controller/bin/x64/Travis/test.easly"))
            {
                using (FileStream fs = new FileStream("./Easly-Controller/bin/x64/Travis/test.easly", FileMode.Open, FileAccess.Read))
                {
                    Serializer Serializer = new Serializer();
                    RootNode = Serializer.Deserialize(fs) as INode;
                }
            }
            else
            {
                RootNode = NodeHelper.CreateEmptyName();
            }
        }

        static INode RootNode;
        #endregion

        #region Sanity Check
        [Test]
        public static void TestInit()
        {
            IReadOnlyRootNodeIndex RootIndex = new ReadOnlyRootNodeIndex(RootNode);
            IReadOnlyController Controller = ReadOnlyController.Create(RootIndex);

            Assert.That(Controller != null, "Sanity Check #0");
            Assert.That(Controller.RootIndex == RootIndex, "Sanity Check #1");
            Assert.That(Controller.RootState != null, "Sanity Check #2");
            Assert.That(Controller.RootState.Node == RootNode, "Sanity Check #3");
            Assert.That(Controller.ContainsNode(RootNode), "Sanity Check #4");
            Assert.That(Controller.NodeToState(RootNode) == Controller.RootState, "Sanity Check #5");
        }
        #endregion

        #region State Tree
        [Test]
        public static void StateTree()
        {
            IReadOnlyRootNodeIndex RootIndex = new ReadOnlyRootNodeIndex(RootNode);
            IReadOnlyController Controller = ReadOnlyController.Create(RootIndex);

            Stats Stats = new Stats();
            BrowseNode(Controller, RootNode, Stats);

            const int ExpectedNodeCount = 170;
            const int ExpectedPlaceholderNodeCount = 157;
            const int ExpectedOptionalNodeCount = 12;
            const int ExpectedAssignedOptionalNodeCount = 4;
            const int ExpectedListCount = 12;
            const int ExpectedBlockListCount = 103;

            Assert.That(Stats.NodeCount == ExpectedNodeCount, $"Failed to browse tree. Expected: {ExpectedNodeCount} node(s), Found: {Stats.NodeCount}");
            Assert.That(Stats.PlaceholderNodeCount == ExpectedPlaceholderNodeCount, $"Failed to browse tree. Expected: {ExpectedPlaceholderNodeCount} placeholder node(s), Found: {Stats.PlaceholderNodeCount}");
            Assert.That(Stats.OptionalNodeCount == ExpectedOptionalNodeCount, $"Failed to browse tree. Expected: {ExpectedOptionalNodeCount } optional node(s), Found: {Stats.OptionalNodeCount}");
            Assert.That(Stats.AssignedOptionalNodeCount == ExpectedAssignedOptionalNodeCount, $"Failed to browse tree. Expected: {ExpectedAssignedOptionalNodeCount} assigned optional node(s), Found: {Stats.AssignedOptionalNodeCount}");
            Assert.That(Stats.ListCount == ExpectedListCount, $"Failed to browse tree. Expected: {ExpectedListCount} list(s), Found: {Stats.ListCount}");
            Assert.That(Stats.BlockListCount == ExpectedBlockListCount, $"Failed to browse tree. Expected: {ExpectedBlockListCount} block list(s), Found: {Stats.BlockListCount}");

            Assert.That(Controller.Stats.NodeCount == ExpectedNodeCount, $"Invalid controller state. Expected: {ExpectedNodeCount} node(s), Found: {Controller.Stats.NodeCount}");
            Assert.That(Controller.Stats.PlaceholderNodeCount == ExpectedPlaceholderNodeCount, $"Invalid controller state. Expected: {ExpectedPlaceholderNodeCount} placeholder node(s), Found: {Controller.Stats.PlaceholderNodeCount}");
            Assert.That(Controller.Stats.OptionalNodeCount == ExpectedOptionalNodeCount, $"Invalid controller state. Expected: {ExpectedOptionalNodeCount } optional node(s), Found: {Controller.Stats.OptionalNodeCount}");
            Assert.That(Controller.Stats.AssignedOptionalNodeCount == ExpectedAssignedOptionalNodeCount, $"Invalid controller state. Expected: {ExpectedAssignedOptionalNodeCount} assigned optional node(s), Found: {Controller.Stats.AssignedOptionalNodeCount}");
            Assert.That(Controller.Stats.ListCount == ExpectedListCount, $"Invalid controller state. Expected: {ExpectedListCount} list(s), Found: {Controller.Stats.ListCount}");
            Assert.That(Controller.Stats.BlockListCount == ExpectedBlockListCount, $"Invalid controller state. Expected: {ExpectedBlockListCount} block list(s), Found: {Controller.Stats.BlockListCount}");
        }

        static void BrowseNode(IReadOnlyController controller, INode node, Stats stats)
        {
            Assert.That(node != null, "State Tree #0");
            Assert.That(controller.ContainsNode(node), "State Tree #1");
            IReadOnlyNodeState State = controller.NodeToState(node);
            Assert.That(State != null, "State Tree #2");
            Assert.That(State.Node == node, "State Tree #3");

            stats.NodeCount++;

            Type ChildNodeType;
            IList<string> PropertyNames = NodeTreeHelper.EnumChildNodeProperties(node);

            foreach (string PropertyName in PropertyNames)
            {
                if (NodeTreeHelper.IsChildNodeProperty(node, PropertyName))
                {
                    stats.PlaceholderNodeCount++;

                    NodeTreeHelper.GetChildNode(node, PropertyName, out bool IsAssigned, out INode ChildNode);
                    Assert.That(IsAssigned, "State Tree #4");

                    BrowseNode(controller, ChildNode, stats);
                }

                else if (NodeTreeHelper.IsOptionalChildNodeProperty(node, PropertyName))
                {
                    stats.OptionalNodeCount++;

                    NodeTreeHelper.GetChildNode(node, PropertyName, out bool IsAssigned, out INode ChildNode);
                    if (IsAssigned)
                        stats.AssignedOptionalNodeCount++;

                    BrowseNode(controller, ChildNode, stats);
                }

                else if (NodeTreeHelper.IsChildNodeList(node, PropertyName, out ChildNodeType))
                {
                    stats.ListCount++;
                    NodeTreeHelper.GetChildNodeList(node, PropertyName, out IReadOnlyList<INode> ChildNodeList);

                    foreach (INode ChildNode in ChildNodeList)
                    {
                        stats.PlaceholderNodeCount++;
                        BrowseNode(controller, ChildNode, stats);
                    }
                }

                else if (NodeTreeHelper.IsChildBlockList(node, PropertyName, out Type ChildInterfaceType, out ChildNodeType))
                {
                    stats.BlockListCount++;

                    NodeTreeHelper.GetChildBlockList(node, PropertyName, out IReadOnlyList<INodeTreeBlock> ChildBlockNodeList);

                    foreach (INodeTreeBlock ChildBlockNode in ChildBlockNodeList)
                    {
                        stats.PlaceholderNodeCount++;
                        BrowseNode(controller, ChildBlockNode.ReplicationPattern, stats);

                        stats.PlaceholderNodeCount++;
                        BrowseNode(controller, ChildBlockNode.SourceIdentifier, stats);

                        foreach (INode ChildNode in ChildBlockNode.NodeList)
                        {
                            stats.PlaceholderNodeCount++;
                            BrowseNode(controller, ChildNode, stats);
                        }
                    }
                }

                else
                {
                    Type NodeType = node.GetType();
                    PropertyInfo Info = NodeType.GetProperty(PropertyName);

                    if (Info.PropertyType == typeof(IDocument))
                    {
                    }
                    else if (Info.PropertyType == typeof(bool))
                    {
                    }
                    else if (Info.PropertyType.IsEnum)
                    {
                    }
                    else if (Info.PropertyType == typeof(string))
                    {
                    }
                    else if (Info.PropertyType == typeof(Guid))
                    {
                    }
                    else
                    {
                        Assert.That(false, $"State Tree unexpected property: {Info.PropertyType.Name}");
                    }
                }
            }
        }
        #endregion
    }
}
