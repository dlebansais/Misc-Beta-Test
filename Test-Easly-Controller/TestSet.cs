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
using EaslyController.Frame;
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

            FileNameTable = new List<string>();
            FirstRootNode = null;
            AddEaslyFiles(RootPath);
        }

        static void AddEaslyFiles(string path)
        {
            foreach (string FileName in Directory.GetFiles(path, "*.easly"))
            {
                FileNameTable.Add(FileName.Replace("\\", "/"));

                if (FirstRootNode == null)
                {
                    using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                    {
                        Serializer Serializer = new Serializer();
                        INode RootNode = Serializer.Deserialize(fs) as INode;

                        FirstRootNode = RootNode;
                    }
                }
            }

            foreach (string Folder in Directory.GetDirectories(path))
                AddEaslyFiles(Folder);
        }

        static IEnumerable<int> FileIndexRange()
        {
            for (int i = 0; i < 172; i++)
                yield return i;
        }

        //static int RandValue = 0;
        static int RandNext(Random rand, int maxValue)
        {
            return rand.Next(maxValue);

            /*if (maxValue == 2)
                RandValue++;
            else
                RandValue += 3;
            if (RandValue >= maxValue)
                RandValue = 0;

            return RandValue;*/
        }

        static List<string> FileNameTable;
        static INode FirstRootNode;
        #endregion

        static bool TestOff = false;

        #region Sanity Check
        [Test]
        public static void TestInit()
        {
            if (TestOff)
                return;

            ControllerTools.ResetExpectedName();

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
        #if READONLY
        [Test]
        [TestCaseSource(nameof(FileIndexRange))]
        public static void ReadOnly(int index)
        {
            if (TestOff)
                return;

            string Name = null;
            INode RootNode = null;
            int n = index;
            foreach (string FileName in FileNameTable)
            {
                if (n == 0)
                {
                    using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                    {
                        Name = FileName;
                        Serializer Serializer = new Serializer();
                        RootNode = Serializer.Deserialize(fs) as INode;
                    }
                    break;
                }

                n--;
            }

            if (n > 0)
                throw new ArgumentOutOfRangeException($"{n} / {FileNameTable.Count}");
            TestReadOnly(index, Name, RootNode);
        }
        #endif

        public static void TestReadOnly(int index, string name, INode rootNode)
        {
            ControllerTools.ResetExpectedName();

            TestReadOnlyStats(index, name, rootNode);
        }

        public static void TestReadOnlyStats(int index, string name, INode rootNode)
        {
            IReadOnlyRootNodeIndex RootIndex = new ReadOnlyRootNodeIndex(rootNode);
            IReadOnlyController Controller = ReadOnlyController.Create(RootIndex);

            Stats Stats = new Stats();
            BrowseNode(Controller, RootIndex, Stats);

            if (name.EndsWith("test.easly"))
            {
                const int ExpectedNodeCount = 155;
                const int ExpectedPlaceholderNodeCount = 142;
                const int ExpectedOptionalNodeCount = 12;
                const int ExpectedAssignedOptionalNodeCount = 4;
                const int ExpectedListCount = 5;
                const int ExpectedBlockListCount = 96;

                Assert.That(Stats.NodeCount == ExpectedNodeCount, $"{name} - Failed to browse tree. Expected: {ExpectedNodeCount} node(s), Found: {Stats.NodeCount}");
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

                else if (NodeTreeHelperList.IsNodeListProperty(Node, PropertyName, out ChildNodeType))
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

                else if (NodeTreeHelperBlockList.IsBlockListProperty(Node, PropertyName, out Type ChildInterfaceType, out ChildNodeType))
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
        #if VIEWS
        [Test]
        [TestCaseSource(nameof(FileIndexRange))]
        public static void StateViews(int index)
        {
            if (TestOff)
                return;

            string Name = null;
            INode RootNode = null;
            int n = index;
            foreach (string FileName in FileNameTable)
            {
                if (n == 0)
                {
                    using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                    {
                        Name = FileName;
                        Serializer Serializer = new Serializer();
                        RootNode = Serializer.Deserialize(fs) as INode;
                    }
                    break;
                }

                n--;
            }

            if (n > 0)
                throw new ArgumentOutOfRangeException($"{n} / {FileNameTable.Count}");
            TestStateView(index, RootNode);
        }
        #endif

        public static void TestStateView(int index, INode rootNode)
        {
            ControllerTools.ResetExpectedName();

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
            Assert.That(ControllerView2.IsEqual(CompareEqual.New(), ControllerView), $"Views #4");
        }
        #endregion

        #region Writeable
        #if WRITEABLE
        [Test]
        [TestCaseSource(nameof(FileIndexRange))]
        public static void Writeable(int index)
        {
            if (TestOff)
                return;

            string Name = null;
            INode RootNode = null;
            int n = index;
            foreach (string FileName in FileNameTable)
            {
                if (n == 0)
                {
                    using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                    {
                        Name = FileName;
                        Serializer Serializer = new Serializer();
                        RootNode = Serializer.Deserialize(fs) as INode;
                    }
                    break;
                }

                n--;
            }

            if (n > 0)
                throw new ArgumentOutOfRangeException($"{n} / {FileNameTable.Count}");
            TestWriteable(index, Name, RootNode);
        }
        #endif
        public static void TestWriteable(int index, string name, INode rootNode)
        {
            ControllerTools.ResetExpectedName();

            TestWriteableStats(index, name, rootNode, out Stats Stats);

            Random rand = new Random(0x123456);

            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            WriteableTestCount = 0;
            WriteableBrowseNode(Controller, RootIndex, JustCount);
            WriteableMaxTestCount = WriteableTestCount;

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
                TestWriteableMove(index, rootNode, rand);
                TestWriteableExpand(index, rootNode, rand);
                TestWriteableReduce(index, rootNode, rand);
            }

            TestWriteableCanonicalize(rootNode);
        }

        public static void TestWriteableCanonicalize(INode rootNode)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            Controller.Canonicalize();

            IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
            Assert.That(NewView.IsEqual(CompareEqual.New(), ControllerView));

            IWriteableRootNodeIndex NewRootIndex = new WriteableRootNodeIndex(Controller.RootIndex.Node);
            IWriteableController NewController = WriteableController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
        }

        static int WriteableTestCount = 0;
        static int WriteableMaxTestCount = 0;

        public static bool JustCount(IWriteableInner inner)
        {
            WriteableTestCount++;
            return true;
        }

        public static void TestWriteableStats(int index, string name, INode rootNode, out Stats stats)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);

            stats = new Stats();
            BrowseNode(Controller, RootIndex, stats);

            if (name.EndsWith("test.easly"))
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

            WriteableTestCount = 0;
            WriteableBrowseNode(Controller, RootIndex, (IWriteableInner inner) => InsertAndCompare(ControllerView, RandNext(rand, WriteableMaxTestCount), rand, inner));
        }

        static bool InsertAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (WriteableTestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;
            bool IsModified = false;

            if (inner is IWriteableListInner<IWriteableBrowsingListNodeIndex> AsListInner)
            {
                if (AsListInner.StateList.Count > 0)
                {
                    INode NewNode = NodeHelper.DeepCloneNode(AsListInner.StateList[0].Node);
                    Assert.That(NewNode != null, $"Type: {AsListInner.InterfaceType}");

                    int Index = RandNext(rand, AsListInner.StateList.Count + 1);
                    IWriteableInsertionListNodeIndex NodeIndex = new WriteableInsertionListNodeIndex(AsListInner.Owner.Node, AsListInner.PropertyName, NewNode, Index);
                    Controller.Insert(AsListInner, NodeIndex, out IWriteableBrowsingCollectionNodeIndex InsertedIndex);
                    Assert.That(Controller.Contains(InsertedIndex));

                    IWriteablePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IWriteablePlaceholderNodeState;
                    Assert.That(ChildState != null);
                    Assert.That(ChildState.Node == NewNode);

                    IsModified = true;
                }
            }
            else if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0 && AsBlockListInner.BlockStateList[0].StateList.Count > 0)
                {
                    INode NewNode = NodeHelper.DeepCloneNode(AsBlockListInner.BlockStateList[0].StateList[0].Node);
                    Assert.That(NewNode != null, $"Type: {AsBlockListInner.InterfaceType}");

                    if (RandNext(rand, 2) == 0)
                    {
                        int BlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                        IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];
                        int Index = RandNext(rand, BlockState.StateList.Count + 1);

                        IWriteableInsertionExistingBlockNodeIndex NodeIndex = new WriteableInsertionExistingBlockNodeIndex(AsBlockListInner.Owner.Node, AsBlockListInner.PropertyName, NewNode, BlockIndex, Index);
                        Controller.Insert(AsBlockListInner, NodeIndex, out IWriteableBrowsingCollectionNodeIndex InsertedIndex);
                        Assert.That(Controller.Contains(InsertedIndex));

                        IWriteablePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IWriteablePlaceholderNodeState;
                        Assert.That(ChildState != null);
                        Assert.That(ChildState.Node == NewNode);

                        IsModified = true;
                    }
                    else
                    {
                        int BlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count + 1);

                        IPattern ReplicationPattern = NodeHelper.CreateSimplePattern("x");
                        IIdentifier SourceIdentifier = NodeHelper.CreateSimpleIdentifier("y");
                        IWriteableInsertionNewBlockNodeIndex NodeIndex = new WriteableInsertionNewBlockNodeIndex(AsBlockListInner.Owner.Node, AsBlockListInner.PropertyName, NewNode, BlockIndex, ReplicationPattern, SourceIdentifier);
                        Controller.Insert(AsBlockListInner, NodeIndex, out IWriteableBrowsingCollectionNodeIndex InsertedIndex);
                        Assert.That(Controller.Contains(InsertedIndex));

                        IWriteablePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IWriteablePlaceholderNodeState;
                        Assert.That(ChildState != null);
                        Assert.That(ChildState.Node == NewNode);

                        IsModified = true;
                    }
                }
            }

            if (IsModified)
            {
                IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                IWriteableRootNodeIndex NewRootIndex = new WriteableRootNodeIndex(Controller.RootIndex.Node);
                IWriteableController NewController = WriteableController.Create(NewRootIndex);
                Assert.That(NewController.IsEqual(CompareEqual.New(), Controller), $"Inner: {inner.PropertyName}, Owner: {inner.Owner.Node}");
            }

            return false;
        }

        public static void TestWriteableReplace(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            WriteableTestCount = 0;
            WriteableBrowseNode(Controller, RootIndex, (IWriteableInner inner) => ReplaceAndCompare(ControllerView, RandNext(rand, WriteableMaxTestCount), rand, inner));
        }

        static bool ReplaceAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (WriteableTestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;
            bool IsModified = false;

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

                IsModified = true;
            }
            else if (inner is IWriteableOptionalInner<IWriteableBrowsingOptionalNodeIndex> AsOptionalInner)
            {
                IWriteableOptionalNodeState State = AsOptionalInner.ChildState;
                IOptionalReference Optional = State.ParentIndex.Optional;
                Type NodeInterfaceType = Optional.GetType().GetGenericArguments()[0];
                INode NewNode = NodeHelper.CreateDefaultFromInterface(NodeInterfaceType);
                Assert.That(NewNode != null, $"Type: {AsOptionalInner.InterfaceType}");

                IWriteableInsertionOptionalNodeIndex NodeIndex = new WriteableInsertionOptionalNodeIndex(AsOptionalInner.Owner.Node, AsOptionalInner.PropertyName, NewNode);
                Controller.Replace(AsOptionalInner, NodeIndex, out IWriteableBrowsingChildIndex InsertedIndex);
                Assert.That(Controller.Contains(InsertedIndex));

                IWriteableOptionalNodeState ChildState = Controller.IndexToState(InsertedIndex) as IWriteableOptionalNodeState;
                Assert.That(ChildState != null);
                Assert.That(ChildState.Node == NewNode);

                IsModified = true;
            }
            else if (inner is IWriteableListInner<IWriteableBrowsingListNodeIndex> AsListInner)
            {
                if (AsListInner.StateList.Count > 0)
                {
                    INode NewNode = NodeHelper.DeepCloneNode(AsListInner.StateList[0].Node);
                    Assert.That(NewNode != null, $"Type: {AsListInner.InterfaceType}");

                    int Index = RandNext(rand, AsListInner.StateList.Count);
                    IWriteableInsertionListNodeIndex NodeIndex = new WriteableInsertionListNodeIndex(AsListInner.Owner.Node, AsListInner.PropertyName, NewNode, Index);
                    Controller.Replace(AsListInner, NodeIndex, out IWriteableBrowsingChildIndex InsertedIndex);
                    Assert.That(Controller.Contains(InsertedIndex));

                    IWriteablePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IWriteablePlaceholderNodeState;
                    Assert.That(ChildState != null);
                    Assert.That(ChildState.Node == NewNode);

                    IsModified = true;
                }
            }
            else if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0 && AsBlockListInner.BlockStateList[0].StateList.Count > 0)
                {
                    INode NewNode = NodeHelper.DeepCloneNode(AsBlockListInner.BlockStateList[0].StateList[0].Node);
                    Assert.That(NewNode != null, $"Type: {AsBlockListInner.InterfaceType}");

                    int BlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                    IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];
                    int Index = RandNext(rand, BlockState.StateList.Count);

                    IWriteableInsertionExistingBlockNodeIndex NodeIndex = new WriteableInsertionExistingBlockNodeIndex(AsBlockListInner.Owner.Node, AsBlockListInner.PropertyName, NewNode, BlockIndex, Index);
                    Controller.Replace(AsBlockListInner, NodeIndex, out IWriteableBrowsingChildIndex InsertedIndex);
                    Assert.That(Controller.Contains(InsertedIndex));

                    IWriteablePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IWriteablePlaceholderNodeState;
                    Assert.That(ChildState != null);
                    Assert.That(ChildState.Node == NewNode);

                    IsModified = true;
                }
            }

            if (IsModified)
            {
                IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                IWriteableRootNodeIndex NewRootIndex = new WriteableRootNodeIndex(Controller.RootIndex.Node);
                IWriteableController NewController = WriteableController.Create(NewRootIndex);
                Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
            }

            return false;
        }

        public static void TestWriteableRemove(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            WriteableTestCount = 0;
            WriteableBrowseNode(Controller, RootIndex, (IWriteableInner inner) => RemoveAndCompare(ControllerView, RandNext(rand, WriteableMaxTestCount), rand, inner));
        }

        static bool RemoveAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (WriteableTestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;
            bool IsModified = false;

            if (inner is IWriteableListInner<IWriteableBrowsingListNodeIndex> AsListInner)
            {
                if (AsListInner.StateList.Count > 0)
                {
                    int Index = RandNext(rand, AsListInner.StateList.Count);
                    IWriteableNodeState ChildState = AsListInner.StateList[Index];
                    IWriteableBrowsingListNodeIndex NodeIndex = ChildState.ParentIndex as IWriteableBrowsingListNodeIndex;
                    Assert.That(NodeIndex != null);

                    Controller.Remove(AsListInner, NodeIndex);

                    IsModified = true;
                }
            }
            else if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0 && AsBlockListInner.BlockStateList[0].StateList.Count > 0)
                {
                    int BlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                    IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];
                    int Index = RandNext(rand, BlockState.StateList.Count);
                    IWriteableNodeState ChildState = BlockState.StateList[Index];
                    IWriteableBrowsingExistingBlockNodeIndex NodeIndex = ChildState.ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                    Assert.That(NodeIndex != null);

                    Controller.Remove(AsBlockListInner, NodeIndex);

                    IsModified = true;
                }
            }

            if (IsModified)
            {
                IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                IWriteableRootNodeIndex NewRootIndex = new WriteableRootNodeIndex(Controller.RootIndex.Node);
                IWriteableController NewController = WriteableController.Create(NewRootIndex);
                Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
            }

            return false;
        }

        public static void TestWriteableAssign(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            WriteableTestCount = 0;
            WriteableBrowseNode(Controller, RootIndex, (IWriteableInner inner) => AssignAndCompare(ControllerView, RandNext(rand, WriteableMaxTestCount), rand, inner));
        }

        static bool AssignAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (WriteableTestCount++ < TestIndex)
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
                    Controller.Assign(OptionalIndex);
                    Assert.That(Optional.IsAssigned);
                    Assert.That(AsOptionalInner.IsAssigned);
                    Assert.That(Optional.Item == ChildState.Node);

                    IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                    Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                    IWriteableRootNodeIndex NewRootIndex = new WriteableRootNodeIndex(Controller.RootIndex.Node);
                    IWriteableController NewController = WriteableController.Create(NewRootIndex);
                    Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
                }
            }

            return false;
        }

        public static void TestWriteableUnassign(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            WriteableTestCount = 0;
            WriteableBrowseNode(Controller, RootIndex, (IWriteableInner inner) => UnassignAndCompare(ControllerView, RandNext(rand, WriteableMaxTestCount), rand, inner));
        }

        static bool UnassignAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (WriteableTestCount++ < TestIndex)
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

                Controller.Unassign(OptionalIndex);
                Assert.That(!Optional.IsAssigned);
                Assert.That(!AsOptionalInner.IsAssigned);

                IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                IWriteableRootNodeIndex NewRootIndex = new WriteableRootNodeIndex(Controller.RootIndex.Node);
                IWriteableController NewController = WriteableController.Create(NewRootIndex);
                Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
            }

            return false;
        }

        public static void TestWriteableChangeReplication(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            WriteableTestCount = 0;
            WriteableBrowseNode(Controller, RootIndex, (IWriteableInner inner) => ChangeReplicationAndCompare(ControllerView, RandNext(rand, WriteableMaxTestCount), rand, inner));
        }

        static bool ChangeReplicationAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (WriteableTestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;

            if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0)
                {
                    int BlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                    IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];

                    ReplicationStatus Replication = (ReplicationStatus)RandNext(rand, 2);
                    Controller.ChangeReplication(AsBlockListInner, BlockIndex, Replication);

                    IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                    Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                    IWriteableRootNodeIndex NewRootIndex = new WriteableRootNodeIndex(Controller.RootIndex.Node);
                    IWriteableController NewController = WriteableController.Create(NewRootIndex);
                    Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
                }
            }

            return false;
        }

        public static void TestWriteableSplit(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            WriteableTestCount = 0;
            WriteableBrowseNode(Controller, RootIndex, (IWriteableInner inner) => SplitAndCompare(ControllerView, RandNext(rand, WriteableMaxTestCount), rand, inner));
        }

        static bool SplitAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (WriteableTestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;

            if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0)
                {
                    int SplitBlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                    IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[SplitBlockIndex];
                    if (BlockState.StateList.Count > 1)
                    {
                        int SplitIndex = 1 + RandNext(rand, BlockState.StateList.Count - 1);
                        IWriteableBrowsingExistingBlockNodeIndex NodeIndex = (IWriteableBrowsingExistingBlockNodeIndex)AsBlockListInner.IndexAt(SplitBlockIndex, SplitIndex);
                        Controller.SplitBlock(AsBlockListInner, NodeIndex);

                        IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                        Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                        IWriteableRootNodeIndex NewRootIndex = new WriteableRootNodeIndex(Controller.RootIndex.Node);
                        IWriteableController NewController = WriteableController.Create(NewRootIndex);
                        Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

                        Assert.That(AsBlockListInner.BlockStateList.Count > 0);
                        int OldBlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                        int NewBlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                        int Direction = NewBlockIndex - OldBlockIndex;
                        Controller.MoveBlock(AsBlockListInner, OldBlockIndex, Direction);

                        IWriteableControllerView NewViewAfterMove = WriteableControllerView.Create(Controller);
                        Assert.That(NewViewAfterMove.IsEqual(CompareEqual.New(), controllerView));

                        IWriteableRootNodeIndex NewRootIndexAfterMove = new WriteableRootNodeIndex(Controller.RootIndex.Node);
                        IWriteableController NewControllerAfterMove = WriteableController.Create(NewRootIndexAfterMove);
                        Assert.That(NewControllerAfterMove.IsEqual(CompareEqual.New(), Controller));
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

            WriteableTestCount = 0;
            WriteableBrowseNode(Controller, RootIndex, (IWriteableInner inner) => MergeAndCompare(ControllerView, RandNext(rand, WriteableMaxTestCount), rand, inner));
        }

        static bool MergeAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (WriteableTestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;

            if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 1)
                {
                    int MergeBlockIndex = 1 + RandNext(rand, AsBlockListInner.BlockStateList.Count - 1);
                    IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[MergeBlockIndex];

                    IWriteableBrowsingExistingBlockNodeIndex NodeIndex = (IWriteableBrowsingExistingBlockNodeIndex)AsBlockListInner.IndexAt(MergeBlockIndex, 0);
                    Controller.MergeBlocks(AsBlockListInner, NodeIndex);

                    IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                    Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                    IWriteableRootNodeIndex NewRootIndex = new WriteableRootNodeIndex(Controller.RootIndex.Node);
                    IWriteableController NewController = WriteableController.Create(NewRootIndex);
                    Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
                }
            }

            return false;
        }

        public static void TestWriteableMove(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            WriteableTestCount = 0;
            WriteableBrowseNode(Controller, RootIndex, (IWriteableInner inner) => MoveAndCompare(ControllerView, RandNext(rand, WriteableMaxTestCount), rand, inner));
        }

        static bool MoveAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (WriteableTestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;
            bool IsModified = false;

            if (inner is IWriteableListInner<IWriteableBrowsingListNodeIndex> AsListInner)
            {
                if (AsListInner.StateList.Count > 0)
                {
                    int OldIndex = RandNext(rand, AsListInner.StateList.Count);
                    int NewIndex = RandNext(rand, AsListInner.StateList.Count);
                    int Direction = NewIndex - OldIndex;

                    IWriteableBrowsingListNodeIndex NodeIndex = AsListInner.IndexAt(OldIndex) as IWriteableBrowsingListNodeIndex;
                    Assert.That(NodeIndex != null);

                    Controller.Move(AsListInner, NodeIndex, Direction);
                    Assert.That(Controller.Contains(NodeIndex));

                    IsModified = true;
                }
            }
            else if (inner is IWriteableBlockListInner<IWriteableBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0)
                {
                    int BlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                    IWriteableBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];

                    if (BlockState.StateList.Count > 0)
                    {
                        int OldIndex = RandNext(rand, BlockState.StateList.Count);
                        int NewIndex = RandNext(rand, BlockState.StateList.Count);
                        int Direction = NewIndex - OldIndex;

                        IWriteableBrowsingExistingBlockNodeIndex NodeIndex = AsBlockListInner.IndexAt(BlockIndex, OldIndex) as IWriteableBrowsingExistingBlockNodeIndex;
                        Assert.That(NodeIndex != null);

                        Controller.Move(AsBlockListInner, NodeIndex, Direction);
                        Assert.That(Controller.Contains(NodeIndex));

                        IsModified = true;
                    }
                }
            }

            if (IsModified)
            {
                IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
                Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                IWriteableRootNodeIndex NewRootIndex = new WriteableRootNodeIndex(Controller.RootIndex.Node);
                IWriteableController NewController = WriteableController.Create(NewRootIndex);
                Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
            }

            return false;
        }

        public static void TestWriteableExpand(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            WriteableTestCount = 0;
            WriteableBrowseNode(Controller, RootIndex, (IWriteableInner inner) => ExpandAndCompare(ControllerView, RandNext(rand, WriteableMaxTestCount), rand, inner));
        }

        static bool ExpandAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (WriteableTestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;
            IWriteableNodeIndex NodeIndex;
            IWriteablePlaceholderNodeState State;

            if (inner is IWriteablePlaceholderInner<IWriteableBrowsingPlaceholderNodeIndex> AsPlaceholderInner)
            {
                NodeIndex = AsPlaceholderInner.ChildState.ParentIndex as IWriteableNodeIndex;
                Assert.That(NodeIndex != null);

                State = Controller.IndexToState(NodeIndex) as IWriteablePlaceholderNodeState;
                Assert.That(State != null);

                NodeTreeHelper.GetArgumentBlocks(State.Node, out IDictionary<string, IBlockList<IArgument, Argument>> ArgumentBlocksTable);
                if (ArgumentBlocksTable.Count == 0)
                    return true;
            }
            else
                return true;

            Controller.Expand(NodeIndex);

            IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
            Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

            IWriteableRootNodeIndex NewRootIndex = new WriteableRootNodeIndex(Controller.RootIndex.Node);
            IWriteableController NewController = WriteableController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

            Controller.Expand(NodeIndex);

            NewController = WriteableController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

            Controller.Reduce(NodeIndex);

            NewController = WriteableController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

            return false;
        }

        public static void TestWriteableReduce(int index, INode rootNode, Random rand)
        {
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(rootNode);
            IWriteableController Controller = WriteableController.Create(RootIndex);
            IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller);

            WriteableTestCount = 0;
            WriteableBrowseNode(Controller, RootIndex, (IWriteableInner inner) => ReduceAndCompare(ControllerView, RandNext(rand, WriteableMaxTestCount), rand, inner));
        }

        static bool ReduceAndCompare(IWriteableControllerView controllerView, int TestIndex, Random rand, IWriteableInner inner)
        {
            if (WriteableTestCount++ < TestIndex)
                return true;

            IWriteableController Controller = controllerView.Controller;
            IWriteableNodeIndex NodeIndex;
            IWriteablePlaceholderNodeState State;

            if (inner is IWriteablePlaceholderInner<IWriteableBrowsingPlaceholderNodeIndex> AsPlaceholderInner)
            {
                NodeIndex = AsPlaceholderInner.ChildState.ParentIndex as IWriteableNodeIndex;
                Assert.That(NodeIndex != null);

                State = Controller.IndexToState(NodeIndex) as IWriteablePlaceholderNodeState;
                Assert.That(State != null);

                NodeTreeHelper.GetArgumentBlocks(State.Node, out IDictionary<string, IBlockList<IArgument, Argument>> ArgumentBlocksTable);
                if (ArgumentBlocksTable.Count == 0)
                    return true;
            }
            else
                return true;

            Controller.Reduce(NodeIndex);

            IWriteableControllerView NewView = WriteableControllerView.Create(Controller);
            Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

            IWriteableRootNodeIndex NewRootIndex = new WriteableRootNodeIndex(Controller.RootIndex.Node);
            IWriteableController NewController = WriteableController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

            Controller.Reduce(NodeIndex);

            NewController = WriteableController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

            Controller.Expand(NodeIndex);

            NewController = WriteableController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

            return false;
        }

        static bool WriteableBrowseNode(IWriteableController controller, IWriteableIndex index, Func<IWriteableInner, bool> test)
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
                    if (!test(Inner))
                        return false;

                    IWriteableNodeState ChildState = Inner.ChildState;
                    IWriteableIndex ChildIndex = ChildState.ParentIndex;
                    if (!WriteableBrowseNode(controller, ChildIndex, test))
                        return false;
                }

                else if (NodeTreeHelperOptional.IsOptionalChildNodeProperty(Node, PropertyName, out ChildNodeType))
                {
                    NodeTreeHelperOptional.GetChildNode(Node, PropertyName, out bool IsAssigned, out INode ChildNode);
                    if (IsAssigned)
                    {
                        IWriteableOptionalInner Inner = (IWriteableOptionalInner)State.PropertyToInner(PropertyName);
                        if (!test(Inner))
                            return false;

                        IWriteableNodeState ChildState = Inner.ChildState;
                        IWriteableIndex ChildIndex = ChildState.ParentIndex;
                        if (!WriteableBrowseNode(controller, ChildIndex, test))
                            return false;
                    }
                }

                else if (NodeTreeHelperList.IsNodeListProperty(Node, PropertyName, out ChildNodeType))
                {
                    IWriteableListInner Inner = (IWriteableListInner)State.PropertyToInner(PropertyName);
                    if (!test(Inner))
                        return false;

                    for (int i = 0; i < Inner.StateList.Count; i++)
                    {
                        IWriteablePlaceholderNodeState ChildState = Inner.StateList[i];
                        IWriteableIndex ChildIndex = ChildState.ParentIndex;
                        if (!WriteableBrowseNode(controller, ChildIndex, test))
                            return false;
                    }
                }

                else if (NodeTreeHelperBlockList.IsBlockListProperty(Node, PropertyName, out Type ChildInterfaceType, out ChildNodeType))
                {
                    IWriteableBlockListInner Inner = (IWriteableBlockListInner)State.PropertyToInner(PropertyName);
                    if (!test(Inner))
                        return false;

                    for (int BlockIndex = 0; BlockIndex < Inner.BlockStateList.Count; BlockIndex++)
                    {
                        IWriteableBlockState BlockState = Inner.BlockStateList[BlockIndex];
                        if (!WriteableBrowseNode(controller, BlockState.PatternIndex, test))
                            return false;
                        if (!WriteableBrowseNode(controller, BlockState.SourceIndex, test))
                            return false;

                        for (int i = 0; i < BlockState.StateList.Count; i++)
                        {
                            IWriteablePlaceholderNodeState ChildState = BlockState.StateList[i];
                            IWriteableIndex ChildIndex = ChildState.ParentIndex;
                            if (!WriteableBrowseNode(controller, ChildIndex, test))
                                return false;
                        }
                    }
                }
            }

            return true;
        }
        #endregion

        #region Frame
        #if FRAME
        [Test]
        [TestCaseSource(nameof(FileIndexRange))]
        public static void Frame(int index)
        {
            if (TestOff)
                return;

            string Name = null;
            INode RootNode = null;
            int n = index;
            foreach (string FileName in FileNameTable)
            {
                if (n == 0)
                {
                    using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                    {
                        Name = FileName;
                        Serializer Serializer = new Serializer();
                        RootNode = Serializer.Deserialize(fs) as INode;
                    }
                    break;
                }

                n--;
            }

            if (n > 0)
                throw new ArgumentOutOfRangeException($"{n} / {FileNameTable.Count}");
            TestFrame(index, Name, RootNode);
        }
