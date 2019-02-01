using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using EaslyController;
using EaslyController.ReadOnly;
using EaslyController.Writeable;
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
        private static Guid LeafGuid7 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887207}");
        private static Guid TreeGuid0 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887250}");
        private static Guid TreeGuid1 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887251}");
        private static Guid TreeGuid2 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887252}");
        private static Guid TreeGuid3 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887253}");
        private static Guid TreeGuid4 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887254}");
        private static Guid TreeGuid5 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887255}");
        private static Guid MainGuid = new Guid("{EDBDC354-C70B-4BAF-AE1B-C342CD9BFADE}");

        private static Leaf CreateLeaf(Guid guid0)
        {
            Leaf NewLeaf = new Leaf();

            BaseNode.IDocument NewLeafDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", guid0);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(NewLeaf, NewLeafDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(NewLeaf, nameof(ILeaf.Text), "leaf");

            return NewLeaf;
        }

        private static Tree CreateTree(Guid guid0, Guid guid1, Guid guid2)
        {
            Leaf Placeholder = CreateLeaf(guid0);

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

            Leaf PlaceholderLeaf = CreateLeaf(imperfection == Imperfections.BadGuid ? MainGuid : LeafGuid0);

            Leaf UnassignedOptionalLeaf = new Leaf();

            BaseNode.IDocument UnassignedOptionalLeafDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", LeafGuid1);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(UnassignedOptionalLeaf, UnassignedOptionalLeafDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(UnassignedOptionalLeaf, nameof(ILeaf.Text), "optional unassigned");

            Easly.IOptionalReference<ILeaf> UnassignedOptional = BaseNodeHelper.OptionalReferenceHelper<ILeaf>.CreateReference(UnassignedOptionalLeaf);

            Leaf AssignedOptionalLeaf = CreateLeaf(LeafGuid2);

            Easly.IOptionalReference<ILeaf> AssignedOptionalForLeaf = BaseNodeHelper.OptionalReferenceHelper<ILeaf>.CreateReference(AssignedOptionalLeaf);
            AssignedOptionalForLeaf.Assign();

            Tree AssignedOptionalTree = CreateTree(TreeGuid3, TreeGuid4, TreeGuid5);
            Easly.IOptionalReference<ITree> AssignedOptionalForTree = BaseNodeHelper.OptionalReferenceHelper<ITree>.CreateReference(AssignedOptionalTree);
            AssignedOptionalForTree.Assign();

            Leaf FirstChild = CreateLeaf(LeafGuid3);
            Leaf SecondChild = CreateLeaf(LeafGuid4);
            Leaf ThirdChild = CreateLeaf(LeafGuid5);

            BaseNode.IBlock<ILeaf, Leaf> SecondBlock = BaseNodeHelper.BlockListHelper<ILeaf, Leaf>.CreateBlock(new List<ILeaf>() { SecondChild, ThirdChild });

            BaseNode.IBlockList<ILeaf, Leaf> LeafBlocks = BaseNodeHelper.BlockListHelper<ILeaf, Leaf>.CreateSimpleBlockList(FirstChild);
            LeafBlocks.NodeBlockList.Add(SecondBlock);

            Leaf FirstPath = CreateLeaf(LeafGuid6);
            Leaf SecondPath = CreateLeaf(LeafGuid7);

            IList<ILeaf> LeafPath = new List<ILeaf>();
            LeafPath.Add(FirstPath);
            LeafPath.Add(SecondPath);

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
            ControllerTools.ResetExpectedName();

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
            ControllerTools.ResetExpectedName();

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

            IReadOnlyPlaceholderInner MainPlaceholderTreeInner = RootState.PropertyToInner(nameof(IMain.PlaceholderTree)) as IReadOnlyPlaceholderInner;
            Assert.That(MainPlaceholderTreeInner != null);
            Assert.That(MainPlaceholderTreeInner.InterfaceType == typeof(ITree));
            Assert.That(MainPlaceholderTreeInner.ChildState != null);
            Assert.That(MainPlaceholderTreeInner.ChildState.ParentInner == MainPlaceholderTreeInner);

            IReadOnlyPlaceholderInner MainPlaceholderLeafInner = RootState.PropertyToInner(nameof(IMain.PlaceholderLeaf)) as IReadOnlyPlaceholderInner;
            Assert.That(MainPlaceholderLeafInner != null);
            Assert.That(MainPlaceholderLeafInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainPlaceholderLeafInner.ChildState != null);
            Assert.That(MainPlaceholderLeafInner.ChildState.ParentInner == MainPlaceholderLeafInner);

            IReadOnlyOptionalInner MainUnassignedOptionalInner = RootState.PropertyToInner(nameof(IMain.UnassignedOptionalLeaf)) as IReadOnlyOptionalInner;
            Assert.That(MainUnassignedOptionalInner != null);
            Assert.That(MainUnassignedOptionalInner.InterfaceType == typeof(ILeaf));
            Assert.That(!MainUnassignedOptionalInner.IsAssigned);
            //System.Diagnostics.Debug.Assert(false);
            Assert.That(MainUnassignedOptionalInner.ChildState != null);
            Assert.That(MainUnassignedOptionalInner.ChildState.ParentInner == MainUnassignedOptionalInner);

            IReadOnlyOptionalInner MainAssignedOptionalTreeInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalTree)) as IReadOnlyOptionalInner;
            Assert.That(MainAssignedOptionalTreeInner != null);
            Assert.That(MainAssignedOptionalTreeInner.InterfaceType == typeof(ITree));
            Assert.That(MainAssignedOptionalTreeInner.IsAssigned);
            Assert.That(MainAssignedOptionalTreeInner.ChildState != null);
            Assert.That(MainAssignedOptionalTreeInner.ChildState.ParentInner == MainAssignedOptionalTreeInner);

            IReadOnlyOptionalInner MainAssignedOptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IReadOnlyOptionalInner;
            Assert.That(MainAssignedOptionalLeafInner != null);
            Assert.That(MainAssignedOptionalLeafInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainAssignedOptionalLeafInner.IsAssigned);
            Assert.That(MainAssignedOptionalLeafInner.ChildState != null);
            Assert.That(MainAssignedOptionalLeafInner.ChildState.ParentInner == MainAssignedOptionalLeafInner);

            IReadOnlyBlockListInner MainLeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IReadOnlyBlockListInner;
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

            IReadOnlyPlaceholderInner PatternInner = LeafBlock.PropertyToInner(nameof(BaseNode.IBlock.ReplicationPattern)) as IReadOnlyPlaceholderInner;
            Assert.That(PatternInner != null);

            IReadOnlyPlaceholderInner SourceInner = LeafBlock.PropertyToInner(nameof(BaseNode.IBlock.SourceIdentifier)) as IReadOnlyPlaceholderInner;
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

            IReadOnlyListInner MainLeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IReadOnlyListInner;
            Assert.That(MainLeafPathInner != null);
            Assert.That(!MainLeafPathInner.IsNeverEmpty);
            Assert.That(MainLeafPathInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainLeafPathInner.Count == 2);
            Assert.That(MainLeafPathInner.StateList != null);
            Assert.That(MainLeafPathInner.StateList.Count == 2);
            Assert.That(MainLeafPathInner.FirstNodeState == MainLeafPathInner.StateList[0]);
            Assert.That(MainLeafPathInner.IndexAt(0) == MainLeafPathInner.FirstNodeState.ParentIndex);

            IReadOnlyNodeStateReadOnlyList AllChildren = RootState.GetAllChildren();
            Assert.That(AllChildren.Count == 16, $"New count: {AllChildren.Count}");

            IReadOnlyPlaceholderInner PlaceholderInner = RootState.InnerTable[nameof(IMain.PlaceholderLeaf)] as IReadOnlyPlaceholderInner;
            Assert.That(PlaceholderInner != null);

            IReadOnlyBrowsingPlaceholderNodeIndex PlaceholderNodeIndex = PlaceholderInner.ChildState.ParentIndex as IReadOnlyBrowsingPlaceholderNodeIndex;
            Assert.That(PlaceholderNodeIndex != null);
            Assert.That(Controller0.Contains(PlaceholderNodeIndex));

            IReadOnlyOptionalInner UnassignedOptionalInner = RootState.InnerTable[nameof(IMain.UnassignedOptionalLeaf)] as IReadOnlyOptionalInner;
            Assert.That(UnassignedOptionalInner != null);

            IReadOnlyBrowsingOptionalNodeIndex UnassignedOptionalNodeIndex = UnassignedOptionalInner.ChildState.ParentIndex;
            Assert.That(UnassignedOptionalNodeIndex != null);
            Assert.That(Controller0.Contains(UnassignedOptionalNodeIndex));
            Assert.That(Controller0.IsAssigned(UnassignedOptionalNodeIndex) == false);

            IReadOnlyOptionalInner AssignedOptionalInner = RootState.InnerTable[nameof(IMain.AssignedOptionalLeaf)] as IReadOnlyOptionalInner;
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
            ControllerTools.ResetExpectedName();

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
            ControllerTools.ResetExpectedName();

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
        }
        #endregion

        #region Writeable
        [Test]
        [Category("Coverage")]
        public static void WriteableCreation()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;
            IWriteableController Controller;

            RootNode = CreateRoot(Imperfections.None);
            Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

            try
            {
                RootIndex = new WriteableRootNodeIndex(RootNode);
                Controller = WriteableController.Create(RootIndex);
            }
            catch (Exception e)
            {
                Assert.Fail($"#0: {e}");
            }

            RootNode = CreateRoot(Imperfections.BadGuid);
            Assert.That(!BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

            try
            {
                RootIndex = new WriteableRootNodeIndex(RootNode);
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
        public static void WriteableProperties()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex0;
            IWriteableRootNodeIndex RootIndex1;

            RootNode = CreateRoot(Imperfections.None);
            Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

            RootIndex0 = new WriteableRootNodeIndex(RootNode);
            Assert.That(RootIndex0.Node == RootNode);
            Assert.That(RootIndex0.IsEqual(CompareEqual.New(), RootIndex0));

            RootIndex1 = new WriteableRootNodeIndex(RootNode);
            Assert.That(RootIndex1.Node == RootNode);
            Assert.That(CompareEqual.CoverIsEqual(RootIndex0, RootIndex1));

            IWriteableController Controller0 = WriteableController.Create(RootIndex0);
            Assert.That(Controller0.RootIndex == RootIndex0);

            Stats Stats = Controller0.Stats;
            Assert.That(Stats.NodeCount >= 0);
            Assert.That(Stats.PlaceholderNodeCount >= 0);
            Assert.That(Stats.OptionalNodeCount >= 0);
            Assert.That(Stats.AssignedOptionalNodeCount >= 0);
            Assert.That(Stats.ListCount >= 0);
            Assert.That(Stats.BlockListCount >= 0);
            Assert.That(Stats.BlockCount >= 0);

            IWriteablePlaceholderNodeState RootState = Controller0.RootState;
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

            IWriteablePlaceholderInner MainPlaceholderTreeInner = RootState.PropertyToInner(nameof(IMain.PlaceholderTree)) as IWriteablePlaceholderInner;
            Assert.That(MainPlaceholderTreeInner != null);
            Assert.That(MainPlaceholderTreeInner.InterfaceType == typeof(ITree));
            Assert.That(MainPlaceholderTreeInner.ChildState != null);
            Assert.That(MainPlaceholderTreeInner.ChildState.ParentInner == MainPlaceholderTreeInner);

            IWriteablePlaceholderInner MainPlaceholderLeafInner = RootState.PropertyToInner(nameof(IMain.PlaceholderLeaf)) as IWriteablePlaceholderInner;
            Assert.That(MainPlaceholderLeafInner != null);
            Assert.That(MainPlaceholderLeafInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainPlaceholderLeafInner.ChildState != null);
            Assert.That(MainPlaceholderLeafInner.ChildState.ParentInner == MainPlaceholderLeafInner);

            IWriteableOptionalInner MainUnassignedOptionalInner = RootState.PropertyToInner(nameof(IMain.UnassignedOptionalLeaf)) as IWriteableOptionalInner;
            Assert.That(MainUnassignedOptionalInner != null);
            Assert.That(MainUnassignedOptionalInner.InterfaceType == typeof(ILeaf));
            Assert.That(!MainUnassignedOptionalInner.IsAssigned);
            Assert.That(MainUnassignedOptionalInner.ChildState != null);
            Assert.That(MainUnassignedOptionalInner.ChildState.ParentInner == MainUnassignedOptionalInner);

            IWriteableOptionalInner MainAssignedOptionalTreeInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalTree)) as IWriteableOptionalInner;
            Assert.That(MainAssignedOptionalTreeInner != null);
            Assert.That(MainAssignedOptionalTreeInner.InterfaceType == typeof(ITree));
            Assert.That(MainAssignedOptionalTreeInner.IsAssigned);
            Assert.That(MainAssignedOptionalTreeInner.ChildState != null);
            Assert.That(MainAssignedOptionalTreeInner.ChildState.ParentInner == MainAssignedOptionalTreeInner);

            IWriteableOptionalInner MainAssignedOptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IWriteableOptionalInner;
            Assert.That(MainAssignedOptionalLeafInner != null);
            Assert.That(MainAssignedOptionalLeafInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainAssignedOptionalLeafInner.IsAssigned);
            Assert.That(MainAssignedOptionalLeafInner.ChildState != null);
            Assert.That(MainAssignedOptionalLeafInner.ChildState.ParentInner == MainAssignedOptionalLeafInner);

            IWriteableBlockListInner MainLeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
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

            IWriteableBlockState LeafBlock = MainLeafBlocksInner.BlockStateList[0];
            Assert.That(LeafBlock != null);
            Assert.That(LeafBlock.StateList != null);
            Assert.That(LeafBlock.StateList.Count == 1);
            Assert.That(MainLeafBlocksInner.FirstNodeState == LeafBlock.StateList[0]);
            Assert.That(MainLeafBlocksInner.IndexAt(0, 0) == MainLeafBlocksInner.FirstNodeState.ParentIndex);

            IWriteablePlaceholderInner PatternInner = LeafBlock.PropertyToInner(nameof(BaseNode.IBlock.ReplicationPattern)) as IWriteablePlaceholderInner;
            Assert.That(PatternInner != null);

            IWriteablePlaceholderInner SourceInner = LeafBlock.PropertyToInner(nameof(BaseNode.IBlock.SourceIdentifier)) as IWriteablePlaceholderInner;
            Assert.That(SourceInner != null);

            IWriteablePatternState PatternState = LeafBlock.PatternState;
            Assert.That(PatternState != null);
            Assert.That(PatternState.ParentInner == PatternInner);
            Assert.That(PatternState.ParentIndex == LeafBlock.PatternIndex);

            IWriteableSourceState SourceState = LeafBlock.SourceState;
            Assert.That(SourceState != null);
            Assert.That(SourceState.ParentInner == SourceInner);
            Assert.That(SourceState.ParentIndex == LeafBlock.SourceIndex);

            Assert.That(MainLeafBlocksInner.FirstNodeState == LeafBlock.StateList[0]);

            IWriteableListInner MainLeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IWriteableListInner;
            Assert.That(MainLeafPathInner != null);
            Assert.That(!MainLeafPathInner.IsNeverEmpty);
            Assert.That(MainLeafPathInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainLeafPathInner.Count == 2);
            Assert.That(MainLeafPathInner.StateList != null);
            Assert.That(MainLeafPathInner.StateList.Count == 2);
            Assert.That(MainLeafPathInner.FirstNodeState == MainLeafPathInner.StateList[0]);
            Assert.That(MainLeafPathInner.IndexAt(0) == MainLeafPathInner.FirstNodeState.ParentIndex);

            IWriteableNodeStateReadOnlyList AllChildren = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
            Assert.That(AllChildren.Count == 16, $"New count: {AllChildren.Count}");

            IWriteablePlaceholderInner PlaceholderInner = RootState.InnerTable[nameof(IMain.PlaceholderLeaf)] as IWriteablePlaceholderInner;
            Assert.That(PlaceholderInner != null);

            IWriteableBrowsingPlaceholderNodeIndex PlaceholderNodeIndex = PlaceholderInner.ChildState.ParentIndex as IWriteableBrowsingPlaceholderNodeIndex;
            Assert.That(PlaceholderNodeIndex != null);
            Assert.That(Controller0.Contains(PlaceholderNodeIndex));

            IWriteableOptionalInner UnassignedOptionalInner = RootState.InnerTable[nameof(IMain.UnassignedOptionalLeaf)] as IWriteableOptionalInner;
            Assert.That(UnassignedOptionalInner != null);

            IWriteableBrowsingOptionalNodeIndex UnassignedOptionalNodeIndex = UnassignedOptionalInner.ChildState.ParentIndex;
            Assert.That(UnassignedOptionalNodeIndex != null);
            Assert.That(Controller0.Contains(UnassignedOptionalNodeIndex));
            Assert.That(Controller0.IsAssigned(UnassignedOptionalNodeIndex) == false);

            IWriteableOptionalInner AssignedOptionalInner = RootState.InnerTable[nameof(IMain.AssignedOptionalLeaf)] as IWriteableOptionalInner;
            Assert.That(AssignedOptionalInner != null);

            IWriteableBrowsingOptionalNodeIndex AssignedOptionalNodeIndex = AssignedOptionalInner.ChildState.ParentIndex;
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

            IWriteableController Controller1 = WriteableController.Create(RootIndex0);
            Assert.That(Controller0.IsEqual(CompareEqual.New(), Controller0));

            //System.Diagnostics.Debug.Assert(false);
            Assert.That(CompareEqual.CoverIsEqual(Controller0, Controller1));
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableClone()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode = CreateRoot(Imperfections.None);

            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(RootNode);
            Assert.That(RootIndex != null);

            IWriteableController Controller = WriteableController.Create(RootIndex);
            Assert.That(Controller != null);

            IWriteablePlaceholderNodeState RootState = Controller.RootState;
            Assert.That(RootState != null);

            BaseNode.INode ClonedNode = RootState.CloneNode();
            Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(ClonedNode));

            IWriteableRootNodeIndex CloneRootIndex = new WriteableRootNodeIndex(ClonedNode);
            Assert.That(CloneRootIndex != null);

            IWriteableController CloneController = WriteableController.Create(CloneRootIndex);
            Assert.That(CloneController != null);

            IWriteablePlaceholderNodeState CloneRootState = Controller.RootState;
            Assert.That(CloneRootState != null);

            IWriteableNodeStateReadOnlyList AllChildren = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
            IWriteableNodeStateReadOnlyList CloneAllChildren = (IWriteableNodeStateReadOnlyList)CloneRootState.GetAllChildren();
            Assert.That(AllChildren.Count == CloneAllChildren.Count);
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableViews()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                using (IWriteableControllerView ControllerView1 = WriteableControllerView.Create(Controller))
                {
                    Assert.That(ControllerView0.IsEqual(CompareEqual.New(), ControllerView0));
                    Assert.That(CompareEqual.CoverIsEqual(ControllerView0, ControllerView1));
                }

                foreach (KeyValuePair<IWriteableBlockState, IWriteableBlockStateView> Entry in ControllerView0.BlockStateViewTable)
                {
                    IWriteableBlockState BlockState = Entry.Key;
                    Assert.That(BlockState != null);

                    IWriteableBlockStateView BlockStateView = Entry.Value;
                    Assert.That(BlockStateView != null);

                    Assert.That(BlockStateView.ControllerView == ControllerView0);
                }

                foreach (KeyValuePair<IWriteableNodeState, IWriteableNodeStateView> Entry in ControllerView0.StateViewTable)
                {
                    IWriteableNodeState State = Entry.Key;
                    Assert.That(State != null);

                    IWriteableNodeStateView StateView = Entry.Value;
                    Assert.That(StateView != null);
                    Assert.That(StateView.State == State);

                    IWriteableIndex ParentIndex = State.ParentIndex;
                    Assert.That(ParentIndex != null);

                    Assert.That(Controller.Contains(ParentIndex));
                    Assert.That(StateView.ControllerView == ControllerView0);

                    switch (StateView)
                    {
                        case IWriteablePatternStateView AsPatternStateView:
                            Assert.That(AsPatternStateView.State == State);
                            break;

                        case IWriteableSourceStateView AsSourceStateView:
                            Assert.That(AsSourceStateView.State == State);
                            break;

                        case IWriteablePlaceholderNodeStateView AsPlaceholderNodeStateView:
                            Assert.That(AsPlaceholderNodeStateView.State == State);
                            break;

                        case IWriteableOptionalNodeStateView AsOptionalNodeStateView:
                            Assert.That(AsOptionalNodeStateView.State == State);
                            break;
                    }
                }
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableInsert()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                Leaf NewItem0 = CreateLeaf(Guid.NewGuid());
                IWriteableInsertionListNodeIndex InsertionIndex0 = new WriteableInsertionListNodeIndex(RootNode, nameof(IMain.LeafPath), NewItem0, 0);

                IWriteableListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IWriteableListInner;
                Assert.That(LeafPathInner != null);

                int PathCount = LeafPathInner.Count;
                Assert.That(PathCount == 2);

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 16, $"New count: {AllChildren0.Count}");

                Controller.Insert(LeafPathInner, InsertionIndex0, out IWriteableBrowsingCollectionNodeIndex NewItemIndex0);
                Assert.That(Controller.Contains(NewItemIndex0));

                Assert.That(LeafPathInner.Count == PathCount + 1);
                Assert.That(LeafPathInner.StateList.Count == PathCount + 1);

                IWriteablePlaceholderNodeState NewItemState0 = LeafPathInner.StateList[0];
                Assert.That(NewItemState0.Node == NewItem0);
                Assert.That(NewItemState0.ParentIndex == NewItemIndex0);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                Leaf NewItem1 = CreateLeaf(Guid.NewGuid());
                IWriteableInsertionExistingBlockNodeIndex InsertionIndex1 = new WriteableInsertionExistingBlockNodeIndex(RootNode, nameof(IMain.LeafBlocks), NewItem1, 0, 0);

                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[0].StateList.Count;
                Assert.That(BlockNodeCount == 3);

                Controller.Insert(LeafBlocksInner, InsertionIndex1, out IWriteableBrowsingCollectionNodeIndex NewItemIndex1);
                Assert.That(Controller.Contains(NewItemIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount + 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == NodeCount + 1);

                IWriteablePlaceholderNodeState NewItemState1 = LeafBlocksInner.BlockStateList[0].StateList[0];
                Assert.That(NewItemState1.Node == NewItem1);
                Assert.That(NewItemState1.ParentIndex == NewItemIndex1);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count + 1, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));




                Leaf NewItem2 = CreateLeaf(Guid.NewGuid());
                BaseNode.IPattern NewPattern = BaseNodeHelper.NodeHelper.CreateSimplePattern("");
                BaseNode.IIdentifier NewSource = BaseNodeHelper.NodeHelper.CreateSimpleIdentifier("");

                IWriteableInsertionNewBlockNodeIndex InsertionIndex2 = new WriteableInsertionNewBlockNodeIndex(RootNode, nameof(IMain.LeafBlocks), NewItem2, 0, NewPattern, NewSource);

                int BlockCount = LeafBlocksInner.BlockStateList.Count;
                Assert.That(BlockCount == 2);

                Controller.Insert(LeafBlocksInner, InsertionIndex2, out IWriteableBrowsingCollectionNodeIndex NewItemIndex2);
                Assert.That(Controller.Contains(NewItemIndex2));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount + 2);
                Assert.That(LeafBlocksInner.BlockStateList.Count == BlockCount + 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == 1, $"Count: {LeafBlocksInner.BlockStateList[0].StateList.Count}");
                Assert.That(LeafBlocksInner.BlockStateList[1].StateList.Count == 2, $"Count: {LeafBlocksInner.BlockStateList[1].StateList.Count}");
                Assert.That(LeafBlocksInner.BlockStateList[2].StateList.Count == 2, $"Count: {LeafBlocksInner.BlockStateList[2].StateList.Count}");

                IWriteablePlaceholderNodeState NewItemState2 = LeafBlocksInner.BlockStateList[0].StateList[0];
                Assert.That(NewItemState2.Node == NewItem2);
                Assert.That(NewItemState2.ParentIndex == NewItemIndex2);

                IWriteableNodeStateReadOnlyList AllChildren3 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count + 3, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableRemove()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IWriteableListInner;
                Assert.That(LeafPathInner != null);

                IWriteableBrowsingListNodeIndex RemovedLeafIndex0 = LeafPathInner.StateList[0].ParentIndex as IWriteableBrowsingListNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex0));

                int PathCount = LeafPathInner.Count;
                Assert.That(PathCount == 2);

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 16, $"New count: {AllChildren0.Count}");

                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedLeafIndex0));

                Controller.Remove(LeafPathInner, RemovedLeafIndex0);
                Assert.That(!Controller.Contains(RemovedLeafIndex0));

                Assert.That(LeafPathInner.Count == PathCount - 1);
                Assert.That(LeafPathInner.StateList.Count == PathCount - 1);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count - 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                RemovedLeafIndex0 = LeafPathInner.StateList[0].ParentIndex as IWriteableBrowsingListNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex0));

                Assert.That(LeafPathInner.Count == 1);

                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedLeafIndex0));

                IDictionary<Type, string[]> NeverEmptyCollectionTable = BaseNodeHelper.NodeHelper.NeverEmptyCollectionTable as IDictionary<Type, string[]>;
                NeverEmptyCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafPath) });
                Assert.That(!Controller.IsRemoveable(LeafPathInner, RemovedLeafIndex0));



                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IWriteableBrowsingExistingBlockNodeIndex RemovedLeafIndex1 = LeafBlocksInner.BlockStateList[1].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex1));

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[1].StateList.Count;
                Assert.That(BlockNodeCount == 3, $"New count: {BlockNodeCount}");

                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex1));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex1);
                Assert.That(!Controller.Contains(RemovedLeafIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount - 1);
                Assert.That(LeafBlocksInner.BlockStateList[1].StateList.Count == NodeCount - 1);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count - 1, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IWriteableBrowsingExistingBlockNodeIndex RemovedLeafIndex2 = LeafBlocksInner.BlockStateList[1].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex2));


                int BlockCount = LeafBlocksInner.BlockStateList.Count;
                Assert.That(BlockCount == 2);

                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex2));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex2);
                Assert.That(!Controller.Contains(RemovedLeafIndex2));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount - 2);
                Assert.That(LeafBlocksInner.BlockStateList.Count == BlockCount - 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == 1, $"Count: {LeafBlocksInner.BlockStateList[0].StateList.Count}");

                IWriteableNodeStateReadOnlyList AllChildren3 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count - 3, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                NeverEmptyCollectionTable.Remove(typeof(IMain));
                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedLeafIndex0));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableMove()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IWriteableListInner;
                Assert.That(LeafPathInner != null);

                IWriteableBrowsingListNodeIndex MovedLeafIndex0 = LeafPathInner.IndexAt(0) as IWriteableBrowsingListNodeIndex;
                Assert.That(Controller.Contains(MovedLeafIndex0));

                int PathCount = LeafPathInner.Count;
                Assert.That(PathCount == 2);

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 16, $"New count: {AllChildren0.Count}");

                Controller.Move(LeafPathInner, MovedLeafIndex0, +1);
                Assert.That(Controller.Contains(MovedLeafIndex0));

                Assert.That(LeafPathInner.Count == PathCount);
                Assert.That(LeafPathInner.StateList.Count == PathCount);

                //System.Diagnostics.Debug.Assert(false);
                IWriteableBrowsingListNodeIndex NewLeafIndex0 = LeafPathInner.IndexAt(1) as IWriteableBrowsingListNodeIndex;
                Assert.That(NewLeafIndex0 == MovedLeafIndex0);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));




                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IWriteableBrowsingExistingBlockNodeIndex MovedLeafIndex1 = LeafBlocksInner.IndexAt(1, 1) as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(MovedLeafIndex1));

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[1].StateList.Count;
                Assert.That(BlockNodeCount == 3, $"New count: {BlockNodeCount}");

                Controller.Move(LeafBlocksInner, MovedLeafIndex1, -1);
                Assert.That(Controller.Contains(MovedLeafIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount);
                Assert.That(LeafBlocksInner.BlockStateList[1].StateList.Count == NodeCount);

                IWriteableBrowsingExistingBlockNodeIndex NewLeafIndex1 = LeafBlocksInner.IndexAt(1, 0) as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(NewLeafIndex1 == MovedLeafIndex1);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableMoveBlock()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == 16, $"New count: {AllChildren1.Count}");

                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IWriteableBrowsingExistingBlockNodeIndex MovedLeafIndex1 = LeafBlocksInner.IndexAt(1, 0) as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(MovedLeafIndex1));

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[1].StateList.Count;
                Assert.That(BlockNodeCount == 3, $"New count: {BlockNodeCount}");

                Controller.MoveBlock(LeafBlocksInner, 1, -1);
                Assert.That(Controller.Contains(MovedLeafIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == NodeCount);

                IWriteableBrowsingExistingBlockNodeIndex NewLeafIndex1 = LeafBlocksInner.IndexAt(0, 0) as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(NewLeafIndex1 == MovedLeafIndex1);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableChangeDiscreteValue()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                Assert.That(BaseNodeHelper.NodeTreeHelper.GetEnumValue(RootState.Node, nameof(IMain.ValueEnum)) == (int)BaseNode.CopySemantic.Value);

                Controller.ChangeDiscreteValue(RootIndex, nameof(IMain.ValueEnum), (int)BaseNode.CopySemantic.Reference);

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(BaseNodeHelper.NodeTreeHelper.GetEnumValue(RootNode, nameof(IMain.ValueEnum)) == (int)BaseNode.CopySemantic.Reference);

                IWriteablePlaceholderInner PlaceholderTreeInner = RootState.PropertyToInner(nameof(IMain.PlaceholderTree)) as IWriteablePlaceholderInner;
                IWriteablePlaceholderNodeState PlaceholderTreeState = PlaceholderTreeInner.ChildState as IWriteablePlaceholderNodeState;

                Assert.That(BaseNodeHelper.NodeTreeHelper.GetEnumValue(PlaceholderTreeState.Node, nameof(ITree.ValueEnum)) == (int)BaseNode.CopySemantic.Value);

                Controller.ChangeDiscreteValue(PlaceholderTreeState.ParentIndex, nameof(ITree.ValueEnum), (int)BaseNode.CopySemantic.Any);

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(BaseNodeHelper.NodeTreeHelper.GetEnumValue(PlaceholderTreeState.Node, nameof(ITree.ValueEnum)) == (int)BaseNode.CopySemantic.Any);
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableReplace()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                Leaf NewItem0 = CreateLeaf(Guid.NewGuid());
                IWriteableInsertionListNodeIndex ReplacementIndex0 = new WriteableInsertionListNodeIndex(RootNode, nameof(IMain.LeafPath), NewItem0, 0);

                IWriteableListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IWriteableListInner;
                Assert.That(LeafPathInner != null);

                int PathCount = LeafPathInner.Count;
                Assert.That(PathCount == 2);

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 16, $"New count: {AllChildren0.Count}");

                Controller.Replace(LeafPathInner, ReplacementIndex0, out IWriteableBrowsingChildIndex NewItemIndex0);
                Assert.That(Controller.Contains(NewItemIndex0));

                Assert.That(LeafPathInner.Count == PathCount);
                Assert.That(LeafPathInner.StateList.Count == PathCount);

                IWriteablePlaceholderNodeState NewItemState0 = LeafPathInner.StateList[0];
                Assert.That(NewItemState0.Node == NewItem0);
                Assert.That(NewItemState0.ParentIndex == NewItemIndex0);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                Leaf NewItem1 = CreateLeaf(Guid.NewGuid());
                IWriteableInsertionExistingBlockNodeIndex ReplacementIndex1 = new WriteableInsertionExistingBlockNodeIndex(RootNode, nameof(IMain.LeafBlocks), NewItem1, 0, 0);

                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[0].StateList.Count;
                Assert.That(BlockNodeCount == 3);

                Controller.Replace(LeafBlocksInner, ReplacementIndex1, out IWriteableBrowsingChildIndex NewItemIndex1);
                Assert.That(Controller.Contains(NewItemIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == NodeCount);

                IWriteablePlaceholderNodeState NewItemState1 = LeafBlocksInner.BlockStateList[0].StateList[0];
                Assert.That(NewItemState1.Node == NewItem1);
                Assert.That(NewItemState1.ParentIndex == NewItemIndex1);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));





                Tree NewItem2 = CreateTree(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
                IWriteableInsertionPlaceholderNodeIndex ReplacementIndex2 = new WriteableInsertionPlaceholderNodeIndex(RootNode, nameof(IMain.PlaceholderTree), NewItem2);

                IWriteablePlaceholderInner PlaceholderTreeInner = RootState.PropertyToInner(nameof(IMain.PlaceholderTree)) as IWriteablePlaceholderInner;
                Assert.That(PlaceholderTreeInner != null);

                Controller.Replace(PlaceholderTreeInner, ReplacementIndex2, out IWriteableBrowsingChildIndex NewItemIndex2);
                Assert.That(Controller.Contains(NewItemIndex2));

                IWriteablePlaceholderNodeState NewItemState2 = PlaceholderTreeInner.ChildState as IWriteablePlaceholderNodeState;
                Assert.That(NewItemState2.Node == NewItem2);
                Assert.That(NewItemState2.ParentIndex == NewItemIndex2);

                IWriteableNodeStateReadOnlyList AllChildren3 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                Leaf NewItem3 = CreateLeaf(Guid.NewGuid());
                IWriteableInsertionPlaceholderNodeIndex ReplacementIndex3 = new WriteableInsertionPlaceholderNodeIndex(NewItem2, nameof(ITree.Placeholder), NewItem3);

                IWriteablePlaceholderInner PlaceholderLeafInner = NewItemState2.PropertyToInner(nameof(ITree.Placeholder)) as IWriteablePlaceholderInner;
                Assert.That(PlaceholderLeafInner != null);

                Controller.Replace(PlaceholderLeafInner, ReplacementIndex3, out IWriteableBrowsingChildIndex NewItemIndex3);
                Assert.That(Controller.Contains(NewItemIndex3));

                IWriteablePlaceholderNodeState NewItemState3 = PlaceholderLeafInner.ChildState as IWriteablePlaceholderNodeState;
                Assert.That(NewItemState3.Node == NewItem3);
                Assert.That(NewItemState3.ParentIndex == NewItemIndex3);

                IWriteableNodeStateReadOnlyList AllChildren4 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren4.Count == AllChildren3.Count, $"New count: {AllChildren4.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                Leaf NewItem4 = CreateLeaf(Guid.NewGuid());
                IWriteableInsertionOptionalNodeIndex ReplacementIndex4 = new WriteableInsertionOptionalNodeIndex(RootNode, nameof(IMain.AssignedOptionalLeaf), NewItem4);

                IWriteableOptionalInner OptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IWriteableOptionalInner;
                Assert.That(OptionalLeafInner != null);

                Controller.Replace(OptionalLeafInner, ReplacementIndex4, out IWriteableBrowsingChildIndex NewItemIndex4);
                Assert.That(Controller.Contains(NewItemIndex4));

                Assert.That(OptionalLeafInner.IsAssigned);
                IWriteableOptionalNodeState NewItemState4 = OptionalLeafInner.ChildState as IWriteableOptionalNodeState;
                Assert.That(NewItemState4.Node == NewItem4);
                Assert.That(NewItemState4.ParentIndex == NewItemIndex4);

                IWriteableNodeStateReadOnlyList AllChildren5 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren5.Count == AllChildren4.Count, $"New count: {AllChildren5.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                Leaf NewItem5 = CreateLeaf(Guid.NewGuid());
                IWriteableInsertionOptionalClearIndex ReplacementIndex5 = new WriteableInsertionOptionalClearIndex(RootNode, nameof(IMain.AssignedOptionalLeaf));

                Controller.Replace(OptionalLeafInner, ReplacementIndex5, out IWriteableBrowsingChildIndex NewItemIndex5);
                Assert.That(Controller.Contains(NewItemIndex5));

                Assert.That(!OptionalLeafInner.IsAssigned);
                IWriteableOptionalNodeState NewItemState5 = OptionalLeafInner.ChildState as IWriteableOptionalNodeState;
                Assert.That(NewItemState5.ParentIndex == NewItemIndex5);

                IWriteableNodeStateReadOnlyList AllChildren6 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren6.Count == AllChildren5.Count - 1, $"New count: {AllChildren6.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableAssign()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableOptionalInner UnassignedOptionalLeafInner = RootState.PropertyToInner(nameof(IMain.UnassignedOptionalLeaf)) as IWriteableOptionalInner;
                Assert.That(UnassignedOptionalLeafInner != null);
                Assert.That(!UnassignedOptionalLeafInner.IsAssigned);

                IWriteableBrowsingOptionalNodeIndex AssignmentIndex0 = UnassignedOptionalLeafInner.ChildState.ParentIndex;
                Assert.That(AssignmentIndex0 != null);

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 16, $"New count: {AllChildren0.Count}");

                Controller.Assign(AssignmentIndex0, out bool IsChanged);
                Assert.That(IsChanged);
                Assert.That(UnassignedOptionalLeafInner.IsAssigned);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Assign(AssignmentIndex0, out IsChanged);
                Assert.That(!IsChanged);
                Assert.That(UnassignedOptionalLeafInner.IsAssigned);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Controller.Unassign(AssignmentIndex0, out IsChanged);
                Assert.That(IsChanged);
                Assert.That(!UnassignedOptionalLeafInner.IsAssigned);

                IWriteableNodeStateReadOnlyList AllChildren3 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count - 1, $"New count: {AllChildren3.Count}");
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableUnassign()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableOptionalInner AssignedOptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IWriteableOptionalInner;
                Assert.That(AssignedOptionalLeafInner != null);
                Assert.That(AssignedOptionalLeafInner.IsAssigned);

                IWriteableBrowsingOptionalNodeIndex AssignmentIndex0 = AssignedOptionalLeafInner.ChildState.ParentIndex;
                Assert.That(AssignmentIndex0 != null);

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 16, $"New count: {AllChildren0.Count}");

                Controller.Unassign(AssignmentIndex0, out bool IsChanged);
                Assert.That(IsChanged);
                Assert.That(!AssignedOptionalLeafInner.IsAssigned);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count - 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Unassign(AssignmentIndex0, out IsChanged);
                Assert.That(!IsChanged);
                Assert.That(!AssignedOptionalLeafInner.IsAssigned);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Controller.Assign(AssignmentIndex0, out IsChanged);
                Assert.That(IsChanged);
                Assert.That(AssignedOptionalLeafInner.IsAssigned);

                IWriteableNodeStateReadOnlyList AllChildren3 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count + 1, $"New count: {AllChildren3.Count}");
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableChangeReplication()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 16, $"New count: {AllChildren0.Count}");

                IWriteableBlockState BlockState = LeafBlocksInner.BlockStateList[0];
                Assert.That(BlockState != null);
                BaseNode.IBlock ChildBlock = BlockState.ChildBlock;
                Assert.That(ChildBlock.Replication == BaseNode.ReplicationStatus.Normal);

                Controller.ChangeReplication(LeafBlocksInner, 0, BaseNode.ReplicationStatus.Replicated);

                Assert.That(ChildBlock.Replication == BaseNode.ReplicationStatus.Replicated);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
            }
        }
        #endregion
    }
}
