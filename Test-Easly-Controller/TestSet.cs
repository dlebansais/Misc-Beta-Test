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

/*

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
            Assert.That(Controller.Contains(RootIndex), "Sanity Check #4");
            Assert.That(Controller.ToState(RootIndex) == Controller.RootState, "Sanity Check #5");
        }
        #endregion

        #region State Tree
        [Test]
        public static void StateTree()
        {
            IReadOnlyRootNodeIndex RootIndex = new ReadOnlyRootNodeIndex(RootNode);
            IReadOnlyController Controller = ReadOnlyController.Create(RootIndex);

            Stats Stats = new Stats();
            BrowseNode(Controller, RootIndex, Stats);

            const int ExpectedNodeCount = 155;
            const int ExpectedPlaceholderNodeCount = 142;
            const int ExpectedOptionalNodeCount = 12;
            const int ExpectedAssignedOptionalNodeCount = 4;
            const int ExpectedListCount = 5;
            const int ExpectedBlockListCount = 96;

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

        static void BrowseNode(IReadOnlyController controller, IReadOnlyIndex index, Stats stats)
        {
            Assert.That(index != null, "State Tree #0");
            Assert.That(controller.Contains(index), "State Tree #1");
            IReadOnlyNodeState State = controller.ToState(index);
            Assert.That(State != null, "State Tree #2");
            Assert.That(State.ParentIndex == index, "State Tree #4");

            INode Node;

            if (State is IReadOnlyPlaceholderNodeState AsPlaceholderState)
                Node = AsPlaceholderState.Node;
            else
            {
                Assert.That(State is IReadOnlyOptionalNodeState, "State Tree #5");
                IReadOnlyOptionalNodeState AsOptionalState = (IReadOnlyOptionalNodeState)State;
                IReadOnlyOptionalInner ParentInner = AsOptionalState.ParentInner;

                Assert.That(ParentInner.IsAssigned, "State Tree #6");

                Node = AsOptionalState.Node;
            }

            stats.NodeCount++;

            Type ChildNodeType;
            IList<string> PropertyNames = NodeTreeHelper.EnumChildNodeProperties(Node);

            foreach (string PropertyName in PropertyNames)
            {
                if (NodeTreeHelper.IsChildNodeProperty(Node, PropertyName, out ChildNodeType))
                {
                    stats.PlaceholderNodeCount++;

                    IReadOnlyPlaceholderInner Inner = (IReadOnlyPlaceholderInner)State.PropertyToInner(PropertyName);
                    IReadOnlyNodeState ChildState = Inner.ChildState;
                    IReadOnlyIndex ChildIndex = ChildState.ParentIndex;
                    BrowseNode(controller, ChildIndex, stats);
                }

                else if (NodeTreeHelper.IsOptionalChildNodeProperty(Node, PropertyName, out ChildNodeType))
                {
                    stats.OptionalNodeCount++;

                    NodeTreeHelper.GetChildNode(Node, PropertyName, out bool IsAssigned, out INode ChildNode);
                    if (IsAssigned)
                    {
                        stats.AssignedOptionalNodeCount++;

                        IReadOnlyOptionalInner Inner = (IReadOnlyOptionalInner)State.PropertyToInner(PropertyName);
                        IReadOnlyNodeState ChildState = Inner.ChildState;
                        IReadOnlyIndex ChildIndex = ChildState.ParentIndex;
                        BrowseNode(controller, ChildIndex, stats);
                    }
                    else
                        stats.NodeCount++;
                }

                else if (NodeTreeHelper.IsChildNodeList(Node, PropertyName, out ChildNodeType))
                {
                    stats.ListCount++;

                    IReadOnlyListInner Inner = (IReadOnlyListInner)State.PropertyToInner(PropertyName);

                    for (int i = 0; i < Inner.StateList.Count; i++)
                    {
                        stats.PlaceholderNodeCount++;

                        IReadOnlyPlaceholderNodeState ChildState = Inner.StateList[i];
                        IReadOnlyIndex ChildIndex = ChildState.ParentIndex;
                        BrowseNode(controller, ChildIndex, stats);
                    }
                }

                else if (NodeTreeHelper.IsChildBlockList(Node, PropertyName, out Type ChildInterfaceType, out ChildNodeType))
                {
                    stats.BlockListCount++;

                    IReadOnlyBlockListInner Inner = (IReadOnlyBlockListInner)State.PropertyToInner(PropertyName);

                    for (int BlockIndex = 0; BlockIndex < Inner.BlockStateList.Count; BlockIndex++)
                    {
                        IReadOnlyBlockState BlockState = Inner.BlockStateList[BlockIndex];

                        stats.PlaceholderNodeCount++;
                        BrowseNode(controller, BlockState.PatternIndex, stats);

                        stats.PlaceholderNodeCount++;
                        BrowseNode(controller, BlockState.SourceIndex, stats);

                        for (int i = 0; i < BlockState.StateList.Count; i++)
                        {
                            stats.PlaceholderNodeCount++;

                            IReadOnlyPlaceholderNodeState ChildState = BlockState.StateList[i];
                            IReadOnlyIndex ChildIndex = ChildState.ParentIndex;
                            BrowseNode(controller, ChildIndex, stats);
                        }
                    }
                }

                else
                {
                    Type NodeType = Node.GetType();
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
*/
    }
}