#endif

        public static void TestFrame(int index, string name, INode rootNode)
        {
            ControllerTools.ResetExpectedName();

            TestFrameStats(index, name, rootNode, out Stats Stats);

            Random rand = new Random(0x123456);

            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);

            IFrameControllerView ControllerView;

            if (TestDebug.CustomTemplateSet.FrameTemplateSet != null)
            {
                ControllerView = FrameControllerView.Create(Controller, TestDebug.CustomTemplateSet.FrameTemplateSet);

                if (ExpectedLastLineTable.ContainsKey(name))
                {
                    int ExpectedLastLineNumber = ExpectedLastLineTable[name];
                    Assert.That(ControllerView.LastLineNumber == ExpectedLastLineNumber, $"Last line number for {name}: {ControllerView.LastLineNumber}, expected: {ExpectedLastLineNumber}");
                }
                else
                {
                    using (FileStream fs = new FileStream("lines.txt", FileMode.Append, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine($"{{ \"{name}\", {ControllerView.LastLineNumber} }},");
                    }
                }
            }
            else
                ControllerView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);

            Assert.That(ControllerView.FirstLineNumber == 1);

            FrameTestCount = 0;
            FrameBrowseNode(Controller, RootIndex, JustCount);
            FrameMaxTestCount = FrameTestCount;

            for (int i = 0; i < 10; i++)
            {
                TestFrameInsert(index, rootNode, rand);
                TestFrameRemove(index, rootNode, rand);
                TestFrameReplace(index, rootNode, rand);
                TestFrameAssign(index, rootNode, rand);
                TestFrameUnassign(index, rootNode, rand);
                TestFrameChangeReplication(index, rootNode, rand);
                TestFrameSplit(index, rootNode, rand);
                TestFrameMerge(index, rootNode, rand);
                TestFrameMove(index, rootNode, rand);
                TestFrameExpand(index, rootNode, rand);
                TestFrameReduce(index, rootNode, rand);
            }

            TestFrameCanonicalize(rootNode);

            Assert.That(ControllerView.FirstLineNumber == 1);
        }

        public static Dictionary<string, int> ExpectedLastLineTable = new Dictionary<string, int>()
        {
            { "./test.easly", 193 },
            { "./EaslyExamples/CoreEditor/Classes/Agent Expression.easly", 193 },
            { "./EaslyExamples/CoreEditor/Classes/Basic Key Event Handler.easly", 855 },
            { "./EaslyExamples/CoreEditor/Classes/Block Editor Node Management.easly", 62 },
            { "./EaslyExamples/CoreEditor/Classes/Block Editor Node.easly", 162 },
            { "./EaslyExamples/CoreEditor/Classes/Block List Editor Node Management.easly", 62 },
            { "./EaslyExamples/CoreEditor/Classes/Block List Editor Node.easly", 252 },
            { "./EaslyExamples/CoreEditor/Classes/Control Key Event Handler.easly", 150 },
            { "./EaslyExamples/CoreEditor/Classes/Control With Decoration Management.easly", 644 },
            { "./EaslyExamples/CoreEditor/Classes/Decoration.easly", 47 },
            { "./EaslyExamples/CoreEditor/Classes/Editor Node Management.easly", 124 },
            { "./EaslyExamples/CoreEditor/Classes/Editor Node.easly", 17 },
            { "./EaslyExamples/CoreEditor/Classes/Horizontal Separator.easly", 35 },
            { "./EaslyExamples/CoreEditor/Classes/Identifier Key Event Handler.easly", 56 },
            { "./EaslyExamples/CoreEditor/Classes/Insertion Position.easly", 34 },
            { "./EaslyExamples/CoreEditor/Classes/Key Descriptor.easly", 83 },
            { "./EaslyExamples/CoreEditor/Classes/Node Selection.easly", 67 },
            { "./EaslyExamples/CoreEditor/Classes/Node With Default.easly", 27 },
            { "./EaslyExamples/CoreEditor/Classes/Properties Show Options.easly", 120 },
            { "./EaslyExamples/CoreEditor/Classes/Property Changed Notifier.easly", 57 },
            { "./EaslyExamples/CoreEditor/Classes/Replace Notification.easly", 44 },
            { "./EaslyExamples/CoreEditor/Classes/Simplify Notification.easly", 29 },
            { "./EaslyExamples/CoreEditor/Classes/Specialized Decoration.easly", 66 },
            { "./EaslyExamples/CoreEditor/Classes/Toggle Notification.easly", 41 },
            { "./EaslyExamples/CoreEditor/Libraries/Constructs.easly", 5 },
            { "./EaslyExamples/CoreEditor/Libraries/Nodes.easly", 5 },
            { "./EaslyExamples/CoreEditor/Libraries/SSC Editor.easly", 21 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Agent Expression.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Anchor Kinds.easly", 33 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Anchored Type.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Argument.easly", 29 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/As Long As Instruction.easly", 43 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Assertion Tag Expression.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Assertion.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Assignment Argument.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Assignment Instruction.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Assignment Type Argument.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Attachment Instruction.easly", 47 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Attachment.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Attribute Feature.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Binary Operator Expression.easly", 43 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Block List.easly", 30 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Block.easly", 17 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Body.easly", 43 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Check Instruction.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Class Constant Expression.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Class Replicate.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Class.easly", 99 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Clone Of Expression.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Clone Type.easly", 33 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Cloneable Status.easly", 33 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Command Instruction.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Command Overload Type.easly", 51 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Command Overload.easly", 43 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Comparable Status.easly", 33 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Comparison Type.easly", 33 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Conditional.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Conformance Type.easly", 33 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Constant Feature.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Constraint.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Continuation.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Copy Semantic.easly", 34 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Create Instruction.easly", 47 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Creation Feature.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Debug Instruction.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Deferred Body.easly", 29 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Discrete.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Effective Body.easly", 43 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Entity Declaration.easly", 43 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Entity Expression.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Equality Expression.easly", 47 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Equality Type.easly", 33 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Event Type.easly", 33 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Exception Handler.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Export Change.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Export Status.easly", 33 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Export.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Expression.easly", 29 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Extern Body.easly", 29 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Feature.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/For Loop Instruction.easly", 55 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Function Feature.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Function Type.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Generic Type.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Generic.easly", 47 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Global Replicate.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Identifier.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/If Then Else Instruction.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Import Type.easly", 34 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Import.easly", 47 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Index Assignment Instruction.easly", 43 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Index Query Expression.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Indexer Feature.easly", 55 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Indexer Type.easly", 75 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Inheritance.easly", 71 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Initialized Object Expression.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Inspect Instruction.easly", 43 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Instruction.easly", 29 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Iteration Type.easly", 33 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Keyword Anchored Type.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Keyword Expression.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Keyword.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Library.easly", 47 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Manifest Character Expression.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Manifest Number Expression.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Manifest String Expression.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Name.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Named Feature.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/New Expression.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Node.easly", 12 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Object Type.easly", 29 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Old Expression.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Once Choice.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Over Loop Instruction.easly", 51 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Parameter End Status.easly", 33 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Pattern.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Positional Argument.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Positional Type Argument.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Precursor Body.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Precursor Expression.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Precursor Index Assignment Instruction.easly", 43 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Precursor Index Expression.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Precursor Instruction.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Preprocessor Expression.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Preprocessor Macro.easly", 40 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Procedure Feature.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Procedure Type.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Property Feature.easly", 51 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Property Type.easly", 59 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Qualified Name.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Query Expression.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Query Overload Type.easly", 55 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Query Overload.easly", 55 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Raise Event Instruction.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Range.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Release Instruction.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Rename.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Replication Status.easly", 33 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Result Of Expression.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Root.easly", 43 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Scope.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Shareable Type.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Sharing Type.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Simple Type.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Throw Instruction.easly", 43 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Tuple Type.easly", 35 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Type Argument.easly", 29 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Typedef.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Unary Operator Expression.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/Utility Type.easly", 34 },
            { "./EaslyExamples/EaslyCoreLanguage/Classes/With.easly", 39 },
            { "./EaslyExamples/EaslyCoreLanguage/Libraries/Constructs.easly", 5 },
            { "./EaslyExamples/EaslyCoreLanguage/Libraries/Nodes.easly", 6 },
            { "./EaslyExamples/EaslyCoreLanguage/Libraries/SSC Language.easly", 15 },
            { "./EaslyExamples/EaslyCoreLanguage/Replicates/SSC Core Language Nodes.easly", 1 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/.NET Event.easly", 43 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/System.ComponentModel.PropertyChangedEventArgs.easly", 44 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/System.ComponentModel.PropertyChangedEventHandler.easly", 48 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/System.Windows.Controls.Orientation.easly", 33 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/System.Windows.Controls.TextBox.easly", 73 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/System.Windows.DependencyObject.easly", 20 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/System.Windows.FrameworkElement.easly", 60 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/System.Windows.Input.FocusNavigationDirection.easly", 39 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/System.Windows.Input.Key.easly", 72 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/System.Windows.Input.Keyboard.easly", 68 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/System.Windows.Input.KeyEventArgs.easly", 40 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/System.Windows.Input.TraversalRequest.easly", 40 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/System.Windows.InputElement.easly", 20 },
            { "./EaslyExamples/MicrosoftDotNet/Classes/System.Windows.Media.VisualTreeHelper.easly", 68 },
            { "./EaslyExamples/MicrosoftDotNet/Libraries/.NET Classes.easly", 5 },
            { "./EaslyExamples/MicrosoftDotNet/Libraries/.NET Enums.easly", 5 },
            { "./EaslyExamples/Verification/Verification Example.easly", 80 },
        };

        public static void TestFrameCanonicalize(INode rootNode)
        {
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);
            IFrameControllerView ControllerView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);

            Controller.Canonicalize();

            IFrameControllerView NewView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);
            Assert.That(NewView.IsEqual(CompareEqual.New(), ControllerView));

            IFrameRootNodeIndex NewRootIndex = new FrameRootNodeIndex(Controller.RootIndex.Node);
            IFrameController NewController = FrameController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
        }

        static int FrameTestCount = 0;
        static int FrameMaxTestCount = 0;

        public static bool JustCount(IFrameInner inner)
        {
            FrameTestCount++;
            return true;
        }

        public static void TestFrameStats(int index, string name, INode rootNode, out Stats stats)
        {
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);

            stats = new Stats();
            BrowseNode(Controller, RootIndex, stats);

            if (name.EndsWith("test.easly"))
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

        public static void TestFrameInsert(int index, INode rootNode, Random rand)
        {
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);
            IFrameControllerView ControllerView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);

            FrameTestCount = 0;
            FrameBrowseNode(Controller, RootIndex, (IFrameInner inner) => InsertAndCompare(ControllerView, RandNext(rand, FrameMaxTestCount), rand, inner));
        }

        static bool InsertAndCompare(IFrameControllerView controllerView, int TestIndex, Random rand, IFrameInner inner)
        {
            if (FrameTestCount++ < TestIndex)
                return true;

            IFrameController Controller = controllerView.Controller;
            bool IsModified = false;

            if (inner is IFrameListInner<IFrameBrowsingListNodeIndex> AsListInner)
            {
                if (AsListInner.StateList.Count > 0)
                {
                    INode NewNode = NodeHelper.DeepCloneNode(AsListInner.StateList[0].Node);
                    Assert.That(NewNode != null, $"Type: {AsListInner.InterfaceType}");

                    int Index = RandNext(rand, AsListInner.StateList.Count + 1);
                    IFrameInsertionListNodeIndex NodeIndex = new FrameInsertionListNodeIndex(AsListInner.Owner.Node, AsListInner.PropertyName, NewNode, Index);
                    Controller.Insert(AsListInner, NodeIndex, out IWriteableBrowsingCollectionNodeIndex InsertedIndex);
                    Assert.That(Controller.Contains(InsertedIndex));

                    IFramePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IFramePlaceholderNodeState;
                    Assert.That(ChildState != null);
                    Assert.That(ChildState.Node == NewNode);

                    IsModified = true;
                }
            }
            else if (inner is IFrameBlockListInner<IFrameBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0 && AsBlockListInner.BlockStateList[0].StateList.Count > 0)
                {
                    INode NewNode = NodeHelper.DeepCloneNode(AsBlockListInner.BlockStateList[0].StateList[0].Node);
                    Assert.That(NewNode != null, $"Type: {AsBlockListInner.InterfaceType}");

                    if (RandNext(rand, 2) == 0)
                    {
                        int BlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                        IFrameBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];
                        int Index = RandNext(rand, BlockState.StateList.Count + 1);

                        IFrameInsertionExistingBlockNodeIndex NodeIndex = new FrameInsertionExistingBlockNodeIndex(AsBlockListInner.Owner.Node, AsBlockListInner.PropertyName, NewNode, BlockIndex, Index);
                        Controller.Insert(AsBlockListInner, NodeIndex, out IWriteableBrowsingCollectionNodeIndex InsertedIndex);
                        Assert.That(Controller.Contains(InsertedIndex));

                        IFramePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IFramePlaceholderNodeState;
                        Assert.That(ChildState != null);
                        Assert.That(ChildState.Node == NewNode);

                        IsModified = true;
                    }
                    else
                    {
                        int BlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count + 1);

                        IPattern ReplicationPattern = NodeHelper.CreateSimplePattern("x");
                        IIdentifier SourceIdentifier = NodeHelper.CreateSimpleIdentifier("y");
                        IFrameInsertionNewBlockNodeIndex NodeIndex = new FrameInsertionNewBlockNodeIndex(AsBlockListInner.Owner.Node, AsBlockListInner.PropertyName, NewNode, BlockIndex, ReplicationPattern, SourceIdentifier);
                        Controller.Insert(AsBlockListInner, NodeIndex, out IWriteableBrowsingCollectionNodeIndex InsertedIndex);
                        Assert.That(Controller.Contains(InsertedIndex));

                        IFramePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IFramePlaceholderNodeState;
                        Assert.That(ChildState != null);
                        Assert.That(ChildState.Node == NewNode);

                        IsModified = true;
                    }
                }
            }

            if (IsModified)
            {
                IFrameControllerView NewView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);
                Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                IFrameRootNodeIndex NewRootIndex = new FrameRootNodeIndex(Controller.RootIndex.Node);
                IFrameController NewController = FrameController.Create(NewRootIndex);
                Assert.That(NewController.IsEqual(CompareEqual.New(), Controller), $"Inner: {inner.PropertyName}, Owner: {inner.Owner.Node}");
            }

            return false;
        }

        public static void TestFrameReplace(int index, INode rootNode, Random rand)
        {
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);
            IFrameControllerView ControllerView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);

            FrameTestCount = 0;
            FrameBrowseNode(Controller, RootIndex, (IFrameInner inner) => ReplaceAndCompare(ControllerView, RandNext(rand, FrameMaxTestCount), rand, inner));
        }

        static bool ReplaceAndCompare(IFrameControllerView controllerView, int TestIndex, Random rand, IFrameInner inner)
        {
            if (FrameTestCount++ < TestIndex)
                return true;

            IFrameController Controller = controllerView.Controller;
            bool IsModified = false;

            if (inner is IFramePlaceholderInner<IFrameBrowsingPlaceholderNodeIndex> AsPlaceholderInner)
            {
                INode NewNode = NodeHelper.DeepCloneNode(AsPlaceholderInner.ChildState.Node);
                Assert.That(NewNode != null, $"Type: {AsPlaceholderInner.InterfaceType}");

                IFrameInsertionPlaceholderNodeIndex NodeIndex = new FrameInsertionPlaceholderNodeIndex(AsPlaceholderInner.Owner.Node, AsPlaceholderInner.PropertyName, NewNode);
                Controller.Replace(AsPlaceholderInner, NodeIndex, out IWriteableBrowsingChildIndex InsertedIndex);
                Assert.That(Controller.Contains(InsertedIndex));

                IFramePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IFramePlaceholderNodeState;
                Assert.That(ChildState != null);
                Assert.That(ChildState.Node == NewNode);

                IsModified = true;
            }
            else if (inner is IFrameOptionalInner<IFrameBrowsingOptionalNodeIndex> AsOptionalInner)
            {
                IFrameOptionalNodeState State = AsOptionalInner.ChildState;
                IOptionalReference Optional = State.ParentIndex.Optional;
                Type NodeInterfaceType = Optional.GetType().GetGenericArguments()[0];
                INode NewNode = NodeHelper.CreateDefaultFromInterface(NodeInterfaceType);
                Assert.That(NewNode != null, $"Type: {AsOptionalInner.InterfaceType}");

                IFrameInsertionOptionalNodeIndex NodeIndex = new FrameInsertionOptionalNodeIndex(AsOptionalInner.Owner.Node, AsOptionalInner.PropertyName, NewNode);
                Controller.Replace(AsOptionalInner, NodeIndex, out IWriteableBrowsingChildIndex InsertedIndex);
                Assert.That(Controller.Contains(InsertedIndex));

                IFrameOptionalNodeState ChildState = Controller.IndexToState(InsertedIndex) as IFrameOptionalNodeState;
                Assert.That(ChildState != null);
                Assert.That(ChildState.Node == NewNode);

                IsModified = true;
            }
            else if (inner is IFrameListInner<IFrameBrowsingListNodeIndex> AsListInner)
            {
                if (AsListInner.StateList.Count > 0)
                {
                    INode NewNode = NodeHelper.DeepCloneNode(AsListInner.StateList[0].Node);
                    Assert.That(NewNode != null, $"Type: {AsListInner.InterfaceType}");

                    int Index = RandNext(rand, AsListInner.StateList.Count);
                    IFrameInsertionListNodeIndex NodeIndex = new FrameInsertionListNodeIndex(AsListInner.Owner.Node, AsListInner.PropertyName, NewNode, Index);
                    Controller.Replace(AsListInner, NodeIndex, out IWriteableBrowsingChildIndex InsertedIndex);
                    Assert.That(Controller.Contains(InsertedIndex));

                    IFramePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IFramePlaceholderNodeState;
                    Assert.That(ChildState != null);
                    Assert.That(ChildState.Node == NewNode);

                    IsModified = true;
                }
            }
            else if (inner is IFrameBlockListInner<IFrameBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0 && AsBlockListInner.BlockStateList[0].StateList.Count > 0)
                {
                    INode NewNode = NodeHelper.DeepCloneNode(AsBlockListInner.BlockStateList[0].StateList[0].Node);
                    Assert.That(NewNode != null, $"Type: {AsBlockListInner.InterfaceType}");

                    int BlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                    IFrameBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];
                    int Index = RandNext(rand, BlockState.StateList.Count);

                    IFrameInsertionExistingBlockNodeIndex NodeIndex = new FrameInsertionExistingBlockNodeIndex(AsBlockListInner.Owner.Node, AsBlockListInner.PropertyName, NewNode, BlockIndex, Index);
                    Controller.Replace(AsBlockListInner, NodeIndex, out IWriteableBrowsingChildIndex InsertedIndex);
                    Assert.That(Controller.Contains(InsertedIndex));

                    IFramePlaceholderNodeState ChildState = Controller.IndexToState(InsertedIndex) as IFramePlaceholderNodeState;
                    Assert.That(ChildState != null);
                    Assert.That(ChildState.Node == NewNode);

                    IsModified = true;
                }
            }

            if (IsModified)
            {
                IFrameControllerView NewView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);
                Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                IFrameRootNodeIndex NewRootIndex = new FrameRootNodeIndex(Controller.RootIndex.Node);
                IFrameController NewController = FrameController.Create(NewRootIndex);
                Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
            }

            return false;
        }

        public static void TestFrameRemove(int index, INode rootNode, Random rand)
        {
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);
            IFrameControllerView ControllerView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);

            FrameTestCount = 0;
            FrameBrowseNode(Controller, RootIndex, (IFrameInner inner) => RemoveAndCompare(ControllerView, RandNext(rand, FrameMaxTestCount), rand, inner));
        }

        static bool RemoveAndCompare(IFrameControllerView controllerView, int TestIndex, Random rand, IFrameInner inner)
        {
            if (FrameTestCount++ < TestIndex)
                return true;

            IFrameController Controller = controllerView.Controller;
            bool IsModified = false;

            if (inner is IFrameListInner<IFrameBrowsingListNodeIndex> AsListInner)
            {
                if (AsListInner.StateList.Count > 0)
                {
                    int Index = RandNext(rand, AsListInner.StateList.Count);
                    IFrameNodeState ChildState = AsListInner.StateList[Index];
                    IFrameBrowsingListNodeIndex NodeIndex = ChildState.ParentIndex as IFrameBrowsingListNodeIndex;
                    Assert.That(NodeIndex != null);

                    Controller.Remove(AsListInner, NodeIndex);

                    IsModified = true;
                }
            }
            else if (inner is IFrameBlockListInner<IFrameBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0 && AsBlockListInner.BlockStateList[0].StateList.Count > 0)
                {
                    int BlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                    IFrameBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];
                    int Index = RandNext(rand, BlockState.StateList.Count);
                    IFrameNodeState ChildState = BlockState.StateList[Index];
                    IFrameBrowsingExistingBlockNodeIndex NodeIndex = ChildState.ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                    Assert.That(NodeIndex != null);

                    Controller.Remove(AsBlockListInner, NodeIndex);

                    IsModified = true;
                }
            }

            if (IsModified)
            {
                IFrameControllerView NewView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);
                Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                IFrameRootNodeIndex NewRootIndex = new FrameRootNodeIndex(Controller.RootIndex.Node);
                IFrameController NewController = FrameController.Create(NewRootIndex);
                Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
            }

            return false;
        }

        public static void TestFrameAssign(int index, INode rootNode, Random rand)
        {
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);
            IFrameControllerView ControllerView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);

            FrameTestCount = 0;
            FrameBrowseNode(Controller, RootIndex, (IFrameInner inner) => AssignAndCompare(ControllerView, RandNext(rand, FrameMaxTestCount), rand, inner));
        }

        static bool AssignAndCompare(IFrameControllerView controllerView, int TestIndex, Random rand, IFrameInner inner)
        {
            if (FrameTestCount++ < TestIndex)
                return true;

            IFrameController Controller = controllerView.Controller;

            if (inner is IFrameOptionalInner<IFrameBrowsingOptionalNodeIndex> AsOptionalInner)
            {
                IFrameOptionalNodeState ChildState = AsOptionalInner.ChildState;
                Assert.That(ChildState != null);

                IFrameBrowsingOptionalNodeIndex OptionalIndex = ChildState.ParentIndex;
                Assert.That(Controller.Contains(OptionalIndex));

                IOptionalReference Optional = OptionalIndex.Optional;
                Assert.That(Optional != null);

                if (Optional.HasItem)
                {
                    Controller.Assign(OptionalIndex);
                    Assert.That(Optional.IsAssigned);
                    Assert.That(AsOptionalInner.IsAssigned);
                    Assert.That(Optional.Item == ChildState.Node);

                    IFrameControllerView NewView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);
                    Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                    IFrameRootNodeIndex NewRootIndex = new FrameRootNodeIndex(Controller.RootIndex.Node);
                    IFrameController NewController = FrameController.Create(NewRootIndex);
                    Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
                }
            }

            return false;
        }

        public static void TestFrameUnassign(int index, INode rootNode, Random rand)
        {
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);
            IFrameControllerView ControllerView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);

            FrameTestCount = 0;
            FrameBrowseNode(Controller, RootIndex, (IFrameInner inner) => UnassignAndCompare(ControllerView, RandNext(rand, FrameMaxTestCount), rand, inner));
        }

        static bool UnassignAndCompare(IFrameControllerView controllerView, int TestIndex, Random rand, IFrameInner inner)
        {
            if (FrameTestCount++ < TestIndex)
                return true;

            IFrameController Controller = controllerView.Controller;

            if (inner is IFrameOptionalInner<IFrameBrowsingOptionalNodeIndex> AsOptionalInner)
            {
                IFrameOptionalNodeState ChildState = AsOptionalInner.ChildState;
                Assert.That(ChildState != null);

                IFrameBrowsingOptionalNodeIndex OptionalIndex = ChildState.ParentIndex;
                Assert.That(Controller.Contains(OptionalIndex));

                IOptionalReference Optional = OptionalIndex.Optional;
                Assert.That(Optional != null);

                Controller.Unassign(OptionalIndex);
                Assert.That(!Optional.IsAssigned);
                Assert.That(!AsOptionalInner.IsAssigned);

                IFrameControllerView NewView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);
                Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                IFrameRootNodeIndex NewRootIndex = new FrameRootNodeIndex(Controller.RootIndex.Node);
                IFrameController NewController = FrameController.Create(NewRootIndex);
                Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
            }

            return false;
        }

        public static void TestFrameChangeReplication(int index, INode rootNode, Random rand)
        {
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);
            IFrameControllerView ControllerView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);

            FrameTestCount = 0;
            FrameBrowseNode(Controller, RootIndex, (IFrameInner inner) => ChangeReplicationAndCompare(ControllerView, RandNext(rand, FrameMaxTestCount), rand, inner));
        }

        static bool ChangeReplicationAndCompare(IFrameControllerView controllerView, int TestIndex, Random rand, IFrameInner inner)
        {
            if (FrameTestCount++ < TestIndex)
                return true;

            IFrameController Controller = controllerView.Controller;

            if (inner is IFrameBlockListInner<IFrameBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0)
                {
                    int BlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                    IFrameBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];

                    ReplicationStatus Replication = (ReplicationStatus)RandNext(rand, 2);
                    Controller.ChangeReplication(AsBlockListInner, BlockIndex, Replication);

                    IFrameControllerView NewView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);
                    Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                    IFrameRootNodeIndex NewRootIndex = new FrameRootNodeIndex(Controller.RootIndex.Node);
                    IFrameController NewController = FrameController.Create(NewRootIndex);
                    Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
                }
            }

            return false;
        }

        public static void TestFrameSplit(int index, INode rootNode, Random rand)
        {
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);
            IFrameControllerView ControllerView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);

            FrameTestCount = 0;
            FrameBrowseNode(Controller, RootIndex, (IFrameInner inner) => SplitAndCompare(ControllerView, RandNext(rand, FrameMaxTestCount), rand, inner));
        }

        static bool SplitAndCompare(IFrameControllerView controllerView, int TestIndex, Random rand, IFrameInner inner)
        {
            if (FrameTestCount++ < TestIndex)
                return true;

            IFrameController Controller = controllerView.Controller;

            if (inner is IFrameBlockListInner<IFrameBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0)
                {
                    int SplitBlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                    IFrameBlockState BlockState = AsBlockListInner.BlockStateList[SplitBlockIndex];
                    if (BlockState.StateList.Count > 1)
                    {
                        int SplitIndex = 1 + RandNext(rand, BlockState.StateList.Count - 1);
                        IFrameBrowsingExistingBlockNodeIndex NodeIndex = (IFrameBrowsingExistingBlockNodeIndex)AsBlockListInner.IndexAt(SplitBlockIndex, SplitIndex);
                        Controller.SplitBlock(AsBlockListInner, NodeIndex);

                        IFrameControllerView NewView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);
                        Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                        IFrameRootNodeIndex NewRootIndex = new FrameRootNodeIndex(Controller.RootIndex.Node);
                        IFrameController NewController = FrameController.Create(NewRootIndex);
                        Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

                        Assert.That(AsBlockListInner.BlockStateList.Count > 0);
                        int OldBlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                        int NewBlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                        int Direction = NewBlockIndex - OldBlockIndex;
                        Controller.MoveBlock(AsBlockListInner, OldBlockIndex, Direction);

                        IFrameControllerView NewViewAfterMove = FrameControllerView.Create(Controller, FrameTemplateSet.Default);
                        Assert.That(NewViewAfterMove.IsEqual(CompareEqual.New(), controllerView));

                        IFrameRootNodeIndex NewRootIndexAfterMove = new FrameRootNodeIndex(Controller.RootIndex.Node);
                        IFrameController NewControllerAfterMove = FrameController.Create(NewRootIndexAfterMove);
                        Assert.That(NewControllerAfterMove.IsEqual(CompareEqual.New(), Controller));
                    }
                }
            }

            return false;
        }

        public static void TestFrameMerge(int index, INode rootNode, Random rand)
        {
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);
            IFrameControllerView ControllerView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);

            FrameTestCount = 0;
            FrameBrowseNode(Controller, RootIndex, (IFrameInner inner) => MergeAndCompare(ControllerView, RandNext(rand, FrameMaxTestCount), rand, inner));
        }

        static bool MergeAndCompare(IFrameControllerView controllerView, int TestIndex, Random rand, IFrameInner inner)
        {
            if (FrameTestCount++ < TestIndex)
                return true;

            IFrameController Controller = controllerView.Controller;

            if (inner is IFrameBlockListInner<IFrameBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 1)
                {
                    int MergeBlockIndex = 1 + RandNext(rand, AsBlockListInner.BlockStateList.Count - 1);
                    IFrameBlockState BlockState = AsBlockListInner.BlockStateList[MergeBlockIndex];

                    IFrameBrowsingExistingBlockNodeIndex NodeIndex = (IFrameBrowsingExistingBlockNodeIndex)AsBlockListInner.IndexAt(MergeBlockIndex, 0);
                    Controller.MergeBlocks(AsBlockListInner, NodeIndex);

                    IFrameControllerView NewView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);
                    Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                    IFrameRootNodeIndex NewRootIndex = new FrameRootNodeIndex(Controller.RootIndex.Node);
                    IFrameController NewController = FrameController.Create(NewRootIndex);
                    Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
                }
            }

            return false;
        }

        public static void TestFrameMove(int index, INode rootNode, Random rand)
        {
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);
            IFrameControllerView ControllerView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);

            FrameTestCount = 0;
            FrameBrowseNode(Controller, RootIndex, (IFrameInner inner) => MoveAndCompare(ControllerView, RandNext(rand, FrameMaxTestCount), rand, inner));
        }

        static bool MoveAndCompare(IFrameControllerView controllerView, int TestIndex, Random rand, IFrameInner inner)
        {
            if (FrameTestCount++ < TestIndex)
                return true;

            IFrameController Controller = controllerView.Controller;
            bool IsModified = false;

            if (inner is IFrameListInner<IFrameBrowsingListNodeIndex> AsListInner)
            {
                if (AsListInner.StateList.Count > 0)
                {
                    int OldIndex = RandNext(rand, AsListInner.StateList.Count);
                    int NewIndex = RandNext(rand, AsListInner.StateList.Count);
                    int Direction = NewIndex - OldIndex;

                    IFrameBrowsingListNodeIndex NodeIndex = AsListInner.IndexAt(OldIndex) as IFrameBrowsingListNodeIndex;
                    Assert.That(NodeIndex != null);

                    Controller.Move(AsListInner, NodeIndex, Direction);
                    Assert.That(Controller.Contains(NodeIndex));

                    IsModified = true;
                }
            }
            else if (inner is IFrameBlockListInner<IFrameBrowsingBlockNodeIndex> AsBlockListInner)
            {
                if (AsBlockListInner.BlockStateList.Count > 0)
                {
                    int BlockIndex = RandNext(rand, AsBlockListInner.BlockStateList.Count);
                    IFrameBlockState BlockState = AsBlockListInner.BlockStateList[BlockIndex];

                    if (BlockState.StateList.Count > 0)
                    {
                        int OldIndex = RandNext(rand, BlockState.StateList.Count);
                        int NewIndex = RandNext(rand, BlockState.StateList.Count);
                        int Direction = NewIndex - OldIndex;

                        IFrameBrowsingExistingBlockNodeIndex NodeIndex = AsBlockListInner.IndexAt(BlockIndex, OldIndex) as IFrameBrowsingExistingBlockNodeIndex;
                        Assert.That(NodeIndex != null);

                        Controller.Move(AsBlockListInner, NodeIndex, Direction);
                        Assert.That(Controller.Contains(NodeIndex));

                        IsModified = true;
                    }
                }
            }

            if (IsModified)
            {
                IFrameControllerView NewView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);
                Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

                IFrameRootNodeIndex NewRootIndex = new FrameRootNodeIndex(Controller.RootIndex.Node);
                IFrameController NewController = FrameController.Create(NewRootIndex);
                Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));
            }

            return false;
        }

        public static void TestFrameExpand(int index, INode rootNode, Random rand)
        {
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);
            IFrameControllerView ControllerView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);

            FrameTestCount = 0;
            FrameBrowseNode(Controller, RootIndex, (IFrameInner inner) => ExpandAndCompare(ControllerView, RandNext(rand, FrameMaxTestCount), rand, inner));
        }

        static bool ExpandAndCompare(IFrameControllerView controllerView, int TestIndex, Random rand, IFrameInner inner)
        {
            if (FrameTestCount++ < TestIndex)
                return true;

            IFrameController Controller = controllerView.Controller;
            IFrameNodeIndex NodeIndex;
            IFramePlaceholderNodeState State;

            if (inner is IFramePlaceholderInner<IFrameBrowsingPlaceholderNodeIndex> AsPlaceholderInner)
            {
                NodeIndex = AsPlaceholderInner.ChildState.ParentIndex as IFrameNodeIndex;
                Assert.That(NodeIndex != null);

                State = Controller.IndexToState(NodeIndex) as IFramePlaceholderNodeState;
                Assert.That(State != null);

                NodeTreeHelper.GetArgumentBlocks(State.Node, out IDictionary<string, IBlockList<IArgument, Argument>> ArgumentBlocksTable);
                if (ArgumentBlocksTable.Count == 0)
                    return true;
            }
            else
                return true;

            Controller.Expand(NodeIndex);

            IFrameControllerView NewView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);
            Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

            IFrameRootNodeIndex NewRootIndex = new FrameRootNodeIndex(Controller.RootIndex.Node);
            IFrameController NewController = FrameController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

            Controller.Expand(NodeIndex);

            NewController = FrameController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

            Controller.Reduce(NodeIndex);

            NewController = FrameController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

            return false;
        }

        public static void TestFrameReduce(int index, INode rootNode, Random rand)
        {
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(rootNode);
            IFrameController Controller = FrameController.Create(RootIndex);
            IFrameControllerView ControllerView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);

            FrameTestCount = 0;
            FrameBrowseNode(Controller, RootIndex, (IFrameInner inner) => ReduceAndCompare(ControllerView, RandNext(rand, FrameMaxTestCount), rand, inner));
        }

        static bool ReduceAndCompare(IFrameControllerView controllerView, int TestIndex, Random rand, IFrameInner inner)
        {
            if (FrameTestCount++ < TestIndex)
                return true;

            IFrameController Controller = controllerView.Controller;
            IFrameNodeIndex NodeIndex;
            IFramePlaceholderNodeState State;

            if (inner is IFramePlaceholderInner<IFrameBrowsingPlaceholderNodeIndex> AsPlaceholderInner)
            {
                NodeIndex = AsPlaceholderInner.ChildState.ParentIndex as IFrameNodeIndex;
                Assert.That(NodeIndex != null);

                State = Controller.IndexToState(NodeIndex) as IFramePlaceholderNodeState;
                Assert.That(State != null);

                NodeTreeHelper.GetArgumentBlocks(State.Node, out IDictionary<string, IBlockList<IArgument, Argument>> ArgumentBlocksTable);
                if (ArgumentBlocksTable.Count == 0)
                    return true;
            }
            else
                return true;

            Controller.Reduce(NodeIndex);

            IFrameControllerView NewView = FrameControllerView.Create(Controller, FrameTemplateSet.Default);
            Assert.That(NewView.IsEqual(CompareEqual.New(), controllerView));

            IFrameRootNodeIndex NewRootIndex = new FrameRootNodeIndex(Controller.RootIndex.Node);
            IFrameController NewController = FrameController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

            Controller.Reduce(NodeIndex);

            NewController = FrameController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

            Controller.Expand(NodeIndex);

            NewController = FrameController.Create(NewRootIndex);
            Assert.That(NewController.IsEqual(CompareEqual.New(), Controller));

            return false;
        }

        static bool FrameBrowseNode(IFrameController controller, IFrameIndex index, Func<IFrameInner, bool> test)
        {
            Assert.That(index != null, "Frame #0");
            Assert.That(controller.Contains(index), "Frame #1");
            IFrameNodeState State = (IFrameNodeState)controller.IndexToState(index);
            Assert.That(State != null, "Frame #2");
            Assert.That(State.ParentIndex == index, "Frame #4");

            INode Node;

            if (State is IFramePlaceholderNodeState AsPlaceholderState)
                Node = AsPlaceholderState.Node;
            else
            {
                Assert.That(State is IFrameOptionalNodeState, "Frame #5");
                IFrameOptionalNodeState AsOptionalState = (IFrameOptionalNodeState)State;
                IFrameOptionalInner<IFrameBrowsingOptionalNodeIndex> ParentInner = AsOptionalState.ParentInner;

                Assert.That(ParentInner.IsAssigned, "Frame #6");

                Node = AsOptionalState.Node;
            }

            Type ChildNodeType;
            IList<string> PropertyNames = NodeTreeHelper.EnumChildNodeProperties(Node);

            foreach (string PropertyName in PropertyNames)
            {
                if (NodeTreeHelperChild.IsChildNodeProperty(Node, PropertyName, out ChildNodeType))
                {
                    IFramePlaceholderInner Inner = (IFramePlaceholderInner)State.PropertyToInner(PropertyName);
                    if (!test(Inner))
                        return false;

                    IFrameNodeState ChildState = Inner.ChildState;
                    IFrameIndex ChildIndex = ChildState.ParentIndex;
                    if (!FrameBrowseNode(controller, ChildIndex, test))
                        return false;
                }

                else if (NodeTreeHelperOptional.IsOptionalChildNodeProperty(Node, PropertyName, out ChildNodeType))
                {
                    NodeTreeHelperOptional.GetChildNode(Node, PropertyName, out bool IsAssigned, out INode ChildNode);
                    if (IsAssigned)
                    {
                        IFrameOptionalInner Inner = (IFrameOptionalInner)State.PropertyToInner(PropertyName);
                        if (!test(Inner))
                            return false;

                        IFrameNodeState ChildState = Inner.ChildState;
                        IFrameIndex ChildIndex = ChildState.ParentIndex;
                        if (!FrameBrowseNode(controller, ChildIndex, test))
                            return false;
                    }
                }

                else if (NodeTreeHelperList.IsNodeListProperty(Node, PropertyName, out ChildNodeType))
                {
                    IFrameListInner Inner = (IFrameListInner)State.PropertyToInner(PropertyName);
                    if (!test(Inner))
                        return false;

                    for (int i = 0; i < Inner.StateList.Count; i++)
                    {
                        IFramePlaceholderNodeState ChildState = Inner.StateList[i];
                        IFrameIndex ChildIndex = ChildState.ParentIndex;
                        if (!FrameBrowseNode(controller, ChildIndex, test))
                            return false;
                    }
                }

                else if (NodeTreeHelperBlockList.IsBlockListProperty(Node, PropertyName, out Type ChildInterfaceType, out ChildNodeType))
                {
                    IFrameBlockListInner Inner = (IFrameBlockListInner)State.PropertyToInner(PropertyName);
                    if (!test(Inner))
                        return false;

                    for (int BlockIndex = 0; BlockIndex < Inner.BlockStateList.Count; BlockIndex++)
                    {
                        IFrameBlockState BlockState = Inner.BlockStateList[BlockIndex];
                        if (!FrameBrowseNode(controller, BlockState.PatternIndex, test))
                            return false;
                        if (!FrameBrowseNode(controller, BlockState.SourceIndex, test))
                            return false;

                        for (int i = 0; i < BlockState.StateList.Count; i++)
                        {
                            IFramePlaceholderNodeState ChildState = BlockState.StateList[i];
                            IFrameIndex ChildIndex = ChildState.ParentIndex;
                            if (!FrameBrowseNode(controller, ChildIndex, test))
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
