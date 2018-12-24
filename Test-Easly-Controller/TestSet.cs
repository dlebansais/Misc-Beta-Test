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
using EaslyController.Writeable;

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

            string RootPath;
            if (File.Exists("./Easly-Controller/bin/x64/Travis/test.easly"))
                RootPath = "./Easly-Controller/bin/x64/Travis/";
            else
                RootPath = "./";

            RootNodeTable = new Dictionary<string, INode>();
            FirstRootNode = null;
            AddEaslyFiles(RootPath);
        }

        static void AddEaslyFiles(string path)
        {
            foreach (string FileName in Directory.GetFiles(path, "*.easly"))
                using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                {
                    Serializer Serializer = new Serializer();
                    INode RootNode = Serializer.Deserialize(fs) as INode;

                    RootNodeTable.Add(FileName, RootNode);
                    if (FirstRootNode == null)
                        FirstRootNode = RootNode;
                }

            foreach (string Folder in Directory.GetDirectories(path))
                AddEaslyFiles(Folder);
        }

        static IEnumerable<int> FileIndexRange()
        {
            for (int i = 0; i < 173; i++)
                yield return i;
        }

        static Dictionary<string, INode> RootNodeTable;
        static INode FirstRootNode;
        #endregion

        static bool TestOff = false;

        #region Sanity Check
        [Test]
        public static void TestInit()
        {
            if (TestOff)
                return;

            IReadOnlyRootNodeIndex RootIndex = new ReadOnlyRootNodeIndex(FirstRootNode);
            IReadOnlyController Controller = ReadOnlyController.Create(RootIndex);

            Assert.That(Controller != null, "Sanity Check #0");
            Assert.That(Controller.RootIndex == RootIndex, "Sanity Check #1");
            Assert.That(Controller.RootState != null, "Sanity Check #2");
            Assert.That(Controller.RootState.Node == FirstRootNode, "Sanity Check #3");
            Assert.That(Controller.Contains(RootIndex), "Sanity Check #4");
            Assert.That(Controller.IndexToState(RootIndex) == Controller.RootState, "Sanity Check #5");
        }
        #endregion

        #region ReadOnly
        [Test]
        [TestCaseSource(nameof(FileIndexRange))]
        public static void ReadOnly(int index)
        {
            if (TestOff)
                return;

            INode RootNode = null;
            int n = index;
            foreach (KeyValuePair<string, INode> Entry in RootNodeTable)
            {
                RootNode = Entry.Value;
                if (n == 0)
                    break;
                n--;
            }

            if (n > 0)
                throw new ArgumentOutOfRangeException($"{n} / {RootNodeTable.Count}");
            TestReadOnly(index, RootNode);
        }

        public static void TestReadOnly(int index, INode rootNode)
        {
            ControllerTools.ResetExpectedName();

            TestReadOnlyStats(index, rootNode);
        }

        public static void TestReadOnlyStats(int index, INode rootNode)
        {
            IReadOnlyRootNodeIndex RootIndex = new ReadOnlyRootNodeIndex(rootNode);
            IReadOnlyController Controller = ReadOnlyController.Create(RootIndex);

            Stats Stats = new Stats();
            BrowseNode(Controller, RootIndex, Stats);

            if (index == 0)
            {
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
            }

            Assert.That(Controller.Stats.NodeCount == Stats.NodeCount, $"Invalid controller state. Expected: {Stats.NodeCount} node(s), Found: {Controller.Stats.NodeCount}");
            Assert.That(Controller.Stats.PlaceholderNodeCount == Stats.PlaceholderNodeCount, $"Invalid controller state. Expected: {Stats.PlaceholderNodeCount} placeholder node(s), Found: {Controller.Stats.PlaceholderNodeCount}");
            Assert.That(Controller.Stats.OptionalNodeCount == Stats.OptionalNodeCount, $"Invalid controller state. Expected: {Stats.OptionalNodeCount } optional node(s), Found: {Controller.Stats.OptionalNodeCount}");
            Assert.That(Controller.Stats.AssignedOptionalNodeCount == Stats.AssignedOptionalNodeCount, $"Invalid controller state. Expected: {Stats.AssignedOptionalNodeCount } assigned optional node(s), Found: {Controller.Stats.AssignedOptionalNodeCount}");
            Assert.That(Controller.Stats.ListCount == Stats.ListCount, $"Invalid controller state. Expected: {Stats.ListCount} list(s), Found: {Controller.Stats.ListCount}");
            Assert.That(Controller.Stats.BlockListCount == Stats.BlockListCount, $"Invalid controller state. Expected: {Stats.BlockListCount} block list(s), Found: {Controller.Stats.BlockListCount}");
        }

        static void BrowseNode(IReadOnlyController controller, IReadOnlyIndex index, Stats stats)
        {
            Assert.That(index != null, "ReadOnly #0");
            Assert.That(controller.Contains(index), "ReadOnly #1");
            IReadOnlyNodeState State = controller.IndexToState(index);
            Assert.That(State != null, "ReadOnly #2");
            Assert.That(State.ParentIndex == index, "ReadOnly #4");

            INode Node;

            if (State is IReadOnlyPlaceholderNodeState AsPlaceholderState)
                Node = AsPlaceholderState.Node;
            else
            {
                Assert.That(State is IReadOnlyOptionalNodeState, "ReadOnly #5");
                IReadOnlyOptionalNodeState AsOptionalState = (IReadOnlyOptionalNodeState)State;
                IReadOnlyOptionalInner<IReadOnlyBrowsingOptionalNodeIndex> ParentInner = AsOptionalState.ParentInner;

                Assert.That(ParentInner.IsAssigned, "ReadOnly #6");

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

        #region Views
        [Test]
        [TestCaseSource(nameof(FileIndexRange))]
        public static void StateViews(int index)
        {
            if (TestOff)
                return;

            INode RootNode = null;
            int n = index;
            foreach (KeyValuePair<string, INode> Entry in RootNodeTable)
            {
                RootNode = Entry.Value;
                if (n == 0)
                    break;
                n--;
            }

            if (n > 0)
                throw new ArgumentOutOfRangeException($"{n} / {RootNodeTable.Count}");
            TestStateView(index, RootNode);
        }

        public static void TestStateView(int index, INode rootNode)
        {
            IReadOnlyRootNodeIndex RootIndex = new ReadOnlyRootNodeIndex(rootNode);
            IReadOnlyController Controller = ReadOnlyController.Create(RootIndex);
            IReadOnlyControllerView ControllerView = ReadOnlyControllerView.Create(Controller);

            Assert.That(ControllerView.StateViewTable.ContainsKey(Controller.RootState), $"Views #0");
            Assert.That(ControllerView.StateViewTable.Count == Controller.Stats.NodeCount, $"Views #1");

            foreach (KeyValuePair<IReadOnlyNodeState, IReadOnlyNodeStateView> Entry in ControllerView.StateViewTable)
            {
                IReadOnlyNodeState State = Entry.Key;
                Assert.That(ControllerView.StateViewTable.ContainsKey(Controller.RootState), $"Views #2, state={State}");

                IReadOnlyNodeStateView View = Entry.Value;
                Assert.That(View.State == State, $"Views #3");
            }

            IReadOnlyControllerView ControllerView2 = ReadOnlyControllerView.Create(Controller);
            Assert.That(ControllerView2.IsEqual(ControllerView), $"Views #4");
        }
        #endregion

        #region Writeable
        [Test]
        [TestCaseSource(nameof(FileIndexRange))]
        public static void Writeable(int index)
        {
            if (TestOff)
                return;

            INode RootNode = null;
            int n = index;
            foreach (KeyValuePair<string, INode> Entry in RootNodeTable)
            {
                RootNode = Entry.Value;
                if (n == 0)
                    break;
                n--;
            }

            if (n > 0)
                throw new ArgumentOutOfRangeException($"{n} / {RootNodeTable.Count}");
            TestWriteable(index, RootNode);
        }

        public static void TestWriteable(int index, INode rootNode)
        {
            ControllerTools.ResetExpectedName();

            TestWriteableStats(index, rootNode, out Stats Stats);

            Random rand = new Random(0x123456);
            TestWriteableInsert(index, rootNode, rand);
        }

        public static void TestWriteableStats(int index, INode rootNode, out Stats stats)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);

            stats = new Stats();
            BrowseNode(Controller, RootIndex, stats);

            if (index == 0)
            {
                const int ExpectedNodeCount = 155;
                const int ExpectedPlaceholderNodeCount = 142;
                const int ExpectedOptionalNodeCount = 12;
                const int ExpectedAssignedOptionalNodeCount = 4;
                const int ExpectedListCount = 5;
                const int ExpectedBlockListCount = 96;

                Assert.That(stats.NodeCount == ExpectedNodeCount, $"Failed to browse tree. Expected: {ExpectedNodeCount} node(s), Found: {stats.NodeCount}");
                Assert.That(stats.PlaceholderNodeCount == ExpectedPlaceholderNodeCount, $"Failed to browse tree. Expected: {ExpectedPlaceholderNodeCount} placeholder node(s), Found: {stats.PlaceholderNodeCount}");
                Assert.That(stats.OptionalNodeCount == ExpectedOptionalNodeCount, $"Failed to browse tree. Expected: {ExpectedOptionalNodeCount } optional node(s), Found: {stats.OptionalNodeCount}");
                Assert.That(stats.AssignedOptionalNodeCount == ExpectedAssignedOptionalNodeCount, $"Failed to browse tree. Expected: {ExpectedAssignedOptionalNodeCount} assigned optional node(s), Found: {stats.AssignedOptionalNodeCount}");
                Assert.That(stats.ListCount == ExpectedListCount, $"Failed to browse tree. Expected: {ExpectedListCount} list(s), Found: {stats.ListCount}");
                Assert.That(stats.BlockListCount == ExpectedBlockListCount, $"Failed to browse tree. Expected: {ExpectedBlockListCount} block list(s), Found: {stats.BlockListCount}");
            }

            Assert.That(Controller.Stats.NodeCount == stats.NodeCount, $"Invalid controller state. Expected: {stats.NodeCount} node(s), Found: {Controller.Stats.NodeCount}");
            Assert.That(Controller.Stats.PlaceholderNodeCount == stats.PlaceholderNodeCount, $"Invalid controller state. Expected: {stats.PlaceholderNodeCount} placeholder node(s), Found: {Controller.Stats.PlaceholderNodeCount}");
            Assert.That(Controller.Stats.OptionalNodeCount == stats.OptionalNodeCount, $"Invalid controller state. Expected: {stats.OptionalNodeCount } optional node(s), Found: {Controller.Stats.OptionalNodeCount}");
            Assert.That(Controller.Stats.AssignedOptionalNodeCount == stats.AssignedOptionalNodeCount, $"Invalid controller state. Expected: {stats.AssignedOptionalNodeCount } assigned optional node(s), Found: {Controller.Stats.AssignedOptionalNodeCount}");
            Assert.That(Controller.Stats.ListCount == stats.ListCount, $"Invalid controller state. Expected: {stats.ListCount} list(s), Found: {Controller.Stats.ListCount}");
            Assert.That(Controller.Stats.BlockListCount == stats.BlockListCount, $"Invalid controller state. Expected: {stats.BlockListCount} block list(s), Found: {Controller.Stats.BlockListCount}");
        }

        public static void TestWriteableInsert(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            BrowseNode(Controller, RootIndex, (IWriteableInner inner) => InsertAndCompare(ControllerView, rand, inner));
        }

        static void InsertAndCompare(IWriteableControllerView controllerView, Random rand, IWriteableInner inner)
        {
            IWriteableController Controller = controllerView.Controller;

            INode NewNode;
            try
            {
                NewNode = NodeHelper.CreateDefault(inner.InterfaceType);
            }
            catch
            {
                NewNode = null;
            }
            //Assert.That(NewNode != null, $"Type: {AsBlockListInner.InterfaceType}");

            if (NewNode == null)
                return;

            if (inner is IWriteableListInner<IWriteableBrowsingListNodeIndex> AsListInner)
            {
                if (AsListInner.StateList.Count > 0)
                {
                    int Index = rand.Next(AsListInner.StateList.Count + 1);
                    IWriteableInsertionListNodeIndex NodeIndex = new WriteableInsertionListNodeIndex(AsListInner.Owner.Node, AsListInner.PropertyName, NewNode, Index);
                    Controller.Insert(AsListInner, NodeIndex);

                    IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                    Assert.That(NewView.IsEqual(controllerView));
                }
            }
            else if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (rand.Next(2) == 0)
                {
                    if (AsBlockListInner.BlockStateList.Count > 0)
                    {
                        int BlockIndex = rand.Next(AsBlockListInner.BlockStateList.Count);
                        IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];
                        int Index = rand.Next(BlockState.StateList.Count + 1);

                        IWriteableInsertionExistingBlockNodeIndex NodeIndex = new WriteableInsertionExistingBlockNodeIndex(AsBlockListInner.Owner.Node, AsBlockListInner.PropertyName, NewNode, BlockIndex, Index);
                        Controller.Insert(AsBlockListInner, NodeIndex);

                        IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                        Assert.That(NewView.IsEqual(controllerView));
                    }
                }
                else
                {
                    int BlockIndex = rand.Next(AsBlockListInner.BlockStateList.Count + 1);
                    IPattern ReplicationPattern = NodeHelper.CreateSimplePattern("x");
                    IIdentifier SourceIdentifier = NodeHelper.CreateSimpleIdentifier("y");
                    IWriteableInsertionNewBlockNodeIndex NodeIndex = new WriteableInsertionNewBlockNodeIndex(AsBlockListInner.Owner.Node, AsBlockListInner.PropertyName, NewNode, BlockIndex, ReplicationPattern, SourceIdentifier);
                    Controller.Insert(AsBlockListInner, NodeIndex);

                    IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                    Assert.That(NewView.IsEqual(controllerView));
                }
            }
        }

        static void BrowseNode(IWriteableController controller, IWriteableIndex index, Action<IWriteableInner> test)
        {
            Assert.That(index != null, "Writeable #0");
            Assert.That(controller.Contains(index), "Writeable #1");
            IWriteableNodeState State = (IWriteableNodeState)controller.IndexToState(index);
            Assert.That(State != null, "Writeable #2");
            Assert.That(State.ParentIndex == index, "Writeable #4");

            INode Node;

            if (State is IWriteablePlaceholderNodeState AsPlaceholderState)
                Node = AsPlaceholderState.Node;
            else
            {
                Assert.That(State is IWriteableOptionalNodeState, "Writeable #5");
                IWriteableOptionalNodeState AsOptionalState = (IWriteableOptionalNodeState)State;
                IWriteableOptionalInner<IWriteableBrowsingOptionalNodeIndex> ParentInner = AsOptionalState.ParentInner;

                Assert.That(ParentInner.IsAssigned, "Writeable #6");

                Node = AsOptionalState.Node;
            }

            Type ChildNodeType;
            IList<string> PropertyNames = NodeTreeHelper.EnumChildNodeProperties(Node);

            foreach (string PropertyName in PropertyNames)
            {
                if (NodeTreeHelper.IsChildNodeProperty(Node, PropertyName, out ChildNodeType))
                {
                    IWriteablePlaceholderInner Inner = (IWriteablePlaceholderInner)State.PropertyToInner(PropertyName);
                    IWriteableNodeState ChildState = Inner.ChildState;
                    IWriteableIndex ChildIndex = ChildState.ParentIndex;
                    BrowseNode(controller, ChildIndex, test);
                }

                else if (NodeTreeHelper.IsOptionalChildNodeProperty(Node, PropertyName, out ChildNodeType))
                {
                    NodeTreeHelper.GetChildNode(Node, PropertyName, out bool IsAssigned, out INode ChildNode);
                    if (IsAssigned)
                    {
                        IWriteableOptionalInner Inner = (IWriteableOptionalInner)State.PropertyToInner(PropertyName);
                        IWriteableNodeState ChildState = Inner.ChildState;
                        IWriteableIndex ChildIndex = ChildState.ParentIndex;
                        BrowseNode(controller, ChildIndex, test);
                    }
                }

                else if (NodeTreeHelper.IsChildNodeList(Node, PropertyName, out ChildNodeType))
                {
                    IWriteableListInner Inner = (IWriteableListInner)State.PropertyToInner(PropertyName);
                    test(Inner);

                    for (int i = 0; i < Inner.StateList.Count; i++)
                    {
                        IWriteablePlaceholderNodeState ChildState = Inner.StateList[i];
                        IWriteableIndex ChildIndex = ChildState.ParentIndex;
                        BrowseNode(controller, ChildIndex, test);
                    }
                }

                else if (NodeTreeHelper.IsChildBlockList(Node, PropertyName, out Type ChildInterfaceType, out ChildNodeType))
                {
                    IWriteableBlockListInner Inner = (IWriteableBlockListInner)State.PropertyToInner(PropertyName);
                    test(Inner);

                    for (int BlockIndex = 0; BlockIndex < Inner.BlockStateList.Count; BlockIndex++)
                    {
                        IWriteableBlockState BlockState = Inner.BlockStateList[BlockIndex];
                        BrowseNode(controller, BlockState.PatternIndex, test);
                        BrowseNode(controller, BlockState.SourceIndex, test);

                        for (int i = 0; i < BlockState.StateList.Count; i++)
                        {
                            IWriteablePlaceholderNodeState ChildState = BlockState.StateList[i];
                            IWriteableIndex ChildIndex = ChildState.ParentIndex;
                            BrowseNode(controller, ChildIndex, test);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
