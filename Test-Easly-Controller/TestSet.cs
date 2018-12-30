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
using Easly;

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
        /*
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
        }*/

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
                if (NodeTreeHelperChild.IsChildNodeProperty(Node, PropertyName, out ChildNodeType))
                {
                    stats.PlaceholderNodeCount++;

                    IReadOnlyPlaceholderInner Inner = (IReadOnlyPlaceholderInner)State.PropertyToInner(PropertyName);
                    IReadOnlyNodeState ChildState = Inner.ChildState;
                    IReadOnlyIndex ChildIndex = ChildState.ParentIndex;
                    BrowseNode(controller, ChildIndex, stats);
                }

                else if (NodeTreeHelperOptional.IsOptionalChildNodeProperty(Node, PropertyName, out ChildNodeType))
                {
                    stats.OptionalNodeCount++;

                    NodeTreeHelperOptional.GetChildNode(Node, PropertyName, out bool IsAssigned, out INode ChildNode);
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

                else if (NodeTreeHelperList.IsChildNodeList(Node, PropertyName, out ChildNodeType))
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

                else if (NodeTreeHelperBlockList.IsChildBlockList(Node, PropertyName, out Type ChildInterfaceType, out ChildNodeType))
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
        /*
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
        */

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

            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            TestCount = 0;
            BrowseNode(Controller, RootIndex, JustCount);
            MaxTestCount = TestCount;

            for (int i = 0; i < 10; i++)
            {
                TestWriteableInsert(index, rootNode, rand);
                TestWriteableRemove(index, rootNode, rand);
                TestWriteableReplace(index, rootNode, rand);
                TestWriteableAssign(index, rootNode, rand);
                TestWriteableUnassign(index, rootNode, rand);
                TestWriteableChangeReplication(index, rootNode, rand);
                TestWriteableSplit(index, rootNode, rand);
                TestWriteableMerge(index, rootNode, rand);
            }
        }

        static int TestCount = 0;
        static int MaxTestCount = 0;

        public static bool JustCount(IWriteableInner inner)
        {
            TestCount++;
            return true;
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

            TestCount = 0;
            BrowseNode(Controller, RootIndex, (IWriteableInner inner) => InsertAndCompare(ControllerView, rand.Next(MaxTestCount), rand, inner));
        }

        static bool InsertAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (TestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;

            if (inner is IWriteableListInner<IWriteableBrowsingListNodeIndex> AsListInner)
            {
                if (AsListInner.StateList.Count > 0)
                {
                    INode NewNode = NodeHelper.DeepCloneNode(AsListInner.StateList[0].Node);
                    Assert.That(NewNode != null, $"Type: {AsListInner.InterfaceType}");

                    int Index = rand.Next(AsListInner.StateList.Count + 1);
                    IWriteableInsertionListNodeIndex NodeIndex = new WriteableInsertionListNodeIndex(AsListInner.Owner.Node, AsListInner.PropertyName, NewNode, Index);
                    Controller.Insert(AsListInner, NodeIndex, out IWriteableBrowsingCollectionNodeIndex InsertedIndex);
                    Assert.That(Controller.Contains(InsertedIndex));

                    IWriteablePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IWriteablePlaceholderNodeState;
                    Assert.That(ChildState != null);
                    Assert.That(ChildState.Node == NewNode);

                    IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                    Assert.That(NewView.IsEqual(controllerView));
                }
            }
            else if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0 && AsBlockListInner.BlockStateList[0].StateList.Count > 0)
                {
                    INode NewNode = NodeHelper.DeepCloneNode(AsBlockListInner.BlockStateList[0].StateList[0].Node);
                    Assert.That(NewNode != null, $"Type: {AsBlockListInner.InterfaceType}");

                    if (rand.Next(2) == 0)
                    {
                        int BlockIndex = rand.Next(AsBlockListInner.BlockStateList.Count);
                        IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];
                        int Index = rand.Next(BlockState.StateList.Count + 1);

                        IWriteableInsertionExistingBlockNodeIndex NodeIndex = new WriteableInsertionExistingBlockNodeIndex(AsBlockListInner.Owner.Node, AsBlockListInner.PropertyName, NewNode, BlockIndex, Index);
                        Controller.Insert(AsBlockListInner, NodeIndex, out IWriteableBrowsingCollectionNodeIndex InsertedIndex);
                        Assert.That(Controller.Contains(InsertedIndex));

                        IWriteablePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IWriteablePlaceholderNodeState;
                        Assert.That(ChildState != null);
                        Assert.That(ChildState.Node == NewNode);

                        IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                        Assert.That(NewView.IsEqual(controllerView));
                    }
                    else
                    {
                        int BlockIndex = rand.Next(AsBlockListInner.BlockStateList.Count + 1);

                        IPattern ReplicationPattern = NodeHelper.CreateSimplePattern("x");
                        IIdentifier SourceIdentifier = NodeHelper.CreateSimpleIdentifier("y");
                        IWriteableInsertionNewBlockNodeIndex NodeIndex = new WriteableInsertionNewBlockNodeIndex(AsBlockListInner.Owner.Node, AsBlockListInner.PropertyName, NewNode, BlockIndex, ReplicationPattern, SourceIdentifier);
                        Controller.Insert(AsBlockListInner, NodeIndex, out IWriteableBrowsingCollectionNodeIndex InsertedIndex);
                        Assert.That(Controller.Contains(InsertedIndex));

                        IWriteablePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IWriteablePlaceholderNodeState;
                        Assert.That(ChildState != null);
                        Assert.That(ChildState.Node == NewNode);

                        IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                        Assert.That(NewView.IsEqual(controllerView));
                    }
                }
            }

            return false;
        }

        public static void TestWriteableReplace(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            TestCount = 0;
            BrowseNode(Controller, RootIndex, (IWriteableInner inner) => ReplaceAndCompare(ControllerView, rand.Next(MaxTestCount), rand, inner));
        }

        static bool ReplaceAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (TestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;

            if (inner is IWriteablePlaceholderInner<IWriteableBrowsingPlaceholderNodeIndex> AsPlaceholderInner)
            {
                INode NewNode = NodeHelper.DeepCloneNode(AsPlaceholderInner.ChildState.Node);
                Assert.That(NewNode != null, $"Type: {AsPlaceholderInner.InterfaceType}");

                IWriteableInsertionPlaceholderNodeIndex NodeIndex = new WriteableInsertionPlaceholderNodeIndex(AsPlaceholderInner.Owner.Node, AsPlaceholderInner.PropertyName, NewNode);
                Controller.Replace(AsPlaceholderInner, NodeIndex, out IWriteableBrowsingChildIndex InsertedIndex);
                Assert.That(Controller.Contains(InsertedIndex));

                IWriteablePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IWriteablePlaceholderNodeState;
                Assert.That(ChildState != null);
                Assert.That(ChildState.Node == NewNode);

                IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                Assert.That(NewView.IsEqual(controllerView));
            }
            else if (inner is IWriteableOptionalInner<IWriteableBrowsingOptionalNodeIndex> AsOptionalInner)
            {
                IWriteableOptionalNodeState State = AsOptionalInner.ChildState;
                IOptionalReference Optional = State.ParentIndex.Optional;
                Type NodeType = Optional.GetType().GetGenericArguments()[0];
                INode NewNode = NodeHelper.CreateEmptyNode(NodeType);
                Assert.That(NewNode != null, $"Type: {AsOptionalInner.InterfaceType}");

                IWriteableInsertionOptionalNodeIndex NodeIndex = new WriteableInsertionOptionalNodeIndex(AsOptionalInner.Owner.Node, AsOptionalInner.PropertyName, NewNode);
                Controller.Replace(AsOptionalInner, NodeIndex, out IWriteableBrowsingChildIndex InsertedIndex);
                Assert.That(Controller.Contains(InsertedIndex));

                IWriteablePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IWriteablePlaceholderNodeState;
                Assert.That(ChildState != null);
                Assert.That(ChildState.Node == NewNode);

                IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                Assert.That(NewView.IsEqual(controllerView));
            }
            else if (inner is IWriteableListInner<IWriteableBrowsingListNodeIndex> AsListInner)
            {
                if (AsListInner.StateList.Count > 0)
                {
                    INode NewNode = NodeHelper.DeepCloneNode(AsListInner.StateList[0].Node);
                    Assert.That(NewNode != null, $"Type: {AsListInner.InterfaceType}");

                    int Index = rand.Next(AsListInner.StateList.Count);
                    IWriteableInsertionListNodeIndex NodeIndex = new WriteableInsertionListNodeIndex(AsListInner.Owner.Node, AsListInner.PropertyName, NewNode, Index);
                    Controller.Replace(AsListInner, NodeIndex, out IWriteableBrowsingChildIndex InsertedIndex);
                    Assert.That(Controller.Contains(InsertedIndex));

                    IWriteablePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IWriteablePlaceholderNodeState;
                    Assert.That(ChildState != null);
                    Assert.That(ChildState.Node == NewNode);

                    IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                    Assert.That(NewView.IsEqual(controllerView));
                }
            }
            else if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0 && AsBlockListInner.BlockStateList[0].StateList.Count > 0)
                {
                    INode NewNode = NodeHelper.DeepCloneNode(AsBlockListInner.BlockStateList[0].StateList[0].Node);
                    Assert.That(NewNode != null, $"Type: {AsBlockListInner.InterfaceType}");

                    int BlockIndex = rand.Next(AsBlockListInner.BlockStateList.Count);
                    IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];
                    int Index = rand.Next(BlockState.StateList.Count);

                    IWriteableInsertionExistingBlockNodeIndex NodeIndex = new WriteableInsertionExistingBlockNodeIndex(AsBlockListInner.Owner.Node, AsBlockListInner.PropertyName, NewNode, BlockIndex, Index);
                    Controller.Replace(AsBlockListInner, NodeIndex, out IWriteableBrowsingChildIndex InsertedIndex);
                    Assert.That(Controller.Contains(InsertedIndex));

                    IWriteablePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IWriteablePlaceholderNodeState;
                    Assert.That(ChildState != null);
                    Assert.That(ChildState.Node == NewNode);

                    IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                    Assert.That(NewView.IsEqual(controllerView));
                }
            }

            return false;
        }

        public static void TestWriteableRemove(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            TestCount = 0;
            BrowseNode(Controller, RootIndex, (IWriteableInner inner) => RemoveAndCompare(ControllerView, rand.Next(MaxTestCount), rand, inner));
        }

        static bool RemoveAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (TestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;

            if (inner is IWriteableListInner<IWriteableBrowsingListNodeIndex> AsListInner)
            {
                if (AsListInner.StateList.Count > 0)
                {
                    int Index = rand.Next(AsListInner.StateList.Count);
                    IWriteableNodeState ChildState = AsListInner.StateList[Index];
                    IWriteableBrowsingListNodeIndex NodeIndex = ChildState.ParentIndex as IWriteableBrowsingListNodeIndex;
                    Assert.That(NodeIndex != null);

                    Controller.Remove(AsListInner, NodeIndex);

                    IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                    Assert.That(NewView.IsEqual(controllerView));
                }
            }
            else if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0 && AsBlockListInner.BlockStateList[0].StateList.Count > 0)
                {
                    int BlockIndex = rand.Next(AsBlockListInner.BlockStateList.Count);
                    IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];
                    int Index = rand.Next(BlockState.StateList.Count);
                    IWriteableNodeState ChildState = BlockState.StateList[Index];
                    IWriteableBrowsingExistingBlockNodeIndex NodeIndex = ChildState.ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                    Assert.That(NodeIndex != null);

                    Controller.Remove(AsBlockListInner, NodeIndex);

                    IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                    Assert.That(NewView.IsEqual(controllerView));
                }
            }

            return false;
        }

        public static void TestWriteableAssign(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            TestCount = 0;
            BrowseNode(Controller, RootIndex, (IWriteableInner inner) => AssignAndCompare(ControllerView, rand.Next(MaxTestCount), rand, inner));
        }

        static bool AssignAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (TestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;

            if (inner is IWriteableOptionalInner<IWriteableBrowsingOptionalNodeIndex> AsOptionalInner)
            {
                IWriteableOptionalNodeState ChildState = AsOptionalInner.ChildState;
                Assert.That(ChildState != null);

                IWriteableBrowsingOptionalNodeIndex OptionalIndex = ChildState.ParentIndex;
                Assert.That(Controller.Contains(OptionalIndex));

                IOptionalReference Optional = OptionalIndex.Optional;
                Assert.That(Optional != null);

                if (Optional.HasItem)
                {
                    Controller.Assign(AsOptionalInner);
                    Assert.That(Optional.IsAssigned);
                    Assert.That(AsOptionalInner.IsAssigned);
                    Assert.That(Optional.Item == ChildState.Node);

                    IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                    Assert.That(NewView.IsEqual(controllerView));
                }
            }

            return false;
        }

        public static void TestWriteableUnassign(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            TestCount = 0;
            BrowseNode(Controller, RootIndex, (IWriteableInner inner) => UnassignAndCompare(ControllerView, rand.Next(MaxTestCount), rand, inner));
        }

        static bool UnassignAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (TestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;

            if (inner is IWriteableOptionalInner<IWriteableBrowsingOptionalNodeIndex> AsOptionalInner)
            {
                IWriteableOptionalNodeState ChildState = AsOptionalInner.ChildState;
                Assert.That(ChildState != null);

                IWriteableBrowsingOptionalNodeIndex OptionalIndex = ChildState.ParentIndex;
                Assert.That(Controller.Contains(OptionalIndex));

                IOptionalReference Optional = OptionalIndex.Optional;
                Assert.That(Optional != null);

                Controller.Unassign(AsOptionalInner);
                Assert.That(!Optional.IsAssigned);
                Assert.That(!AsOptionalInner.IsAssigned);

                IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                Assert.That(NewView.IsEqual(controllerView));
            }

            return false;
        }

        public static void TestWriteableChangeReplication(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            TestCount = 0;
            BrowseNode(Controller, RootIndex, (IWriteableInner inner) => ChangeReplicationAndCompare(ControllerView, rand.Next(MaxTestCount), rand, inner));
        }

        static bool ChangeReplicationAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (TestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;

            if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0)
                {
                    int BlockIndex = rand.Next(AsBlockListInner.BlockStateList.Count);
                    IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];

                    ReplicationStatus Replication = (ReplicationStatus)rand.Next(2);
                    Controller.ChangeReplication(AsBlockListInner, BlockIndex, Replication);

                    IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                    Assert.That(NewView.IsEqual(controllerView));
                }
            }

            return false;
        }

        public static void TestWriteableSplit(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            TestCount = 0;
            BrowseNode(Controller, RootIndex, (IWriteableInner inner) => SplitAndCompare(ControllerView, rand.Next(MaxTestCount), rand, inner));
        }

        static bool SplitAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (TestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;

            if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0)
                {
                    int SplitBlockIndex = rand.Next(AsBlockListInner.BlockStateList.Count);
                    IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[SplitBlockIndex];
                    if (BlockState.StateList.Count > 1)
                    {
                        int SplitIndex = 1 + rand.Next(BlockState.StateList.Count - 1);
                        IWriteableBrowsingExistingBlockNodeIndex NodeIndex = (IWriteableBrowsingExistingBlockNodeIndex)AsBlockListInner.IndexAt(SplitBlockIndex, SplitIndex);
                        Controller.SplitBlock(AsBlockListInner, NodeIndex);

                        IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                        Assert.That(NewView.IsEqual(controllerView));
                    }
                }
            }

            return false;
        }

        public static void TestWriteableMerge(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            TestCount = 0;
            BrowseNode(Controller, RootIndex, (IWriteableInner inner) => MergeAndCompare(ControllerView, rand.Next(MaxTestCount), rand, inner));
        }

        static bool MergeAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (TestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;

            if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 1)
                {
                    int MergeBlockIndex = 1 + rand.Next(AsBlockListInner.BlockStateList.Count - 1);
                    IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[MergeBlockIndex];

                    IWriteableBrowsingExistingBlockNodeIndex NodeIndex = (IWriteableBrowsingExistingBlockNodeIndex)AsBlockListInner.IndexAt(MergeBlockIndex, 0);
                    Controller.MergeBlocks(AsBlockListInner, NodeIndex);

                    IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                    Assert.That(NewView.IsEqual(controllerView));
                }
            }

            return false;
        }

        static bool BrowseNode(IWriteableController controller, IWriteableIndex index, Func<IWriteableInner, bool> test)
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
                if (NodeTreeHelperChild.IsChildNodeProperty(Node, PropertyName, out ChildNodeType))
                {
                    IWriteablePlaceholderInner Inner = (IWriteablePlaceholderInner)State.PropertyToInner(PropertyName);
                    IWriteableNodeState ChildState = Inner.ChildState;
                    IWriteableIndex ChildIndex = ChildState.ParentIndex;
                    if (!BrowseNode(controller, ChildIndex, test))
                        return false;
                }

                else if (NodeTreeHelperOptional.IsOptionalChildNodeProperty(Node, PropertyName, out ChildNodeType))
                {
                    NodeTreeHelperOptional.GetChildNode(Node, PropertyName, out bool IsAssigned, out INode ChildNode);
                    if (IsAssigned)
                    {
                        IWriteableOptionalInner Inner = (IWriteableOptionalInner)State.PropertyToInner(PropertyName);
                        IWriteableNodeState ChildState = Inner.ChildState;
                        IWriteableIndex ChildIndex = ChildState.ParentIndex;
                        if (!BrowseNode(controller, ChildIndex, test))
                            return false;
                    }
                }

                else if (NodeTreeHelperList.IsChildNodeList(Node, PropertyName, out ChildNodeType))
                {
                    IWriteableListInner Inner = (IWriteableListInner)State.PropertyToInner(PropertyName);
                    if (!test(Inner))
                        return false;

                    for (int i = 0; i < Inner.StateList.Count; i++)
                    {
                        IWriteablePlaceholderNodeState ChildState = Inner.StateList[i];
                        IWriteableIndex ChildIndex = ChildState.ParentIndex;
                        if (!BrowseNode(controller, ChildIndex, test))
                            return false;
                    }
                }

                else if (NodeTreeHelperBlockList.IsChildBlockList(Node, PropertyName, out Type ChildInterfaceType, out ChildNodeType))
                {
                    IWriteableBlockListInner Inner = (IWriteableBlockListInner)State.PropertyToInner(PropertyName);
                    if (!test(Inner))
                        return false;

                    for (int BlockIndex = 0; BlockIndex < Inner.BlockStateList.Count; BlockIndex++)
                    {
                        IWriteableBlockState BlockState = Inner.BlockStateList[BlockIndex];
                        if (!BrowseNode(controller, BlockState.PatternIndex, test))
                            return false;
                        if (!BrowseNode(controller, BlockState.SourceIndex, test))
                            return false;

                        for (int i = 0; i < BlockState.StateList.Count; i++)
                        {
                            IWriteablePlaceholderNodeState ChildState = BlockState.StateList[i];
                            IWriteableIndex ChildIndex = ChildState.ParentIndex;
                            if (!BrowseNode(controller, ChildIndex, test))
                                return false;
                        }
                    }
                }
            }

            return true;
        }
        #endregion
    }
}
