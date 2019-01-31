using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using EaslyController;
using EaslyController.ReadOnly;
using NUnit.Framework;

namespace Coverage
{
    [TestFixture]
    public class CoverageSet
    {
        #region Setup
        [OneTimeSetUp]
        public static void InitCoverageSession()
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

        }
        #endregion

        #region Tools
        private enum Imperfections
        {
            None,
            BadGuid,
        };

        private static Guid ValueGuid = new Guid("{FFFFFFFF-C70B-4BAF-AE1B-C342CD9BFADE}");
        private static Guid LeafGuid0 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887200}");
        private static Guid LeafGuid1 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887201}");
        private static Guid LeafGuid2 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887202}");
        private static Guid LeafGuid3 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887203}");
        private static Guid LeafGuid4 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887204}");
        private static Guid LeafGuid5 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887205}");
        private static Guid LeafGuid6 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887206}");
        private static Guid TreeGuid0 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887250}");
        private static Guid TreeGuid1 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887251}");
        private static Guid TreeGuid2 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887252}");
        private static Guid TreeGuid3 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887253}");
        private static Guid TreeGuid4 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887254}");
        private static Guid TreeGuid5 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887255}");
        private static Guid MainGuid = new Guid("{EDBDC354-C70B-4BAF-AE1B-C342CD9BFADE}");

        private static Tree CreateTree(Guid guid0, Guid guid1, Guid guid2)
        {
            Leaf Placeholder = new Leaf();

            BaseNode.IDocument PlaceholderDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", guid0);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(Placeholder, PlaceholderDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(Placeholder, nameof(ILeaf.Text), "placeholder");

            Tree TreeInstance = new Tree();

            BaseNode.IDocument TreeDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("tree doc", guid1);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(TreeInstance, TreeDocument);
            BaseNodeHelper.NodeTreeHelperChild.SetChildNode(TreeInstance, nameof(ITree.Placeholder), Placeholder);
            BaseNodeHelper.NodeTreeHelper.SetBooleanProperty(TreeInstance, nameof(IMain.ValueBoolean), true);
            BaseNodeHelper.NodeTreeHelper.SetEnumProperty(TreeInstance, nameof(IMain.ValueEnum), (int)BaseNode.CopySemantic.Value);
            BaseNodeHelper.NodeTreeHelper.SetGuidProperty(TreeInstance, nameof(IMain.ValueGuid), guid2);

            return TreeInstance;
        }

        private static IMain CreateRoot(Imperfections imperfection)
        {
            Tree PlaceholderTree = CreateTree(TreeGuid0, TreeGuid1, TreeGuid2);

            Leaf PlaceholderLeaf = new Leaf();

            BaseNode.IDocument PlaceholderDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", imperfection == Imperfections.BadGuid ? MainGuid : LeafGuid0);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(PlaceholderLeaf, PlaceholderDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(PlaceholderLeaf, nameof(ILeaf.Text), "placeholder");

            Leaf UnassignedOptionalLeaf = new Leaf();

            BaseNode.IDocument UnassignedOptionalLeafDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", LeafGuid1);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(UnassignedOptionalLeaf, UnassignedOptionalLeafDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(UnassignedOptionalLeaf, nameof(ILeaf.Text), "optional unassigned");

            Easly.IOptionalReference<ILeaf> UnassignedOptional = BaseNodeHelper.OptionalReferenceHelper<ILeaf>.CreateReference(UnassignedOptionalLeaf);

            Leaf AssignedOptionalLeaf = new Leaf();

            BaseNode.IDocument AssignedOptionalLeafDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", LeafGuid2);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(AssignedOptionalLeaf, AssignedOptionalLeafDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(AssignedOptionalLeaf, nameof(ILeaf.Text), "optional unassigned");

            Easly.IOptionalReference<ILeaf> AssignedOptionalForLeaf = BaseNodeHelper.OptionalReferenceHelper<ILeaf>.CreateReference(AssignedOptionalLeaf);
            AssignedOptionalForLeaf.Assign();

            Tree AssignedOptionalTree = CreateTree(TreeGuid3, TreeGuid4, TreeGuid5);
            Easly.IOptionalReference<ITree> AssignedOptionalForTree = BaseNodeHelper.OptionalReferenceHelper<ITree>.CreateReference(AssignedOptionalTree);
            AssignedOptionalForTree.Assign();

            Leaf FirstChild = new Leaf();

            BaseNode.IDocument FirstChildDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", LeafGuid3);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(FirstChild, FirstChildDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(FirstChild, nameof(ILeaf.Text), "first child");

            Leaf SecondChild = new Leaf();

            BaseNode.IDocument SecondChildDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", LeafGuid4);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(SecondChild, SecondChildDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(SecondChild, nameof(ILeaf.Text), "second child");

            Leaf ThirdChild = new Leaf();

            BaseNode.IDocument ThirdChildDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", LeafGuid5);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(ThirdChild, ThirdChildDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(ThirdChild, nameof(ILeaf.Text), "third child");

            BaseNode.IBlock<ILeaf, Leaf> SecondBlock = BaseNodeHelper.BlockListHelper<ILeaf, Leaf>.CreateBlock(new List<ILeaf>() { SecondChild, ThirdChild });

            BaseNode.IBlockList<ILeaf, Leaf> LeafBlocks = BaseNodeHelper.BlockListHelper<ILeaf, Leaf>.CreateSimpleBlockList(FirstChild);
            LeafBlocks.NodeBlockList.Add(SecondBlock);

            Leaf FirstPath = new Leaf();

            BaseNode.IDocument FirstPathDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", LeafGuid6);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(FirstPath, FirstPathDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(FirstPath, nameof(ILeaf.Text), "first child");

            IList<ILeaf> LeafPath = new List<ILeaf>();
            LeafPath.Add(FirstPath);

            Main Root = new Main();

            BaseNode.IDocument RootDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("main doc", MainGuid);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(Root, RootDocument);
            BaseNodeHelper.NodeTreeHelperChild.SetChildNode(Root, nameof(IMain.PlaceholderTree), PlaceholderTree);
            BaseNodeHelper.NodeTreeHelperChild.SetChildNode(Root, nameof(IMain.PlaceholderLeaf), PlaceholderLeaf);
            BaseNodeHelper.NodeTreeHelperOptional.SetOptionalReference(Root, nameof(IMain.UnassignedOptionalLeaf), (Easly.IOptionalReference)UnassignedOptional);
            BaseNodeHelper.NodeTreeHelperOptional.SetOptionalReference(Root, nameof(IMain.AssignedOptionalTree), (Easly.IOptionalReference)AssignedOptionalForTree);
            BaseNodeHelper.NodeTreeHelperOptional.SetOptionalReference(Root, nameof(IMain.AssignedOptionalLeaf), (Easly.IOptionalReference)AssignedOptionalForLeaf);
            BaseNodeHelper.NodeTreeHelperBlockList.SetBlockList(Root, nameof(IMain.LeafBlocks), (BaseNode.IBlockList)LeafBlocks);
            BaseNodeHelper.NodeTreeHelperList.SetChildNodeList(Root, nameof(IMain.LeafPath), (IList)LeafPath);
            BaseNodeHelper.NodeTreeHelper.SetBooleanProperty(Root, nameof(IMain.ValueBoolean), true);
            BaseNodeHelper.NodeTreeHelper.SetEnumProperty(Root, nameof(IMain.ValueEnum), (int)BaseNode.CopySemantic.Value);
            BaseNodeHelper.NodeTreeHelper.SetStringProperty(Root, nameof(IMain.ValueString), "string");
            BaseNodeHelper.NodeTreeHelper.SetGuidProperty(Root, nameof(IMain.ValueGuid), ValueGuid);

            return Root;
        }
        #endregion

        #region ReadOnly
        [Test]
        [Category("Coverage")]
        public static void ReadOnlyCreation()
        {
            IMain RootNode;
            IReadOnlyRootNodeIndex RootIndex;
            IReadOnlyController Controller;

            RootNode = CreateRoot(Imperfections.None);
            Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

            try
            {
                RootIndex = new ReadOnlyRootNodeIndex(RootNode);
                Controller = ReadOnlyController.Create(RootIndex);
            }
            catch (Exception e)
            {
                Assert.Fail($"#0: {e}");
            }

            RootNode = CreateRoot(Imperfections.BadGuid);
            Assert.That(!BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

            try
            {
                RootIndex = new ReadOnlyRootNodeIndex(RootNode);
                Assert.Fail($"#1: no exception");
            }
            catch (ArgumentException e)
            {
                Assert.That(e.Message == "node", $"#1: wrong exception message '{e.Message}'");
            }
            catch (Exception e)
            {
                Assert.Fail($"#1: {e}");
            }
        }

        [Test]
        [Category("Coverage")]
        public static void ReadOnlyProperties()
        {
            IMain RootNode;
            IReadOnlyRootNodeIndex RootIndex0;
            IReadOnlyRootNodeIndex RootIndex1;

            RootNode = CreateRoot(Imperfections.None);
            Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

            RootIndex0 = new ReadOnlyRootNodeIndex(RootNode);
            Assert.That(RootIndex0.Node == RootNode);
            Assert.That(RootIndex0.IsEqual(CompareEqual.New(), RootIndex0));

            RootIndex1 = new ReadOnlyRootNodeIndex(RootNode);
            Assert.That(RootIndex1.Node == RootNode);
            Assert.That(CompareEqual.CoverIsEqual(RootIndex0, RootIndex1));

            IReadOnlyController Controller0 = ReadOnlyController.Create(RootIndex0);
            Assert.That(Controller0.RootIndex == RootIndex0);

            Stats Stats = Controller0.Stats;
            Assert.That(Stats.NodeCount >= 0);
            Assert.That(Stats.PlaceholderNodeCount >= 0);
            Assert.That(Stats.OptionalNodeCount >= 0);
            Assert.That(Stats.AssignedOptionalNodeCount >= 0);
            Assert.That(Stats.ListCount >= 0);
            Assert.That(Stats.BlockListCount >= 0);
            Assert.That(Stats.BlockCount >= 0);

            IReadOnlyPlaceholderNodeState RootState = Controller0.RootState;
            Assert.That(RootState.ParentIndex == RootIndex0);

            Assert.That(Controller0.Contains(RootIndex0));
            Assert.That(Controller0.IndexToState(RootIndex0) == RootState);

            Assert.That(RootState.InnerTable.Count == 7);
            Assert.That(RootState.InnerTable.ContainsKey(nameof(IMain.PlaceholderTree)));
            Assert.That(RootState.InnerTable.ContainsKey(nameof(IMain.PlaceholderLeaf)));
            Assert.That(RootState.InnerTable.ContainsKey(nameof(IMain.UnassignedOptionalLeaf)));
            Assert.That(RootState.InnerTable.ContainsKey(nameof(IMain.AssignedOptionalTree)));
            Assert.That(RootState.InnerTable.ContainsKey(nameof(IMain.AssignedOptionalLeaf)));
            Assert.That(RootState.InnerTable.ContainsKey(nameof(IMain.LeafBlocks)));
            Assert.That(RootState.InnerTable.ContainsKey(nameof(IMain.LeafPath)));

            IReadOnlyPlaceholderInner<IReadOnlyBrowsingPlaceholderNodeIndex> MainPlaceholderTreeInner = RootState.PropertyToInner(nameof(IMain.PlaceholderTree)) as IReadOnlyPlaceholderInner<IReadOnlyBrowsingPlaceholderNodeIndex>;
            Assert.That(MainPlaceholderTreeInner != null);
            Assert.That(MainPlaceholderTreeInner.InterfaceType == typeof(ITree));
            Assert.That(MainPlaceholderTreeInner.ChildState != null);
            Assert.That(MainPlaceholderTreeInner.ChildState.ParentInner == MainPlaceholderTreeInner);

            IReadOnlyPlaceholderInner<IReadOnlyBrowsingPlaceholderNodeIndex> MainPlaceholderLeafInner = RootState.PropertyToInner(nameof(IMain.PlaceholderLeaf)) as IReadOnlyPlaceholderInner<IReadOnlyBrowsingPlaceholderNodeIndex>;
            Assert.That(MainPlaceholderLeafInner != null);
            Assert.That(MainPlaceholderLeafInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainPlaceholderLeafInner.ChildState != null);
            Assert.That(MainPlaceholderLeafInner.ChildState.ParentInner == MainPlaceholderLeafInner);

            IReadOnlyOptionalInner<IReadOnlyBrowsingOptionalNodeIndex> MainUnassignedOptionalInner = RootState.PropertyToInner(nameof(IMain.UnassignedOptionalLeaf)) as IReadOnlyOptionalInner<IReadOnlyBrowsingOptionalNodeIndex>;
            Assert.That(MainUnassignedOptionalInner != null);
            Assert.That(MainUnassignedOptionalInner.InterfaceType == typeof(ILeaf));
            Assert.That(!MainUnassignedOptionalInner.IsAssigned);
            //System.Diagnostics.Debug.Assert(false);
            Assert.That(MainUnassignedOptionalInner.ChildState != null);
            Assert.That(MainUnassignedOptionalInner.ChildState.ParentInner == MainUnassignedOptionalInner);

            IReadOnlyOptionalInner<IReadOnlyBrowsingOptionalNodeIndex> MainAssignedOptionalTreeInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalTree)) as IReadOnlyOptionalInner<IReadOnlyBrowsingOptionalNodeIndex>;
            Assert.That(MainAssignedOptionalTreeInner != null);
            Assert.That(MainAssignedOptionalTreeInner.InterfaceType == typeof(ITree));
            Assert.That(MainAssignedOptionalTreeInner.IsAssigned);
            Assert.That(MainAssignedOptionalTreeInner.ChildState != null);
            Assert.That(MainAssignedOptionalTreeInner.ChildState.ParentInner == MainAssignedOptionalTreeInner);

            IReadOnlyOptionalInner<IReadOnlyBrowsingOptionalNodeIndex> MainAssignedOptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IReadOnlyOptionalInner<IReadOnlyBrowsingOptionalNodeIndex>;
            Assert.That(MainAssignedOptionalLeafInner != null);
            Assert.That(MainAssignedOptionalLeafInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainAssignedOptionalLeafInner.IsAssigned);
            Assert.That(MainAssignedOptionalLeafInner.ChildState != null);
            Assert.That(MainAssignedOptionalLeafInner.ChildState.ParentInner == MainAssignedOptionalLeafInner);

            IReadOnlyBlockListInner<IReadOnlyBrowsingBlockNodeIndex> MainLeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IReadOnlyBlockListInner<IReadOnlyBrowsingBlockNodeIndex>;
            Assert.That(MainLeafBlocksInner != null);
            Assert.That(!MainLeafBlocksInner.IsNeverEmpty);
            Assert.That(!MainLeafBlocksInner.IsEmpty);
            Assert.That(!MainLeafBlocksInner.IsSingle);
            Assert.That(MainLeafBlocksInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainLeafBlocksInner.BlockType == typeof(BaseNode.IBlock<ILeaf, Leaf>));
            Assert.That(MainLeafBlocksInner.ItemType == typeof(Leaf));
            Assert.That(MainLeafBlocksInner.Count == 3);
            Assert.That(MainLeafBlocksInner.BlockStateList != null);
            Assert.That(MainLeafBlocksInner.BlockStateList.Count == 2);

            IReadOnlyBlockState LeafBlock = MainLeafBlocksInner.BlockStateList[0];
            Assert.That(LeafBlock != null);
            Assert.That(LeafBlock.StateList != null);
            Assert.That(LeafBlock.StateList.Count == 1);
            Assert.That(MainLeafBlocksInner.FirstNodeState == LeafBlock.StateList[0]);
            Assert.That(MainLeafBlocksInner.IndexAt(0, 0) == MainLeafBlocksInner.FirstNodeState.ParentIndex);

            IReadOnlyPlaceholderInner<IReadOnlyBrowsingPlaceholderNodeIndex> PatternInner = LeafBlock.PropertyToInner(nameof(BaseNode.IBlock.ReplicationPattern)) as IReadOnlyPlaceholderInner<IReadOnlyBrowsingPlaceholderNodeIndex>;
            Assert.That(PatternInner != null);

            IReadOnlyPlaceholderInner<IReadOnlyBrowsingPlaceholderNodeIndex> SourceInner = LeafBlock.PropertyToInner(nameof(BaseNode.IBlock.SourceIdentifier)) as IReadOnlyPlaceholderInner<IReadOnlyBrowsingPlaceholderNodeIndex>;
            Assert.That(SourceInner != null);

            IReadOnlyPatternState PatternState = LeafBlock.PatternState;
            Assert.That(PatternState != null);
            Assert.That(PatternState.ParentInner == PatternInner);
            Assert.That(PatternState.ParentIndex == LeafBlock.PatternIndex);

            IReadOnlySourceState SourceState = LeafBlock.SourceState;
            Assert.That(SourceState != null);
            Assert.That(SourceState.ParentInner == SourceInner);
            Assert.That(SourceState.ParentIndex == LeafBlock.SourceIndex);

            Assert.That(MainLeafBlocksInner.FirstNodeState == LeafBlock.StateList[0]);

            IReadOnlyListInner<IReadOnlyBrowsingListNodeIndex> MainLeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IReadOnlyListInner<IReadOnlyBrowsingListNodeIndex>;
            Assert.That(MainLeafPathInner != null);
            Assert.That(!MainLeafPathInner.IsNeverEmpty);
            Assert.That(MainLeafPathInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainLeafPathInner.Count == 1);
            Assert.That(MainLeafPathInner.StateList != null);
            Assert.That(MainLeafPathInner.StateList.Count == 1);
            Assert.That(MainLeafPathInner.FirstNodeState == MainLeafPathInner.StateList[0]);
            Assert.That(MainLeafPathInner.IndexAt(0) == MainLeafPathInner.FirstNodeState.ParentIndex);

            IReadOnlyNodeStateReadOnlyList AllChildren = RootState.GetAllChildren();
            Assert.That(AllChildren.Count == 15, $"New count: {AllChildren.Count}");

            IReadOnlyPlaceholderInner<IReadOnlyBrowsingPlaceholderNodeIndex> PlaceholderInner = RootState.InnerTable[nameof(IMain.PlaceholderLeaf)] as IReadOnlyPlaceholderInner<IReadOnlyBrowsingPlaceholderNodeIndex>;
            Assert.That(PlaceholderInner != null);

            IReadOnlyBrowsingPlaceholderNodeIndex PlaceholderNodeIndex = PlaceholderInner.ChildState.ParentIndex as IReadOnlyBrowsingPlaceholderNodeIndex;
            Assert.That(PlaceholderNodeIndex != null);
            Assert.That(Controller0.Contains(PlaceholderNodeIndex));

            IReadOnlyOptionalInner<IReadOnlyBrowsingOptionalNodeIndex> UnassignedOptionalInner = RootState.InnerTable[nameof(IMain.UnassignedOptionalLeaf)] as IReadOnlyOptionalInner<IReadOnlyBrowsingOptionalNodeIndex>;
            Assert.That(UnassignedOptionalInner != null);

            IReadOnlyBrowsingOptionalNodeIndex UnassignedOptionalNodeIndex = UnassignedOptionalInner.ChildState.ParentIndex;
            Assert.That(UnassignedOptionalNodeIndex != null);
            Assert.That(Controller0.Contains(UnassignedOptionalNodeIndex));
            Assert.That(Controller0.IsAssigned(UnassignedOptionalNodeIndex) == false);

            IReadOnlyOptionalInner<IReadOnlyBrowsingOptionalNodeIndex> AssignedOptionalInner = RootState.InnerTable[nameof(IMain.AssignedOptionalLeaf)] as IReadOnlyOptionalInner<IReadOnlyBrowsingOptionalNodeIndex>;
            Assert.That(AssignedOptionalInner != null);

            IReadOnlyBrowsingOptionalNodeIndex AssignedOptionalNodeIndex = AssignedOptionalInner.ChildState.ParentIndex;
            Assert.That(AssignedOptionalNodeIndex != null);
            Assert.That(Controller0.Contains(AssignedOptionalNodeIndex));
            Assert.That(Controller0.IsAssigned(AssignedOptionalNodeIndex) == true);

            int Min, Max;
            object ReadValue;

            RootState.PropertyToValue(nameof(IMain.ValueBoolean), out ReadValue, out Min, out Max);
            bool ReadAsBoolean = ((int)ReadValue) != 0;
            Assert.That(ReadAsBoolean == true);
            Assert.That(Controller0.GetDiscreteValue(RootIndex0, nameof(IMain.ValueBoolean)) == (ReadAsBoolean ? 1 : 0));
            Assert.That(Min == 0);
            Assert.That(Max == 1);

            RootState.PropertyToValue(nameof(IMain.ValueEnum), out ReadValue, out Min, out Max);
            BaseNode.CopySemantic ReadAsEnum = (BaseNode.CopySemantic)(int)ReadValue;
            Assert.That(ReadAsEnum == BaseNode.CopySemantic.Value);
            Assert.That(Controller0.GetDiscreteValue(RootIndex0, nameof(IMain.ValueEnum)) == (int)ReadAsEnum);
            Assert.That(Min == 0);
            Assert.That(Max == 2);

            RootState.PropertyToValue(nameof(IMain.ValueString), out ReadValue, out Min, out Max);
            string ReadAsString = ReadValue as string;
            Assert.That(ReadAsString == "string");
            Assert.That(Controller0.GetStringValue(RootIndex0, nameof(IMain.ValueString)) == ReadAsString);

            RootState.PropertyToValue(nameof(IMain.ValueGuid), out ReadValue, out Min, out Max);
            Guid ReadAsGuid = (Guid)ReadValue;
            Assert.That(ReadAsGuid == ValueGuid);
            Assert.That(Controller0.GetGuidValue(RootIndex0, nameof(IMain.ValueGuid)) == ReadAsGuid);
            
            IReadOnlyController Controller1 = ReadOnlyController.Create(RootIndex0);
            Assert.That(Controller0.IsEqual(CompareEqual.New(), Controller0));

            Assert.That(CompareEqual.CoverIsEqual(Controller0, Controller1));
        }

        [Test]
        [Category("Coverage")]
        public static void ReadOnlyClone()
        {
            IMain RootNode = CreateRoot(Imperfections.None);

            IReadOnlyRootNodeIndex RootIndex = new ReadOnlyRootNodeIndex(RootNode);
            Assert.That(RootIndex != null);

            IReadOnlyController Controller = ReadOnlyController.Create(RootIndex);
            Assert.That(Controller != null);

            IReadOnlyPlaceholderNodeState RootState = Controller.RootState;
            Assert.That(RootState != null);

            BaseNode.INode ClonedNode = RootState.CloneNode();
            Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(ClonedNode));

            IReadOnlyRootNodeIndex CloneRootIndex = new ReadOnlyRootNodeIndex(ClonedNode);
            Assert.That(CloneRootIndex != null);

            IReadOnlyController CloneController = ReadOnlyController.Create(CloneRootIndex);
            Assert.That(CloneController != null);

            IReadOnlyPlaceholderNodeState CloneRootState = Controller.RootState;
            Assert.That(CloneRootState != null);

            IReadOnlyNodeStateReadOnlyList AllChildren = RootState.GetAllChildren();
            IReadOnlyNodeStateReadOnlyList CloneAllChildren = CloneRootState.GetAllChildren();
            Assert.That(AllChildren.Count == CloneAllChildren.Count);
        }

        [Test]
        [Category("Coverage")]
        public static void ReadOnlyViews()
        {
            IMain RootNode;
            IReadOnlyRootNodeIndex RootIndex;

            RootNode = CreateRoot(Imperfections.None);
            RootIndex = new ReadOnlyRootNodeIndex(RootNode);

            IReadOnlyController Controller = ReadOnlyController.Create(RootIndex);

            using (IReadOnlyControllerView ControllerView0 = ReadOnlyControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                using (IReadOnlyControllerView ControllerView1 = ReadOnlyControllerView.Create(Controller))
                {
                    Assert.That(ControllerView0.IsEqual(CompareEqual.New(), ControllerView0));
                    Assert.That(CompareEqual.CoverIsEqual(ControllerView0, ControllerView1));
                }

                foreach (KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView> Entry in ControllerView0.BlockStateViewTable)
                {
                    IReadOnlyBlockState BlockState = Entry.Key;
                    Assert.That(BlockState != null);

                    IReadOnlyBlockStateView BlockStateView = Entry.Value;
                    Assert.That(BlockStateView != null);

                    Assert.That(BlockStateView.ControllerView == ControllerView0);
                }

                foreach (KeyValuePair<IReadOnlyNodeState, IReadOnlyNodeStateView> Entry in ControllerView0.StateViewTable)
                {
                    IReadOnlyNodeState State = Entry.Key;
                    Assert.That(State != null);

                    IReadOnlyNodeStateView StateView = Entry.Value;
                    Assert.That(StateView != null);
                    Assert.That(StateView.State == State);

                    IReadOnlyIndex ParentIndex = State.ParentIndex;
                    Assert.That(ParentIndex != null);

                    Assert.That(Controller.Contains(ParentIndex));
                    Assert.That(StateView.ControllerView == ControllerView0);

                    switch (StateView)
                    {
                        case IReadOnlyPatternStateView AsPatternStateView:
                            Assert.That(AsPatternStateView.State == State);
                            break;

                        case IReadOnlySourceStateView AsSourceStateView:
                            Assert.That(AsSourceStateView.State == State);
                            break;

                        case IReadOnlyPlaceholderNodeStateView AsPlaceholderNodeStateView:
                            Assert.That(AsPlaceholderNodeStateView.State == State);
                            break;

                        case IReadOnlyOptionalNodeStateView AsOptionalNodeStateView:
                            Assert.That(AsOptionalNodeStateView.State == State);
                            break;
                    }
                }
            }

            GC.Collect();
        }
        #endregion
    }
}
