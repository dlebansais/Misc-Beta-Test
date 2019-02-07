﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using EaslyController;
using EaslyController.Focus;
using EaslyController.Frame;
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

        private static Guid ValueGuid0 = new Guid("{FFFFFFFF-C70B-4BAF-AE1B-C342CD9BFA00}");
        private static Guid ValueGuid1 = new Guid("{FFFFFFFF-C70B-4BAF-AE1B-C342CD9BFA01}");

        private static Leaf CreateLeaf(Guid guid0)
        {
            Leaf NewLeaf = new Leaf();

            BaseNode.IDocument NewLeafDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", guid0);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(NewLeaf, NewLeafDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(NewLeaf, nameof(ILeaf.Text), "leaf");

            return NewLeaf;
        }

        private static Tree CreateTree()
        {
            Leaf Placeholder = CreateLeaf(Guid.NewGuid());

            Tree TreeInstance = new Tree();

            BaseNode.IDocument TreeDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("tree doc", Guid.NewGuid());
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(TreeInstance, TreeDocument);
            BaseNodeHelper.NodeTreeHelperChild.SetChildNode(TreeInstance, nameof(ITree.Placeholder), Placeholder);
            BaseNodeHelper.NodeTreeHelper.SetBooleanProperty(TreeInstance, nameof(IMain.ValueBoolean), true);
            BaseNodeHelper.NodeTreeHelper.SetEnumProperty(TreeInstance, nameof(IMain.ValueEnum), (int)BaseNode.CopySemantic.Value);
            BaseNodeHelper.NodeTreeHelper.SetGuidProperty(TreeInstance, nameof(IMain.ValueGuid), Guid.NewGuid());

            return TreeInstance;
        }

        private static IMain CreateRoot(Guid valueGuid, Imperfections imperfection)
        {
            Guid MainGuid = Guid.NewGuid();
            Guid LeafGuid0 = Guid.NewGuid();

            Tree PlaceholderTree = CreateTree();

            Leaf PlaceholderLeaf = CreateLeaf(imperfection == Imperfections.BadGuid ? MainGuid : LeafGuid0);

            Leaf UnassignedOptionalLeaf = new Leaf();

            BaseNode.IDocument UnassignedOptionalLeafDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", Guid.NewGuid());
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(UnassignedOptionalLeaf, UnassignedOptionalLeafDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(UnassignedOptionalLeaf, nameof(ILeaf.Text), "optional unassigned");

            Easly.IOptionalReference<ILeaf> UnassignedOptional = BaseNodeHelper.OptionalReferenceHelper<ILeaf>.CreateReference(UnassignedOptionalLeaf);

            Leaf AssignedOptionalLeaf = CreateLeaf(Guid.NewGuid());

            Easly.IOptionalReference<ILeaf> AssignedOptionalForLeaf = BaseNodeHelper.OptionalReferenceHelper<ILeaf>.CreateReference(AssignedOptionalLeaf);
            AssignedOptionalForLeaf.Assign();

            Tree AssignedOptionalTree = CreateTree();
            Easly.IOptionalReference<ITree> AssignedOptionalForTree = BaseNodeHelper.OptionalReferenceHelper<ITree>.CreateReference(AssignedOptionalTree);
            AssignedOptionalForTree.Assign();

            Leaf FirstChild = CreateLeaf(Guid.NewGuid());
            Leaf SecondChild = CreateLeaf(Guid.NewGuid());
            Leaf ThirdChild = CreateLeaf(Guid.NewGuid());
            Leaf FourthChild = CreateLeaf(Guid.NewGuid());

            BaseNode.IBlock<ILeaf, Leaf> SecondBlock = BaseNodeHelper.BlockListHelper<ILeaf, Leaf>.CreateBlock(new List<ILeaf>() { SecondChild, ThirdChild });
            BaseNode.IBlock<ILeaf, Leaf> ThirdBlock = BaseNodeHelper.BlockListHelper<ILeaf, Leaf>.CreateBlock(new List<ILeaf>() { FourthChild });

            BaseNode.IBlockList<ILeaf, Leaf> LeafBlocks = BaseNodeHelper.BlockListHelper<ILeaf, Leaf>.CreateSimpleBlockList(FirstChild);
            LeafBlocks.NodeBlockList.Add(SecondBlock);
            LeafBlocks.NodeBlockList.Add(ThirdBlock);

            Leaf FirstPath = CreateLeaf(Guid.NewGuid());
            Leaf SecondPath = CreateLeaf(Guid.NewGuid());

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
            BaseNodeHelper.NodeTreeHelper.SetGuidProperty(Root, nameof(IMain.ValueGuid), valueGuid);

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

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
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

            RootNode = CreateRoot(ValueGuid0, Imperfections.BadGuid);
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

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
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
            Assert.That(MainUnassignedOptionalInner.ChildState != null);
            Assert.That(MainUnassignedOptionalInner.ChildState.ParentInner == MainUnassignedOptionalInner);

            IReadOnlyOptionalInner MainAssignedOptionalTreeInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalTree)) as IReadOnlyOptionalInner;
            Assert.That(MainAssignedOptionalTreeInner != null);
            Assert.That(MainAssignedOptionalTreeInner.InterfaceType == typeof(ITree));
            Assert.That(MainAssignedOptionalTreeInner.IsAssigned);

            IReadOnlyNodeState AssignedOptionalTreeState = MainAssignedOptionalTreeInner.ChildState;
            Assert.That(AssignedOptionalTreeState != null);
            Assert.That(AssignedOptionalTreeState.ParentInner == MainAssignedOptionalTreeInner);
            Assert.That(AssignedOptionalTreeState.ParentState == RootState);

            IReadOnlyNodeStateReadOnlyList AssignedOptionalTreeAllChildren = AssignedOptionalTreeState.GetAllChildren();
            Assert.That(AssignedOptionalTreeAllChildren.Count == 2, $"New count: {AssignedOptionalTreeAllChildren.Count}");

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
            Assert.That(MainLeafBlocksInner.Count == 4);
            Assert.That(MainLeafBlocksInner.BlockStateList != null);
            Assert.That(MainLeafBlocksInner.BlockStateList.Count == 3);
            Assert.That(MainLeafBlocksInner.AllIndexes().Count == MainLeafBlocksInner.Count);

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
            Assert.That(MainLeafPathInner.AllIndexes().Count == MainLeafPathInner.Count);

            IReadOnlyNodeStateReadOnlyList AllChildren = RootState.GetAllChildren();
            Assert.That(AllChildren.Count == 19, $"New count: {AllChildren.Count}");

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
            Assert.That(ReadAsGuid == ValueGuid0);
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

            IMain RootNode = CreateRoot(ValueGuid0, Imperfections.None);

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

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
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

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
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

            RootNode = CreateRoot(ValueGuid0, Imperfections.BadGuid);
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

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
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

            IWriteableNodeState AssignedOptionalTreeState = MainAssignedOptionalTreeInner.ChildState;
            Assert.That(AssignedOptionalTreeState != null);
            Assert.That(AssignedOptionalTreeState.ParentInner == MainAssignedOptionalTreeInner);
            Assert.That(AssignedOptionalTreeState.ParentState == RootState);

            IWriteableNodeStateReadOnlyList AssignedOptionalTreeAllChildren = AssignedOptionalTreeState.GetAllChildren() as IWriteableNodeStateReadOnlyList;
            Assert.That(AssignedOptionalTreeAllChildren != null);
            Assert.That(AssignedOptionalTreeAllChildren.Count == 2, $"New count: {AssignedOptionalTreeAllChildren.Count}");

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
            Assert.That(MainLeafBlocksInner.Count == 4);
            Assert.That(MainLeafBlocksInner.BlockStateList != null);
            Assert.That(MainLeafBlocksInner.BlockStateList.Count == 3);
            Assert.That(MainLeafBlocksInner.AllIndexes().Count == MainLeafBlocksInner.Count);

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
            Assert.That(PatternState.ParentBlockState == LeafBlock);
            Assert.That(PatternState.ParentInner == PatternInner);
            Assert.That(PatternState.ParentIndex == LeafBlock.PatternIndex);
            Assert.That(PatternState.ParentState == RootState);
            Assert.That(PatternState.InnerTable.Count == 0);
            Assert.That(PatternState is IWriteableNodeState AsPlaceholderPatternNodeState && AsPlaceholderPatternNodeState.ParentIndex == LeafBlock.PatternIndex);
            Assert.That(PatternState.GetAllChildren().Count == 1);

            IWriteableSourceState SourceState = LeafBlock.SourceState;
            Assert.That(SourceState != null);
            Assert.That(SourceState.ParentBlockState == LeafBlock);
            Assert.That(SourceState.ParentInner == SourceInner);
            Assert.That(SourceState.ParentIndex == LeafBlock.SourceIndex);
            Assert.That(SourceState.ParentState == RootState);
            Assert.That(SourceState.InnerTable.Count == 0);
            Assert.That(SourceState is IWriteableNodeState AsPlaceholderSourceNodeState && AsPlaceholderSourceNodeState.ParentIndex == LeafBlock.SourceIndex);
            Assert.That(SourceState.GetAllChildren().Count == 1);

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
            Assert.That(MainLeafPathInner.AllIndexes().Count == MainLeafPathInner.Count);

            IWriteableNodeStateReadOnlyList AllChildren = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
            Assert.That(AllChildren.Count == 19, $"New count: {AllChildren.Count}");

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
            Assert.That(ReadAsGuid == ValueGuid0);
            Assert.That(Controller0.GetGuidValue(RootIndex0, nameof(IMain.ValueGuid)) == ReadAsGuid);

            IWriteableController Controller1 = WriteableController.Create(RootIndex0);
            Assert.That(Controller0.IsEqual(CompareEqual.New(), Controller0));

            //System.Diagnostics.Debug.Assert(false);
            Assert.That(CompareEqual.CoverIsEqual(Controller0, Controller1));

            Assert.That(!Controller0.CanUndo);
            Assert.That(!Controller0.CanRedo);
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableClone()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode = CreateRoot(ValueGuid0, Imperfections.None);

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

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
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
                    Assert.That(BlockStateView.BlockState == BlockState);

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
                            Assert.That(AsPatternStateView is IWriteableNodeStateView AsPlaceholderPatternNodeStateView && AsPlaceholderPatternNodeStateView.State == State);
                            break;

                        case IWriteableSourceStateView AsSourceStateView:
                            Assert.That(AsSourceStateView.State == State);
                            Assert.That(AsSourceStateView is IWriteableNodeStateView AsPlaceholderSourceNodeStateView && AsPlaceholderSourceNodeStateView.State == State);
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

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IWriteableListInner;
                Assert.That(LeafPathInner != null);

                int PathCount = LeafPathInner.Count;
                Assert.That(PathCount == 2);

                IWriteableBrowsingListNodeIndex ExistingIndex = LeafPathInner.IndexAt(0) as IWriteableBrowsingListNodeIndex;

                Leaf NewItem0 = CreateLeaf(Guid.NewGuid());

                IWriteableInsertionListNodeIndex InsertionIndex0;
                InsertionIndex0 = ExistingIndex.ToInsertionIndex(RootNode, NewItem0) as IWriteableInsertionListNodeIndex;
                Assert.That(InsertionIndex0.ParentNode == RootNode);
                Assert.That(InsertionIndex0.Node == NewItem0);
                Assert.That(CompareEqual.CoverIsEqual(InsertionIndex0, InsertionIndex0));

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Controller.Insert(LeafPathInner, InsertionIndex0, out IWriteableBrowsingCollectionNodeIndex NewItemIndex0);
                Assert.That(Controller.Contains(NewItemIndex0));

                IWriteableBrowsingListNodeIndex DuplicateExistingIndex0 = InsertionIndex0.ToBrowsingIndex() as IWriteableBrowsingListNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex0 as IWriteableBrowsingListNodeIndex, DuplicateExistingIndex0));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex0, NewItemIndex0 as IWriteableBrowsingListNodeIndex));

                Assert.That(LeafPathInner.Count == PathCount + 1);
                Assert.That(LeafPathInner.StateList.Count == PathCount + 1);

                IWriteablePlaceholderNodeState NewItemState0 = LeafPathInner.StateList[0];
                Assert.That(NewItemState0.Node == NewItem0);
                Assert.That(NewItemState0.ParentIndex == NewItemIndex0);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[0].StateList.Count;
                Assert.That(BlockNodeCount == 4);

                IWriteableBrowsingExistingBlockNodeIndex ExistingIndex1 = LeafBlocksInner.IndexAt(0, 0) as IWriteableBrowsingExistingBlockNodeIndex;

                Leaf NewItem1 = CreateLeaf(Guid.NewGuid());
                IWriteableInsertionExistingBlockNodeIndex InsertionIndex1;
                InsertionIndex1 = ExistingIndex1.ToInsertionIndex(RootNode, NewItem1) as IWriteableInsertionExistingBlockNodeIndex;
                Assert.That(InsertionIndex1.ParentNode == RootNode);
                Assert.That(InsertionIndex1.Node == NewItem1);
                Assert.That(CompareEqual.CoverIsEqual(InsertionIndex1, InsertionIndex1));

                Controller.Insert(LeafBlocksInner, InsertionIndex1, out IWriteableBrowsingCollectionNodeIndex NewItemIndex1);
                Assert.That(Controller.Contains(NewItemIndex1));

                IWriteableBrowsingExistingBlockNodeIndex DuplicateExistingIndex1 = InsertionIndex1.ToBrowsingIndex() as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex1 as IWriteableBrowsingExistingBlockNodeIndex, DuplicateExistingIndex1));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex1, NewItemIndex1 as IWriteableBrowsingExistingBlockNodeIndex));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount + 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == NodeCount + 1);

                IWriteablePlaceholderNodeState NewItemState1 = LeafBlocksInner.BlockStateList[0].StateList[0];
                Assert.That(NewItemState1.Node == NewItem1);
                Assert.That(NewItemState1.ParentIndex == NewItemIndex1);
                Assert.That(NewItemState1.ParentState == RootState);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count + 1, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));




                Leaf NewItem2 = CreateLeaf(Guid.NewGuid());
                BaseNode.IPattern NewPattern = BaseNodeHelper.NodeHelper.CreateSimplePattern("");
                BaseNode.IIdentifier NewSource = BaseNodeHelper.NodeHelper.CreateSimpleIdentifier("");

                IWriteableInsertionNewBlockNodeIndex InsertionIndex2 = new WriteableInsertionNewBlockNodeIndex(RootNode, nameof(IMain.LeafBlocks), NewItem2, 0, NewPattern, NewSource);
                Assert.That(CompareEqual.CoverIsEqual(InsertionIndex2, InsertionIndex2));

                int BlockCount = LeafBlocksInner.BlockStateList.Count;
                Assert.That(BlockCount == 3);

                Controller.Insert(LeafBlocksInner, InsertionIndex2, out IWriteableBrowsingCollectionNodeIndex NewItemIndex2);
                Assert.That(Controller.Contains(NewItemIndex2));

                IWriteableBrowsingExistingBlockNodeIndex DuplicateExistingIndex2 = InsertionIndex2.ToBrowsingIndex() as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex2 as IWriteableBrowsingExistingBlockNodeIndex, DuplicateExistingIndex2));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex2, NewItemIndex2 as IWriteableBrowsingExistingBlockNodeIndex));

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

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableRemove()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
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
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

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
                Assert.That(BlockNodeCount == 4, $"New count: {BlockNodeCount}");

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
                Assert.That(BlockCount == 3);

                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex2));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex2);
                Assert.That(!Controller.Contains(RemovedLeafIndex2));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount - 2);
                Assert.That(LeafBlocksInner.BlockStateList.Count == BlockCount - 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == 1, $"Count: {LeafBlocksInner.BlockStateList[0].StateList.Count}");

                IWriteableNodeStateReadOnlyList AllChildren3 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count - 3, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));


                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();


                NeverEmptyCollectionTable.Remove(typeof(IMain));
                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedLeafIndex0));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableMove()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
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
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Assert.That(Controller.IsMoveable(LeafPathInner, MovedLeafIndex0, +1));

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
                Assert.That(BlockNodeCount == 4, $"New count: {BlockNodeCount}");

                Assert.That(Controller.IsMoveable(LeafBlocksInner, MovedLeafIndex1, -1));
                Controller.Move(LeafBlocksInner, MovedLeafIndex1, -1);
                Assert.That(Controller.Contains(MovedLeafIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount);
                Assert.That(LeafBlocksInner.BlockStateList[1].StateList.Count == NodeCount);

                IWriteableBrowsingExistingBlockNodeIndex NewLeafIndex1 = LeafBlocksInner.IndexAt(1, 0) as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(NewLeafIndex1 == MovedLeafIndex1);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableMoveBlock()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == 19, $"New count: {AllChildren1.Count}");

                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IWriteableBrowsingExistingBlockNodeIndex MovedLeafIndex1 = LeafBlocksInner.IndexAt(1, 0) as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(MovedLeafIndex1));

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[1].StateList.Count;
                Assert.That(BlockNodeCount == 4, $"New count: {BlockNodeCount}");

                Assert.That(Controller.IsBlockMoveable(LeafBlocksInner, 1, -1));
                Controller.MoveBlock(LeafBlocksInner, 1, -1);
                Assert.That(Controller.Contains(MovedLeafIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == NodeCount);

                IWriteableBrowsingExistingBlockNodeIndex NewLeafIndex1 = LeafBlocksInner.IndexAt(0, 0) as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(NewLeafIndex1 == MovedLeafIndex1);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableChangeDiscreteValue()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
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

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableReplace()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
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
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

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

                IWriteableBlockState BlockState = LeafBlocksInner.BlockStateList[0];

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = BlockState.StateList.Count;
                Assert.That(BlockNodeCount == 4);

                Controller.Replace(LeafBlocksInner, ReplacementIndex1, out IWriteableBrowsingChildIndex NewItemIndex1);
                Assert.That(Controller.Contains(NewItemIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount);
                Assert.That(BlockState.StateList.Count == NodeCount);

                IWriteablePlaceholderNodeState NewItemState1 = BlockState.StateList[0];
                Assert.That(NewItemState1.Node == NewItem1);
                Assert.That(NewItemState1.ParentIndex == NewItemIndex1);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IWriteablePlaceholderInner PlaceholderTreeInner = RootState.PropertyToInner(nameof(IMain.PlaceholderTree)) as IWriteablePlaceholderInner;
                Assert.That(PlaceholderTreeInner != null);

                IWriteableBrowsingPlaceholderNodeIndex ExistingIndex2 = PlaceholderTreeInner.ChildState.ParentIndex as IWriteableBrowsingPlaceholderNodeIndex;

                Tree NewItem2 = CreateTree();
                IWriteableInsertionPlaceholderNodeIndex ReplacementIndex2;
                ReplacementIndex2 = ExistingIndex2.ToInsertionIndex(RootNode, NewItem2) as IWriteableInsertionPlaceholderNodeIndex;

                Controller.Replace(PlaceholderTreeInner, ReplacementIndex2, out IWriteableBrowsingChildIndex NewItemIndex2);
                Assert.That(Controller.Contains(NewItemIndex2));

                IWriteablePlaceholderNodeState NewItemState2 = PlaceholderTreeInner.ChildState as IWriteablePlaceholderNodeState;
                Assert.That(NewItemState2.Node == NewItem2);
                Assert.That(NewItemState2.ParentIndex == NewItemIndex2);

                IWriteableBrowsingPlaceholderNodeIndex DuplicateExistingIndex2 = ReplacementIndex2.ToBrowsingIndex() as IWriteableBrowsingPlaceholderNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex2 as IWriteableBrowsingPlaceholderNodeIndex, DuplicateExistingIndex2));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex2, NewItemIndex2 as IWriteableBrowsingPlaceholderNodeIndex));

                IWriteableNodeStateReadOnlyList AllChildren3 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IWriteablePlaceholderInner PlaceholderLeafInner = NewItemState2.PropertyToInner(nameof(ITree.Placeholder)) as IWriteablePlaceholderInner;
                Assert.That(PlaceholderLeafInner != null);

                IWriteableBrowsingPlaceholderNodeIndex ExistingIndex3 = PlaceholderLeafInner.ChildState.ParentIndex as IWriteableBrowsingPlaceholderNodeIndex;

                Leaf NewItem3 = CreateLeaf(Guid.NewGuid());
                IWriteableInsertionPlaceholderNodeIndex ReplacementIndex3;
                ReplacementIndex3 = ExistingIndex3.ToInsertionIndex(NewItem2, NewItem3) as IWriteableInsertionPlaceholderNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(ReplacementIndex3, ReplacementIndex3));

                Controller.Replace(PlaceholderLeafInner, ReplacementIndex3, out IWriteableBrowsingChildIndex NewItemIndex3);
                Assert.That(Controller.Contains(NewItemIndex3));

                IWriteablePlaceholderNodeState NewItemState3 = PlaceholderLeafInner.ChildState as IWriteablePlaceholderNodeState;
                Assert.That(NewItemState3.Node == NewItem3);
                Assert.That(NewItemState3.ParentIndex == NewItemIndex3);

                IWriteableNodeStateReadOnlyList AllChildren4 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren4.Count == AllChildren3.Count, $"New count: {AllChildren4.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));




                IWriteableOptionalInner OptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IWriteableOptionalInner;
                Assert.That(OptionalLeafInner != null);

                IWriteableBrowsingOptionalNodeIndex ExistingIndex4 = OptionalLeafInner.ChildState.ParentIndex as IWriteableBrowsingOptionalNodeIndex;

                Leaf NewItem4 = CreateLeaf(Guid.NewGuid());
                IWriteableInsertionOptionalNodeIndex ReplacementIndex4;
                ReplacementIndex4 = ExistingIndex4.ToInsertionIndex(RootNode, NewItem4) as IWriteableInsertionOptionalNodeIndex;
                Assert.That(ReplacementIndex4.ParentNode == RootNode);
                Assert.That(ReplacementIndex4.PropertyName == OptionalLeafInner.PropertyName);
                Assert.That(CompareEqual.CoverIsEqual(ReplacementIndex4, ReplacementIndex4));

                Controller.Replace(OptionalLeafInner, ReplacementIndex4, out IWriteableBrowsingChildIndex NewItemIndex4);
                Assert.That(Controller.Contains(NewItemIndex4));

                Assert.That(OptionalLeafInner.IsAssigned);
                IWriteableOptionalNodeState NewItemState4 = OptionalLeafInner.ChildState as IWriteableOptionalNodeState;
                Assert.That(NewItemState4.Node == NewItem4);
                Assert.That(NewItemState4.ParentIndex == NewItemIndex4);

                IWriteableBrowsingOptionalNodeIndex DuplicateExistingIndex4 = ReplacementIndex4.ToBrowsingIndex() as IWriteableBrowsingOptionalNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex4 as IWriteableBrowsingOptionalNodeIndex, DuplicateExistingIndex4));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex4, NewItemIndex4 as IWriteableBrowsingOptionalNodeIndex));

                IWriteableNodeStateReadOnlyList AllChildren5 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren5.Count == AllChildren4.Count, $"New count: {AllChildren5.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IWriteableBrowsingOptionalNodeIndex ExistingIndex5 = OptionalLeafInner.ChildState.ParentIndex as IWriteableBrowsingOptionalNodeIndex;

                Leaf NewItem5 = CreateLeaf(Guid.NewGuid());
                IWriteableInsertionOptionalClearIndex ReplacementIndex5;
                ReplacementIndex5 = ExistingIndex5.ToInsertionIndex(RootNode, null) as IWriteableInsertionOptionalClearIndex;
                Assert.That(ReplacementIndex5.ParentNode == RootNode);
                Assert.That(ReplacementIndex5.PropertyName == OptionalLeafInner.PropertyName);
                Assert.That(CompareEqual.CoverIsEqual(ReplacementIndex5, ReplacementIndex5));

                Controller.Replace(OptionalLeafInner, ReplacementIndex5, out IWriteableBrowsingChildIndex NewItemIndex5);
                Assert.That(Controller.Contains(NewItemIndex5));

                Assert.That(!OptionalLeafInner.IsAssigned);
                IWriteableOptionalNodeState NewItemState5 = OptionalLeafInner.ChildState as IWriteableOptionalNodeState;
                Assert.That(NewItemState5.ParentIndex == NewItemIndex5);

                IWriteableBrowsingOptionalNodeIndex DuplicateExistingIndex5 = ReplacementIndex5.ToBrowsingIndex() as IWriteableBrowsingOptionalNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex5 as IWriteableBrowsingOptionalNodeIndex, DuplicateExistingIndex5));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex5, NewItemIndex5 as IWriteableBrowsingOptionalNodeIndex));

                IWriteableNodeStateReadOnlyList AllChildren6 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren6.Count == AllChildren5.Count - 1, $"New count: {AllChildren6.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableAssign()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
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
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

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

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableUnassign()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
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
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

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

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableChangeReplication()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                IWriteableBlockState BlockState = LeafBlocksInner.BlockStateList[0];
                Assert.That(BlockState != null);
                Assert.That(BlockState.ParentInner == LeafBlocksInner);
                BaseNode.IBlock ChildBlock = BlockState.ChildBlock;
                Assert.That(ChildBlock.Replication == BaseNode.ReplicationStatus.Normal);

                Controller.ChangeReplication(LeafBlocksInner, 0, BaseNode.ReplicationStatus.Replicated);

                Assert.That(ChildBlock.Replication == BaseNode.ReplicationStatus.Replicated);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableSplit()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                IWriteableBlockState BlockState0 = LeafBlocksInner.BlockStateList[0];
                Assert.That(BlockState0 != null);
                BaseNode.IBlock ChildBlock0 = BlockState0.ChildBlock;
                Assert.That(ChildBlock0.NodeList.Count == 1);

                IWriteableBlockState BlockState1 = LeafBlocksInner.BlockStateList[1];
                Assert.That(BlockState1 != null);
                BaseNode.IBlock ChildBlock1 = BlockState1.ChildBlock;
                Assert.That(ChildBlock1.NodeList.Count == 2);

                Assert.That(LeafBlocksInner.Count == 4);
                Assert.That(LeafBlocksInner.BlockStateList.Count == 3);

                IWriteableBrowsingExistingBlockNodeIndex SplitIndex0 = LeafBlocksInner.IndexAt(1, 1) as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.IsSplittable(LeafBlocksInner, SplitIndex0));

                Controller.SplitBlock(LeafBlocksInner, SplitIndex0);

                Assert.That(LeafBlocksInner.BlockStateList.Count == 4);
                Assert.That(ChildBlock0 == LeafBlocksInner.BlockStateList[0].ChildBlock);
                Assert.That(ChildBlock1 == LeafBlocksInner.BlockStateList[2].ChildBlock);
                Assert.That(ChildBlock1.NodeList.Count == 1);

                IWriteableBlockState BlockState12 = LeafBlocksInner.BlockStateList[1];
                Assert.That(BlockState12.ChildBlock.NodeList.Count == 1);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 2, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableMerge()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                IWriteableBlockState BlockState0 = LeafBlocksInner.BlockStateList[0];
                Assert.That(BlockState0 != null);
                BaseNode.IBlock ChildBlock0 = BlockState0.ChildBlock;
                Assert.That(ChildBlock0.NodeList.Count == 1);

                IWriteableBlockState BlockState1 = LeafBlocksInner.BlockStateList[1];
                Assert.That(BlockState1 != null);
                BaseNode.IBlock ChildBlock1 = BlockState1.ChildBlock;
                Assert.That(ChildBlock1.NodeList.Count == 2);

                Assert.That(LeafBlocksInner.Count == 4);

                IWriteableBrowsingExistingBlockNodeIndex MergeIndex0 = LeafBlocksInner.IndexAt(1, 0) as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.IsMergeable(LeafBlocksInner, MergeIndex0));

                Assert.That(LeafBlocksInner.BlockStateList.Count == 3);

                Controller.MergeBlocks(LeafBlocksInner, MergeIndex0);

                Assert.That(LeafBlocksInner.BlockStateList.Count == 2);
                Assert.That(ChildBlock1 == LeafBlocksInner.BlockStateList[0].ChildBlock);
                Assert.That(ChildBlock1.NodeList.Count == 3);

                Assert.That(LeafBlocksInner.BlockStateList[0] == BlockState1);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count - 2, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableExpand()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Controller.Expand(RootIndex, out bool IsChanged);
                Assert.That(IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(!IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                IWriteableOptionalInner OptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IWriteableOptionalInner;
                Assert.That(OptionalLeafInner != null);

                IWriteableInsertionOptionalClearIndex ReplacementIndex5 = new WriteableInsertionOptionalClearIndex(RootNode, nameof(IMain.AssignedOptionalLeaf));

                Controller.Replace(OptionalLeafInner, ReplacementIndex5, out IWriteableBrowsingChildIndex NewItemIndex5);
                Assert.That(Controller.Contains(NewItemIndex5));

                IWriteableNodeStateReadOnlyList AllChildren3 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count - 1, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren4 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren4.Count == AllChildren3.Count + 1, $"New count: {AllChildren4.Count}");



                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IWriteableBrowsingExistingBlockNodeIndex RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                IWriteableNodeStateReadOnlyList AllChildren5 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren5.Count == AllChildren4.Count - 10, $"New count: {AllChildren5.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(LeafBlocksInner.IsEmpty);

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(!IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren6 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren6.Count == AllChildren5.Count, $"New count: {AllChildren6.Count}");

                IDictionary<Type, string[]> WithExpandCollectionTable = BaseNodeHelper.NodeHelper.WithExpandCollectionTable as IDictionary<Type, string[]>;
                WithExpandCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren7 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren7.Count == AllChildren6.Count + 3, $"New count: {AllChildren7.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(!LeafBlocksInner.IsEmpty);
                Assert.That(LeafBlocksInner.IsSingle);

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                WithExpandCollectionTable.Remove(typeof(IMain));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableReduce()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;
            bool IsChanged;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IWriteableBrowsingExistingBlockNodeIndex RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(LeafBlocksInner.IsEmpty);

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 9, $"New count: {AllChildren0.Count}");

                IDictionary<Type, string[]> WithExpandCollectionTable = BaseNodeHelper.NodeHelper.WithExpandCollectionTable as IDictionary<Type, string[]>;
                WithExpandCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 4, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                //System.Diagnostics.Debug.Assert(false);
                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count - 7, $"New count: {AllChildren2.Count}");

                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(!IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren3 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count, $"New count: {AllChildren3.Count}");

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren4 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren4.Count == AllChildren3.Count + 7, $"New count: {AllChildren4.Count}");

                BaseNode.IBlock ChildBlock = LeafBlocksInner.BlockStateList[0].ChildBlock;
                ILeaf FirstNode = ChildBlock.NodeList[0] as ILeaf;
                Assert.That(FirstNode != null);
                BaseNodeHelper.NodeTreeHelper.SetString(FirstNode, nameof(ILeaf.Text), "!");

                //System.Diagnostics.Debug.Assert(false);
                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren5 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren5.Count == AllChildren4.Count - 4, $"New count: {AllChildren5.Count}");

                BaseNodeHelper.NodeTreeHelper.SetString(FirstNode, nameof(ILeaf.Text), "");

                //System.Diagnostics.Debug.Assert(false);
                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren6 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren6.Count == AllChildren5.Count - 3, $"New count: {AllChildren6.Count}");

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                WithExpandCollectionTable.Remove(typeof(IMain));

                //System.Diagnostics.Debug.Assert(false);
                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren7 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren7.Count == AllChildren6.Count + 3, $"New count: {AllChildren7.Count}");

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                WithExpandCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                WithExpandCollectionTable.Remove(typeof(IMain));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableCanonicalize()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;
            bool IsChanged;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IWriteableBrowsingExistingBlockNodeIndex RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                Assert.That(Controller.CanUndo);
                IWriteableOperationGroup LastOperation = Controller.OperationStack[Controller.RedoIndex - 1];
                Assert.That(LastOperation.MainOperation is IWriteableRemoveOperation);
                Assert.That(LastOperation.OperationList.Count > 0);
                Assert.That(LastOperation.Refresh == null);

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IWriteableBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(LeafBlocksInner.IsEmpty);

                IWriteableListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IWriteableListInner;
                Assert.That(LeafPathInner != null);
                Assert.That(LeafPathInner.Count == 2);

                IWriteableBrowsingListNodeIndex RemovedListLeafIndex = LeafPathInner.StateList[0].ParentIndex as IWriteableBrowsingListNodeIndex;
                Assert.That(Controller.Contains(RemovedListLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedListLeafIndex));

                Controller.Remove(LeafPathInner, RemovedListLeafIndex);
                Assert.That(!Controller.Contains(RemovedListLeafIndex));

                IDictionary<Type, string[]> NeverEmptyCollectionTable = BaseNodeHelper.NodeHelper.NeverEmptyCollectionTable as IDictionary<Type, string[]>;
                NeverEmptyCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.PlaceholderTree) });

                RemovedListLeafIndex = LeafPathInner.StateList[0].ParentIndex as IWriteableBrowsingListNodeIndex;
                Assert.That(Controller.Contains(RemovedListLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedListLeafIndex));

                Controller.Remove(LeafPathInner, RemovedListLeafIndex);
                Assert.That(!Controller.Contains(RemovedListLeafIndex));
                Assert.That(LeafPathInner.Count == 0);

                NeverEmptyCollectionTable.Remove(typeof(IMain));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                IWriteableNodeStateReadOnlyList AllChildren0 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 12, $"New count: {AllChildren0.Count}");

                IDictionary<Type, string[]> WithExpandCollectionTable = BaseNodeHelper.NodeHelper.WithExpandCollectionTable as IDictionary<Type, string[]>;
                WithExpandCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren1 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                //System.Diagnostics.Debug.Assert(false);
                Controller.Canonicalize(out IsChanged);
                Assert.That(IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren2 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count - 4, $"New count: {AllChildren2.Count}");

                Controller.Undo();
                Controller.Redo();

                Controller.Canonicalize(out IsChanged);
                Assert.That(!IsChanged);

                IWriteableNodeStateReadOnlyList AllChildren3 = (IWriteableNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count, $"New count: {AllChildren3.Count}");

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                NeverEmptyCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });
                Assert.That(LeafBlocksInner.BlockStateList.Count == 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == 1, LeafBlocksInner.BlockStateList[0].StateList.Count.ToString());

                Controller.Canonicalize(out IsChanged);
                Assert.That(IsChanged);

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                NeverEmptyCollectionTable.Remove(typeof(IMain));

                WithExpandCollectionTable.Remove(typeof(IMain));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteablePrune()
        {
            ControllerTools.ResetExpectedName();

            IMain MainItem = CreateRoot(ValueGuid0, Imperfections.None);
            IRoot RootNode = new Root();
            BaseNode.IDocument RootDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("root doc", Guid.NewGuid());
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(RootNode, RootDocument);
            BaseNode.IBlockList<IMain, Main> MainBlocks = BaseNodeHelper.BlockListHelper<IMain, Main>.CreateSimpleBlockList(MainItem);

            IMain UnassignedOptionalMain = CreateRoot(ValueGuid1, Imperfections.None);
            Easly.IOptionalReference<IMain> UnassignedOptional = BaseNodeHelper.OptionalReferenceHelper<IMain>.CreateReference(UnassignedOptionalMain);

            IList<ILeaf> LeafPath = new List<ILeaf>();

            BaseNodeHelper.NodeTreeHelperBlockList.SetBlockList(RootNode, nameof(IRoot.MainBlocks), (BaseNode.IBlockList)MainBlocks);
            BaseNodeHelper.NodeTreeHelperOptional.SetOptionalReference(RootNode, nameof(IRoot.UnassignedOptionalMain), (Easly.IOptionalReference)UnassignedOptional);
            BaseNodeHelper.NodeTreeHelper.SetString(RootNode, nameof(IRoot.ValueString), "root string");
            BaseNodeHelper.NodeTreeHelperList.SetChildNodeList(RootNode, nameof(IRoot.LeafPath), (IList)LeafPath);

            //System.Diagnostics.Debug.Assert(false);
            IWriteableRootNodeIndex RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
            IWriteableController Controller = WriteableController.Create(RootIndex);

            using (IWriteableControllerView ControllerView0 = WriteableControllerView.Create(Controller))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableBlockListInner MainInner = RootState.PropertyToInner(nameof(IRoot.MainBlocks)) as IWriteableBlockListInner;
                Assert.That(MainInner != null);

                IWriteableBrowsingExistingBlockNodeIndex MainIndex = MainInner.IndexAt(0, 0) as IWriteableBrowsingExistingBlockNodeIndex;
                Controller.Remove(MainInner, MainIndex);

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                MainIndex = MainInner.IndexAt(0, 0) as IWriteableBrowsingExistingBlockNodeIndex;
                Controller.Remove(MainInner, MainIndex);

                Controller.Undo();
                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void WriteableCollections()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IWriteableRootNodeIndex RootIndex;
            bool IsReadOnly;
            IReadOnlyBlockState FirstBlockState;
            IReadOnlyBrowsingBlockNodeIndex FirstBlockNodeIndex;
            IReadOnlyBrowsingListNodeIndex FirstListNodeIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new WriteableRootNodeIndex(RootNode);

            IWriteableController ControllerBase = WriteableController.Create(RootIndex);
            IWriteableController Controller = WriteableController.Create(RootIndex);

            IReadOnlyIndexNodeStateDictionary ControllerStateTable = DebugObjects.GetReferenceByInterface(typeof(IWriteableIndexNodeStateDictionary)) as IReadOnlyIndexNodeStateDictionary;

            using (IWriteableControllerView ControllerView = WriteableControllerView.Create(Controller))
            {
                // IxxxBlockStateViewDictionary 

                IReadOnlyBlockStateViewDictionary ReadOnlyBlockStateViewTable = ControllerView.BlockStateViewTable;

                foreach (KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView> Entry in ReadOnlyBlockStateViewTable)
                {
                    IReadOnlyBlockStateView StateView = ReadOnlyBlockStateViewTable[Entry.Key];
                    ReadOnlyBlockStateViewTable.TryGetValue(Entry.Key, out IReadOnlyBlockStateView Value);
                    ReadOnlyBlockStateViewTable.Contains(Entry);
                    ReadOnlyBlockStateViewTable.Remove(Entry.Key);
                    ReadOnlyBlockStateViewTable.Add(Entry.Key, Entry.Value);
                    ICollection<IReadOnlyBlockState> Keys = ReadOnlyBlockStateViewTable.Keys;
                    ICollection<IReadOnlyBlockStateView> Values = ReadOnlyBlockStateViewTable.Values;

                    break;
                }

                IDictionary<IReadOnlyBlockState, IReadOnlyBlockStateView> ReadOnlyBlockStateViewTableAsDictionary = ReadOnlyBlockStateViewTable;
                foreach (KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView> Entry in ReadOnlyBlockStateViewTableAsDictionary)
                {
                    IReadOnlyBlockStateView StateView = ReadOnlyBlockStateViewTableAsDictionary[Entry.Key];
                    break;
                }

                ICollection<KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView>> ReadOnlyBlockStateViewTableAsCollection = ReadOnlyBlockStateViewTable;
                IsReadOnly = ReadOnlyBlockStateViewTableAsCollection.IsReadOnly;
                foreach (KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView> Entry in ReadOnlyBlockStateViewTableAsCollection)
                {
                    ReadOnlyBlockStateViewTableAsCollection.Contains(Entry);
                    ReadOnlyBlockStateViewTableAsCollection.Remove(Entry);
                    ReadOnlyBlockStateViewTableAsCollection.Add(Entry);
                    ReadOnlyBlockStateViewTableAsCollection.CopyTo(new KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView>[ReadOnlyBlockStateViewTableAsCollection.Count], 0);
                    break;
                }

                IEnumerable<KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView>> ReadOnlyBlockStateViewTableAsEnumerable = ReadOnlyBlockStateViewTable;
                foreach (KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView> Entry in ReadOnlyBlockStateViewTableAsEnumerable)
                {
                    break;
                }

                // IWriteableBlockStateList

                IWriteableNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IWriteableBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IWriteableBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IWriteableListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IWriteableListInner;
                Assert.That(LeafPathInner != null);

                //System.Diagnostics.Debug.Assert(false);
                IWriteablePlaceholderNodeState FirstNodeState = LeafBlocksInner.FirstNodeState;
                IWriteableBlockStateList DebugBlockStateList = DebugObjects.GetReferenceByInterface(typeof(IWriteableBlockStateList)) as IWriteableBlockStateList;
                if (DebugBlockStateList != null)
                {
                    Assert.That(DebugBlockStateList.Count > 0);
                    IsReadOnly = ((IReadOnlyBlockStateList)DebugBlockStateList).IsReadOnly;
                    FirstBlockState = DebugBlockStateList[0];
                    Assert.That(DebugBlockStateList.Contains(FirstBlockState));
                    Assert.That(DebugBlockStateList.IndexOf(FirstBlockState) == 0);
                    DebugBlockStateList.Remove(FirstBlockState);
                    DebugBlockStateList.Add(FirstBlockState);
                    DebugBlockStateList.Remove(FirstBlockState);
                    DebugBlockStateList.Insert(0, FirstBlockState);
                    DebugBlockStateList.CopyTo((IReadOnlyBlockState[])(new IWriteableBlockState[DebugBlockStateList.Count]), 0);

                    IEnumerable<IReadOnlyBlockState> BlockStateListAsEnumerable = DebugBlockStateList;
                    foreach (IReadOnlyBlockState Item in BlockStateListAsEnumerable)
                    {
                        break;
                    }

                    IList<IReadOnlyBlockState> BlockStateListAsIlist = DebugBlockStateList;
                    Assert.That(BlockStateListAsIlist[0] == FirstBlockState);

                    IReadOnlyList<IReadOnlyBlockState> BlockStateListAsIReadOnlylist = DebugBlockStateList;
                    Assert.That(BlockStateListAsIReadOnlylist[0] == FirstBlockState);
                }

                IWriteableBlockStateReadOnlyList WriteableBlockStateList = LeafBlocksInner.BlockStateList;
                Assert.That(WriteableBlockStateList.Count > 0);
                FirstBlockState = WriteableBlockStateList[0];
                Assert.That(WriteableBlockStateList.Contains(FirstBlockState));
                Assert.That(WriteableBlockStateList.IndexOf(FirstBlockState) == 0);
                Assert.That(WriteableBlockStateList.Contains((IWriteableBlockState)FirstBlockState));
                Assert.That(WriteableBlockStateList.IndexOf((IWriteableBlockState)FirstBlockState) == 0);

                IEnumerable<IWriteableBlockState> WriteableBlockStateListAsIEnumerable = WriteableBlockStateList;
                IEnumerator<IWriteableBlockState> WriteableBlockStateListAsIEnumerableEnumerator = WriteableBlockStateListAsIEnumerable.GetEnumerator();

                // IWriteableBrowsingBlockNodeIndexList

                IWriteableBrowsingBlockNodeIndexList BlockNodeIndexList = LeafBlocksInner.AllIndexes() as IWriteableBrowsingBlockNodeIndexList;
                Assert.That(BlockNodeIndexList.Count > 0);
                IsReadOnly = ((IReadOnlyBrowsingBlockNodeIndexList)BlockNodeIndexList).IsReadOnly;
                FirstBlockNodeIndex = BlockNodeIndexList[0];
                Assert.That(BlockNodeIndexList.Contains(FirstBlockNodeIndex));
                Assert.That(BlockNodeIndexList.IndexOf(FirstBlockNodeIndex) == 0);
                BlockNodeIndexList.Remove(FirstBlockNodeIndex);
                BlockNodeIndexList.Add(FirstBlockNodeIndex);
                BlockNodeIndexList.Remove(FirstBlockNodeIndex);
                BlockNodeIndexList.Insert(0, FirstBlockNodeIndex);
                BlockNodeIndexList.CopyTo((IReadOnlyBrowsingBlockNodeIndex[])(new IWriteableBrowsingBlockNodeIndex[BlockNodeIndexList.Count]), 0);

                IEnumerable<IReadOnlyBrowsingBlockNodeIndex> BlockNodeIndexListAsEnumerable = BlockNodeIndexList;
                foreach (IReadOnlyBrowsingBlockNodeIndex Item in BlockNodeIndexListAsEnumerable)
                {
                    break;
                }

                IList<IReadOnlyBrowsingBlockNodeIndex> BlockNodeIndexListAsIlist = BlockNodeIndexList;
                Assert.That(BlockNodeIndexListAsIlist[0] == FirstBlockNodeIndex);

                IReadOnlyList<IReadOnlyBrowsingBlockNodeIndex> BlockNodeIndexListAsIReadOnlylist = BlockNodeIndexList;
                Assert.That(BlockNodeIndexListAsIReadOnlylist[0] == FirstBlockNodeIndex);

                IReadOnlyBrowsingBlockNodeIndexList BlockNodeIndexListAsReadOnly = BlockNodeIndexList;
                Assert.That(BlockNodeIndexListAsReadOnly[0] == FirstBlockNodeIndex);

                // IWriteableBrowsingListNodeIndexList

                IWriteableBrowsingListNodeIndexList ListNodeIndexList = LeafPathInner.AllIndexes() as IWriteableBrowsingListNodeIndexList;
                Assert.That(ListNodeIndexList.Count > 0);
                IsReadOnly = ((IReadOnlyBrowsingListNodeIndexList)ListNodeIndexList).IsReadOnly;
                FirstListNodeIndex = ListNodeIndexList[0];
                Assert.That(ListNodeIndexList.Contains(FirstListNodeIndex));
                Assert.That(ListNodeIndexList.IndexOf(FirstListNodeIndex) == 0);
                ListNodeIndexList.Remove(FirstListNodeIndex);
                ListNodeIndexList.Add(FirstListNodeIndex);
                ListNodeIndexList.Remove(FirstListNodeIndex);
                ListNodeIndexList.Insert(0, FirstListNodeIndex);
                ListNodeIndexList.CopyTo((IReadOnlyBrowsingListNodeIndex[])(new IWriteableBrowsingListNodeIndex[ListNodeIndexList.Count]), 0);

                IEnumerable<IReadOnlyBrowsingListNodeIndex> ListNodeIndexListAsEnumerable = ListNodeIndexList;
                foreach (IReadOnlyBrowsingListNodeIndex Item in ListNodeIndexListAsEnumerable)
                {
                    break;
                }

                IList<IReadOnlyBrowsingListNodeIndex> ListNodeIndexListAsIlist = ListNodeIndexList;
                Assert.That(ListNodeIndexListAsIlist[0] == FirstListNodeIndex);

                IReadOnlyList<IReadOnlyBrowsingListNodeIndex> ListNodeIndexListAsIReadOnlylist = ListNodeIndexList;
                Assert.That(ListNodeIndexListAsIReadOnlylist[0] == FirstListNodeIndex);

                IReadOnlyBrowsingListNodeIndexList ListNodeIndexListAsReadOnly = ListNodeIndexList;
                Assert.That(ListNodeIndexListAsReadOnly[0] == FirstListNodeIndex);

                // IWriteableIndexNodeStateDictionary
                if (ControllerStateTable != null)
                {
                    foreach (KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState> Entry in ControllerStateTable)
                    {
                        IReadOnlyNodeState StateView = ControllerStateTable[Entry.Key];
                        ControllerStateTable.TryGetValue(Entry.Key, out IReadOnlyNodeState Value);
                        ControllerStateTable.Contains(Entry);
                        ControllerStateTable.Remove(Entry.Key);
                        ControllerStateTable.Add(Entry.Key, Entry.Value);
                        ICollection<IReadOnlyIndex> Keys = ControllerStateTable.Keys;
                        ICollection<IReadOnlyNodeState> Values = ControllerStateTable.Values;

                        break;
                    }

                    IDictionary<IReadOnlyIndex, IReadOnlyNodeState> ControllerStateTableAsDictionary = ControllerStateTable;
                    foreach (KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState> Entry in ControllerStateTableAsDictionary)
                    {
                        IReadOnlyNodeState StateView = ControllerStateTableAsDictionary[Entry.Key];
                        Assert.That(ControllerStateTableAsDictionary.ContainsKey(Entry.Key));
                        break;
                    }

                    ICollection<KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState>> ControllerStateTableAsCollection = ControllerStateTable;
                    IsReadOnly = ControllerStateTableAsCollection.IsReadOnly;
                    foreach (KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState> Entry in ControllerStateTableAsCollection)
                    {
                        ControllerStateTableAsCollection.Contains(Entry);
                        ControllerStateTableAsCollection.Remove(Entry);
                        ControllerStateTableAsCollection.Add(Entry);
                        ControllerStateTableAsCollection.CopyTo(new KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState>[ControllerStateTableAsCollection.Count], 0);
                        break;
                    }

                    IEnumerable<KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState>> ControllerStateTableAsEnumerable = ControllerStateTable;
                    foreach (KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState> Entry in ControllerStateTableAsEnumerable)
                    {
                        break;
                    }
                }

                // IWriteableIndexNodeStateReadOnlyDictionary

                IReadOnlyIndexNodeStateReadOnlyDictionary StateTable = Controller.StateTable;
                IReadOnlyDictionary<IReadOnlyIndex, IReadOnlyNodeState> StateTableAsDictionary = StateTable;
                Assert.That(StateTable.TryGetValue(RootIndex, out IReadOnlyNodeState RootStateValue) == StateTableAsDictionary.TryGetValue(RootIndex, out IReadOnlyNodeState RootStateValueFromDictionary) && RootStateValue == RootStateValueFromDictionary);
                Assert.That(StateTableAsDictionary.Keys != null);
                Assert.That(StateTableAsDictionary.Values != null);

                // IWriteableInnerDictionary

                //System.Diagnostics.Debug.Assert(false);
                IWriteableInnerDictionary<string> InnerTableModify = DebugObjects.GetReferenceByInterface(typeof(IWriteableInnerDictionary<string>)) as IWriteableInnerDictionary<string>;
                Assert.That(InnerTableModify != null);
                Assert.That(InnerTableModify.Count > 0);

                IDictionary<string, IReadOnlyInner> InnerTableModifyAsDictionary = InnerTableModify;
                Assert.That(InnerTableModifyAsDictionary.Keys != null);
                Assert.That(InnerTableModifyAsDictionary.Values != null);

                foreach (KeyValuePair<string, IWriteableInner> Entry in InnerTableModify)
                {
                    Assert.That(InnerTableModifyAsDictionary.ContainsKey(Entry.Key));
                    Assert.That(InnerTableModifyAsDictionary[Entry.Key] == Entry.Value);
                }

                ICollection<KeyValuePair<string, IReadOnlyInner>> InnerTableModifyAsCollection = InnerTableModify;
                Assert.That(!InnerTableModifyAsCollection.IsReadOnly);

                IEnumerable<KeyValuePair<string, IReadOnlyInner>> InnerTableModifyAsEnumerable = InnerTableModify;
                IEnumerator<KeyValuePair<string, IReadOnlyInner>> InnerTableModifyAsEnumerableEnumerator = InnerTableModifyAsEnumerable.GetEnumerator();

                foreach (KeyValuePair<string, IReadOnlyInner> Entry in InnerTableModifyAsEnumerable)
                {
                    Assert.That(InnerTableModifyAsDictionary.ContainsKey(Entry.Key));
                    Assert.That(InnerTableModifyAsDictionary[Entry.Key] == Entry.Value);
                    Assert.That(InnerTableModify.TryGetValue(Entry.Key, out IReadOnlyInner ReadOnlyInnerValue) == InnerTableModify.TryGetValue(Entry.Key, out IWriteableInner WriteableInnerValue));

                    Assert.That(InnerTableModify.Contains(Entry));
                    InnerTableModify.Remove(Entry);
                    InnerTableModify.Add(Entry);
                    InnerTableModify.CopyTo(new KeyValuePair<string, IReadOnlyInner>[InnerTableModify.Count], 0);
                    break;
                }

                // IWriteableInnerReadOnlyDictionary

                IWriteableInnerReadOnlyDictionary<string> InnerTable = RootState.InnerTable;

                IReadOnlyDictionary<string, IReadOnlyInner> InnerTableAsDictionary = InnerTable;
                Assert.That(InnerTableAsDictionary.Keys != null);
                Assert.That(InnerTableAsDictionary.Values != null);

                foreach (KeyValuePair<string, IWriteableInner> Entry in InnerTable)
                {
                    Assert.That(InnerTable.TryGetValue(Entry.Key, out IReadOnlyInner ReadOnlyInnerValue) == InnerTable.TryGetValue(Entry.Key, out IWriteableInner WriteableInnerValue));
                    break;
                }

                // WriteableNodeStateList

                FirstNodeState = LeafPathInner.FirstNodeState;
                Assert.That(FirstNodeState != null);

                IWriteableNodeStateList NodeStateListModify = DebugObjects.GetReferenceByInterface(typeof(IWriteableNodeStateList)) as IWriteableNodeStateList;
                Assert.That(NodeStateListModify != null);
                Assert.That(NodeStateListModify.Count > 0);
                FirstNodeState = NodeStateListModify[0] as IWriteablePlaceholderNodeState;
                Assert.That(NodeStateListModify.Contains((IReadOnlyNodeState)FirstNodeState));
                Assert.That(NodeStateListModify.IndexOf((IReadOnlyNodeState)FirstNodeState) == 0);

                NodeStateListModify.Remove((IReadOnlyNodeState)FirstNodeState);
                NodeStateListModify.Insert(0, (IReadOnlyNodeState)FirstNodeState);
                NodeStateListModify.CopyTo((IReadOnlyNodeState[])(new IWriteableNodeState[NodeStateListModify.Count]), 0);

                IReadOnlyNodeStateList NodeStateListModifyAsReadOnly = NodeStateListModify as IReadOnlyNodeStateList;
                Assert.That(NodeStateListModifyAsReadOnly != null);
                Assert.That(NodeStateListModifyAsReadOnly[0] == NodeStateListModify[0]);

                IList<IReadOnlyNodeState> NodeStateListModifyAsIList = NodeStateListModify as IList<IReadOnlyNodeState>;
                Assert.That(NodeStateListModifyAsIList != null);
                Assert.That(NodeStateListModifyAsIList[0] == NodeStateListModify[0]);

                IReadOnlyList<IReadOnlyNodeState> NodeStateListModifyAsIReadOnlyList = NodeStateListModify as IReadOnlyList<IReadOnlyNodeState>;
                Assert.That(NodeStateListModifyAsIReadOnlyList != null);
                Assert.That(NodeStateListModifyAsIReadOnlyList[0] == NodeStateListModify[0]);

                ICollection<IReadOnlyNodeState> NodeStateListModifyAsCollection = NodeStateListModify as ICollection<IReadOnlyNodeState>;
                Assert.That(NodeStateListModifyAsCollection != null);
                Assert.That(!NodeStateListModifyAsCollection.IsReadOnly);

                IEnumerable<IReadOnlyNodeState> NodeStateListModifyAsEnumerable = NodeStateListModify as IEnumerable<IReadOnlyNodeState>;
                Assert.That(NodeStateListModifyAsEnumerable != null);
                Assert.That(NodeStateListModifyAsEnumerable.GetEnumerator() != null);

                // WriteableNodeStateReadOnlyList

                IWriteableNodeStateReadOnlyList NodeStateList = NodeStateListModify.ToReadOnly() as IWriteableNodeStateReadOnlyList;
                Assert.That(NodeStateList != null);
                Assert.That(NodeStateList.Count > 0);
                FirstNodeState = NodeStateList[0] as IWriteablePlaceholderNodeState;
                Assert.That(NodeStateList.Contains((IReadOnlyNodeState)FirstNodeState));
                Assert.That(NodeStateList.IndexOf((IReadOnlyNodeState)FirstNodeState) == 0);

                IReadOnlyList<IReadOnlyNodeState> NodeStateListAsIReadOnlyList = NodeStateList as IReadOnlyList<IReadOnlyNodeState>;
                Assert.That(NodeStateListAsIReadOnlyList[0] == FirstNodeState);

                IEnumerable<IReadOnlyNodeState> NodeStateListAsEnumerable = NodeStateList as IEnumerable<IReadOnlyNodeState>;
                Assert.That(NodeStateListAsEnumerable != null);
                Assert.That(NodeStateListAsEnumerable.GetEnumerator() != null);

                // WriteablePlaceholderNodeStateList

                //System.Diagnostics.Debug.Assert(false);
                FirstNodeState = LeafPathInner.FirstNodeState;
                Assert.That(FirstNodeState != null);

                IWriteablePlaceholderNodeStateList PlaceholderNodeStateListModify = DebugObjects.GetReferenceByInterface(typeof(IWriteablePlaceholderNodeStateList)) as IWriteablePlaceholderNodeStateList;
                if (PlaceholderNodeStateListModify != null)
                {
                    Assert.That(PlaceholderNodeStateListModify.Count > 0);
                    FirstNodeState = PlaceholderNodeStateListModify[0] as IWriteablePlaceholderNodeState;
                    Assert.That(PlaceholderNodeStateListModify.Contains((IReadOnlyPlaceholderNodeState)FirstNodeState));
                    Assert.That(PlaceholderNodeStateListModify.IndexOf((IReadOnlyPlaceholderNodeState)FirstNodeState) == 0);

                    PlaceholderNodeStateListModify.Remove((IReadOnlyPlaceholderNodeState)FirstNodeState);
                    PlaceholderNodeStateListModify.Add((IReadOnlyPlaceholderNodeState)FirstNodeState);
                    PlaceholderNodeStateListModify.Remove((IReadOnlyPlaceholderNodeState)FirstNodeState);
                    PlaceholderNodeStateListModify.Insert(0, (IReadOnlyPlaceholderNodeState)FirstNodeState);
                    PlaceholderNodeStateListModify.CopyTo((IReadOnlyPlaceholderNodeState[])(new IWriteablePlaceholderNodeState[PlaceholderNodeStateListModify.Count]), 0);

                    IReadOnlyPlaceholderNodeStateList PlaceholderNodeStateListModifyAsReadOnly = PlaceholderNodeStateListModify as IReadOnlyPlaceholderNodeStateList;
                    Assert.That(PlaceholderNodeStateListModifyAsReadOnly != null);
                    Assert.That(PlaceholderNodeStateListModifyAsReadOnly[0] == PlaceholderNodeStateListModify[0]);

                    IList<IReadOnlyPlaceholderNodeState> PlaceholderNodeStateListModifyAsIList = PlaceholderNodeStateListModify as IList<IReadOnlyPlaceholderNodeState>;
                    Assert.That(PlaceholderNodeStateListModifyAsIList != null);
                    Assert.That(PlaceholderNodeStateListModifyAsIList[0] == PlaceholderNodeStateListModify[0]);

                    IReadOnlyList<IReadOnlyPlaceholderNodeState> PlaceholderNodeStateListModifyAsIReadOnlyList = PlaceholderNodeStateListModify as IReadOnlyList<IReadOnlyPlaceholderNodeState>;
                    Assert.That(PlaceholderNodeStateListModifyAsIReadOnlyList != null);
                    Assert.That(PlaceholderNodeStateListModifyAsIReadOnlyList[0] == PlaceholderNodeStateListModify[0]);

                    ICollection<IReadOnlyPlaceholderNodeState> PlaceholderNodeStateListModifyAsCollection = PlaceholderNodeStateListModify as ICollection<IReadOnlyPlaceholderNodeState>;
                    Assert.That(PlaceholderNodeStateListModifyAsCollection != null);
                    Assert.That(!PlaceholderNodeStateListModifyAsCollection.IsReadOnly);

                    IEnumerable<IReadOnlyPlaceholderNodeState> PlaceholderNodeStateListModifyAsEnumerable = PlaceholderNodeStateListModify as IEnumerable<IReadOnlyPlaceholderNodeState>;
                    Assert.That(PlaceholderNodeStateListModifyAsEnumerable != null);
                    Assert.That(PlaceholderNodeStateListModifyAsEnumerable.GetEnumerator() != null);
                }

                // WriteablePlaceholderNodeStateReadOnlyList

                IWriteablePlaceholderNodeStateReadOnlyList PlaceholderNodeStateList = LeafPathInner.StateList;
                Assert.That(PlaceholderNodeStateList != null);
                Assert.That(PlaceholderNodeStateList.Count > 0);
                FirstNodeState = PlaceholderNodeStateList[0] as IWriteablePlaceholderNodeState;
                Assert.That(PlaceholderNodeStateList.Contains((IReadOnlyPlaceholderNodeState)FirstNodeState));
                Assert.That(PlaceholderNodeStateList.IndexOf((IReadOnlyPlaceholderNodeState)FirstNodeState) == 0);

                IReadOnlyList<IReadOnlyPlaceholderNodeState> PlaceholderNodeStateListAsIReadOnlyList = PlaceholderNodeStateList as IReadOnlyList<IReadOnlyPlaceholderNodeState>;
                Assert.That(PlaceholderNodeStateListAsIReadOnlyList[0] == FirstNodeState);

                IEnumerable<IReadOnlyPlaceholderNodeState> PlaceholderNodeStateListAsEnumerable = PlaceholderNodeStateList as IEnumerable<IReadOnlyPlaceholderNodeState>;
                Assert.That(PlaceholderNodeStateListAsEnumerable != null);
                Assert.That(PlaceholderNodeStateListAsEnumerable.GetEnumerator() != null);

                // IWriteableStateViewDictionary
                IWriteableStateViewDictionary StateViewTable = ControllerView.StateViewTable;

                IDictionary<IReadOnlyNodeState, IReadOnlyNodeStateView> StateViewTableAsDictionary = StateViewTable;
                Assert.That(StateViewTableAsDictionary != null);
                Assert.That(StateViewTableAsDictionary.TryGetValue(RootState, out IReadOnlyNodeStateView StateViewTableAsDictionaryValue) == StateViewTable.TryGetValue(RootState, out IReadOnlyNodeStateView StateViewTableValue));
                Assert.That(StateViewTableAsDictionary.Keys != null);
                Assert.That(StateViewTableAsDictionary.Values != null);

                ICollection<KeyValuePair<IReadOnlyNodeState, IReadOnlyNodeStateView>> StateViewTableAsCollection = StateViewTable;
                Assert.That(!StateViewTableAsCollection.IsReadOnly);

                foreach (KeyValuePair<IReadOnlyNodeState, IReadOnlyNodeStateView> Entry in StateViewTableAsCollection)
                {
                    Assert.That(StateViewTableAsCollection.Contains(Entry));
                    StateViewTableAsCollection.Remove(Entry);
                    StateViewTableAsCollection.Add(Entry);
                    StateViewTableAsCollection.CopyTo(new KeyValuePair<IReadOnlyNodeState, IReadOnlyNodeStateView>[StateViewTable.Count], 0);
                    break;
                }
            }
        }
        #endregion

        #region Frame
        [Test]
        [Category("Coverage")]
        public static void FrameCreation()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;
            IFrameController Controller;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

            try
            {
                RootIndex = new FrameRootNodeIndex(RootNode);
                Controller = FrameController.Create(RootIndex);
            }
            catch (Exception e)
            {
                Assert.Fail($"#0: {e}");
            }

            RootNode = CreateRoot(ValueGuid0, Imperfections.BadGuid);
            Assert.That(!BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

            try
            {
                RootIndex = new FrameRootNodeIndex(RootNode);
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
        public static void FrameProperties()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex0;
            IFrameRootNodeIndex RootIndex1;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

            RootIndex0 = new FrameRootNodeIndex(RootNode);
            Assert.That(RootIndex0.Node == RootNode);
            Assert.That(RootIndex0.IsEqual(CompareEqual.New(), RootIndex0));

            RootIndex1 = new FrameRootNodeIndex(RootNode);
            Assert.That(RootIndex1.Node == RootNode);
            Assert.That(CompareEqual.CoverIsEqual(RootIndex0, RootIndex1));

            IFrameController Controller0 = FrameController.Create(RootIndex0);
            Assert.That(Controller0.RootIndex == RootIndex0);

            Stats Stats = Controller0.Stats;
            Assert.That(Stats.NodeCount >= 0);
            Assert.That(Stats.PlaceholderNodeCount >= 0);
            Assert.That(Stats.OptionalNodeCount >= 0);
            Assert.That(Stats.AssignedOptionalNodeCount >= 0);
            Assert.That(Stats.ListCount >= 0);
            Assert.That(Stats.BlockListCount >= 0);
            Assert.That(Stats.BlockCount >= 0);

            IFramePlaceholderNodeState RootState = Controller0.RootState;
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

            IFramePlaceholderInner MainPlaceholderTreeInner = RootState.PropertyToInner(nameof(IMain.PlaceholderTree)) as IFramePlaceholderInner;
            Assert.That(MainPlaceholderTreeInner != null);
            Assert.That(MainPlaceholderTreeInner.InterfaceType == typeof(ITree));
            Assert.That(MainPlaceholderTreeInner.ChildState != null);
            Assert.That(MainPlaceholderTreeInner.ChildState.ParentInner == MainPlaceholderTreeInner);

            IFramePlaceholderInner MainPlaceholderLeafInner = RootState.PropertyToInner(nameof(IMain.PlaceholderLeaf)) as IFramePlaceholderInner;
            Assert.That(MainPlaceholderLeafInner != null);
            Assert.That(MainPlaceholderLeafInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainPlaceholderLeafInner.ChildState != null);
            Assert.That(MainPlaceholderLeafInner.ChildState.ParentInner == MainPlaceholderLeafInner);

            IFrameOptionalInner MainUnassignedOptionalInner = RootState.PropertyToInner(nameof(IMain.UnassignedOptionalLeaf)) as IFrameOptionalInner;
            Assert.That(MainUnassignedOptionalInner != null);
            Assert.That(MainUnassignedOptionalInner.InterfaceType == typeof(ILeaf));
            Assert.That(!MainUnassignedOptionalInner.IsAssigned);
            Assert.That(MainUnassignedOptionalInner.ChildState != null);
            Assert.That(MainUnassignedOptionalInner.ChildState.ParentInner == MainUnassignedOptionalInner);

            IFrameOptionalInner MainAssignedOptionalTreeInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalTree)) as IFrameOptionalInner;
            Assert.That(MainAssignedOptionalTreeInner != null);
            Assert.That(MainAssignedOptionalTreeInner.InterfaceType == typeof(ITree));
            Assert.That(MainAssignedOptionalTreeInner.IsAssigned);

            IFrameNodeState AssignedOptionalTreeState = MainAssignedOptionalTreeInner.ChildState;
            Assert.That(AssignedOptionalTreeState != null);
            Assert.That(AssignedOptionalTreeState.ParentInner == MainAssignedOptionalTreeInner);
            Assert.That(AssignedOptionalTreeState.ParentState == RootState);

            IFrameNodeStateReadOnlyList AssignedOptionalTreeAllChildren = AssignedOptionalTreeState.GetAllChildren() as IFrameNodeStateReadOnlyList;
            Assert.That(AssignedOptionalTreeAllChildren != null);
            Assert.That(AssignedOptionalTreeAllChildren.Count == 2, $"New count: {AssignedOptionalTreeAllChildren.Count}");

            IFrameOptionalInner MainAssignedOptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IFrameOptionalInner;
            Assert.That(MainAssignedOptionalLeafInner != null);
            Assert.That(MainAssignedOptionalLeafInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainAssignedOptionalLeafInner.IsAssigned);
            Assert.That(MainAssignedOptionalLeafInner.ChildState != null);
            Assert.That(MainAssignedOptionalLeafInner.ChildState.ParentInner == MainAssignedOptionalLeafInner);

            IFrameBlockListInner MainLeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFrameBlockListInner;
            Assert.That(MainLeafBlocksInner != null);
            Assert.That(!MainLeafBlocksInner.IsNeverEmpty);
            Assert.That(!MainLeafBlocksInner.IsEmpty);
            Assert.That(!MainLeafBlocksInner.IsSingle);
            Assert.That(MainLeafBlocksInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainLeafBlocksInner.BlockType == typeof(BaseNode.IBlock<ILeaf, Leaf>));
            Assert.That(MainLeafBlocksInner.ItemType == typeof(Leaf));
            Assert.That(MainLeafBlocksInner.Count == 4);
            Assert.That(MainLeafBlocksInner.BlockStateList != null);
            Assert.That(MainLeafBlocksInner.BlockStateList.Count == 3);
            Assert.That(MainLeafBlocksInner.AllIndexes().Count == MainLeafBlocksInner.Count);

            IFrameBlockState LeafBlock = MainLeafBlocksInner.BlockStateList[0];
            Assert.That(LeafBlock != null);
            Assert.That(LeafBlock.StateList != null);
            Assert.That(LeafBlock.StateList.Count == 1);
            Assert.That(MainLeafBlocksInner.FirstNodeState == LeafBlock.StateList[0]);
            Assert.That(MainLeafBlocksInner.IndexAt(0, 0) == MainLeafBlocksInner.FirstNodeState.ParentIndex);

            IFramePlaceholderInner PatternInner = LeafBlock.PropertyToInner(nameof(BaseNode.IBlock.ReplicationPattern)) as IFramePlaceholderInner;
            Assert.That(PatternInner != null);

            IFramePlaceholderInner SourceInner = LeafBlock.PropertyToInner(nameof(BaseNode.IBlock.SourceIdentifier)) as IFramePlaceholderInner;
            Assert.That(SourceInner != null);

            IFramePatternState PatternState = LeafBlock.PatternState;
            Assert.That(PatternState != null);
            Assert.That(PatternState.ParentBlockState == LeafBlock);
            Assert.That(PatternState.ParentInner == PatternInner);
            Assert.That(PatternState.ParentIndex == LeafBlock.PatternIndex);
            Assert.That(PatternState.ParentState == RootState);
            Assert.That(PatternState.InnerTable.Count == 0);
            Assert.That(PatternState is IFrameNodeState AsPlaceholderPatternNodeState && AsPlaceholderPatternNodeState.ParentIndex == LeafBlock.PatternIndex);
            Assert.That(PatternState.GetAllChildren().Count == 1);

            IFrameSourceState SourceState = LeafBlock.SourceState;
            Assert.That(SourceState != null);
            Assert.That(SourceState.ParentBlockState == LeafBlock);
            Assert.That(SourceState.ParentInner == SourceInner);
            Assert.That(SourceState.ParentIndex == LeafBlock.SourceIndex);
            Assert.That(SourceState.ParentState == RootState);
            Assert.That(SourceState.InnerTable.Count == 0);
            Assert.That(SourceState is IFrameNodeState AsPlaceholderSourceNodeState && AsPlaceholderSourceNodeState.ParentIndex == LeafBlock.SourceIndex);
            Assert.That(SourceState.GetAllChildren().Count == 1);

            Assert.That(MainLeafBlocksInner.FirstNodeState == LeafBlock.StateList[0]);

            IFrameListInner MainLeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFrameListInner;
            Assert.That(MainLeafPathInner != null);
            Assert.That(!MainLeafPathInner.IsNeverEmpty);
            Assert.That(MainLeafPathInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainLeafPathInner.Count == 2);
            Assert.That(MainLeafPathInner.StateList != null);
            Assert.That(MainLeafPathInner.StateList.Count == 2);
            Assert.That(MainLeafPathInner.FirstNodeState == MainLeafPathInner.StateList[0]);
            Assert.That(MainLeafPathInner.IndexAt(0) == MainLeafPathInner.FirstNodeState.ParentIndex);
            Assert.That(MainLeafPathInner.AllIndexes().Count == MainLeafPathInner.Count);

            IFrameNodeStateReadOnlyList AllChildren = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
            Assert.That(AllChildren.Count == 19, $"New count: {AllChildren.Count}");

            IFramePlaceholderInner PlaceholderInner = RootState.InnerTable[nameof(IMain.PlaceholderLeaf)] as IFramePlaceholderInner;
            Assert.That(PlaceholderInner != null);

            IFrameBrowsingPlaceholderNodeIndex PlaceholderNodeIndex = PlaceholderInner.ChildState.ParentIndex as IFrameBrowsingPlaceholderNodeIndex;
            Assert.That(PlaceholderNodeIndex != null);
            Assert.That(Controller0.Contains(PlaceholderNodeIndex));

            IFrameOptionalInner UnassignedOptionalInner = RootState.InnerTable[nameof(IMain.UnassignedOptionalLeaf)] as IFrameOptionalInner;
            Assert.That(UnassignedOptionalInner != null);

            IFrameBrowsingOptionalNodeIndex UnassignedOptionalNodeIndex = UnassignedOptionalInner.ChildState.ParentIndex;
            Assert.That(UnassignedOptionalNodeIndex != null);
            Assert.That(Controller0.Contains(UnassignedOptionalNodeIndex));
            Assert.That(Controller0.IsAssigned(UnassignedOptionalNodeIndex) == false);

            IFrameOptionalInner AssignedOptionalInner = RootState.InnerTable[nameof(IMain.AssignedOptionalLeaf)] as IFrameOptionalInner;
            Assert.That(AssignedOptionalInner != null);

            IFrameBrowsingOptionalNodeIndex AssignedOptionalNodeIndex = AssignedOptionalInner.ChildState.ParentIndex;
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
            Assert.That(ReadAsGuid == ValueGuid0);
            Assert.That(Controller0.GetGuidValue(RootIndex0, nameof(IMain.ValueGuid)) == ReadAsGuid);

            IFrameController Controller1 = FrameController.Create(RootIndex0);
            Assert.That(Controller0.IsEqual(CompareEqual.New(), Controller0));

            //System.Diagnostics.Debug.Assert(false);
            Assert.That(CompareEqual.CoverIsEqual(Controller0, Controller1));

            Assert.That(!Controller0.CanUndo);
            Assert.That(!Controller0.CanRedo);
        }

        [Test]
        [Category("Coverage")]
        public static void FrameClone()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode = CreateRoot(ValueGuid0, Imperfections.None);

            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(RootNode);
            Assert.That(RootIndex != null);

            IFrameController Controller = FrameController.Create(RootIndex);
            Assert.That(Controller != null);

            IFramePlaceholderNodeState RootState = Controller.RootState;
            Assert.That(RootState != null);

            BaseNode.INode ClonedNode = RootState.CloneNode();
            Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(ClonedNode));

            IFrameRootNodeIndex CloneRootIndex = new FrameRootNodeIndex(ClonedNode);
            Assert.That(CloneRootIndex != null);

            IFrameController CloneController = FrameController.Create(CloneRootIndex);
            Assert.That(CloneController != null);

            IFramePlaceholderNodeState CloneRootState = Controller.RootState;
            Assert.That(CloneRootState != null);

            IFrameNodeStateReadOnlyList AllChildren = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
            IFrameNodeStateReadOnlyList CloneAllChildren = (IFrameNodeStateReadOnlyList)CloneRootState.GetAllChildren();
            Assert.That(AllChildren.Count == CloneAllChildren.Count);
        }

        [Test]
        [Category("Coverage")]
        public static void FrameViews()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController Controller = FrameController.Create(RootIndex);
            IFrameTemplateSet DefaultTemplateSet = FrameTemplateSet.Default;
            DefaultTemplateSet = FrameTemplateSet.Default;

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                using (IFrameControllerView ControllerView1 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
                {
                    Assert.That(ControllerView0.IsEqual(CompareEqual.New(), ControllerView0));
                    Assert.That(CompareEqual.CoverIsEqual(ControllerView0, ControllerView1));
                }

                foreach (KeyValuePair<IFrameBlockState, IFrameBlockStateView> Entry in ControllerView0.BlockStateViewTable)
                {
                    IFrameBlockState BlockState = Entry.Key;
                    Assert.That(BlockState != null);

                    IFrameBlockStateView BlockStateView = Entry.Value;
                    Assert.That(BlockStateView != null);
                    Assert.That(BlockStateView.BlockState == BlockState);

                    Assert.That(BlockStateView.ControllerView == ControllerView0);
                }

                foreach (KeyValuePair<IFrameNodeState, IFrameNodeStateView> Entry in ControllerView0.StateViewTable)
                {
                    IFrameNodeState State = Entry.Key;
                    Assert.That(State != null);

                    IFrameNodeStateView StateView = Entry.Value;
                    Assert.That(StateView != null);
                    Assert.That(StateView.State == State);

                    IFrameIndex ParentIndex = State.ParentIndex;
                    Assert.That(ParentIndex != null);

                    Assert.That(Controller.Contains(ParentIndex));
                    Assert.That(StateView.ControllerView == ControllerView0);

                    switch (StateView)
                    {
                        case IFramePatternStateView AsPatternStateView:
                            Assert.That(AsPatternStateView.State == State);
                            Assert.That(AsPatternStateView is IFrameNodeStateView AsPlaceholderPatternNodeStateView && AsPlaceholderPatternNodeStateView.State == State);
                            break;

                        case IFrameSourceStateView AsSourceStateView:
                            Assert.That(AsSourceStateView.State == State);
                            Assert.That(AsSourceStateView is IFrameNodeStateView AsPlaceholderSourceNodeStateView && AsPlaceholderSourceNodeStateView.State == State);
                            break;

                        case IFramePlaceholderNodeStateView AsPlaceholderNodeStateView:
                            Assert.That(AsPlaceholderNodeStateView.State == State);
                            break;

                        case IFrameOptionalNodeStateView AsOptionalNodeStateView:
                            Assert.That(AsOptionalNodeStateView.State == State);
                            break;
                    }
                }

                IFrameVisibleCellViewList VisibleCellViewList = new FrameVisibleCellViewList();
                ControllerView0.EnumerateVisibleCellViews(VisibleCellViewList);
                ControllerView0.PrintCellViewTree(true);
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameInsert()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFrameListInner;
                Assert.That(LeafPathInner != null);

                int PathCount = LeafPathInner.Count;
                Assert.That(PathCount == 2);

                IFrameBrowsingListNodeIndex ExistingIndex = LeafPathInner.IndexAt(0) as IFrameBrowsingListNodeIndex;

                Leaf NewItem0 = CreateLeaf(Guid.NewGuid());

                IFrameInsertionListNodeIndex InsertionIndex0;
                InsertionIndex0 = ExistingIndex.ToInsertionIndex(RootNode, NewItem0) as IFrameInsertionListNodeIndex;
                Assert.That(InsertionIndex0.ParentNode == RootNode);
                Assert.That(InsertionIndex0.Node == NewItem0);
                Assert.That(CompareEqual.CoverIsEqual(InsertionIndex0, InsertionIndex0));

                IFrameNodeStateReadOnlyList AllChildren0 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Controller.Insert(LeafPathInner, InsertionIndex0, out IWriteableBrowsingCollectionNodeIndex NewItemIndex0);
                Assert.That(Controller.Contains(NewItemIndex0));

                IFrameBrowsingListNodeIndex DuplicateExistingIndex0 = InsertionIndex0.ToBrowsingIndex() as IFrameBrowsingListNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex0 as IFrameBrowsingListNodeIndex, DuplicateExistingIndex0));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex0, NewItemIndex0 as IFrameBrowsingListNodeIndex));

                Assert.That(LeafPathInner.Count == PathCount + 1);
                Assert.That(LeafPathInner.StateList.Count == PathCount + 1);

                IFramePlaceholderNodeState NewItemState0 = LeafPathInner.StateList[0];
                Assert.That(NewItemState0.Node == NewItem0);
                Assert.That(NewItemState0.ParentIndex == NewItemIndex0);

                IFrameNodeStateReadOnlyList AllChildren1 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IFrameBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFrameBlockListInner;
                Assert.That(LeafBlocksInner != null);

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[0].StateList.Count;
                Assert.That(BlockNodeCount == 4);

                IFrameBrowsingExistingBlockNodeIndex ExistingIndex1 = LeafBlocksInner.IndexAt(0, 0) as IFrameBrowsingExistingBlockNodeIndex;

                Leaf NewItem1 = CreateLeaf(Guid.NewGuid());
                IFrameInsertionExistingBlockNodeIndex InsertionIndex1;
                InsertionIndex1 = ExistingIndex1.ToInsertionIndex(RootNode, NewItem1) as IFrameInsertionExistingBlockNodeIndex;
                Assert.That(InsertionIndex1.ParentNode == RootNode);
                Assert.That(InsertionIndex1.Node == NewItem1);
                Assert.That(CompareEqual.CoverIsEqual(InsertionIndex1, InsertionIndex1));

                Controller.Insert(LeafBlocksInner, InsertionIndex1, out IWriteableBrowsingCollectionNodeIndex NewItemIndex1);
                Assert.That(Controller.Contains(NewItemIndex1));

                IFrameBrowsingExistingBlockNodeIndex DuplicateExistingIndex1 = InsertionIndex1.ToBrowsingIndex() as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex1 as IFrameBrowsingExistingBlockNodeIndex, DuplicateExistingIndex1));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex1, NewItemIndex1 as IFrameBrowsingExistingBlockNodeIndex));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount + 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == NodeCount + 1);

                IFramePlaceholderNodeState NewItemState1 = LeafBlocksInner.BlockStateList[0].StateList[0];
                Assert.That(NewItemState1.Node == NewItem1);
                Assert.That(NewItemState1.ParentIndex == NewItemIndex1);
                Assert.That(NewItemState1.ParentState == RootState);

                IFrameNodeStateReadOnlyList AllChildren2 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count + 1, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));




                Leaf NewItem2 = CreateLeaf(Guid.NewGuid());
                BaseNode.IPattern NewPattern = BaseNodeHelper.NodeHelper.CreateSimplePattern("");
                BaseNode.IIdentifier NewSource = BaseNodeHelper.NodeHelper.CreateSimpleIdentifier("");

                IFrameInsertionNewBlockNodeIndex InsertionIndex2 = new FrameInsertionNewBlockNodeIndex(RootNode, nameof(IMain.LeafBlocks), NewItem2, 0, NewPattern, NewSource);
                Assert.That(CompareEqual.CoverIsEqual(InsertionIndex2, InsertionIndex2));

                int BlockCount = LeafBlocksInner.BlockStateList.Count;
                Assert.That(BlockCount == 3);

                Controller.Insert(LeafBlocksInner, InsertionIndex2, out IWriteableBrowsingCollectionNodeIndex NewItemIndex2);
                Assert.That(Controller.Contains(NewItemIndex2));

                IFrameBrowsingExistingBlockNodeIndex DuplicateExistingIndex2 = InsertionIndex2.ToBrowsingIndex() as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex2 as IFrameBrowsingExistingBlockNodeIndex, DuplicateExistingIndex2));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex2, NewItemIndex2 as IFrameBrowsingExistingBlockNodeIndex));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount + 2);
                Assert.That(LeafBlocksInner.BlockStateList.Count == BlockCount + 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == 1, $"Count: {LeafBlocksInner.BlockStateList[0].StateList.Count}");
                Assert.That(LeafBlocksInner.BlockStateList[1].StateList.Count == 2, $"Count: {LeafBlocksInner.BlockStateList[1].StateList.Count}");
                Assert.That(LeafBlocksInner.BlockStateList[2].StateList.Count == 2, $"Count: {LeafBlocksInner.BlockStateList[2].StateList.Count}");

                IFramePlaceholderNodeState NewItemState2 = LeafBlocksInner.BlockStateList[0].StateList[0];
                Assert.That(NewItemState2.Node == NewItem2);
                Assert.That(NewItemState2.ParentIndex == NewItemIndex2);

                IFrameNodeStateReadOnlyList AllChildren3 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count + 3, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameRemove()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFrameListInner;
                Assert.That(LeafPathInner != null);

                IFrameBrowsingListNodeIndex RemovedLeafIndex0 = LeafPathInner.StateList[0].ParentIndex as IFrameBrowsingListNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex0));

                int PathCount = LeafPathInner.Count;
                Assert.That(PathCount == 2);

                IFrameNodeStateReadOnlyList AllChildren0 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedLeafIndex0));

                Controller.Remove(LeafPathInner, RemovedLeafIndex0);
                Assert.That(!Controller.Contains(RemovedLeafIndex0));

                Assert.That(LeafPathInner.Count == PathCount - 1);
                Assert.That(LeafPathInner.StateList.Count == PathCount - 1);

                IFrameNodeStateReadOnlyList AllChildren1 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count - 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                RemovedLeafIndex0 = LeafPathInner.StateList[0].ParentIndex as IFrameBrowsingListNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex0));

                Assert.That(LeafPathInner.Count == 1);

                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedLeafIndex0));

                IDictionary<Type, string[]> NeverEmptyCollectionTable = BaseNodeHelper.NodeHelper.NeverEmptyCollectionTable as IDictionary<Type, string[]>;
                NeverEmptyCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafPath) });
                Assert.That(!Controller.IsRemoveable(LeafPathInner, RemovedLeafIndex0));



                IFrameBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFrameBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFrameBrowsingExistingBlockNodeIndex RemovedLeafIndex1 = LeafBlocksInner.BlockStateList[1].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex1));

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[1].StateList.Count;
                Assert.That(BlockNodeCount == 4, $"New count: {BlockNodeCount}");

                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex1));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex1);
                Assert.That(!Controller.Contains(RemovedLeafIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount - 1);
                Assert.That(LeafBlocksInner.BlockStateList[1].StateList.Count == NodeCount - 1);

                IFrameNodeStateReadOnlyList AllChildren2 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count - 1, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IFrameBrowsingExistingBlockNodeIndex RemovedLeafIndex2 = LeafBlocksInner.BlockStateList[1].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex2));


                int BlockCount = LeafBlocksInner.BlockStateList.Count;
                Assert.That(BlockCount == 3);

                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex2));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex2);
                Assert.That(!Controller.Contains(RemovedLeafIndex2));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount - 2);
                Assert.That(LeafBlocksInner.BlockStateList.Count == BlockCount - 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == 1, $"Count: {LeafBlocksInner.BlockStateList[0].StateList.Count}");

                IFrameNodeStateReadOnlyList AllChildren3 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count - 3, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));


                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();


                NeverEmptyCollectionTable.Remove(typeof(IMain));
                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedLeafIndex0));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameMove()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFrameListInner;
                Assert.That(LeafPathInner != null);

                IFrameBrowsingListNodeIndex MovedLeafIndex0 = LeafPathInner.IndexAt(0) as IFrameBrowsingListNodeIndex;
                Assert.That(Controller.Contains(MovedLeafIndex0));

                int PathCount = LeafPathInner.Count;
                Assert.That(PathCount == 2);

                IFrameNodeStateReadOnlyList AllChildren0 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Assert.That(Controller.IsMoveable(LeafPathInner, MovedLeafIndex0, +1));

                Controller.Move(LeafPathInner, MovedLeafIndex0, +1);
                Assert.That(Controller.Contains(MovedLeafIndex0));

                Assert.That(LeafPathInner.Count == PathCount);
                Assert.That(LeafPathInner.StateList.Count == PathCount);

                //System.Diagnostics.Debug.Assert(false);
                IFrameBrowsingListNodeIndex NewLeafIndex0 = LeafPathInner.IndexAt(1) as IFrameBrowsingListNodeIndex;
                Assert.That(NewLeafIndex0 == MovedLeafIndex0);

                IFrameNodeStateReadOnlyList AllChildren1 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));




                IFrameBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFrameBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFrameBrowsingExistingBlockNodeIndex MovedLeafIndex1 = LeafBlocksInner.IndexAt(1, 1) as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(MovedLeafIndex1));

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[1].StateList.Count;
                Assert.That(BlockNodeCount == 4, $"New count: {BlockNodeCount}");

                Assert.That(Controller.IsMoveable(LeafBlocksInner, MovedLeafIndex1, -1));
                Controller.Move(LeafBlocksInner, MovedLeafIndex1, -1);
                Assert.That(Controller.Contains(MovedLeafIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount);
                Assert.That(LeafBlocksInner.BlockStateList[1].StateList.Count == NodeCount);

                IFrameBrowsingExistingBlockNodeIndex NewLeafIndex1 = LeafBlocksInner.IndexAt(1, 0) as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(NewLeafIndex1 == MovedLeafIndex1);

                IFrameNodeStateReadOnlyList AllChildren2 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameMoveBlock()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameNodeStateReadOnlyList AllChildren1 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == 19, $"New count: {AllChildren1.Count}");

                IFrameBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFrameBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFrameBrowsingExistingBlockNodeIndex MovedLeafIndex1 = LeafBlocksInner.IndexAt(1, 0) as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(MovedLeafIndex1));

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[1].StateList.Count;
                Assert.That(BlockNodeCount == 4, $"New count: {BlockNodeCount}");

                Assert.That(Controller.IsBlockMoveable(LeafBlocksInner, 1, -1));
                Controller.MoveBlock(LeafBlocksInner, 1, -1);
                Assert.That(Controller.Contains(MovedLeafIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == NodeCount);

                IFrameBrowsingExistingBlockNodeIndex NewLeafIndex1 = LeafBlocksInner.IndexAt(0, 0) as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(NewLeafIndex1 == MovedLeafIndex1);

                IFrameNodeStateReadOnlyList AllChildren2 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameChangeDiscreteValue()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                Assert.That(BaseNodeHelper.NodeTreeHelper.GetEnumValue(RootState.Node, nameof(IMain.ValueEnum)) == (int)BaseNode.CopySemantic.Value);

                Controller.ChangeDiscreteValue(RootIndex, nameof(IMain.ValueEnum), (int)BaseNode.CopySemantic.Reference);

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(BaseNodeHelper.NodeTreeHelper.GetEnumValue(RootNode, nameof(IMain.ValueEnum)) == (int)BaseNode.CopySemantic.Reference);

                IFramePlaceholderInner PlaceholderTreeInner = RootState.PropertyToInner(nameof(IMain.PlaceholderTree)) as IFramePlaceholderInner;
                IFramePlaceholderNodeState PlaceholderTreeState = PlaceholderTreeInner.ChildState as IFramePlaceholderNodeState;

                Assert.That(BaseNodeHelper.NodeTreeHelper.GetEnumValue(PlaceholderTreeState.Node, nameof(ITree.ValueEnum)) == (int)BaseNode.CopySemantic.Value);

                Controller.ChangeDiscreteValue(PlaceholderTreeState.ParentIndex, nameof(ITree.ValueEnum), (int)BaseNode.CopySemantic.Any);

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(BaseNodeHelper.NodeTreeHelper.GetEnumValue(PlaceholderTreeState.Node, nameof(ITree.ValueEnum)) == (int)BaseNode.CopySemantic.Any);

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameReplace()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                Leaf NewItem0 = CreateLeaf(Guid.NewGuid());
                IFrameInsertionListNodeIndex ReplacementIndex0 = new FrameInsertionListNodeIndex(RootNode, nameof(IMain.LeafPath), NewItem0, 0);

                IFrameListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFrameListInner;
                Assert.That(LeafPathInner != null);

                int PathCount = LeafPathInner.Count;
                Assert.That(PathCount == 2);

                IFrameNodeStateReadOnlyList AllChildren0 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Controller.Replace(LeafPathInner, ReplacementIndex0, out IWriteableBrowsingChildIndex NewItemIndex0);
                Assert.That(Controller.Contains(NewItemIndex0));

                Assert.That(LeafPathInner.Count == PathCount);
                Assert.That(LeafPathInner.StateList.Count == PathCount);

                IFramePlaceholderNodeState NewItemState0 = LeafPathInner.StateList[0];
                Assert.That(NewItemState0.Node == NewItem0);
                Assert.That(NewItemState0.ParentIndex == NewItemIndex0);

                IFrameNodeStateReadOnlyList AllChildren1 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                Leaf NewItem1 = CreateLeaf(Guid.NewGuid());
                IFrameInsertionExistingBlockNodeIndex ReplacementIndex1 = new FrameInsertionExistingBlockNodeIndex(RootNode, nameof(IMain.LeafBlocks), NewItem1, 0, 0);

                IFrameBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFrameBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFrameBlockState BlockState = LeafBlocksInner.BlockStateList[0];

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = BlockState.StateList.Count;
                Assert.That(BlockNodeCount == 4);

                Controller.Replace(LeafBlocksInner, ReplacementIndex1, out IWriteableBrowsingChildIndex NewItemIndex1);
                Assert.That(Controller.Contains(NewItemIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount);
                Assert.That(BlockState.StateList.Count == NodeCount);

                IFramePlaceholderNodeState NewItemState1 = BlockState.StateList[0];
                Assert.That(NewItemState1.Node == NewItem1);
                Assert.That(NewItemState1.ParentIndex == NewItemIndex1);

                IFrameNodeStateReadOnlyList AllChildren2 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IFramePlaceholderInner PlaceholderTreeInner = RootState.PropertyToInner(nameof(IMain.PlaceholderTree)) as IFramePlaceholderInner;
                Assert.That(PlaceholderTreeInner != null);

                IFrameBrowsingPlaceholderNodeIndex ExistingIndex2 = PlaceholderTreeInner.ChildState.ParentIndex as IFrameBrowsingPlaceholderNodeIndex;

                Tree NewItem2 = CreateTree();
                IFrameInsertionPlaceholderNodeIndex ReplacementIndex2;
                ReplacementIndex2 = ExistingIndex2.ToInsertionIndex(RootNode, NewItem2) as IFrameInsertionPlaceholderNodeIndex;

                Controller.Replace(PlaceholderTreeInner, ReplacementIndex2, out IWriteableBrowsingChildIndex NewItemIndex2);
                Assert.That(Controller.Contains(NewItemIndex2));

                IFramePlaceholderNodeState NewItemState2 = PlaceholderTreeInner.ChildState as IFramePlaceholderNodeState;
                Assert.That(NewItemState2.Node == NewItem2);
                Assert.That(NewItemState2.ParentIndex == NewItemIndex2);

                IFrameBrowsingPlaceholderNodeIndex DuplicateExistingIndex2 = ReplacementIndex2.ToBrowsingIndex() as IFrameBrowsingPlaceholderNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex2 as IFrameBrowsingPlaceholderNodeIndex, DuplicateExistingIndex2));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex2, NewItemIndex2 as IFrameBrowsingPlaceholderNodeIndex));

                IFrameNodeStateReadOnlyList AllChildren3 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IFramePlaceholderInner PlaceholderLeafInner = NewItemState2.PropertyToInner(nameof(ITree.Placeholder)) as IFramePlaceholderInner;
                Assert.That(PlaceholderLeafInner != null);

                IFrameBrowsingPlaceholderNodeIndex ExistingIndex3 = PlaceholderLeafInner.ChildState.ParentIndex as IFrameBrowsingPlaceholderNodeIndex;

                Leaf NewItem3 = CreateLeaf(Guid.NewGuid());
                IFrameInsertionPlaceholderNodeIndex ReplacementIndex3;
                ReplacementIndex3 = ExistingIndex3.ToInsertionIndex(NewItem2, NewItem3) as IFrameInsertionPlaceholderNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(ReplacementIndex3, ReplacementIndex3));

                Controller.Replace(PlaceholderLeafInner, ReplacementIndex3, out IWriteableBrowsingChildIndex NewItemIndex3);
                Assert.That(Controller.Contains(NewItemIndex3));

                IFramePlaceholderNodeState NewItemState3 = PlaceholderLeafInner.ChildState as IFramePlaceholderNodeState;
                Assert.That(NewItemState3.Node == NewItem3);
                Assert.That(NewItemState3.ParentIndex == NewItemIndex3);

                IFrameNodeStateReadOnlyList AllChildren4 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren4.Count == AllChildren3.Count, $"New count: {AllChildren4.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));




                IFrameOptionalInner OptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IFrameOptionalInner;
                Assert.That(OptionalLeafInner != null);

                IFrameBrowsingOptionalNodeIndex ExistingIndex4 = OptionalLeafInner.ChildState.ParentIndex as IFrameBrowsingOptionalNodeIndex;

                Leaf NewItem4 = CreateLeaf(Guid.NewGuid());
                IFrameInsertionOptionalNodeIndex ReplacementIndex4;
                ReplacementIndex4 = ExistingIndex4.ToInsertionIndex(RootNode, NewItem4) as IFrameInsertionOptionalNodeIndex;
                Assert.That(ReplacementIndex4.ParentNode == RootNode);
                Assert.That(ReplacementIndex4.PropertyName == OptionalLeafInner.PropertyName);
                Assert.That(CompareEqual.CoverIsEqual(ReplacementIndex4, ReplacementIndex4));

                Controller.Replace(OptionalLeafInner, ReplacementIndex4, out IWriteableBrowsingChildIndex NewItemIndex4);
                Assert.That(Controller.Contains(NewItemIndex4));

                Assert.That(OptionalLeafInner.IsAssigned);
                IFrameOptionalNodeState NewItemState4 = OptionalLeafInner.ChildState as IFrameOptionalNodeState;
                Assert.That(NewItemState4.Node == NewItem4);
                Assert.That(NewItemState4.ParentIndex == NewItemIndex4);

                IFrameBrowsingOptionalNodeIndex DuplicateExistingIndex4 = ReplacementIndex4.ToBrowsingIndex() as IFrameBrowsingOptionalNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex4 as IFrameBrowsingOptionalNodeIndex, DuplicateExistingIndex4));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex4, NewItemIndex4 as IFrameBrowsingOptionalNodeIndex));

                IFrameNodeStateReadOnlyList AllChildren5 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren5.Count == AllChildren4.Count, $"New count: {AllChildren5.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IFrameBrowsingOptionalNodeIndex ExistingIndex5 = OptionalLeafInner.ChildState.ParentIndex as IFrameBrowsingOptionalNodeIndex;

                Leaf NewItem5 = CreateLeaf(Guid.NewGuid());
                IFrameInsertionOptionalClearIndex ReplacementIndex5;
                ReplacementIndex5 = ExistingIndex5.ToInsertionIndex(RootNode, null) as IFrameInsertionOptionalClearIndex;
                Assert.That(ReplacementIndex5.ParentNode == RootNode);
                Assert.That(ReplacementIndex5.PropertyName == OptionalLeafInner.PropertyName);
                Assert.That(CompareEqual.CoverIsEqual(ReplacementIndex5, ReplacementIndex5));

                Controller.Replace(OptionalLeafInner, ReplacementIndex5, out IWriteableBrowsingChildIndex NewItemIndex5);
                Assert.That(Controller.Contains(NewItemIndex5));

                Assert.That(!OptionalLeafInner.IsAssigned);
                IFrameOptionalNodeState NewItemState5 = OptionalLeafInner.ChildState as IFrameOptionalNodeState;
                Assert.That(NewItemState5.ParentIndex == NewItemIndex5);

                IFrameBrowsingOptionalNodeIndex DuplicateExistingIndex5 = ReplacementIndex5.ToBrowsingIndex() as IFrameBrowsingOptionalNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex5 as IFrameBrowsingOptionalNodeIndex, DuplicateExistingIndex5));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex5, NewItemIndex5 as IFrameBrowsingOptionalNodeIndex));

                IFrameNodeStateReadOnlyList AllChildren6 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren6.Count == AllChildren5.Count - 1, $"New count: {AllChildren6.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameAssign()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            //System.Diagnostics.Debug.Assert(false);
            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameOptionalInner UnassignedOptionalLeafInner = RootState.PropertyToInner(nameof(IMain.UnassignedOptionalLeaf)) as IFrameOptionalInner;
                Assert.That(UnassignedOptionalLeafInner != null);
                Assert.That(!UnassignedOptionalLeafInner.IsAssigned);

                IFrameBrowsingOptionalNodeIndex AssignmentIndex0 = UnassignedOptionalLeafInner.ChildState.ParentIndex;
                Assert.That(AssignmentIndex0 != null);

                IFrameNodeStateReadOnlyList AllChildren0 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Controller.Assign(AssignmentIndex0, out bool IsChanged);
                Assert.That(IsChanged);
                Assert.That(UnassignedOptionalLeafInner.IsAssigned);

                IFrameNodeStateReadOnlyList AllChildren1 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Assign(AssignmentIndex0, out IsChanged);
                Assert.That(!IsChanged);
                Assert.That(UnassignedOptionalLeafInner.IsAssigned);

                IFrameNodeStateReadOnlyList AllChildren2 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Controller.Unassign(AssignmentIndex0, out IsChanged);
                Assert.That(IsChanged);
                Assert.That(!UnassignedOptionalLeafInner.IsAssigned);

                IFrameNodeStateReadOnlyList AllChildren3 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count - 1, $"New count: {AllChildren3.Count}");

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameUnassign()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameOptionalInner AssignedOptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IFrameOptionalInner;
                Assert.That(AssignedOptionalLeafInner != null);
                Assert.That(AssignedOptionalLeafInner.IsAssigned);

                IFrameBrowsingOptionalNodeIndex AssignmentIndex0 = AssignedOptionalLeafInner.ChildState.ParentIndex;
                Assert.That(AssignmentIndex0 != null);

                IFrameNodeStateReadOnlyList AllChildren0 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Controller.Unassign(AssignmentIndex0, out bool IsChanged);
                Assert.That(IsChanged);
                Assert.That(!AssignedOptionalLeafInner.IsAssigned);

                IFrameNodeStateReadOnlyList AllChildren1 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count - 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Unassign(AssignmentIndex0, out IsChanged);
                Assert.That(!IsChanged);
                Assert.That(!AssignedOptionalLeafInner.IsAssigned);

                IFrameNodeStateReadOnlyList AllChildren2 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Controller.Assign(AssignmentIndex0, out IsChanged);
                Assert.That(IsChanged);
                Assert.That(AssignedOptionalLeafInner.IsAssigned);

                IFrameNodeStateReadOnlyList AllChildren3 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count + 1, $"New count: {AllChildren3.Count}");

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameChangeReplication()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFrameBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFrameNodeStateReadOnlyList AllChildren0 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                IFrameBlockState BlockState = LeafBlocksInner.BlockStateList[0];
                Assert.That(BlockState != null);
                Assert.That(BlockState.ParentInner == LeafBlocksInner);
                BaseNode.IBlock ChildBlock = BlockState.ChildBlock;
                Assert.That(ChildBlock.Replication == BaseNode.ReplicationStatus.Normal);

                Controller.ChangeReplication(LeafBlocksInner, 0, BaseNode.ReplicationStatus.Replicated);

                Assert.That(ChildBlock.Replication == BaseNode.ReplicationStatus.Replicated);

                IFrameNodeStateReadOnlyList AllChildren1 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameSplit()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFrameBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFrameNodeStateReadOnlyList AllChildren0 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                IFrameBlockState BlockState0 = LeafBlocksInner.BlockStateList[0];
                Assert.That(BlockState0 != null);
                BaseNode.IBlock ChildBlock0 = BlockState0.ChildBlock;
                Assert.That(ChildBlock0.NodeList.Count == 1);

                IFrameBlockState BlockState1 = LeafBlocksInner.BlockStateList[1];
                Assert.That(BlockState1 != null);
                BaseNode.IBlock ChildBlock1 = BlockState1.ChildBlock;
                Assert.That(ChildBlock1.NodeList.Count == 2);

                Assert.That(LeafBlocksInner.Count == 4);
                Assert.That(LeafBlocksInner.BlockStateList.Count == 3);

                IFrameBrowsingExistingBlockNodeIndex SplitIndex0 = LeafBlocksInner.IndexAt(1, 1) as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.IsSplittable(LeafBlocksInner, SplitIndex0));

                Controller.SplitBlock(LeafBlocksInner, SplitIndex0);

                Assert.That(LeafBlocksInner.BlockStateList.Count == 4);
                Assert.That(ChildBlock0 == LeafBlocksInner.BlockStateList[0].ChildBlock);
                Assert.That(ChildBlock1 == LeafBlocksInner.BlockStateList[2].ChildBlock);
                Assert.That(ChildBlock1.NodeList.Count == 1);

                IFrameBlockState BlockState12 = LeafBlocksInner.BlockStateList[1];
                Assert.That(BlockState12.ChildBlock.NodeList.Count == 1);

                IFrameNodeStateReadOnlyList AllChildren1 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 2, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameMerge()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFrameBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFrameNodeStateReadOnlyList AllChildren0 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                IFrameBlockState BlockState0 = LeafBlocksInner.BlockStateList[0];
                Assert.That(BlockState0 != null);
                BaseNode.IBlock ChildBlock0 = BlockState0.ChildBlock;
                Assert.That(ChildBlock0.NodeList.Count == 1);

                IFrameBlockState BlockState1 = LeafBlocksInner.BlockStateList[1];
                Assert.That(BlockState1 != null);
                BaseNode.IBlock ChildBlock1 = BlockState1.ChildBlock;
                Assert.That(ChildBlock1.NodeList.Count == 2);

                Assert.That(LeafBlocksInner.Count == 4);

                IFrameBrowsingExistingBlockNodeIndex MergeIndex0 = LeafBlocksInner.IndexAt(1, 0) as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.IsMergeable(LeafBlocksInner, MergeIndex0));

                Assert.That(LeafBlocksInner.BlockStateList.Count == 3);

                Controller.MergeBlocks(LeafBlocksInner, MergeIndex0);

                Assert.That(LeafBlocksInner.BlockStateList.Count == 2);
                Assert.That(ChildBlock1 == LeafBlocksInner.BlockStateList[0].ChildBlock);
                Assert.That(ChildBlock1.NodeList.Count == 3);

                Assert.That(LeafBlocksInner.BlockStateList[0] == BlockState1);

                IFrameNodeStateReadOnlyList AllChildren1 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count - 2, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameExpand()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameNodeStateReadOnlyList AllChildren0 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Controller.Expand(RootIndex, out bool IsChanged);
                Assert.That(IsChanged);

                IFrameNodeStateReadOnlyList AllChildren1 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(!IsChanged);

                IFrameNodeStateReadOnlyList AllChildren2 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                IFrameOptionalInner OptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IFrameOptionalInner;
                Assert.That(OptionalLeafInner != null);

                IFrameInsertionOptionalClearIndex ReplacementIndex5 = new FrameInsertionOptionalClearIndex(RootNode, nameof(IMain.AssignedOptionalLeaf));

                Controller.Replace(OptionalLeafInner, ReplacementIndex5, out IWriteableBrowsingChildIndex NewItemIndex5);
                Assert.That(Controller.Contains(NewItemIndex5));

                IFrameNodeStateReadOnlyList AllChildren3 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count - 1, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFrameNodeStateReadOnlyList AllChildren4 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren4.Count == AllChildren3.Count + 1, $"New count: {AllChildren4.Count}");



                IFrameBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFrameBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFrameBrowsingExistingBlockNodeIndex RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                IFrameNodeStateReadOnlyList AllChildren5 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren5.Count == AllChildren4.Count - 10, $"New count: {AllChildren5.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(LeafBlocksInner.IsEmpty);

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(!IsChanged);

                IFrameNodeStateReadOnlyList AllChildren6 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren6.Count == AllChildren5.Count, $"New count: {AllChildren6.Count}");

                IDictionary<Type, string[]> WithExpandCollectionTable = BaseNodeHelper.NodeHelper.WithExpandCollectionTable as IDictionary<Type, string[]>;
                WithExpandCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFrameNodeStateReadOnlyList AllChildren7 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren7.Count == AllChildren6.Count + 3, $"New count: {AllChildren7.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(!LeafBlocksInner.IsEmpty);
                Assert.That(LeafBlocksInner.IsSingle);

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                WithExpandCollectionTable.Remove(typeof(IMain));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameReduce()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;
            bool IsChanged;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFrameBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFrameBrowsingExistingBlockNodeIndex RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(LeafBlocksInner.IsEmpty);

                IFrameNodeStateReadOnlyList AllChildren0 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 9, $"New count: {AllChildren0.Count}");

                IDictionary<Type, string[]> WithExpandCollectionTable = BaseNodeHelper.NodeHelper.WithExpandCollectionTable as IDictionary<Type, string[]>;
                WithExpandCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFrameNodeStateReadOnlyList AllChildren1 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 4, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                //System.Diagnostics.Debug.Assert(false);
                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFrameNodeStateReadOnlyList AllChildren2 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count - 7, $"New count: {AllChildren2.Count}");

                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(!IsChanged);

                IFrameNodeStateReadOnlyList AllChildren3 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count, $"New count: {AllChildren3.Count}");

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFrameNodeStateReadOnlyList AllChildren4 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren4.Count == AllChildren3.Count + 7, $"New count: {AllChildren4.Count}");

                BaseNode.IBlock ChildBlock = LeafBlocksInner.BlockStateList[0].ChildBlock;
                ILeaf FirstNode = ChildBlock.NodeList[0] as ILeaf;
                Assert.That(FirstNode != null);
                BaseNodeHelper.NodeTreeHelper.SetString(FirstNode, nameof(ILeaf.Text), "!");

                //System.Diagnostics.Debug.Assert(false);
                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFrameNodeStateReadOnlyList AllChildren5 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren5.Count == AllChildren4.Count - 4, $"New count: {AllChildren5.Count}");

                BaseNodeHelper.NodeTreeHelper.SetString(FirstNode, nameof(ILeaf.Text), "");

                //System.Diagnostics.Debug.Assert(false);
                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFrameNodeStateReadOnlyList AllChildren6 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren6.Count == AllChildren5.Count - 3, $"New count: {AllChildren6.Count}");

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                WithExpandCollectionTable.Remove(typeof(IMain));

                //System.Diagnostics.Debug.Assert(false);
                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFrameNodeStateReadOnlyList AllChildren7 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren7.Count == AllChildren6.Count + 3, $"New count: {AllChildren7.Count}");

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                WithExpandCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                WithExpandCollectionTable.Remove(typeof(IMain));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameCanonicalize()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;
            bool IsChanged;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFrameBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFrameBrowsingExistingBlockNodeIndex RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                Assert.That(Controller.CanUndo);
                IFrameOperationGroup LastOperation = Controller.OperationStack[Controller.RedoIndex - 1];
                Assert.That(LastOperation.MainOperation is IFrameRemoveOperation);
                Assert.That(LastOperation.OperationList.Count > 0);
                Assert.That(LastOperation.Refresh == null);

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFrameBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(LeafBlocksInner.IsEmpty);

                IFrameListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFrameListInner;
                Assert.That(LeafPathInner != null);
                Assert.That(LeafPathInner.Count == 2);

                IFrameBrowsingListNodeIndex RemovedListLeafIndex = LeafPathInner.StateList[0].ParentIndex as IFrameBrowsingListNodeIndex;
                Assert.That(Controller.Contains(RemovedListLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedListLeafIndex));

                Controller.Remove(LeafPathInner, RemovedListLeafIndex);
                Assert.That(!Controller.Contains(RemovedListLeafIndex));

                IDictionary<Type, string[]> NeverEmptyCollectionTable = BaseNodeHelper.NodeHelper.NeverEmptyCollectionTable as IDictionary<Type, string[]>;
                NeverEmptyCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.PlaceholderTree) });

                RemovedListLeafIndex = LeafPathInner.StateList[0].ParentIndex as IFrameBrowsingListNodeIndex;
                Assert.That(Controller.Contains(RemovedListLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedListLeafIndex));

                Controller.Remove(LeafPathInner, RemovedListLeafIndex);
                Assert.That(!Controller.Contains(RemovedListLeafIndex));
                Assert.That(LeafPathInner.Count == 0);

                NeverEmptyCollectionTable.Remove(typeof(IMain));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                IFrameNodeStateReadOnlyList AllChildren0 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 12, $"New count: {AllChildren0.Count}");

                IDictionary<Type, string[]> WithExpandCollectionTable = BaseNodeHelper.NodeHelper.WithExpandCollectionTable as IDictionary<Type, string[]>;
                WithExpandCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });

                //System.Diagnostics.Debug.Assert(false);
                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFrameNodeStateReadOnlyList AllChildren1 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Canonicalize(out IsChanged);
                Assert.That(IsChanged);

                IFrameNodeStateReadOnlyList AllChildren2 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count - 4, $"New count: {AllChildren2.Count}");

                Controller.Undo();
                Controller.Redo();

                Controller.Canonicalize(out IsChanged);
                Assert.That(!IsChanged);

                IFrameNodeStateReadOnlyList AllChildren3 = (IFrameNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count, $"New count: {AllChildren3.Count}");

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                NeverEmptyCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });
                Assert.That(LeafBlocksInner.BlockStateList.Count == 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == 1, LeafBlocksInner.BlockStateList[0].StateList.Count.ToString());

                Controller.Canonicalize(out IsChanged);
                Assert.That(IsChanged);

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                NeverEmptyCollectionTable.Remove(typeof(IMain));

                WithExpandCollectionTable.Remove(typeof(IMain));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FramePrune()
        {
            ControllerTools.ResetExpectedName();

            IMain MainItem = CreateRoot(ValueGuid0, Imperfections.None);
            IRoot RootNode = new Root();
            BaseNode.IDocument RootDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("root doc", Guid.NewGuid());
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(RootNode, RootDocument);
            BaseNode.IBlockList<IMain, Main> MainBlocks = BaseNodeHelper.BlockListHelper<IMain, Main>.CreateSimpleBlockList(MainItem);

            IMain UnassignedOptionalMain = CreateRoot(ValueGuid1, Imperfections.None);
            Easly.IOptionalReference<IMain> UnassignedOptional = BaseNodeHelper.OptionalReferenceHelper<IMain>.CreateReference(UnassignedOptionalMain);

            IList<ILeaf> LeafPath = new List<ILeaf>();

            BaseNodeHelper.NodeTreeHelperBlockList.SetBlockList(RootNode, nameof(IRoot.MainBlocks), (BaseNode.IBlockList)MainBlocks);
            BaseNodeHelper.NodeTreeHelperOptional.SetOptionalReference(RootNode, nameof(IRoot.UnassignedOptionalMain), (Easly.IOptionalReference)UnassignedOptional);
            BaseNodeHelper.NodeTreeHelper.SetString(RootNode, nameof(IRoot.ValueString), "root string");
            BaseNodeHelper.NodeTreeHelperList.SetChildNodeList(RootNode, nameof(IRoot.LeafPath), (IList)LeafPath);

            //System.Diagnostics.Debug.Assert(false);
            IFrameRootNodeIndex RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            using (IFrameControllerView ControllerView0 = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameBlockListInner MainInner = RootState.PropertyToInner(nameof(IRoot.MainBlocks)) as IFrameBlockListInner;
                Assert.That(MainInner != null);

                IFrameBrowsingExistingBlockNodeIndex MainIndex = MainInner.IndexAt(0, 0) as IFrameBrowsingExistingBlockNodeIndex;
                Controller.Remove(MainInner, MainIndex);

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                MainIndex = MainInner.IndexAt(0, 0) as IFrameBrowsingExistingBlockNodeIndex;
                Controller.Remove(MainInner, MainIndex);

                Controller.Undo();
                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FrameCollections()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFrameRootNodeIndex RootIndex;
            bool IsReadOnly;
            IReadOnlyBlockState FirstBlockState;
            IReadOnlyBrowsingBlockNodeIndex FirstBlockNodeIndex;
            IReadOnlyBrowsingListNodeIndex FirstListNodeIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FrameRootNodeIndex(RootNode);

            IFrameController ControllerBase = FrameController.Create(RootIndex);
            IFrameController Controller = FrameController.Create(RootIndex);

            IReadOnlyIndexNodeStateDictionary ControllerStateTable = DebugObjects.GetReferenceByInterface(typeof(IFrameIndexNodeStateDictionary)) as IReadOnlyIndexNodeStateDictionary;

            using (IFrameControllerView ControllerView = FrameControllerView.Create(Controller, TestDebug.CoverageFrameTemplateSet.FrameTemplateSet))
            {
                Controller.Canonicalize(out bool IsChanged);

                // IxxxBlockStateViewDictionary 

                IReadOnlyBlockStateViewDictionary ReadOnlyBlockStateViewTable = ControllerView.BlockStateViewTable;

                foreach (KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView> Entry in ReadOnlyBlockStateViewTable)
                {
                    IReadOnlyBlockStateView StateView = ReadOnlyBlockStateViewTable[Entry.Key];
                    ReadOnlyBlockStateViewTable.TryGetValue(Entry.Key, out IReadOnlyBlockStateView Value);
                    ReadOnlyBlockStateViewTable.Contains(Entry);
                    ReadOnlyBlockStateViewTable.Remove(Entry.Key);
                    ReadOnlyBlockStateViewTable.Add(Entry.Key, Entry.Value);
                    ICollection<IReadOnlyBlockState> Keys = ReadOnlyBlockStateViewTable.Keys;
                    ICollection<IReadOnlyBlockStateView> Values = ReadOnlyBlockStateViewTable.Values;

                    break;
                }

                IDictionary<IReadOnlyBlockState, IReadOnlyBlockStateView> ReadOnlyBlockStateViewTableAsDictionary = ReadOnlyBlockStateViewTable;
                foreach (KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView> Entry in ReadOnlyBlockStateViewTableAsDictionary)
                {
                    IReadOnlyBlockStateView StateView = ReadOnlyBlockStateViewTableAsDictionary[Entry.Key];
                    break;
                }

                ICollection<KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView>> ReadOnlyBlockStateViewTableAsCollection = ReadOnlyBlockStateViewTable;
                IsReadOnly = ReadOnlyBlockStateViewTableAsCollection.IsReadOnly;

                foreach (KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView> Entry in ReadOnlyBlockStateViewTableAsCollection)
                {
                    ReadOnlyBlockStateViewTableAsCollection.Contains(Entry);
                    ReadOnlyBlockStateViewTableAsCollection.Remove(Entry);
                    ReadOnlyBlockStateViewTableAsCollection.Add(Entry);
                    ReadOnlyBlockStateViewTableAsCollection.CopyTo(new KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView>[ReadOnlyBlockStateViewTableAsCollection.Count], 0);
                    break;
                }

                IEnumerable<KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView>> ReadOnlyBlockStateViewTableAsEnumerable = ReadOnlyBlockStateViewTable;
                foreach (KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView> Entry in ReadOnlyBlockStateViewTableAsEnumerable)
                {
                    break;
                }

                IWriteableBlockStateViewDictionary WriteableBlockStateViewTable = ControllerView.BlockStateViewTable;

                foreach (KeyValuePair<IWriteableBlockState, IWriteableBlockStateView> Entry in WriteableBlockStateViewTable)
                {
                    IWriteableBlockStateView StateView = WriteableBlockStateViewTable[Entry.Key];
                    WriteableBlockStateViewTable.TryGetValue(Entry.Key, out IWriteableBlockStateView Value);
                    WriteableBlockStateViewTable.Contains(Entry);
                    WriteableBlockStateViewTable.Remove(Entry.Key);
                    WriteableBlockStateViewTable.Add(Entry.Key, Entry.Value);
                    ICollection<IWriteableBlockState> Keys = ((IDictionary<IWriteableBlockState, IWriteableBlockStateView>)WriteableBlockStateViewTable).Keys;
                    ICollection<IWriteableBlockStateView> Values = ((IDictionary<IWriteableBlockState, IWriteableBlockStateView>)WriteableBlockStateViewTable).Values;

                    break;
                }

                IDictionary<IWriteableBlockState, IWriteableBlockStateView> WriteableBlockStateViewTableAsDictionary = WriteableBlockStateViewTable;
                foreach (KeyValuePair<IWriteableBlockState, IWriteableBlockStateView> Entry in WriteableBlockStateViewTableAsDictionary)
                {
                    IWriteableBlockStateView StateView = WriteableBlockStateViewTableAsDictionary[Entry.Key];
                    break;
                }

                ICollection<KeyValuePair<IWriteableBlockState, IWriteableBlockStateView>> WriteableBlockStateViewTableAsCollection = WriteableBlockStateViewTable;
                IsReadOnly = WriteableBlockStateViewTableAsCollection.IsReadOnly;

                foreach (KeyValuePair<IWriteableBlockState, IWriteableBlockStateView> Entry in WriteableBlockStateViewTableAsCollection)
                {
                    WriteableBlockStateViewTableAsCollection.Contains(Entry);
                    WriteableBlockStateViewTableAsCollection.Remove(Entry);
                    WriteableBlockStateViewTableAsCollection.Add(Entry);
                    WriteableBlockStateViewTableAsCollection.CopyTo(new KeyValuePair<IWriteableBlockState, IWriteableBlockStateView>[WriteableBlockStateViewTableAsCollection.Count], 0);
                    break;
                }

                IEnumerable<KeyValuePair<IWriteableBlockState, IWriteableBlockStateView>> WriteableBlockStateViewTableAsEnumerable = WriteableBlockStateViewTable;
                foreach (KeyValuePair<IWriteableBlockState, IWriteableBlockStateView> Entry in WriteableBlockStateViewTableAsEnumerable)
                {
                    break;
                }

                // IFrameBlockStateList

                IFrameNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFrameBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFrameBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFrameListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFrameListInner;
                Assert.That(LeafPathInner != null);

                IFramePlaceholderNodeState FirstNodeState = LeafBlocksInner.FirstNodeState;
                IFrameBlockStateList DebugBlockStateList = DebugObjects.GetReferenceByInterface(typeof(IFrameBlockStateList)) as IFrameBlockStateList;
                if (DebugBlockStateList != null)
                {
                    Assert.That(DebugBlockStateList.Count > 0);
                    IsReadOnly = ((ICollection<IReadOnlyBlockState>)DebugBlockStateList).IsReadOnly;
                    IsReadOnly = ((IList<IReadOnlyBlockState>)DebugBlockStateList).IsReadOnly;
                    IsReadOnly = ((ICollection<IWriteableBlockState>)DebugBlockStateList).IsReadOnly;
                    IsReadOnly = ((IList<IWriteableBlockState>)DebugBlockStateList).IsReadOnly;
                    FirstBlockState = DebugBlockStateList[0];
                    Assert.That(((IWriteableBlockStateList)DebugBlockStateList)[0] == FirstBlockState);
                    Assert.That(DebugBlockStateList.Contains(FirstBlockState));
                    Assert.That(DebugBlockStateList.IndexOf(FirstBlockState) == 0);
                    DebugBlockStateList.Remove(FirstBlockState);
                    DebugBlockStateList.Add(FirstBlockState);
                    DebugBlockStateList.Remove(FirstBlockState);
                    DebugBlockStateList.Insert(0, FirstBlockState);
                    DebugBlockStateList.CopyTo((IReadOnlyBlockState[])(new IFrameBlockState[DebugBlockStateList.Count]), 0);
                    DebugBlockStateList.CopyTo((IWriteableBlockState[])(new IFrameBlockState[DebugBlockStateList.Count]), 0);

                    IEnumerable<IReadOnlyBlockState> BlockStateListAsReadOnlyEnumerable = DebugBlockStateList;
                    foreach (IReadOnlyBlockState Item in BlockStateListAsReadOnlyEnumerable)
                    {
                        break;
                    }

                    IEnumerable<IWriteableBlockState> BlockStateListAsWriteableEnumerable = DebugBlockStateList;
                    foreach (IWriteableBlockState Item in BlockStateListAsWriteableEnumerable)
                    {
                        break;
                    }

                    IList<IReadOnlyBlockState> BlockStateListAsReadOnlyIlist = DebugBlockStateList;
                    Assert.That(BlockStateListAsReadOnlyIlist[0] == FirstBlockState);

                    IList<IWriteableBlockState> BlockStateListAsWriteableIList = DebugBlockStateList;
                    Assert.That(BlockStateListAsWriteableIList[0] == FirstBlockState);
                    Assert.That(BlockStateListAsWriteableIList.Contains((IWriteableBlockState)FirstBlockState));
                    Assert.That(BlockStateListAsWriteableIList.IndexOf((IWriteableBlockState)FirstBlockState) == 0);

                    ICollection<IWriteableBlockState> BlockStateListAsWriteableICollection = DebugBlockStateList;
                    Assert.That(BlockStateListAsWriteableICollection.Contains((IWriteableBlockState)FirstBlockState));
                    BlockStateListAsWriteableICollection.Remove((IWriteableBlockState)FirstBlockState);
                    BlockStateListAsWriteableICollection.Add((IWriteableBlockState)FirstBlockState);
                    BlockStateListAsWriteableICollection.Remove((IWriteableBlockState)FirstBlockState);
                    BlockStateListAsWriteableIList.Insert(0, (IWriteableBlockState)FirstBlockState);

                    IReadOnlyList<IReadOnlyBlockState> BlockStateListAsReadOnlyIReadOnlylist = DebugBlockStateList;
                    Assert.That(BlockStateListAsReadOnlyIReadOnlylist[0] == FirstBlockState);

                    IReadOnlyList<IWriteableBlockState> BlockStateListAsWriteableIReadOnlylist = DebugBlockStateList;
                    Assert.That(BlockStateListAsWriteableIReadOnlylist[0] == FirstBlockState);

                    IEnumerator<IWriteableBlockState> DebugBlockStateListWriteableEnumerator = ((IWriteableBlockStateList)DebugBlockStateList).GetEnumerator();
                }

                IFrameBlockStateReadOnlyList FrameBlockStateList = LeafBlocksInner.BlockStateList;
                Assert.That(FrameBlockStateList.Count > 0);
                FirstBlockState = FrameBlockStateList[0];
                Assert.That(FrameBlockStateList.Contains(FirstBlockState));
                Assert.That(FrameBlockStateList.IndexOf(FirstBlockState) == 0);
                Assert.That(FrameBlockStateList.Contains((IWriteableBlockState)FirstBlockState));
                Assert.That(FrameBlockStateList.IndexOf((IWriteableBlockState)FirstBlockState) == 0);
                Assert.That(FrameBlockStateList.Contains((IFrameBlockState)FirstBlockState));
                Assert.That(FrameBlockStateList.IndexOf((IFrameBlockState)FirstBlockState) == 0);

                IEnumerable<IWriteableBlockState> FrameBlockStateListAsIEnumerable = FrameBlockStateList;
                IEnumerator<IWriteableBlockState> FrameBlockStateListAsIEnumerableEnumerator = FrameBlockStateListAsIEnumerable.GetEnumerator();

                IReadOnlyList<IWriteableBlockState> FrameBlockStateListAsIReadOnlyList = FrameBlockStateList;
                Assert.That(FrameBlockStateListAsIReadOnlyList[0] == FirstBlockState);

                // IFrameBrowsingBlockNodeIndexList

                IFrameBrowsingBlockNodeIndexList BlockNodeIndexList = LeafBlocksInner.AllIndexes() as IFrameBrowsingBlockNodeIndexList;
                Assert.That(BlockNodeIndexList.Count > 0);
                IsReadOnly = ((ICollection<IReadOnlyBrowsingBlockNodeIndex>)BlockNodeIndexList).IsReadOnly;
                IsReadOnly = ((IList<IReadOnlyBrowsingBlockNodeIndex>)BlockNodeIndexList).IsReadOnly;
                IsReadOnly = ((ICollection<IWriteableBrowsingBlockNodeIndex>)BlockNodeIndexList).IsReadOnly;
                IsReadOnly = ((IList<IWriteableBrowsingBlockNodeIndex>)BlockNodeIndexList).IsReadOnly;
                FirstBlockNodeIndex = BlockNodeIndexList[0];
                Assert.That(((IWriteableBrowsingBlockNodeIndexList)BlockNodeIndexList)[0] == FirstBlockNodeIndex);
                Assert.That(BlockNodeIndexList.Contains(FirstBlockNodeIndex));
                Assert.That(BlockNodeIndexList.IndexOf(FirstBlockNodeIndex) == 0);
                BlockNodeIndexList.Remove(FirstBlockNodeIndex);
                BlockNodeIndexList.Add(FirstBlockNodeIndex);
                BlockNodeIndexList.Remove(FirstBlockNodeIndex);
                BlockNodeIndexList.Insert(0, FirstBlockNodeIndex);
                BlockNodeIndexList.CopyTo((IReadOnlyBrowsingBlockNodeIndex[])(new IFrameBrowsingBlockNodeIndex[BlockNodeIndexList.Count]), 0);
                BlockNodeIndexList.CopyTo((IWriteableBrowsingBlockNodeIndex[])(new IFrameBrowsingBlockNodeIndex[BlockNodeIndexList.Count]), 0);

                IEnumerable<IReadOnlyBrowsingBlockNodeIndex> BlockNodeIndexListAsReadOnlyEnumerable = BlockNodeIndexList;
                foreach (IReadOnlyBrowsingBlockNodeIndex Item in BlockNodeIndexListAsReadOnlyEnumerable)
                {
                    break;
                }

                IEnumerable<IWriteableBrowsingBlockNodeIndex> BlockNodeIndexListAsWriteableEnumerable = BlockNodeIndexList;
                foreach (IWriteableBrowsingBlockNodeIndex Item in BlockNodeIndexListAsWriteableEnumerable)
                {
                    break;
                }

                IList<IReadOnlyBrowsingBlockNodeIndex> BlockNodeIndexListAsReadOnlyIList = BlockNodeIndexList;
                Assert.That(BlockNodeIndexListAsReadOnlyIList[0] == FirstBlockNodeIndex);

                IList<IWriteableBrowsingBlockNodeIndex> BlockNodeIndexListAsWriteableIList = BlockNodeIndexList;
                Assert.That(BlockNodeIndexListAsWriteableIList[0] == FirstBlockNodeIndex);
                Assert.That(BlockNodeIndexListAsWriteableIList.Contains((IWriteableBrowsingBlockNodeIndex)FirstBlockNodeIndex));
                Assert.That(BlockNodeIndexListAsWriteableIList.IndexOf((IWriteableBrowsingBlockNodeIndex)FirstBlockNodeIndex) == 0);

                ICollection<IWriteableBrowsingBlockNodeIndex> BrowsingBlockNodeIndexListAsWriteableICollection = BlockNodeIndexList;
                Assert.That(BrowsingBlockNodeIndexListAsWriteableICollection.Contains((IWriteableBrowsingBlockNodeIndex)FirstBlockNodeIndex));
                BrowsingBlockNodeIndexListAsWriteableICollection.Remove((IWriteableBrowsingBlockNodeIndex)FirstBlockNodeIndex);
                BrowsingBlockNodeIndexListAsWriteableICollection.Add((IWriteableBrowsingBlockNodeIndex)FirstBlockNodeIndex);
                BrowsingBlockNodeIndexListAsWriteableICollection.Remove((IWriteableBrowsingBlockNodeIndex)FirstBlockNodeIndex);
                BlockNodeIndexListAsWriteableIList.Insert(0, (IWriteableBrowsingBlockNodeIndex)FirstBlockNodeIndex);

                IReadOnlyList<IReadOnlyBrowsingBlockNodeIndex> BlockNodeIndexListAsReadOnlyIReadOnlylist = BlockNodeIndexList;
                Assert.That(BlockNodeIndexListAsReadOnlyIReadOnlylist[0] == FirstBlockNodeIndex);

                IReadOnlyList<IWriteableBrowsingBlockNodeIndex> BlockNodeIndexListAsWriteableIReadOnlylist = BlockNodeIndexList;
                Assert.That(BlockNodeIndexListAsWriteableIReadOnlylist[0] == FirstBlockNodeIndex);

                IReadOnlyBrowsingBlockNodeIndexList BlockNodeIndexListAsReadOnly = BlockNodeIndexList;
                Assert.That(BlockNodeIndexListAsReadOnly[0] == FirstBlockNodeIndex);

                IEnumerator<IWriteableBrowsingBlockNodeIndex> BlockNodeIndexListWriteableEnumerator = ((IWriteableBrowsingBlockNodeIndexList)BlockNodeIndexList).GetEnumerator();

                // IFrameBrowsingListNodeIndexList

                IFrameBrowsingListNodeIndexList ListNodeIndexList = LeafPathInner.AllIndexes() as IFrameBrowsingListNodeIndexList;
                Assert.That(ListNodeIndexList.Count > 0);
                IsReadOnly = ((ICollection<IReadOnlyBrowsingListNodeIndex>)ListNodeIndexList).IsReadOnly;
                IsReadOnly = ((IList<IReadOnlyBrowsingListNodeIndex>)ListNodeIndexList).IsReadOnly;
                IsReadOnly = ((ICollection<IWriteableBrowsingListNodeIndex>)ListNodeIndexList).IsReadOnly;
                IsReadOnly = ((IList<IWriteableBrowsingListNodeIndex>)ListNodeIndexList).IsReadOnly;
                FirstListNodeIndex = ListNodeIndexList[0];
                Assert.That(((IWriteableBrowsingListNodeIndexList)ListNodeIndexList)[0] == FirstListNodeIndex);
                Assert.That(ListNodeIndexList.Contains(FirstListNodeIndex));
                Assert.That(ListNodeIndexList.IndexOf(FirstListNodeIndex) == 0);
                ListNodeIndexList.Remove(FirstListNodeIndex);
                ListNodeIndexList.Add(FirstListNodeIndex);
                ListNodeIndexList.Remove(FirstListNodeIndex);
                ListNodeIndexList.Insert(0, FirstListNodeIndex);
                ListNodeIndexList.CopyTo((IReadOnlyBrowsingListNodeIndex[])(new IFrameBrowsingListNodeIndex[ListNodeIndexList.Count]), 0);
                ListNodeIndexList.CopyTo((IWriteableBrowsingListNodeIndex[])(new IFrameBrowsingListNodeIndex[ListNodeIndexList.Count]), 0);

                IEnumerable<IReadOnlyBrowsingListNodeIndex> ListNodeIndexListAsReadOnlyEnumerable = ListNodeIndexList;
                foreach (IReadOnlyBrowsingListNodeIndex Item in ListNodeIndexListAsReadOnlyEnumerable)
                {
                    break;
                }

                IEnumerable<IWriteableBrowsingListNodeIndex> ListNodeIndexListAsWriteableEnumerable = ListNodeIndexList;
                foreach (IWriteableBrowsingListNodeIndex Item in ListNodeIndexListAsWriteableEnumerable)
                {
                    break;
                }

                IList<IReadOnlyBrowsingListNodeIndex> ListNodeIndexListAsReadOnlyIList = ListNodeIndexList;
                Assert.That(ListNodeIndexListAsReadOnlyIList[0] == FirstListNodeIndex);

                IList<IWriteableBrowsingListNodeIndex> ListNodeIndexListAsWriteableIList = ListNodeIndexList;
                Assert.That(ListNodeIndexListAsWriteableIList[0] == FirstListNodeIndex);
                Assert.That(ListNodeIndexListAsWriteableIList.Contains((IWriteableBrowsingListNodeIndex)FirstListNodeIndex));
                Assert.That(ListNodeIndexListAsWriteableIList.IndexOf((IWriteableBrowsingListNodeIndex)FirstListNodeIndex) == 0);

                ICollection<IWriteableBrowsingListNodeIndex> BrowsingListNodeIndexListAsWriteableICollection = ListNodeIndexList;
                Assert.That(BrowsingListNodeIndexListAsWriteableICollection.Contains((IWriteableBrowsingListNodeIndex)FirstListNodeIndex));
                BrowsingListNodeIndexListAsWriteableICollection.Remove((IWriteableBrowsingListNodeIndex)FirstListNodeIndex);
                BrowsingListNodeIndexListAsWriteableICollection.Add((IWriteableBrowsingListNodeIndex)FirstListNodeIndex);
                BrowsingListNodeIndexListAsWriteableICollection.Remove((IWriteableBrowsingListNodeIndex)FirstListNodeIndex);
                ListNodeIndexListAsWriteableIList.Insert(0, (IWriteableBrowsingListNodeIndex)FirstListNodeIndex);

                IReadOnlyList<IReadOnlyBrowsingListNodeIndex> ListNodeIndexListAsReadOnlyIReadOnlylist = ListNodeIndexList;
                Assert.That(ListNodeIndexListAsReadOnlyIReadOnlylist[0] == FirstListNodeIndex);

                IReadOnlyList<IWriteableBrowsingListNodeIndex> ListNodeIndexListAsWriteableIReadOnlylist = ListNodeIndexList;
                Assert.That(ListNodeIndexListAsWriteableIReadOnlylist[0] == FirstListNodeIndex);

                IReadOnlyBrowsingListNodeIndexList ListNodeIndexListAsReadOnly = ListNodeIndexList;
                Assert.That(ListNodeIndexListAsReadOnly[0] == FirstListNodeIndex);

                IEnumerator<IWriteableBrowsingListNodeIndex> ListNodeIndexListWriteableEnumerator = ((IWriteableBrowsingListNodeIndexList)ListNodeIndexList).GetEnumerator();

                // IFrameIndexNodeStateDictionary
                if (ControllerStateTable != null)
                {
                    foreach (KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState> Entry in ControllerStateTable)
                    {
                        IReadOnlyNodeState StateView = ControllerStateTable[Entry.Key];
                        ControllerStateTable.TryGetValue(Entry.Key, out IReadOnlyNodeState Value);
                        ControllerStateTable.Contains(Entry);
                        ControllerStateTable.Remove(Entry.Key);
                        ControllerStateTable.Add(Entry.Key, Entry.Value);
                        ICollection<IReadOnlyIndex> Keys = ControllerStateTable.Keys;
                        ICollection<IReadOnlyNodeState> Values = ControllerStateTable.Values;

                        break;
                    }

                    foreach (KeyValuePair<IWriteableIndex, IWriteableNodeState> Entry in (IWriteableIndexNodeStateDictionary)ControllerStateTable)
                    {
                        break;
                    }

                    IDictionary<IReadOnlyIndex, IReadOnlyNodeState> ReadOnlyControllerStateTableAsDictionary = ControllerStateTable;
                    foreach (KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState> Entry in ReadOnlyControllerStateTableAsDictionary)
                    {
                        IReadOnlyNodeState StateView = ReadOnlyControllerStateTableAsDictionary[Entry.Key];
                        Assert.That(ReadOnlyControllerStateTableAsDictionary.ContainsKey(Entry.Key));
                        break;
                    }

                    IDictionary<IWriteableIndex, IWriteableNodeState> WriteableControllerStateTableAsDictionary = ControllerStateTable as IDictionary<IWriteableIndex, IWriteableNodeState>;
                    foreach (KeyValuePair<IWriteableIndex, IWriteableNodeState> Entry in WriteableControllerStateTableAsDictionary)
                    {
                        IWriteableNodeState StateView = WriteableControllerStateTableAsDictionary[Entry.Key];
                        Assert.That(WriteableControllerStateTableAsDictionary.ContainsKey(Entry.Key));
                        WriteableControllerStateTableAsDictionary.Remove(Entry.Key);
                        WriteableControllerStateTableAsDictionary.Add(Entry.Key, Entry.Value);
                        Assert.That(WriteableControllerStateTableAsDictionary.Keys != null);
                        Assert.That(WriteableControllerStateTableAsDictionary.Values != null);
                        Assert.That(WriteableControllerStateTableAsDictionary.TryGetValue(Entry.Key, out IWriteableNodeState Value));
                        break;
                    }

                    ICollection<KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState>> ReadOnlyControllerStateTableAsCollection = ControllerStateTable;
                    IsReadOnly = ReadOnlyControllerStateTableAsCollection.IsReadOnly;
                    foreach (KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState> Entry in ReadOnlyControllerStateTableAsCollection)
                    {
                        Assert.That(ReadOnlyControllerStateTableAsCollection.Contains(Entry));
                        ReadOnlyControllerStateTableAsCollection.Remove(Entry);
                        ReadOnlyControllerStateTableAsCollection.Add(Entry);
                        ReadOnlyControllerStateTableAsCollection.CopyTo(new KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState>[ReadOnlyControllerStateTableAsCollection.Count], 0);
                        break;
                    }

                    ICollection<KeyValuePair<IWriteableIndex, IWriteableNodeState>> WriteableControllerStateTableAsCollection = ControllerStateTable as ICollection<KeyValuePair<IWriteableIndex, IWriteableNodeState>>;
                    IsReadOnly = WriteableControllerStateTableAsCollection.IsReadOnly;
                    foreach (KeyValuePair<IWriteableIndex, IWriteableNodeState> Entry in WriteableControllerStateTableAsCollection)
                    {
                        Assert.That(WriteableControllerStateTableAsCollection.Contains(Entry));
                        WriteableControllerStateTableAsCollection.Remove(Entry);
                        WriteableControllerStateTableAsCollection.Add(Entry);
                        WriteableControllerStateTableAsCollection.CopyTo(new KeyValuePair<IWriteableIndex, IWriteableNodeState>[WriteableControllerStateTableAsCollection.Count], 0);
                        break;
                    }

                    IEnumerable<KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState>> ReadOnlyControllerStateTableAsEnumerable = ControllerStateTable;
                    foreach (KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState> Entry in ReadOnlyControllerStateTableAsEnumerable)
                    {
                        break;
                    }

                    IEnumerable<KeyValuePair<IWriteableIndex, IWriteableNodeState>> WriteableControllerStateTableAsEnumerable = ControllerStateTable as IEnumerable<KeyValuePair<IWriteableIndex, IWriteableNodeState>>;
                    foreach (KeyValuePair<IWriteableIndex, IWriteableNodeState> Entry in WriteableControllerStateTableAsEnumerable)
                    {
                        break;
                    }
                }

                // IFrameIndexNodeStateReadOnlyDictionary

                IReadOnlyIndexNodeStateReadOnlyDictionary ReadOnlyStateTable = Controller.StateTable;
                IWriteableIndexNodeStateReadOnlyDictionary WriteableStateTable = Controller.StateTable;
                Assert.That(WriteableStateTable.ContainsKey(RootIndex));
                Assert.That(WriteableStateTable[RootIndex] == ReadOnlyStateTable[RootIndex]);
                WriteableStateTable.GetEnumerator();

                IReadOnlyDictionary<IReadOnlyIndex, IReadOnlyNodeState> ReadOnlyStateTableAsDictionary = ReadOnlyStateTable;
                Assert.That(ReadOnlyStateTable.TryGetValue(RootIndex, out IReadOnlyNodeState ReadOnlyRootStateValue) == ReadOnlyStateTableAsDictionary.TryGetValue(RootIndex, out IReadOnlyNodeState ReadOnlyRootStateValueFromDictionary) && ReadOnlyRootStateValue == ReadOnlyRootStateValueFromDictionary);
                Assert.That(ReadOnlyStateTableAsDictionary.Keys != null);
                Assert.That(ReadOnlyStateTableAsDictionary.Values != null);
                ReadOnlyStateTableAsDictionary.GetEnumerator();

                IReadOnlyDictionary<IWriteableIndex, IWriteableNodeState> WriteableStateTableAsDictionary = ReadOnlyStateTable as IReadOnlyDictionary<IWriteableIndex, IWriteableNodeState>;
                Assert.That(WriteableStateTable.TryGetValue(RootIndex, out IWriteableNodeState WriteableRootStateValue) == WriteableStateTableAsDictionary.TryGetValue(RootIndex, out IWriteableNodeState WriteableRootStateValueFromDictionary) && WriteableRootStateValue == WriteableRootStateValueFromDictionary);
                Assert.That(WriteableStateTableAsDictionary.ContainsKey(RootIndex));
                Assert.That(WriteableStateTableAsDictionary[RootIndex] == ReadOnlyStateTable[RootIndex]);
                Assert.That(WriteableStateTableAsDictionary.Keys != null);
                Assert.That(WriteableStateTableAsDictionary.Values != null);

                IEnumerable<KeyValuePair<IWriteableIndex, IWriteableNodeState>> WriteableStateTableAsEnumerable = ReadOnlyStateTable as IEnumerable<KeyValuePair<IWriteableIndex, IWriteableNodeState>>;
                WriteableStateTableAsEnumerable.GetEnumerator();

                // IFrameInnerDictionary

                IFrameInnerDictionary<string> FrameInnerTableModify = DebugObjects.GetReferenceByInterface(typeof(IFrameInnerDictionary<string>)) as IFrameInnerDictionary<string>;
                Assert.That(FrameInnerTableModify != null);
                Assert.That(FrameInnerTableModify.Count > 0);

                IWriteableInnerDictionary<string> WriteableInnerTableModify = FrameInnerTableModify;
                WriteableInnerTableModify.GetEnumerator();

                IDictionary<string, IReadOnlyInner> ReadOnlyInnerTableModifyAsDictionary = FrameInnerTableModify;
                Assert.That(ReadOnlyInnerTableModifyAsDictionary.Keys != null);
                Assert.That(ReadOnlyInnerTableModifyAsDictionary.Values != null);

                foreach (KeyValuePair<string, IFrameInner> Entry in FrameInnerTableModify)
                {
                    Assert.That(ReadOnlyInnerTableModifyAsDictionary.ContainsKey(Entry.Key));
                    Assert.That(ReadOnlyInnerTableModifyAsDictionary[Entry.Key] == Entry.Value);
                }

                IDictionary<string, IWriteableInner> WriteableInnerTableModifyAsDictionary = FrameInnerTableModify;
                Assert.That(WriteableInnerTableModifyAsDictionary.Keys != null);
                Assert.That(WriteableInnerTableModifyAsDictionary.Values != null);

                foreach (KeyValuePair<string, IFrameInner> Entry in FrameInnerTableModify)
                {
                    Assert.That(WriteableInnerTableModifyAsDictionary.ContainsKey(Entry.Key));
                    Assert.That(WriteableInnerTableModifyAsDictionary[Entry.Key] == Entry.Value);
                    WriteableInnerTableModifyAsDictionary.Remove(Entry.Key);
                    WriteableInnerTableModifyAsDictionary.Add(Entry.Key, Entry.Value);
                    WriteableInnerTableModifyAsDictionary.TryGetValue(Entry.Key, out IWriteableInner InnerValue);
                    break;
                }

                ICollection<KeyValuePair<string, IReadOnlyInner>> ReadOnlyInnerTableModifyAsCollection = FrameInnerTableModify;
                Assert.That(!ReadOnlyInnerTableModifyAsCollection.IsReadOnly);

                ICollection<KeyValuePair<string, IWriteableInner>> WriteableInnerTableModifyAsCollection = FrameInnerTableModify;
                Assert.That(!WriteableInnerTableModifyAsCollection.IsReadOnly);
                WriteableInnerTableModifyAsCollection.CopyTo(new KeyValuePair<string, IWriteableInner>[WriteableInnerTableModifyAsCollection.Count], 0);

                foreach (KeyValuePair<string, IWriteableInner> Entry in WriteableInnerTableModifyAsCollection)
                {
                    Assert.That(WriteableInnerTableModifyAsCollection.Contains(Entry));
                    WriteableInnerTableModifyAsCollection.Remove(Entry);
                    WriteableInnerTableModifyAsCollection.Add(Entry);
                    break;
                }

                IEnumerable<KeyValuePair<string, IReadOnlyInner>> ReadOnlyInnerTableModifyAsEnumerable = FrameInnerTableModify;
                IEnumerator<KeyValuePair<string, IReadOnlyInner>> ReadOnlyInnerTableModifyAsEnumerableEnumerator = ReadOnlyInnerTableModifyAsEnumerable.GetEnumerator();

                foreach (KeyValuePair<string, IReadOnlyInner> Entry in ReadOnlyInnerTableModifyAsEnumerable)
                {
                    Assert.That(ReadOnlyInnerTableModifyAsDictionary.ContainsKey(Entry.Key));
                    Assert.That(ReadOnlyInnerTableModifyAsDictionary[Entry.Key] == Entry.Value);
                    Assert.That(FrameInnerTableModify.TryGetValue(Entry.Key, out IReadOnlyInner ReadOnlyInnerValue) == FrameInnerTableModify.TryGetValue(Entry.Key, out IFrameInner FrameInnerValue));

                    Assert.That(FrameInnerTableModify.Contains(Entry));
                    FrameInnerTableModify.Remove(Entry);
                    FrameInnerTableModify.Add(Entry);
                    FrameInnerTableModify.CopyTo(new KeyValuePair<string, IReadOnlyInner>[FrameInnerTableModify.Count], 0);
                    break;
                }

                IEnumerable<KeyValuePair<string, IWriteableInner>> WriteableInnerTableModifyAsEnumerable = FrameInnerTableModify;
                IEnumerator<KeyValuePair<string, IWriteableInner>> WriteableInnerTableModifyAsEnumerableEnumerator = WriteableInnerTableModifyAsEnumerable.GetEnumerator();

                // IFrameInnerReadOnlyDictionary

                IFrameInnerReadOnlyDictionary<string> FrameInnerTable = RootState.InnerTable;
                IWriteableInnerReadOnlyDictionary<string> WriteableInnerTable = RootState.InnerTable;

                IReadOnlyDictionary<string, IReadOnlyInner> ReadOnlyInnerTableAsDictionary = FrameInnerTable;
                Assert.That(ReadOnlyInnerTableAsDictionary.Keys != null);
                Assert.That(ReadOnlyInnerTableAsDictionary.Values != null);

                IReadOnlyDictionary<string, IWriteableInner> WriteableInnerTableAsDictionary = FrameInnerTable;
                Assert.That(WriteableInnerTableAsDictionary.Keys != null);
                Assert.That(WriteableInnerTableAsDictionary.Values != null);

                IEnumerable<KeyValuePair<string, IWriteableInner>> WriteableInnerTableAsIEnumerable = FrameInnerTable;
                WriteableInnerTableAsIEnumerable.GetEnumerator();

                foreach (KeyValuePair<string, IFrameInner> Entry in FrameInnerTable)
                {
                    Assert.That(WriteableInnerTableAsDictionary[Entry.Key] == Entry.Value);
                    Assert.That(FrameInnerTable.TryGetValue(Entry.Key, out IReadOnlyInner ReadOnlyInnerValue) == FrameInnerTable.TryGetValue(Entry.Key, out IFrameInner FrameInnerValue));
                    Assert.That(FrameInnerTable.TryGetValue(Entry.Key, out IWriteableInner WriteableInnerValue) == FrameInnerTable.TryGetValue(Entry.Key, out FrameInnerValue));
                    break;
                }

                // FrameNodeStateList

                FirstNodeState = LeafPathInner.FirstNodeState;
                Assert.That(FirstNodeState != null);

                IFrameNodeStateList FrameNodeStateListModify = DebugObjects.GetReferenceByInterface(typeof(IFrameNodeStateList)) as IFrameNodeStateList;
                Assert.That(FrameNodeStateListModify != null);
                Assert.That(FrameNodeStateListModify.Count > 0);
                FirstNodeState = FrameNodeStateListModify[0] as IFramePlaceholderNodeState;
                Assert.That(FrameNodeStateListModify.Contains((IReadOnlyNodeState)FirstNodeState));
                Assert.That(FrameNodeStateListModify.IndexOf((IReadOnlyNodeState)FirstNodeState) == 0);

                FrameNodeStateListModify.Remove((IReadOnlyNodeState)FirstNodeState);
                FrameNodeStateListModify.Insert(0, (IReadOnlyNodeState)FirstNodeState);
                FrameNodeStateListModify.CopyTo((IReadOnlyNodeState[])(new IFrameNodeState[FrameNodeStateListModify.Count]), 0);

                IReadOnlyNodeStateList FrameNodeStateListModifyAsReadOnly = FrameNodeStateListModify as IReadOnlyNodeStateList;
                Assert.That(FrameNodeStateListModifyAsReadOnly != null);
                Assert.That(FrameNodeStateListModifyAsReadOnly[0] == FrameNodeStateListModify[0]);

                IWriteableNodeStateList FrameNodeStateListModifyAsWriteable = FrameNodeStateListModify as IWriteableNodeStateList;
                Assert.That(FrameNodeStateListModifyAsWriteable != null);
                Assert.That(FrameNodeStateListModifyAsWriteable[0] == FrameNodeStateListModify[0]);
                FrameNodeStateListModifyAsWriteable.GetEnumerator();

                IList<IReadOnlyNodeState> ReadOnlyNodeStateListModifyAsIList = FrameNodeStateListModify as IList<IReadOnlyNodeState>;
                Assert.That(ReadOnlyNodeStateListModifyAsIList != null);
                Assert.That(ReadOnlyNodeStateListModifyAsIList[0] == FrameNodeStateListModify[0]);

                IList<IWriteableNodeState> WriteableNodeStateListModifyAsIList = FrameNodeStateListModify as IList<IWriteableNodeState>;
                Assert.That(WriteableNodeStateListModifyAsIList != null);
                Assert.That(WriteableNodeStateListModifyAsIList[0] == FrameNodeStateListModify[0]);
                Assert.That(WriteableNodeStateListModifyAsIList.IndexOf(FirstNodeState) == 0);
                WriteableNodeStateListModifyAsIList.Remove(FirstNodeState);
                WriteableNodeStateListModifyAsIList.Insert(0, FirstNodeState);

                IReadOnlyList<IReadOnlyNodeState> ReadOnlyNodeStateListModifyAsIReadOnlyList = FrameNodeStateListModify as IReadOnlyList<IReadOnlyNodeState>;
                Assert.That(ReadOnlyNodeStateListModifyAsIReadOnlyList != null);
                Assert.That(ReadOnlyNodeStateListModifyAsIReadOnlyList[0] == FrameNodeStateListModify[0]);

                IReadOnlyList<IWriteableNodeState> WriteableNodeStateListModifyAsIReadOnlyList = FrameNodeStateListModify as IReadOnlyList<IWriteableNodeState>;
                Assert.That(WriteableNodeStateListModifyAsIReadOnlyList != null);
                Assert.That(WriteableNodeStateListModifyAsIReadOnlyList[0] == FrameNodeStateListModify[0]);

                ICollection<IReadOnlyNodeState> ReadOnlyNodeStateListModifyAsCollection = FrameNodeStateListModify as ICollection<IReadOnlyNodeState>;
                Assert.That(ReadOnlyNodeStateListModifyAsCollection != null);
                Assert.That(!ReadOnlyNodeStateListModifyAsCollection.IsReadOnly);

                ICollection<IWriteableNodeState> WriteableNodeStateListModifyAsCollection = FrameNodeStateListModify as ICollection<IWriteableNodeState>;
                Assert.That(WriteableNodeStateListModifyAsCollection != null);
                Assert.That(!WriteableNodeStateListModifyAsCollection.IsReadOnly);
                Assert.That(WriteableNodeStateListModifyAsCollection.Contains(FirstNodeState));
                WriteableNodeStateListModifyAsCollection.Remove(FirstNodeState);
                WriteableNodeStateListModifyAsCollection.Add(FirstNodeState);
                WriteableNodeStateListModifyAsCollection.Remove(FirstNodeState);
                FrameNodeStateListModify.Insert(0, FirstNodeState);
                WriteableNodeStateListModifyAsCollection.CopyTo(new IFrameNodeState[WriteableNodeStateListModifyAsCollection.Count], 0);

                IEnumerable<IReadOnlyNodeState> ReadOnlyNodeStateListModifyAsEnumerable = FrameNodeStateListModify as IEnumerable<IReadOnlyNodeState>;
                Assert.That(ReadOnlyNodeStateListModifyAsEnumerable != null);
                Assert.That(ReadOnlyNodeStateListModifyAsEnumerable.GetEnumerator() != null);

                IEnumerable<IWriteableNodeState> WriteableNodeStateListModifyAsEnumerable = FrameNodeStateListModify as IEnumerable<IWriteableNodeState>;
                Assert.That(WriteableNodeStateListModifyAsEnumerable != null);
                Assert.That(WriteableNodeStateListModifyAsEnumerable.GetEnumerator() != null);

                // FrameNodeStateReadOnlyList

                IFrameNodeStateReadOnlyList FrameNodeStateList = FrameNodeStateListModify.ToReadOnly() as IFrameNodeStateReadOnlyList;
                Assert.That(FrameNodeStateList != null);
                Assert.That(FrameNodeStateList.Count > 0);
                FirstNodeState = FrameNodeStateList[0] as IFramePlaceholderNodeState;
                Assert.That(FrameNodeStateList.Contains((IReadOnlyNodeState)FirstNodeState));
                Assert.That(FrameNodeStateList.IndexOf((IReadOnlyNodeState)FirstNodeState) == 0);

                IWriteableNodeStateReadOnlyList WriteableNodeStateList = FrameNodeStateList;
                Assert.That(WriteableNodeStateList.Contains(FirstNodeState));
                Assert.That(WriteableNodeStateList.IndexOf(FirstNodeState) == 0);
                Assert.That(WriteableNodeStateList[0] == FrameNodeStateList[0]);
                WriteableNodeStateList.GetEnumerator();

                IReadOnlyList<IReadOnlyNodeState> ReadOnlyNodeStateListAsIReadOnlyList = FrameNodeStateList as IReadOnlyList<IReadOnlyNodeState>;
                Assert.That(ReadOnlyNodeStateListAsIReadOnlyList[0] == FirstNodeState);

                IReadOnlyList<IWriteableNodeState> WriteableNodeStateListAsIReadOnlyList = FrameNodeStateList as IReadOnlyList<IWriteableNodeState>;
                Assert.That(WriteableNodeStateListAsIReadOnlyList[0] == FirstNodeState);

                IEnumerable<IReadOnlyNodeState> ReadOnlyNodeStateListAsEnumerable = FrameNodeStateList as IEnumerable<IReadOnlyNodeState>;
                Assert.That(ReadOnlyNodeStateListAsEnumerable != null);
                Assert.That(ReadOnlyNodeStateListAsEnumerable.GetEnumerator() != null);

                IEnumerable<IWriteableNodeState> WriteableNodeStateListAsEnumerable = FrameNodeStateList as IEnumerable<IWriteableNodeState>;
                Assert.That(WriteableNodeStateListAsEnumerable != null);
                Assert.That(WriteableNodeStateListAsEnumerable.GetEnumerator() != null);

                // IFrameOperationGroupList

                IFrameOperationGroupReadOnlyList FrameOperationStack = Controller.OperationStack;

                Assert.That(FrameOperationStack.Count > 0);
                IFrameOperationGroup FirstOperationGroup = FrameOperationStack[0];

                IFrameOperationGroupList FrameOperationGroupList = DebugObjects.GetReferenceByInterface(typeof(IFrameOperationGroupList)) as IFrameOperationGroupList;
                if (FrameOperationGroupList != null)
                {
                    IWriteableOperationGroupList WriteableOperationGroupList = FrameOperationGroupList;
                    Assert.That(WriteableOperationGroupList.Count > 0);
                    Assert.That(WriteableOperationGroupList[0] == FirstOperationGroup);

                    IList<IWriteableOperationGroup> WriteableOperationGroupAsIList = WriteableOperationGroupList;
                    Assert.That(WriteableOperationGroupAsIList.Count > 0);
                    Assert.That(WriteableOperationGroupAsIList[0] == FirstOperationGroup);
                    Assert.That(WriteableOperationGroupAsIList.IndexOf(FirstOperationGroup) == 0);
                    WriteableOperationGroupAsIList.Remove(FirstOperationGroup);
                    WriteableOperationGroupAsIList.Insert(0, FirstOperationGroup);

                    ICollection<IWriteableOperationGroup> WriteableOperationGroupAsICollection = WriteableOperationGroupList;
                    Assert.That(WriteableOperationGroupAsICollection.Count > 0);
                    Assert.That(!WriteableOperationGroupAsICollection.IsReadOnly);
                    Assert.That(WriteableOperationGroupAsICollection.Contains(FirstOperationGroup));
                    WriteableOperationGroupAsICollection.Remove(FirstOperationGroup);
                    WriteableOperationGroupAsICollection.Add(FirstOperationGroup);
                    WriteableOperationGroupAsICollection.Remove(FirstOperationGroup);
                    WriteableOperationGroupAsIList.Insert(0, FirstOperationGroup);
                    WriteableOperationGroupAsICollection.CopyTo(new IFrameOperationGroup[WriteableOperationGroupAsICollection.Count], 0);

                    IEnumerable<IWriteableOperationGroup> WriteableOperationGroupAsIEnumerable = WriteableOperationGroupList;
                    WriteableOperationGroupAsIEnumerable.GetEnumerator();

                    IReadOnlyList<IWriteableOperationGroup> WriteableOperationGroupAsIReadOnlyList = WriteableOperationGroupList;
                    Assert.That(WriteableOperationGroupAsIReadOnlyList.Count > 0);
                    Assert.That(WriteableOperationGroupAsIReadOnlyList[0] == FirstOperationGroup);
                }

                // IFrameOperationGroupReadOnlyList

                IWriteableOperationGroupReadOnlyList WriteableOperationStack = FrameOperationStack;
                Assert.That(WriteableOperationStack.Contains(FirstOperationGroup));
                Assert.That(WriteableOperationStack.IndexOf(FirstOperationGroup) == 0);

                IEnumerable<IWriteableOperationGroup> WriteableOperationStackAsIEnumerable = WriteableOperationStack;
                WriteableOperationStackAsIEnumerable.GetEnumerator();

                // IFrameOperationList

                IFrameOperationReadOnlyList FrameOperationReadOnlyList = FirstOperationGroup.OperationList;

                Assert.That(FrameOperationReadOnlyList.Count > 0);
                IFrameOperation FirstOperation = FrameOperationReadOnlyList[0];

                IFrameOperationList FrameOperationList = DebugObjects.GetReferenceByInterface(typeof(IFrameOperationList)) as IFrameOperationList;
                if (FrameOperationList != null)
                {
                    IWriteableOperationList WriteableOperationList = FrameOperationList;
                    Assert.That(WriteableOperationList.Count > 0);
                    Assert.That(WriteableOperationList[0] == FirstOperation);

                    IList<IWriteableOperation> WriteableOperationAsIList = WriteableOperationList;
                    Assert.That(WriteableOperationAsIList.Count > 0);
                    Assert.That(WriteableOperationAsIList[0] == FirstOperation);
                    Assert.That(WriteableOperationAsIList.IndexOf(FirstOperation) == 0);
                    WriteableOperationAsIList.Remove(FirstOperation);
                    WriteableOperationAsIList.Insert(0, FirstOperation);

                    ICollection<IWriteableOperation> WriteableOperationAsICollection = WriteableOperationList;
                    Assert.That(WriteableOperationAsICollection.Count > 0);
                    Assert.That(!WriteableOperationAsICollection.IsReadOnly);
                    Assert.That(WriteableOperationAsICollection.Contains(FirstOperation));
                    WriteableOperationAsICollection.Remove(FirstOperation);
                    WriteableOperationAsICollection.Add(FirstOperation);
                    WriteableOperationAsICollection.Remove(FirstOperation);
                    WriteableOperationAsIList.Insert(0, FirstOperation);
                    WriteableOperationAsICollection.CopyTo(new IFrameOperation[WriteableOperationAsICollection.Count], 0);

                    IEnumerable<IWriteableOperation> WriteableOperationAsIEnumerable = WriteableOperationList;
                    WriteableOperationAsIEnumerable.GetEnumerator();

                    IReadOnlyList<IWriteableOperation> WriteableOperationAsIReadOnlyList = WriteableOperationList;
                    Assert.That(WriteableOperationAsIReadOnlyList.Count > 0);
                    Assert.That(WriteableOperationAsIReadOnlyList[0] == FirstOperation);
                }

                // IFrameOperationReadOnlyList
                IWriteableOperationReadOnlyList WriteableOperationReadOnlyList = FrameOperationReadOnlyList;
                Assert.That(WriteableOperationReadOnlyList.Contains(FirstOperation));
                Assert.That(WriteableOperationReadOnlyList.IndexOf(FirstOperation) == 0);

                IEnumerable<IWriteableOperation> WriteableOperationReadOnlyListAsIEnumerable = WriteableOperationReadOnlyList;
                WriteableOperationReadOnlyListAsIEnumerable.GetEnumerator();

                // FramePlaceholderNodeStateList

                //System.Diagnostics.Debug.Assert(false);
                FirstNodeState = LeafPathInner.FirstNodeState;
                Assert.That(FirstNodeState != null);

                IFramePlaceholderNodeStateList FramePlaceholderNodeStateListModify = DebugObjects.GetReferenceByInterface(typeof(IFramePlaceholderNodeStateList)) as IFramePlaceholderNodeStateList;
                if (FramePlaceholderNodeStateListModify != null)
                {
                    Assert.That(FramePlaceholderNodeStateListModify.Count > 0);
                    FirstNodeState = FramePlaceholderNodeStateListModify[0] as IFramePlaceholderNodeState;
                    Assert.That(FramePlaceholderNodeStateListModify.Contains((IReadOnlyPlaceholderNodeState)FirstNodeState));
                    Assert.That(FramePlaceholderNodeStateListModify.IndexOf((IReadOnlyPlaceholderNodeState)FirstNodeState) == 0);

                    FramePlaceholderNodeStateListModify.Remove((IReadOnlyPlaceholderNodeState)FirstNodeState);
                    FramePlaceholderNodeStateListModify.Insert(0, (IReadOnlyPlaceholderNodeState)FirstNodeState);
                    FramePlaceholderNodeStateListModify.CopyTo((IReadOnlyPlaceholderNodeState[])(new IFramePlaceholderNodeState[FramePlaceholderNodeStateListModify.Count]), 0);

                    IReadOnlyPlaceholderNodeStateList FramePlaceholderNodeStateListModifyAsReadOnly = FramePlaceholderNodeStateListModify as IReadOnlyPlaceholderNodeStateList;
                    Assert.That(FramePlaceholderNodeStateListModifyAsReadOnly != null);
                    Assert.That(FramePlaceholderNodeStateListModifyAsReadOnly[0] == FramePlaceholderNodeStateListModify[0]);

                    IWriteablePlaceholderNodeStateList FramePlaceholderNodeStateListModifyAsWriteable = FramePlaceholderNodeStateListModify as IWriteablePlaceholderNodeStateList;
                    Assert.That(FramePlaceholderNodeStateListModifyAsWriteable != null);
                    Assert.That(FramePlaceholderNodeStateListModifyAsWriteable[0] == FramePlaceholderNodeStateListModify[0]);
                    FramePlaceholderNodeStateListModifyAsWriteable.GetEnumerator();

                    IList<IReadOnlyPlaceholderNodeState> ReadOnlyPlaceholderNodeStateListModifyAsIList = FramePlaceholderNodeStateListModify as IList<IReadOnlyPlaceholderNodeState>;
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsIList != null);
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsIList[0] == FramePlaceholderNodeStateListModify[0]);

                    IList<IWriteablePlaceholderNodeState> WriteablePlaceholderNodeStateListModifyAsIList = FramePlaceholderNodeStateListModify as IList<IWriteablePlaceholderNodeState>;
                    Assert.That(WriteablePlaceholderNodeStateListModifyAsIList != null);
                    Assert.That(WriteablePlaceholderNodeStateListModifyAsIList[0] == FramePlaceholderNodeStateListModify[0]);
                    Assert.That(WriteablePlaceholderNodeStateListModifyAsIList.IndexOf(FirstNodeState) == 0);
                    WriteablePlaceholderNodeStateListModifyAsIList.Remove(FirstNodeState);
                    WriteablePlaceholderNodeStateListModifyAsIList.Insert(0, FirstNodeState);

                    IReadOnlyList<IReadOnlyPlaceholderNodeState> ReadOnlyPlaceholderNodeStateListModifyAsIReadOnlyList = FramePlaceholderNodeStateListModify as IReadOnlyList<IReadOnlyPlaceholderNodeState>;
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsIReadOnlyList != null);
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsIReadOnlyList[0] == FramePlaceholderNodeStateListModify[0]);

                    IReadOnlyList<IWriteablePlaceholderNodeState> WriteablePlaceholderNodeStateListModifyAsIReadOnlyList = FramePlaceholderNodeStateListModify as IReadOnlyList<IWriteablePlaceholderNodeState>;
                    Assert.That(WriteablePlaceholderNodeStateListModifyAsIReadOnlyList != null);
                    Assert.That(WriteablePlaceholderNodeStateListModifyAsIReadOnlyList[0] == FramePlaceholderNodeStateListModify[0]);

                    ICollection<IReadOnlyPlaceholderNodeState> ReadOnlyPlaceholderNodeStateListModifyAsCollection = FramePlaceholderNodeStateListModify as ICollection<IReadOnlyPlaceholderNodeState>;
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsCollection != null);
                    Assert.That(!ReadOnlyPlaceholderNodeStateListModifyAsCollection.IsReadOnly);
                    ReadOnlyPlaceholderNodeStateListModifyAsCollection.Remove(FirstNodeState);
                    ReadOnlyPlaceholderNodeStateListModifyAsCollection.Add(FirstNodeState);
                    ReadOnlyPlaceholderNodeStateListModifyAsCollection.Remove(FirstNodeState);
                    WriteablePlaceholderNodeStateListModifyAsIList.Insert(0, FirstNodeState);

                    ICollection<IWriteablePlaceholderNodeState> WriteablePlaceholderNodeStateListModifyAsCollection = FramePlaceholderNodeStateListModify as ICollection<IWriteablePlaceholderNodeState>;
                    Assert.That(WriteablePlaceholderNodeStateListModifyAsCollection != null);
                    Assert.That(!WriteablePlaceholderNodeStateListModifyAsCollection.IsReadOnly);
                    Assert.That(WriteablePlaceholderNodeStateListModifyAsCollection.Contains(FirstNodeState));
                    WriteablePlaceholderNodeStateListModifyAsCollection.Remove(FirstNodeState);
                    WriteablePlaceholderNodeStateListModifyAsCollection.Add(FirstNodeState);
                    WriteablePlaceholderNodeStateListModifyAsCollection.Remove(FirstNodeState);
                    FramePlaceholderNodeStateListModify.Insert(0, FirstNodeState);
                    WriteablePlaceholderNodeStateListModifyAsCollection.CopyTo(new IFramePlaceholderNodeState[WriteablePlaceholderNodeStateListModifyAsCollection.Count], 0);

                    IEnumerable<IReadOnlyPlaceholderNodeState> ReadOnlyPlaceholderNodeStateListModifyAsEnumerable = FramePlaceholderNodeStateListModify as IEnumerable<IReadOnlyPlaceholderNodeState>;
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsEnumerable != null);
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsEnumerable.GetEnumerator() != null);

                    IEnumerable<IWriteablePlaceholderNodeState> WriteablePlaceholderNodeStateListModifyAsEnumerable = FramePlaceholderNodeStateListModify as IEnumerable<IWriteablePlaceholderNodeState>;
                    Assert.That(WriteablePlaceholderNodeStateListModifyAsEnumerable != null);
                    Assert.That(WriteablePlaceholderNodeStateListModifyAsEnumerable.GetEnumerator() != null);
                }

                // FramePlaceholderNodeStateReadOnlyList

                IFramePlaceholderNodeStateReadOnlyList FramePlaceholderNodeStateList = FramePlaceholderNodeStateListModify != null ? FramePlaceholderNodeStateListModify.ToReadOnly() as IFramePlaceholderNodeStateReadOnlyList : null;

                if (FramePlaceholderNodeStateList != null)
                {
                    Assert.That(FramePlaceholderNodeStateList.Count > 0);
                    FirstNodeState = FramePlaceholderNodeStateList[0] as IFramePlaceholderNodeState;
                    Assert.That(FramePlaceholderNodeStateList.Contains((IReadOnlyPlaceholderNodeState)FirstNodeState));
                    Assert.That(FramePlaceholderNodeStateList.IndexOf((IReadOnlyPlaceholderNodeState)FirstNodeState) == 0);

                    IWriteablePlaceholderNodeStateReadOnlyList WriteablePlaceholderNodeStateList = FramePlaceholderNodeStateList;
                    Assert.That(WriteablePlaceholderNodeStateList.Contains(FirstNodeState));
                    Assert.That(WriteablePlaceholderNodeStateList.IndexOf(FirstNodeState) == 0);
                    Assert.That(WriteablePlaceholderNodeStateList[0] == FramePlaceholderNodeStateList[0]);
                    WriteablePlaceholderNodeStateList.GetEnumerator();

                    IReadOnlyList<IReadOnlyPlaceholderNodeState> ReadOnlyPlaceholderNodeStateListAsIReadOnlyList = FramePlaceholderNodeStateList as IReadOnlyList<IReadOnlyPlaceholderNodeState>;
                    Assert.That(ReadOnlyPlaceholderNodeStateListAsIReadOnlyList[0] == FirstNodeState);

                    IReadOnlyList<IWriteablePlaceholderNodeState> WriteablePlaceholderNodeStateListAsIReadOnlyList = FramePlaceholderNodeStateList as IReadOnlyList<IWriteablePlaceholderNodeState>;
                    Assert.That(WriteablePlaceholderNodeStateListAsIReadOnlyList[0] == FirstNodeState);

                    IEnumerable<IReadOnlyPlaceholderNodeState> ReadOnlyPlaceholderNodeStateListAsEnumerable = FramePlaceholderNodeStateList as IEnumerable<IReadOnlyPlaceholderNodeState>;
                    Assert.That(ReadOnlyPlaceholderNodeStateListAsEnumerable != null);
                    Assert.That(ReadOnlyPlaceholderNodeStateListAsEnumerable.GetEnumerator() != null);

                    IEnumerable<IWriteablePlaceholderNodeState> WriteablePlaceholderNodeStateListAsEnumerable = FramePlaceholderNodeStateList as IEnumerable<IWriteablePlaceholderNodeState>;
                    Assert.That(WriteablePlaceholderNodeStateListAsEnumerable != null);
                    Assert.That(WriteablePlaceholderNodeStateListAsEnumerable.GetEnumerator() != null);
                }

                // IFrameStateViewDictionary
                IFrameStateViewDictionary FrameStateViewTable = ControllerView.StateViewTable;
                IWriteableStateViewDictionary WriteableStateViewTable = ControllerView.StateViewTable;
                WriteableStateViewTable.GetEnumerator();

                IDictionary<IReadOnlyNodeState, IReadOnlyNodeStateView> ReadOnlyStateViewTableAsDictionary = FrameStateViewTable;
                Assert.That(ReadOnlyStateViewTableAsDictionary != null);
                Assert.That(ReadOnlyStateViewTableAsDictionary.TryGetValue(RootState, out IReadOnlyNodeStateView StateViewTableAsDictionaryValue) == FrameStateViewTable.TryGetValue(RootState, out IReadOnlyNodeStateView StateViewTableValue));
                Assert.That(ReadOnlyStateViewTableAsDictionary.Keys != null);
                Assert.That(ReadOnlyStateViewTableAsDictionary.Values != null);

                ICollection<KeyValuePair<IReadOnlyNodeState, IReadOnlyNodeStateView>> ReadOnlyStateViewTableAsCollection = FrameStateViewTable;
                Assert.That(!ReadOnlyStateViewTableAsCollection.IsReadOnly);

                ICollection<KeyValuePair<IWriteableNodeState, IWriteableNodeStateView>> WriteableStateViewTableAsCollection = FrameStateViewTable;
                Assert.That(!WriteableStateViewTableAsCollection.IsReadOnly);

                IDictionary<IWriteableNodeState, IWriteableNodeStateView> WriteableStateViewTableAsDictionary = FrameStateViewTable;
                Assert.That(WriteableStateViewTableAsDictionary != null);
                Assert.That(WriteableStateViewTableAsDictionary.TryGetValue(RootState, out IWriteableNodeStateView WriteableStateViewTableAsDictionaryValue) == FrameStateViewTable.TryGetValue(RootState, out StateViewTableValue));
                Assert.That(WriteableStateViewTableAsDictionary.Keys != null);
                Assert.That(WriteableStateViewTableAsDictionary.Values != null);

                foreach (KeyValuePair<IReadOnlyNodeState, IReadOnlyNodeStateView> Entry in ReadOnlyStateViewTableAsCollection)
                {
                    Assert.That(ReadOnlyStateViewTableAsCollection.Contains(Entry));
                    ReadOnlyStateViewTableAsCollection.Remove(Entry);
                    ReadOnlyStateViewTableAsCollection.Add(Entry);
                    ReadOnlyStateViewTableAsCollection.CopyTo(new KeyValuePair<IReadOnlyNodeState, IReadOnlyNodeStateView>[FrameStateViewTable.Count], 0);

                    Assert.That(WriteableStateViewTableAsDictionary.ContainsKey((IWriteableNodeState)Entry.Key));
                    WriteableStateViewTableAsDictionary.Remove((IWriteableNodeState)Entry.Key);
                    WriteableStateViewTableAsDictionary.Add((IWriteableNodeState)Entry.Key, (IWriteableNodeStateView)Entry.Value);

                    break;
                }

                foreach (KeyValuePair<IWriteableNodeState, IWriteableNodeStateView> Entry in WriteableStateViewTableAsCollection)
                {
                    Assert.That(WriteableStateViewTableAsDictionary.ContainsKey(Entry.Key));
                    Assert.That(WriteableStateViewTableAsDictionary[Entry.Key] == Entry.Value);
                    WriteableStateViewTableAsDictionary.Remove(Entry.Key);
                    WriteableStateViewTableAsDictionary.Add(Entry.Key, Entry.Value);

                    Assert.That(WriteableStateViewTableAsCollection.Contains(Entry));
                    WriteableStateViewTableAsCollection.Remove(Entry);
                    WriteableStateViewTableAsCollection.Add(Entry);
                    WriteableStateViewTableAsCollection.CopyTo(new KeyValuePair<IWriteableNodeState, IWriteableNodeStateView>[FrameStateViewTable.Count], 0);

                    break;
                }

                IEnumerable<KeyValuePair<IWriteableNodeState, IWriteableNodeStateView>> WriteableStateViewTableAsEnumerable = FrameStateViewTable;
                WriteableStateViewTableAsEnumerable.GetEnumerator();

            }
        }
        #endregion

        #region Focus
        [Test]
        [Category("Coverage")]
        public static void FocusCreation()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;
            IFocusController Controller;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

            try
            {
                RootIndex = new FocusRootNodeIndex(RootNode);
                Controller = FocusController.Create(RootIndex);
            }
            catch (Exception e)
            {
                Assert.Fail($"#0: {e}");
            }

            RootNode = CreateRoot(ValueGuid0, Imperfections.BadGuid);
            Assert.That(!BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

            try
            {
                RootIndex = new FocusRootNodeIndex(RootNode);
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
        public static void FocusProperties()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex0;
            IFocusRootNodeIndex RootIndex1;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

            RootIndex0 = new FocusRootNodeIndex(RootNode);
            Assert.That(RootIndex0.Node == RootNode);
            Assert.That(RootIndex0.IsEqual(CompareEqual.New(), RootIndex0));

            RootIndex1 = new FocusRootNodeIndex(RootNode);
            Assert.That(RootIndex1.Node == RootNode);
            Assert.That(CompareEqual.CoverIsEqual(RootIndex0, RootIndex1));

            IFocusController Controller0 = FocusController.Create(RootIndex0);
            Assert.That(Controller0.RootIndex == RootIndex0);

            Stats Stats = Controller0.Stats;
            Assert.That(Stats.NodeCount >= 0);
            Assert.That(Stats.PlaceholderNodeCount >= 0);
            Assert.That(Stats.OptionalNodeCount >= 0);
            Assert.That(Stats.AssignedOptionalNodeCount >= 0);
            Assert.That(Stats.ListCount >= 0);
            Assert.That(Stats.BlockListCount >= 0);
            Assert.That(Stats.BlockCount >= 0);

            IFocusPlaceholderNodeState RootState = Controller0.RootState;
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

            IFocusPlaceholderInner MainPlaceholderTreeInner = RootState.PropertyToInner(nameof(IMain.PlaceholderTree)) as IFocusPlaceholderInner;
            Assert.That(MainPlaceholderTreeInner != null);
            Assert.That(MainPlaceholderTreeInner.InterfaceType == typeof(ITree));
            Assert.That(MainPlaceholderTreeInner.ChildState != null);
            Assert.That(MainPlaceholderTreeInner.ChildState.ParentInner == MainPlaceholderTreeInner);

            IFocusPlaceholderInner MainPlaceholderLeafInner = RootState.PropertyToInner(nameof(IMain.PlaceholderLeaf)) as IFocusPlaceholderInner;
            Assert.That(MainPlaceholderLeafInner != null);
            Assert.That(MainPlaceholderLeafInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainPlaceholderLeafInner.ChildState != null);
            Assert.That(MainPlaceholderLeafInner.ChildState.ParentInner == MainPlaceholderLeafInner);

            IFocusOptionalInner MainUnassignedOptionalInner = RootState.PropertyToInner(nameof(IMain.UnassignedOptionalLeaf)) as IFocusOptionalInner;
            Assert.That(MainUnassignedOptionalInner != null);
            Assert.That(MainUnassignedOptionalInner.InterfaceType == typeof(ILeaf));
            Assert.That(!MainUnassignedOptionalInner.IsAssigned);
            Assert.That(MainUnassignedOptionalInner.ChildState != null);
            Assert.That(MainUnassignedOptionalInner.ChildState.ParentInner == MainUnassignedOptionalInner);

            IFocusOptionalInner MainAssignedOptionalTreeInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalTree)) as IFocusOptionalInner;
            Assert.That(MainAssignedOptionalTreeInner != null);
            Assert.That(MainAssignedOptionalTreeInner.InterfaceType == typeof(ITree));
            Assert.That(MainAssignedOptionalTreeInner.IsAssigned);

            IFocusNodeState AssignedOptionalTreeState = MainAssignedOptionalTreeInner.ChildState;
            Assert.That(AssignedOptionalTreeState != null);
            Assert.That(AssignedOptionalTreeState.ParentInner == MainAssignedOptionalTreeInner);
            Assert.That(AssignedOptionalTreeState.ParentState == RootState);

            IFocusNodeStateReadOnlyList AssignedOptionalTreeAllChildren = AssignedOptionalTreeState.GetAllChildren() as IFocusNodeStateReadOnlyList;
            Assert.That(AssignedOptionalTreeAllChildren != null);
            Assert.That(AssignedOptionalTreeAllChildren.Count == 2, $"New count: {AssignedOptionalTreeAllChildren.Count}");

            IFocusOptionalInner MainAssignedOptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IFocusOptionalInner;
            Assert.That(MainAssignedOptionalLeafInner != null);
            Assert.That(MainAssignedOptionalLeafInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainAssignedOptionalLeafInner.IsAssigned);
            Assert.That(MainAssignedOptionalLeafInner.ChildState != null);
            Assert.That(MainAssignedOptionalLeafInner.ChildState.ParentInner == MainAssignedOptionalLeafInner);

            IFocusBlockListInner MainLeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFocusBlockListInner;
            Assert.That(MainLeafBlocksInner != null);
            Assert.That(!MainLeafBlocksInner.IsNeverEmpty);
            Assert.That(!MainLeafBlocksInner.IsEmpty);
            Assert.That(!MainLeafBlocksInner.IsSingle);
            Assert.That(MainLeafBlocksInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainLeafBlocksInner.BlockType == typeof(BaseNode.IBlock<ILeaf, Leaf>));
            Assert.That(MainLeafBlocksInner.ItemType == typeof(Leaf));
            Assert.That(MainLeafBlocksInner.Count == 4);
            Assert.That(MainLeafBlocksInner.BlockStateList != null);
            Assert.That(MainLeafBlocksInner.BlockStateList.Count == 3);
            Assert.That(MainLeafBlocksInner.AllIndexes().Count == MainLeafBlocksInner.Count);

            IFocusBlockState LeafBlock = MainLeafBlocksInner.BlockStateList[0];
            Assert.That(LeafBlock != null);
            Assert.That(LeafBlock.StateList != null);
            Assert.That(LeafBlock.StateList.Count == 1);
            Assert.That(MainLeafBlocksInner.FirstNodeState == LeafBlock.StateList[0]);
            Assert.That(MainLeafBlocksInner.IndexAt(0, 0) == MainLeafBlocksInner.FirstNodeState.ParentIndex);

            IFocusPlaceholderInner PatternInner = LeafBlock.PropertyToInner(nameof(BaseNode.IBlock.ReplicationPattern)) as IFocusPlaceholderInner;
            Assert.That(PatternInner != null);

            IFocusPlaceholderInner SourceInner = LeafBlock.PropertyToInner(nameof(BaseNode.IBlock.SourceIdentifier)) as IFocusPlaceholderInner;
            Assert.That(SourceInner != null);

            IFocusPatternState PatternState = LeafBlock.PatternState;
            Assert.That(PatternState != null);
            Assert.That(PatternState.ParentBlockState == LeafBlock);
            Assert.That(PatternState.ParentInner == PatternInner);
            Assert.That(PatternState.ParentIndex == LeafBlock.PatternIndex);
            Assert.That(PatternState.ParentState == RootState);
            Assert.That(PatternState.InnerTable.Count == 0);
            Assert.That(PatternState is IFocusNodeState AsPlaceholderPatternNodeState && AsPlaceholderPatternNodeState.ParentIndex == LeafBlock.PatternIndex);
            Assert.That(PatternState.GetAllChildren().Count == 1);

            IFocusSourceState SourceState = LeafBlock.SourceState;
            Assert.That(SourceState != null);
            Assert.That(SourceState.ParentBlockState == LeafBlock);
            Assert.That(SourceState.ParentInner == SourceInner);
            Assert.That(SourceState.ParentIndex == LeafBlock.SourceIndex);
            Assert.That(SourceState.ParentState == RootState);
            Assert.That(SourceState.InnerTable.Count == 0);
            Assert.That(SourceState is IFocusNodeState AsPlaceholderSourceNodeState && AsPlaceholderSourceNodeState.ParentIndex == LeafBlock.SourceIndex);
            Assert.That(SourceState.GetAllChildren().Count == 1);

            Assert.That(MainLeafBlocksInner.FirstNodeState == LeafBlock.StateList[0]);

            IFocusListInner MainLeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFocusListInner;
            Assert.That(MainLeafPathInner != null);
            Assert.That(!MainLeafPathInner.IsNeverEmpty);
            Assert.That(MainLeafPathInner.InterfaceType == typeof(ILeaf));
            Assert.That(MainLeafPathInner.Count == 2);
            Assert.That(MainLeafPathInner.StateList != null);
            Assert.That(MainLeafPathInner.StateList.Count == 2);
            Assert.That(MainLeafPathInner.FirstNodeState == MainLeafPathInner.StateList[0]);
            Assert.That(MainLeafPathInner.IndexAt(0) == MainLeafPathInner.FirstNodeState.ParentIndex);
            Assert.That(MainLeafPathInner.AllIndexes().Count == MainLeafPathInner.Count);

            IFocusNodeStateReadOnlyList AllChildren = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
            Assert.That(AllChildren.Count == 19, $"New count: {AllChildren.Count}");

            IFocusPlaceholderInner PlaceholderInner = RootState.InnerTable[nameof(IMain.PlaceholderLeaf)] as IFocusPlaceholderInner;
            Assert.That(PlaceholderInner != null);

            IFocusBrowsingPlaceholderNodeIndex PlaceholderNodeIndex = PlaceholderInner.ChildState.ParentIndex as IFocusBrowsingPlaceholderNodeIndex;
            Assert.That(PlaceholderNodeIndex != null);
            Assert.That(Controller0.Contains(PlaceholderNodeIndex));

            IFocusOptionalInner UnassignedOptionalInner = RootState.InnerTable[nameof(IMain.UnassignedOptionalLeaf)] as IFocusOptionalInner;
            Assert.That(UnassignedOptionalInner != null);

            IFocusBrowsingOptionalNodeIndex UnassignedOptionalNodeIndex = UnassignedOptionalInner.ChildState.ParentIndex;
            Assert.That(UnassignedOptionalNodeIndex != null);
            Assert.That(Controller0.Contains(UnassignedOptionalNodeIndex));
            Assert.That(Controller0.IsAssigned(UnassignedOptionalNodeIndex) == false);

            IFocusOptionalInner AssignedOptionalInner = RootState.InnerTable[nameof(IMain.AssignedOptionalLeaf)] as IFocusOptionalInner;
            Assert.That(AssignedOptionalInner != null);

            IFocusBrowsingOptionalNodeIndex AssignedOptionalNodeIndex = AssignedOptionalInner.ChildState.ParentIndex;
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
            Assert.That(ReadAsGuid == ValueGuid0);
            Assert.That(Controller0.GetGuidValue(RootIndex0, nameof(IMain.ValueGuid)) == ReadAsGuid);

            IFocusController Controller1 = FocusController.Create(RootIndex0);
            Assert.That(Controller0.IsEqual(CompareEqual.New(), Controller0));

            //System.Diagnostics.Debug.Assert(false);
            Assert.That(CompareEqual.CoverIsEqual(Controller0, Controller1));

            Assert.That(!Controller0.CanUndo);
            Assert.That(!Controller0.CanRedo);
        }

        [Test]
        [Category("Coverage")]
        public static void FocusClone()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode = CreateRoot(ValueGuid0, Imperfections.None);

            IFocusRootNodeIndex RootIndex = new FocusRootNodeIndex(RootNode);
            Assert.That(RootIndex != null);

            IFocusController Controller = FocusController.Create(RootIndex);
            Assert.That(Controller != null);

            IFocusPlaceholderNodeState RootState = Controller.RootState;
            Assert.That(RootState != null);

            BaseNode.INode ClonedNode = RootState.CloneNode();
            Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(ClonedNode));

            IFocusRootNodeIndex CloneRootIndex = new FocusRootNodeIndex(ClonedNode);
            Assert.That(CloneRootIndex != null);

            IFocusController CloneController = FocusController.Create(CloneRootIndex);
            Assert.That(CloneController != null);

            IFocusPlaceholderNodeState CloneRootState = Controller.RootState;
            Assert.That(CloneRootState != null);

            IFocusNodeStateReadOnlyList AllChildren = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
            IFocusNodeStateReadOnlyList CloneAllChildren = (IFocusNodeStateReadOnlyList)CloneRootState.GetAllChildren();
            Assert.That(AllChildren.Count == CloneAllChildren.Count);
        }

        [Test]
        [Category("Coverage")]
        public static void FocusViews()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController Controller = FocusController.Create(RootIndex);
            IFocusTemplateSet DefaultTemplateSet = FocusTemplateSet.Default;
            DefaultTemplateSet = FocusTemplateSet.Default;

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);
                Assert.That(ControllerView0.TemplateSet == TestDebug.CoverageFocusTemplateSet.FocusTemplateSet);
                Assert.That(ControllerView0.CaretMode == EaslyController.Constants.CaretModes.Insertion);

                ControllerView0.SetCaretMode(EaslyController.Constants.CaretModes.Override);
                Assert.That(ControllerView0.CaretMode == EaslyController.Constants.CaretModes.Override);

                using (IFocusControllerView ControllerView1 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
                {
                    Assert.That(ControllerView0.IsEqual(CompareEqual.New(), ControllerView0));
                    Assert.That(CompareEqual.CoverIsEqual(ControllerView0, ControllerView1));
                }

                foreach (KeyValuePair<IFocusBlockState, IFocusBlockStateView> Entry in ControllerView0.BlockStateViewTable)
                {
                    IFocusBlockState BlockState = Entry.Key;
                    Assert.That(BlockState != null);

                    IFocusBlockStateView BlockStateView = Entry.Value;
                    Assert.That(BlockStateView != null);
                    Assert.That(BlockStateView.BlockState == BlockState);

                    Assert.That(BlockStateView.ControllerView == ControllerView0);
                }

                foreach (KeyValuePair<IFocusNodeState, IFocusNodeStateView> Entry in ControllerView0.StateViewTable)
                {
                    IFocusNodeState State = Entry.Key;
                    Assert.That(State != null);

                    IFocusNodeStateView StateView = Entry.Value;
                    Assert.That(StateView != null);
                    Assert.That(StateView.State == State);

                    IFocusIndex ParentIndex = State.ParentIndex;
                    Assert.That(ParentIndex != null);

                    Assert.That(Controller.Contains(ParentIndex));
                    Assert.That(StateView.ControllerView == ControllerView0);

                    switch (StateView)
                    {
                        case IFocusPatternStateView AsPatternStateView:
                            Assert.That(AsPatternStateView.State == State);
                            Assert.That(AsPatternStateView is IFocusNodeStateView AsPlaceholderPatternNodeStateView && AsPlaceholderPatternNodeStateView.State == State);
                            break;

                        case IFocusSourceStateView AsSourceStateView:
                            Assert.That(AsSourceStateView.State == State);
                            Assert.That(AsSourceStateView is IFocusNodeStateView AsPlaceholderSourceNodeStateView && AsPlaceholderSourceNodeStateView.State == State);
                            break;

                        case IFocusPlaceholderNodeStateView AsPlaceholderNodeStateView:
                            Assert.That(AsPlaceholderNodeStateView.State == State);
                            break;

                        case IFocusOptionalNodeStateView AsOptionalNodeStateView:
                            Assert.That(AsOptionalNodeStateView.State == State);
                            break;
                    }
                }

                IFocusVisibleCellViewList VisibleCellViewList = new FocusVisibleCellViewList();
                ControllerView0.EnumerateVisibleCellViews(VisibleCellViewList);
                ControllerView0.PrintCellViewTree(true);

                Assert.That(ControllerView0.MinFocusMove == 0);

                ControllerView0.MoveFocus(-1);
                Assert.That(ControllerView0.MinFocusMove == 0);

                Assert.That(ControllerView0.MaxFocusMove > 0);
                Assert.That(ControllerView0.FocusedText != null);
                Assert.That(!ControllerView0.SetCaretPosition(0));

                ControllerView0.MoveFocus(+1);
                Assert.That(ControllerView0.FocusedText == null);
                Assert.That(!ControllerView0.IsUserVisible);
                Assert.That(!ControllerView0.SetCaretPosition(0));

                while (ControllerView0.MaxFocusMove > 0)
                    ControllerView0.MoveFocus(+1);

                Assert.That(ControllerView0.MaxFocusMove == 0);
                ControllerView0.MoveFocus(+1);
                Assert.That(ControllerView0.MaxFocusMove == 0);

                ControllerView0.MoveFocus(ControllerView0.MinFocusMove);
                Assert.That(ControllerView0.MinFocusMove == 0);

                while (ControllerView0.MaxFocusMove > 0)
                {
                    IFocusInner Inner;
                    IFocusInsertionChildIndex InsertionIndex;
                    IFocusCollectionInner CollectionInner;
                    IFocusBlockListInner BlockListInner;
                    IFocusListInner ListInner;
                    IFocusInsertionCollectionNodeIndex InsertionCollectionIndex;
                    IFocusBrowsingCollectionNodeIndex BrowsingCollectionIndex;
                    IFocusBrowsingExistingBlockNodeIndex ExistingBlockNodeIndex;
                    IFocusInsertionListNodeIndex ReplacementListNodeIndex, InsertionListNodeIndex;
                    int BlockIndex;
                    BaseNode.ReplicationStatus Replication;

                    bool IsNewItemInsertable = ControllerView0.IsNewItemInsertable(out CollectionInner, out InsertionCollectionIndex);
                    bool IsItemRemoveable = ControllerView0.IsItemRemoveable(out CollectionInner, out BrowsingCollectionIndex);
                    bool IsItemMoveable = ControllerView0.IsItemMoveable(-1, out CollectionInner, out BrowsingCollectionIndex);
                    bool IsItemSplittable = ControllerView0.IsItemSplittable(out BlockListInner, out ExistingBlockNodeIndex);
                    bool IsReplicationModifiable = ControllerView0.IsReplicationModifiable(out BlockListInner, out BlockIndex, out Replication);
                    bool IsItemMergeable = ControllerView0.IsItemMergeable(out BlockListInner, out ExistingBlockNodeIndex);
                    bool IsBlockMoveable = ControllerView0.IsBlockMoveable(-1, out BlockListInner, out BlockIndex);
                    bool IsItemSimplifiable = ControllerView0.IsItemSimplifiable(out Inner, out InsertionIndex);
                    bool IsIdentifierSplittable = ControllerView0.IsIdentifierSplittable(out ListInner, out ReplacementListNodeIndex, out InsertionListNodeIndex);

                    ControllerView0.MoveFocus(+1);
                }
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusInsert()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFocusListInner;
                Assert.That(LeafPathInner != null);

                int PathCount = LeafPathInner.Count;
                Assert.That(PathCount == 2);

                IFocusBrowsingListNodeIndex ExistingIndex = LeafPathInner.IndexAt(0) as IFocusBrowsingListNodeIndex;

                Leaf NewItem0 = CreateLeaf(Guid.NewGuid());

                IFocusInsertionListNodeIndex InsertionIndex0;
                InsertionIndex0 = ExistingIndex.ToInsertionIndex(RootNode, NewItem0) as IFocusInsertionListNodeIndex;
                Assert.That(InsertionIndex0.ParentNode == RootNode);
                Assert.That(InsertionIndex0.Node == NewItem0);
                Assert.That(CompareEqual.CoverIsEqual(InsertionIndex0, InsertionIndex0));

                IFocusNodeStateReadOnlyList AllChildren0 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Controller.Insert(LeafPathInner, InsertionIndex0, out IWriteableBrowsingCollectionNodeIndex NewItemIndex0);
                Assert.That(Controller.Contains(NewItemIndex0));

                IFocusBrowsingListNodeIndex DuplicateExistingIndex0 = InsertionIndex0.ToBrowsingIndex() as IFocusBrowsingListNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex0 as IFocusBrowsingListNodeIndex, DuplicateExistingIndex0));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex0, NewItemIndex0 as IFocusBrowsingListNodeIndex));

                Assert.That(LeafPathInner.Count == PathCount + 1);
                Assert.That(LeafPathInner.StateList.Count == PathCount + 1);

                IFocusPlaceholderNodeState NewItemState0 = LeafPathInner.StateList[0];
                Assert.That(NewItemState0.Node == NewItem0);
                Assert.That(NewItemState0.ParentIndex == NewItemIndex0);

                IFocusNodeStateReadOnlyList AllChildren1 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IFocusBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFocusBlockListInner;
                Assert.That(LeafBlocksInner != null);

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[0].StateList.Count;
                Assert.That(BlockNodeCount == 4);

                IFocusBrowsingExistingBlockNodeIndex ExistingIndex1 = LeafBlocksInner.IndexAt(0, 0) as IFocusBrowsingExistingBlockNodeIndex;

                Leaf NewItem1 = CreateLeaf(Guid.NewGuid());
                IFocusInsertionExistingBlockNodeIndex InsertionIndex1;
                InsertionIndex1 = ExistingIndex1.ToInsertionIndex(RootNode, NewItem1) as IFocusInsertionExistingBlockNodeIndex;
                Assert.That(InsertionIndex1.ParentNode == RootNode);
                Assert.That(InsertionIndex1.Node == NewItem1);
                Assert.That(CompareEqual.CoverIsEqual(InsertionIndex1, InsertionIndex1));

                Controller.Insert(LeafBlocksInner, InsertionIndex1, out IWriteableBrowsingCollectionNodeIndex NewItemIndex1);
                Assert.That(Controller.Contains(NewItemIndex1));

                IFocusBrowsingExistingBlockNodeIndex DuplicateExistingIndex1 = InsertionIndex1.ToBrowsingIndex() as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex1 as IFocusBrowsingExistingBlockNodeIndex, DuplicateExistingIndex1));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex1, NewItemIndex1 as IFocusBrowsingExistingBlockNodeIndex));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount + 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == NodeCount + 1);

                IFocusPlaceholderNodeState NewItemState1 = LeafBlocksInner.BlockStateList[0].StateList[0];
                Assert.That(NewItemState1.Node == NewItem1);
                Assert.That(NewItemState1.ParentIndex == NewItemIndex1);
                Assert.That(NewItemState1.ParentState == RootState);

                IFocusNodeStateReadOnlyList AllChildren2 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count + 1, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));




                Leaf NewItem2 = CreateLeaf(Guid.NewGuid());
                BaseNode.IPattern NewPattern = BaseNodeHelper.NodeHelper.CreateSimplePattern("");
                BaseNode.IIdentifier NewSource = BaseNodeHelper.NodeHelper.CreateSimpleIdentifier("");

                IFocusInsertionNewBlockNodeIndex InsertionIndex2 = new FocusInsertionNewBlockNodeIndex(RootNode, nameof(IMain.LeafBlocks), NewItem2, 0, NewPattern, NewSource);
                Assert.That(CompareEqual.CoverIsEqual(InsertionIndex2, InsertionIndex2));

                int BlockCount = LeafBlocksInner.BlockStateList.Count;
                Assert.That(BlockCount == 3);

                Controller.Insert(LeafBlocksInner, InsertionIndex2, out IWriteableBrowsingCollectionNodeIndex NewItemIndex2);
                Assert.That(Controller.Contains(NewItemIndex2));

                IFocusBrowsingExistingBlockNodeIndex DuplicateExistingIndex2 = InsertionIndex2.ToBrowsingIndex() as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex2 as IFocusBrowsingExistingBlockNodeIndex, DuplicateExistingIndex2));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex2, NewItemIndex2 as IFocusBrowsingExistingBlockNodeIndex));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount + 2);
                Assert.That(LeafBlocksInner.BlockStateList.Count == BlockCount + 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == 1, $"Count: {LeafBlocksInner.BlockStateList[0].StateList.Count}");
                Assert.That(LeafBlocksInner.BlockStateList[1].StateList.Count == 2, $"Count: {LeafBlocksInner.BlockStateList[1].StateList.Count}");
                Assert.That(LeafBlocksInner.BlockStateList[2].StateList.Count == 2, $"Count: {LeafBlocksInner.BlockStateList[2].StateList.Count}");

                IFocusPlaceholderNodeState NewItemState2 = LeafBlocksInner.BlockStateList[0].StateList[0];
                Assert.That(NewItemState2.Node == NewItem2);
                Assert.That(NewItemState2.ParentIndex == NewItemIndex2);

                IFocusNodeStateReadOnlyList AllChildren3 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count + 3, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusRemove()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFocusListInner;
                Assert.That(LeafPathInner != null);

                IFocusBrowsingListNodeIndex RemovedLeafIndex0 = LeafPathInner.StateList[0].ParentIndex as IFocusBrowsingListNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex0));

                int PathCount = LeafPathInner.Count;
                Assert.That(PathCount == 2);

                IFocusNodeStateReadOnlyList AllChildren0 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedLeafIndex0));

                Controller.Remove(LeafPathInner, RemovedLeafIndex0);
                Assert.That(!Controller.Contains(RemovedLeafIndex0));

                Assert.That(LeafPathInner.Count == PathCount - 1);
                Assert.That(LeafPathInner.StateList.Count == PathCount - 1);

                IFocusNodeStateReadOnlyList AllChildren1 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count - 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                RemovedLeafIndex0 = LeafPathInner.StateList[0].ParentIndex as IFocusBrowsingListNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex0));

                Assert.That(LeafPathInner.Count == 1);

                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedLeafIndex0));

                IDictionary<Type, string[]> NeverEmptyCollectionTable = BaseNodeHelper.NodeHelper.NeverEmptyCollectionTable as IDictionary<Type, string[]>;
                NeverEmptyCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafPath) });
                Assert.That(!Controller.IsRemoveable(LeafPathInner, RemovedLeafIndex0));



                IFocusBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFocusBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFocusBrowsingExistingBlockNodeIndex RemovedLeafIndex1 = LeafBlocksInner.BlockStateList[1].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex1));

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[1].StateList.Count;
                Assert.That(BlockNodeCount == 4, $"New count: {BlockNodeCount}");

                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex1));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex1);
                Assert.That(!Controller.Contains(RemovedLeafIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount - 1);
                Assert.That(LeafBlocksInner.BlockStateList[1].StateList.Count == NodeCount - 1);

                IFocusNodeStateReadOnlyList AllChildren2 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count - 1, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IFocusBrowsingExistingBlockNodeIndex RemovedLeafIndex2 = LeafBlocksInner.BlockStateList[1].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex2));


                int BlockCount = LeafBlocksInner.BlockStateList.Count;
                Assert.That(BlockCount == 3);

                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex2));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex2);
                Assert.That(!Controller.Contains(RemovedLeafIndex2));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount - 2);
                Assert.That(LeafBlocksInner.BlockStateList.Count == BlockCount - 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == 1, $"Count: {LeafBlocksInner.BlockStateList[0].StateList.Count}");

                IFocusNodeStateReadOnlyList AllChildren3 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count - 3, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));


                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();


                NeverEmptyCollectionTable.Remove(typeof(IMain));
                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedLeafIndex0));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusMove()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFocusListInner;
                Assert.That(LeafPathInner != null);

                IFocusBrowsingListNodeIndex MovedLeafIndex0 = LeafPathInner.IndexAt(0) as IFocusBrowsingListNodeIndex;
                Assert.That(Controller.Contains(MovedLeafIndex0));

                int PathCount = LeafPathInner.Count;
                Assert.That(PathCount == 2);

                IFocusNodeStateReadOnlyList AllChildren0 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Assert.That(Controller.IsMoveable(LeafPathInner, MovedLeafIndex0, +1));

                Controller.Move(LeafPathInner, MovedLeafIndex0, +1);
                Assert.That(Controller.Contains(MovedLeafIndex0));

                Assert.That(LeafPathInner.Count == PathCount);
                Assert.That(LeafPathInner.StateList.Count == PathCount);

                //System.Diagnostics.Debug.Assert(false);
                IFocusBrowsingListNodeIndex NewLeafIndex0 = LeafPathInner.IndexAt(1) as IFocusBrowsingListNodeIndex;
                Assert.That(NewLeafIndex0 == MovedLeafIndex0);

                IFocusNodeStateReadOnlyList AllChildren1 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));




                IFocusBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFocusBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFocusBrowsingExistingBlockNodeIndex MovedLeafIndex1 = LeafBlocksInner.IndexAt(1, 1) as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(MovedLeafIndex1));

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[1].StateList.Count;
                Assert.That(BlockNodeCount == 4, $"New count: {BlockNodeCount}");

                Assert.That(Controller.IsMoveable(LeafBlocksInner, MovedLeafIndex1, -1));
                Controller.Move(LeafBlocksInner, MovedLeafIndex1, -1);
                Assert.That(Controller.Contains(MovedLeafIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount);
                Assert.That(LeafBlocksInner.BlockStateList[1].StateList.Count == NodeCount);

                IFocusBrowsingExistingBlockNodeIndex NewLeafIndex1 = LeafBlocksInner.IndexAt(1, 0) as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(NewLeafIndex1 == MovedLeafIndex1);

                IFocusNodeStateReadOnlyList AllChildren2 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusMoveBlock()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusNodeStateReadOnlyList AllChildren1 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == 19, $"New count: {AllChildren1.Count}");

                IFocusBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFocusBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFocusBrowsingExistingBlockNodeIndex MovedLeafIndex1 = LeafBlocksInner.IndexAt(1, 0) as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(MovedLeafIndex1));

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = LeafBlocksInner.BlockStateList[1].StateList.Count;
                Assert.That(BlockNodeCount == 4, $"New count: {BlockNodeCount}");

                Assert.That(Controller.IsBlockMoveable(LeafBlocksInner, 1, -1));
                Controller.MoveBlock(LeafBlocksInner, 1, -1);
                Assert.That(Controller.Contains(MovedLeafIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == NodeCount);

                IFocusBrowsingExistingBlockNodeIndex NewLeafIndex1 = LeafBlocksInner.IndexAt(0, 0) as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(NewLeafIndex1 == MovedLeafIndex1);

                IFocusNodeStateReadOnlyList AllChildren2 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusChangeDiscreteValue()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                Assert.That(BaseNodeHelper.NodeTreeHelper.GetEnumValue(RootState.Node, nameof(IMain.ValueEnum)) == (int)BaseNode.CopySemantic.Value);

                Controller.ChangeDiscreteValue(RootIndex, nameof(IMain.ValueEnum), (int)BaseNode.CopySemantic.Reference);

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(BaseNodeHelper.NodeTreeHelper.GetEnumValue(RootNode, nameof(IMain.ValueEnum)) == (int)BaseNode.CopySemantic.Reference);

                IFocusPlaceholderInner PlaceholderTreeInner = RootState.PropertyToInner(nameof(IMain.PlaceholderTree)) as IFocusPlaceholderInner;
                IFocusPlaceholderNodeState PlaceholderTreeState = PlaceholderTreeInner.ChildState as IFocusPlaceholderNodeState;

                Assert.That(BaseNodeHelper.NodeTreeHelper.GetEnumValue(PlaceholderTreeState.Node, nameof(ITree.ValueEnum)) == (int)BaseNode.CopySemantic.Value);

                Controller.ChangeDiscreteValue(PlaceholderTreeState.ParentIndex, nameof(ITree.ValueEnum), (int)BaseNode.CopySemantic.Any);

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(BaseNodeHelper.NodeTreeHelper.GetEnumValue(PlaceholderTreeState.Node, nameof(ITree.ValueEnum)) == (int)BaseNode.CopySemantic.Any);

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusReplace()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                Leaf NewItem0 = CreateLeaf(Guid.NewGuid());
                IFocusInsertionListNodeIndex ReplacementIndex0 = new FocusInsertionListNodeIndex(RootNode, nameof(IMain.LeafPath), NewItem0, 0);

                IFocusListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFocusListInner;
                Assert.That(LeafPathInner != null);

                int PathCount = LeafPathInner.Count;
                Assert.That(PathCount == 2);

                IFocusNodeStateReadOnlyList AllChildren0 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Controller.Replace(LeafPathInner, ReplacementIndex0, out IWriteableBrowsingChildIndex NewItemIndex0);
                Assert.That(Controller.Contains(NewItemIndex0));

                Assert.That(LeafPathInner.Count == PathCount);
                Assert.That(LeafPathInner.StateList.Count == PathCount);

                IFocusPlaceholderNodeState NewItemState0 = LeafPathInner.StateList[0];
                Assert.That(NewItemState0.Node == NewItem0);
                Assert.That(NewItemState0.ParentIndex == NewItemIndex0);

                IFocusNodeStateReadOnlyList AllChildren1 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                Leaf NewItem1 = CreateLeaf(Guid.NewGuid());
                IFocusInsertionExistingBlockNodeIndex ReplacementIndex1 = new FocusInsertionExistingBlockNodeIndex(RootNode, nameof(IMain.LeafBlocks), NewItem1, 0, 0);

                IFocusBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFocusBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFocusBlockState BlockState = LeafBlocksInner.BlockStateList[0];

                int BlockNodeCount = LeafBlocksInner.Count;
                int NodeCount = BlockState.StateList.Count;
                Assert.That(BlockNodeCount == 4);

                Controller.Replace(LeafBlocksInner, ReplacementIndex1, out IWriteableBrowsingChildIndex NewItemIndex1);
                Assert.That(Controller.Contains(NewItemIndex1));

                Assert.That(LeafBlocksInner.Count == BlockNodeCount);
                Assert.That(BlockState.StateList.Count == NodeCount);

                IFocusPlaceholderNodeState NewItemState1 = BlockState.StateList[0];
                Assert.That(NewItemState1.Node == NewItem1);
                Assert.That(NewItemState1.ParentIndex == NewItemIndex1);

                IFocusNodeStateReadOnlyList AllChildren2 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IFocusPlaceholderInner PlaceholderTreeInner = RootState.PropertyToInner(nameof(IMain.PlaceholderTree)) as IFocusPlaceholderInner;
                Assert.That(PlaceholderTreeInner != null);

                IFocusBrowsingPlaceholderNodeIndex ExistingIndex2 = PlaceholderTreeInner.ChildState.ParentIndex as IFocusBrowsingPlaceholderNodeIndex;

                Tree NewItem2 = CreateTree();
                IFocusInsertionPlaceholderNodeIndex ReplacementIndex2;
                ReplacementIndex2 = ExistingIndex2.ToInsertionIndex(RootNode, NewItem2) as IFocusInsertionPlaceholderNodeIndex;

                Controller.Replace(PlaceholderTreeInner, ReplacementIndex2, out IWriteableBrowsingChildIndex NewItemIndex2);
                Assert.That(Controller.Contains(NewItemIndex2));

                IFocusPlaceholderNodeState NewItemState2 = PlaceholderTreeInner.ChildState as IFocusPlaceholderNodeState;
                Assert.That(NewItemState2.Node == NewItem2);
                Assert.That(NewItemState2.ParentIndex == NewItemIndex2);

                IFocusBrowsingPlaceholderNodeIndex DuplicateExistingIndex2 = ReplacementIndex2.ToBrowsingIndex() as IFocusBrowsingPlaceholderNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex2 as IFocusBrowsingPlaceholderNodeIndex, DuplicateExistingIndex2));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex2, NewItemIndex2 as IFocusBrowsingPlaceholderNodeIndex));

                IFocusNodeStateReadOnlyList AllChildren3 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IFocusPlaceholderInner PlaceholderLeafInner = NewItemState2.PropertyToInner(nameof(ITree.Placeholder)) as IFocusPlaceholderInner;
                Assert.That(PlaceholderLeafInner != null);

                IFocusBrowsingPlaceholderNodeIndex ExistingIndex3 = PlaceholderLeafInner.ChildState.ParentIndex as IFocusBrowsingPlaceholderNodeIndex;

                Leaf NewItem3 = CreateLeaf(Guid.NewGuid());
                IFocusInsertionPlaceholderNodeIndex ReplacementIndex3;
                ReplacementIndex3 = ExistingIndex3.ToInsertionIndex(NewItem2, NewItem3) as IFocusInsertionPlaceholderNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(ReplacementIndex3, ReplacementIndex3));

                Controller.Replace(PlaceholderLeafInner, ReplacementIndex3, out IWriteableBrowsingChildIndex NewItemIndex3);
                Assert.That(Controller.Contains(NewItemIndex3));

                IFocusPlaceholderNodeState NewItemState3 = PlaceholderLeafInner.ChildState as IFocusPlaceholderNodeState;
                Assert.That(NewItemState3.Node == NewItem3);
                Assert.That(NewItemState3.ParentIndex == NewItemIndex3);

                IFocusNodeStateReadOnlyList AllChildren4 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren4.Count == AllChildren3.Count, $"New count: {AllChildren4.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));




                IFocusOptionalInner OptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IFocusOptionalInner;
                Assert.That(OptionalLeafInner != null);

                IFocusBrowsingOptionalNodeIndex ExistingIndex4 = OptionalLeafInner.ChildState.ParentIndex as IFocusBrowsingOptionalNodeIndex;

                Leaf NewItem4 = CreateLeaf(Guid.NewGuid());
                IFocusInsertionOptionalNodeIndex ReplacementIndex4;
                ReplacementIndex4 = ExistingIndex4.ToInsertionIndex(RootNode, NewItem4) as IFocusInsertionOptionalNodeIndex;
                Assert.That(ReplacementIndex4.ParentNode == RootNode);
                Assert.That(ReplacementIndex4.PropertyName == OptionalLeafInner.PropertyName);
                Assert.That(CompareEqual.CoverIsEqual(ReplacementIndex4, ReplacementIndex4));

                Controller.Replace(OptionalLeafInner, ReplacementIndex4, out IWriteableBrowsingChildIndex NewItemIndex4);
                Assert.That(Controller.Contains(NewItemIndex4));

                Assert.That(OptionalLeafInner.IsAssigned);
                IFocusOptionalNodeState NewItemState4 = OptionalLeafInner.ChildState as IFocusOptionalNodeState;
                Assert.That(NewItemState4.Node == NewItem4);
                Assert.That(NewItemState4.ParentIndex == NewItemIndex4);

                IFocusBrowsingOptionalNodeIndex DuplicateExistingIndex4 = ReplacementIndex4.ToBrowsingIndex() as IFocusBrowsingOptionalNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex4 as IFocusBrowsingOptionalNodeIndex, DuplicateExistingIndex4));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex4, NewItemIndex4 as IFocusBrowsingOptionalNodeIndex));

                IFocusNodeStateReadOnlyList AllChildren5 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren5.Count == AllChildren4.Count, $"New count: {AllChildren5.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));



                IFocusBrowsingOptionalNodeIndex ExistingIndex5 = OptionalLeafInner.ChildState.ParentIndex as IFocusBrowsingOptionalNodeIndex;

                Leaf NewItem5 = CreateLeaf(Guid.NewGuid());
                IFocusInsertionOptionalClearIndex ReplacementIndex5;
                ReplacementIndex5 = ExistingIndex5.ToInsertionIndex(RootNode, null) as IFocusInsertionOptionalClearIndex;
                Assert.That(ReplacementIndex5.ParentNode == RootNode);
                Assert.That(ReplacementIndex5.PropertyName == OptionalLeafInner.PropertyName);
                Assert.That(CompareEqual.CoverIsEqual(ReplacementIndex5, ReplacementIndex5));

                Controller.Replace(OptionalLeafInner, ReplacementIndex5, out IWriteableBrowsingChildIndex NewItemIndex5);
                Assert.That(Controller.Contains(NewItemIndex5));

                Assert.That(!OptionalLeafInner.IsAssigned);
                IFocusOptionalNodeState NewItemState5 = OptionalLeafInner.ChildState as IFocusOptionalNodeState;
                Assert.That(NewItemState5.ParentIndex == NewItemIndex5);

                IFocusBrowsingOptionalNodeIndex DuplicateExistingIndex5 = ReplacementIndex5.ToBrowsingIndex() as IFocusBrowsingOptionalNodeIndex;
                Assert.That(CompareEqual.CoverIsEqual(NewItemIndex5 as IFocusBrowsingOptionalNodeIndex, DuplicateExistingIndex5));
                Assert.That(CompareEqual.CoverIsEqual(DuplicateExistingIndex5, NewItemIndex5 as IFocusBrowsingOptionalNodeIndex));

                IFocusNodeStateReadOnlyList AllChildren6 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren6.Count == AllChildren5.Count - 1, $"New count: {AllChildren6.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusAssign()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            //System.Diagnostics.Debug.Assert(false);
            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusOptionalInner UnassignedOptionalLeafInner = RootState.PropertyToInner(nameof(IMain.UnassignedOptionalLeaf)) as IFocusOptionalInner;
                Assert.That(UnassignedOptionalLeafInner != null);
                Assert.That(!UnassignedOptionalLeafInner.IsAssigned);

                IFocusBrowsingOptionalNodeIndex AssignmentIndex0 = UnassignedOptionalLeafInner.ChildState.ParentIndex;
                Assert.That(AssignmentIndex0 != null);

                IFocusNodeStateReadOnlyList AllChildren0 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Controller.Assign(AssignmentIndex0, out bool IsChanged);
                Assert.That(IsChanged);
                Assert.That(UnassignedOptionalLeafInner.IsAssigned);

                IFocusNodeStateReadOnlyList AllChildren1 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Assign(AssignmentIndex0, out IsChanged);
                Assert.That(!IsChanged);
                Assert.That(UnassignedOptionalLeafInner.IsAssigned);

                IFocusNodeStateReadOnlyList AllChildren2 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Controller.Unassign(AssignmentIndex0, out IsChanged);
                Assert.That(IsChanged);
                Assert.That(!UnassignedOptionalLeafInner.IsAssigned);

                IFocusNodeStateReadOnlyList AllChildren3 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count - 1, $"New count: {AllChildren3.Count}");

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusUnassign()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusOptionalInner AssignedOptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IFocusOptionalInner;
                Assert.That(AssignedOptionalLeafInner != null);
                Assert.That(AssignedOptionalLeafInner.IsAssigned);

                IFocusBrowsingOptionalNodeIndex AssignmentIndex0 = AssignedOptionalLeafInner.ChildState.ParentIndex;
                Assert.That(AssignmentIndex0 != null);

                IFocusNodeStateReadOnlyList AllChildren0 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Controller.Unassign(AssignmentIndex0, out bool IsChanged);
                Assert.That(IsChanged);
                Assert.That(!AssignedOptionalLeafInner.IsAssigned);

                IFocusNodeStateReadOnlyList AllChildren1 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count - 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Unassign(AssignmentIndex0, out IsChanged);
                Assert.That(!IsChanged);
                Assert.That(!AssignedOptionalLeafInner.IsAssigned);

                IFocusNodeStateReadOnlyList AllChildren2 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Controller.Assign(AssignmentIndex0, out IsChanged);
                Assert.That(IsChanged);
                Assert.That(AssignedOptionalLeafInner.IsAssigned);

                IFocusNodeStateReadOnlyList AllChildren3 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count + 1, $"New count: {AllChildren3.Count}");

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusChangeReplication()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFocusBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFocusNodeStateReadOnlyList AllChildren0 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                IFocusBlockState BlockState = LeafBlocksInner.BlockStateList[0];
                Assert.That(BlockState != null);
                Assert.That(BlockState.ParentInner == LeafBlocksInner);
                BaseNode.IBlock ChildBlock = BlockState.ChildBlock;
                Assert.That(ChildBlock.Replication == BaseNode.ReplicationStatus.Normal);

                Controller.ChangeReplication(LeafBlocksInner, 0, BaseNode.ReplicationStatus.Replicated);

                Assert.That(ChildBlock.Replication == BaseNode.ReplicationStatus.Replicated);

                IFocusNodeStateReadOnlyList AllChildren1 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusSplit()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFocusBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFocusNodeStateReadOnlyList AllChildren0 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                IFocusBlockState BlockState0 = LeafBlocksInner.BlockStateList[0];
                Assert.That(BlockState0 != null);
                BaseNode.IBlock ChildBlock0 = BlockState0.ChildBlock;
                Assert.That(ChildBlock0.NodeList.Count == 1);

                IFocusBlockState BlockState1 = LeafBlocksInner.BlockStateList[1];
                Assert.That(BlockState1 != null);
                BaseNode.IBlock ChildBlock1 = BlockState1.ChildBlock;
                Assert.That(ChildBlock1.NodeList.Count == 2);

                Assert.That(LeafBlocksInner.Count == 4);
                Assert.That(LeafBlocksInner.BlockStateList.Count == 3);

                IFocusBrowsingExistingBlockNodeIndex SplitIndex0 = LeafBlocksInner.IndexAt(1, 1) as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.IsSplittable(LeafBlocksInner, SplitIndex0));

                Controller.SplitBlock(LeafBlocksInner, SplitIndex0);

                Assert.That(LeafBlocksInner.BlockStateList.Count == 4);
                Assert.That(ChildBlock0 == LeafBlocksInner.BlockStateList[0].ChildBlock);
                Assert.That(ChildBlock1 == LeafBlocksInner.BlockStateList[2].ChildBlock);
                Assert.That(ChildBlock1.NodeList.Count == 1);

                IFocusBlockState BlockState12 = LeafBlocksInner.BlockStateList[1];
                Assert.That(BlockState12.ChildBlock.NodeList.Count == 1);

                IFocusNodeStateReadOnlyList AllChildren1 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 2, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusMerge()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFocusBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFocusNodeStateReadOnlyList AllChildren0 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                IFocusBlockState BlockState0 = LeafBlocksInner.BlockStateList[0];
                Assert.That(BlockState0 != null);
                BaseNode.IBlock ChildBlock0 = BlockState0.ChildBlock;
                Assert.That(ChildBlock0.NodeList.Count == 1);

                IFocusBlockState BlockState1 = LeafBlocksInner.BlockStateList[1];
                Assert.That(BlockState1 != null);
                BaseNode.IBlock ChildBlock1 = BlockState1.ChildBlock;
                Assert.That(ChildBlock1.NodeList.Count == 2);

                Assert.That(LeafBlocksInner.Count == 4);

                IFocusBrowsingExistingBlockNodeIndex MergeIndex0 = LeafBlocksInner.IndexAt(1, 0) as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.IsMergeable(LeafBlocksInner, MergeIndex0));

                Assert.That(LeafBlocksInner.BlockStateList.Count == 3);

                Controller.MergeBlocks(LeafBlocksInner, MergeIndex0);

                Assert.That(LeafBlocksInner.BlockStateList.Count == 2);
                Assert.That(ChildBlock1 == LeafBlocksInner.BlockStateList[0].ChildBlock);
                Assert.That(ChildBlock1.NodeList.Count == 3);

                Assert.That(LeafBlocksInner.BlockStateList[0] == BlockState1);

                IFocusNodeStateReadOnlyList AllChildren1 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count - 2, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusExpand()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusNodeStateReadOnlyList AllChildren0 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 19, $"New count: {AllChildren0.Count}");

                Controller.Expand(RootIndex, out bool IsChanged);
                Assert.That(IsChanged);

                IFocusNodeStateReadOnlyList AllChildren1 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(!IsChanged);

                IFocusNodeStateReadOnlyList AllChildren2 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count, $"New count: {AllChildren2.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                IFocusOptionalInner OptionalLeafInner = RootState.PropertyToInner(nameof(IMain.AssignedOptionalLeaf)) as IFocusOptionalInner;
                Assert.That(OptionalLeafInner != null);

                IFocusInsertionOptionalClearIndex ReplacementIndex5 = new FocusInsertionOptionalClearIndex(RootNode, nameof(IMain.AssignedOptionalLeaf));

                Controller.Replace(OptionalLeafInner, ReplacementIndex5, out IWriteableBrowsingChildIndex NewItemIndex5);
                Assert.That(Controller.Contains(NewItemIndex5));

                IFocusNodeStateReadOnlyList AllChildren3 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count - 1, $"New count: {AllChildren3.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFocusNodeStateReadOnlyList AllChildren4 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren4.Count == AllChildren3.Count + 1, $"New count: {AllChildren4.Count}");



                IFocusBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFocusBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFocusBrowsingExistingBlockNodeIndex RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                IFocusNodeStateReadOnlyList AllChildren5 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren5.Count == AllChildren4.Count - 10, $"New count: {AllChildren5.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(LeafBlocksInner.IsEmpty);

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(!IsChanged);

                IFocusNodeStateReadOnlyList AllChildren6 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren6.Count == AllChildren5.Count, $"New count: {AllChildren6.Count}");

                IDictionary<Type, string[]> WithExpandCollectionTable = BaseNodeHelper.NodeHelper.WithExpandCollectionTable as IDictionary<Type, string[]>;
                WithExpandCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFocusNodeStateReadOnlyList AllChildren7 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren7.Count == AllChildren6.Count + 3, $"New count: {AllChildren7.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(!LeafBlocksInner.IsEmpty);
                Assert.That(LeafBlocksInner.IsSingle);

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                WithExpandCollectionTable.Remove(typeof(IMain));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusReduce()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;
            bool IsChanged;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFocusBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFocusBrowsingExistingBlockNodeIndex RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(LeafBlocksInner.IsEmpty);

                IFocusNodeStateReadOnlyList AllChildren0 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 9, $"New count: {AllChildren0.Count}");

                IDictionary<Type, string[]> WithExpandCollectionTable = BaseNodeHelper.NodeHelper.WithExpandCollectionTable as IDictionary<Type, string[]>;
                WithExpandCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFocusNodeStateReadOnlyList AllChildren1 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 4, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                //System.Diagnostics.Debug.Assert(false);
                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFocusNodeStateReadOnlyList AllChildren2 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count - 7, $"New count: {AllChildren2.Count}");

                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(!IsChanged);

                IFocusNodeStateReadOnlyList AllChildren3 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count, $"New count: {AllChildren3.Count}");

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFocusNodeStateReadOnlyList AllChildren4 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren4.Count == AllChildren3.Count + 7, $"New count: {AllChildren4.Count}");

                BaseNode.IBlock ChildBlock = LeafBlocksInner.BlockStateList[0].ChildBlock;
                ILeaf FirstNode = ChildBlock.NodeList[0] as ILeaf;
                Assert.That(FirstNode != null);
                BaseNodeHelper.NodeTreeHelper.SetString(FirstNode, nameof(ILeaf.Text), "!");

                //System.Diagnostics.Debug.Assert(false);
                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFocusNodeStateReadOnlyList AllChildren5 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren5.Count == AllChildren4.Count - 4, $"New count: {AllChildren5.Count}");

                BaseNodeHelper.NodeTreeHelper.SetString(FirstNode, nameof(ILeaf.Text), "");

                //System.Diagnostics.Debug.Assert(false);
                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFocusNodeStateReadOnlyList AllChildren6 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren6.Count == AllChildren5.Count - 3, $"New count: {AllChildren6.Count}");

                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                WithExpandCollectionTable.Remove(typeof(IMain));

                //System.Diagnostics.Debug.Assert(false);
                Controller.Reduce(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFocusNodeStateReadOnlyList AllChildren7 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren7.Count == AllChildren6.Count + 3, $"New count: {AllChildren7.Count}");

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                WithExpandCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                WithExpandCollectionTable.Remove(typeof(IMain));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusCanonicalize()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;
            bool IsChanged;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFocusBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFocusBrowsingExistingBlockNodeIndex RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                Assert.That(Controller.CanUndo);
                IFocusOperationGroup LastOperation = Controller.OperationStack[Controller.RedoIndex - 1];
                Assert.That(LastOperation.MainOperation is IFocusRemoveOperation);
                Assert.That(LastOperation.OperationList.Count > 0);
                Assert.That(LastOperation.Refresh == null);

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                RemovedLeafIndex = LeafBlocksInner.BlockStateList[0].StateList[0].ParentIndex as IFocusBrowsingExistingBlockNodeIndex;
                Assert.That(Controller.Contains(RemovedLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafBlocksInner, RemovedLeafIndex));

                Controller.Remove(LeafBlocksInner, RemovedLeafIndex);
                Assert.That(!Controller.Contains(RemovedLeafIndex));

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));
                Assert.That(LeafBlocksInner.IsEmpty);

                IFocusListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFocusListInner;
                Assert.That(LeafPathInner != null);
                Assert.That(LeafPathInner.Count == 2);

                IFocusBrowsingListNodeIndex RemovedListLeafIndex = LeafPathInner.StateList[0].ParentIndex as IFocusBrowsingListNodeIndex;
                Assert.That(Controller.Contains(RemovedListLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedListLeafIndex));

                Controller.Remove(LeafPathInner, RemovedListLeafIndex);
                Assert.That(!Controller.Contains(RemovedListLeafIndex));

                IDictionary<Type, string[]> NeverEmptyCollectionTable = BaseNodeHelper.NodeHelper.NeverEmptyCollectionTable as IDictionary<Type, string[]>;
                NeverEmptyCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.PlaceholderTree) });

                RemovedListLeafIndex = LeafPathInner.StateList[0].ParentIndex as IFocusBrowsingListNodeIndex;
                Assert.That(Controller.Contains(RemovedListLeafIndex));
                Assert.That(Controller.IsRemoveable(LeafPathInner, RemovedListLeafIndex));

                Controller.Remove(LeafPathInner, RemovedListLeafIndex);
                Assert.That(!Controller.Contains(RemovedListLeafIndex));
                Assert.That(LeafPathInner.Count == 0);

                NeverEmptyCollectionTable.Remove(typeof(IMain));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                IFocusNodeStateReadOnlyList AllChildren0 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren0.Count == 12, $"New count: {AllChildren0.Count}");

                IDictionary<Type, string[]> WithExpandCollectionTable = BaseNodeHelper.NodeHelper.WithExpandCollectionTable as IDictionary<Type, string[]>;
                WithExpandCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });

                //System.Diagnostics.Debug.Assert(false);
                Controller.Expand(RootIndex, out IsChanged);
                Assert.That(IsChanged);

                IFocusNodeStateReadOnlyList AllChildren1 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren1.Count == AllChildren0.Count + 1, $"New count: {AllChildren1.Count}");

                Assert.That(BaseNodeHelper.NodeTreeDiagnostic.IsValid(RootNode));

                Controller.Canonicalize(out IsChanged);
                Assert.That(IsChanged);

                IFocusNodeStateReadOnlyList AllChildren2 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren2.Count == AllChildren1.Count - 4, $"New count: {AllChildren2.Count}");

                Controller.Undo();
                Controller.Redo();

                Controller.Canonicalize(out IsChanged);
                Assert.That(!IsChanged);

                IFocusNodeStateReadOnlyList AllChildren3 = (IFocusNodeStateReadOnlyList)RootState.GetAllChildren();
                Assert.That(AllChildren3.Count == AllChildren2.Count, $"New count: {AllChildren3.Count}");

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                NeverEmptyCollectionTable.Add(typeof(IMain), new string[] { nameof(IMain.LeafBlocks) });
                Assert.That(LeafBlocksInner.BlockStateList.Count == 1);
                Assert.That(LeafBlocksInner.BlockStateList[0].StateList.Count == 1, LeafBlocksInner.BlockStateList[0].StateList.Count.ToString());

                Controller.Canonicalize(out IsChanged);
                Assert.That(IsChanged);

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                NeverEmptyCollectionTable.Remove(typeof(IMain));

                WithExpandCollectionTable.Remove(typeof(IMain));

                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();
                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusReplaceWithCycle()
        {
            ControllerTools.ResetExpectedName();

            BaseNode.IClass RootNode;
            IFocusRootNodeIndex RootIndex;

            RootNode = BaseNodeHelper.NodeHelper.CreateSimpleClass("");

            BaseNode.IFeature RootFeature = BaseNodeHelper.NodeHelper.CreateEmptyFunctionFeature();
            ((BaseNode.Class)RootNode).FeatureBlocks = BaseNodeHelper.BlockListHelper<BaseNode.IFeature, BaseNode.Feature>.CreateSimpleBlockList(RootFeature);

            BaseNode.ICommandInstruction FirstInstruction = BaseNodeHelper.NodeHelper.CreateSimpleCommandInstruction("test!") as BaseNode.ICommandInstruction;
            BaseNode.IFunctionFeature FirstFeature = (BaseNode.IFunctionFeature)RootNode.FeatureBlocks.NodeBlockList[0].NodeList[0];
            BaseNode.IQueryOverload FirstOverload = FirstFeature.OverloadBlocks.NodeBlockList[0].NodeList[0];
            BaseNode.EffectiveBody FirstOverloadBody = (BaseNode.EffectiveBody)FirstOverload.QueryBody;

            FirstOverloadBody.BodyInstructionBlocks = BaseNodeHelper.BlockListHelper<BaseNode.IInstruction, BaseNode.Instruction>.CreateSimpleBlockList(FirstInstruction);

            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                IFocusCyclableNodeState State;
                int CyclePosition;
                bool IsItemCyclableThrough;

                IsItemCyclableThrough = ControllerView0.IsItemCyclableThrough(out State, out CyclePosition);
                Assert.That(!IsItemCyclableThrough);

                while (ControllerView0.MaxFocusMove > 0 && !(ControllerView0.FocusedCellView.StateView.State.Node is BaseNode.IFeature))
                    ControllerView0.MoveFocus(+1);

                IFocusNodeStateView StateView = ControllerView0.FocusedCellView.StateView;
                Assert.That(ControllerView0.CollectionHasItems(StateView, nameof(BaseNode.IFunctionFeature.OverloadBlocks)));
                Assert.That(ControllerView0.IsFirstItem(StateView));

                IFocusNodeState CurrentState = StateView.State;
                Assert.That(CurrentState != null && CurrentState.Node is BaseNode.IFeature);

                IFocusInsertionChildNodeIndexList CycleIndexList;
                int CycleCount = 10;
                IFocusBrowsingChildIndex NewItemIndex0;

                ControllerView0.SetUserVisible(true);
                ControllerView0.SetUserVisible(false);

                for (int i = 0; i < CycleCount; i++)
                {
                    IsItemCyclableThrough = ControllerView0.IsItemCyclableThrough(out State, out CyclePosition);
                    Assert.That(IsItemCyclableThrough);

                    CycleIndexList = State.CycleIndexList;

                    CyclePosition = (CyclePosition + 1) % CycleIndexList.Count;
                    Controller.Replace(State.ParentInner, CycleIndexList, CyclePosition, out NewItemIndex0);
                }

                for (int i = 0; i < CycleCount; i++)
                {
                    Assert.That(Controller.CanUndo);
                    Controller.Undo();
                }

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));



                //System.Diagnostics.Debug.Assert(false);

                for (int i = 0; i < CycleCount; i++)
                {
                    ControllerView0.MoveFocus(ControllerView0.MinFocusMove);

                    while (ControllerView0.MaxFocusMove > 0)
                    {
                        if (ControllerView0.FocusedCellView.StateView.State.Node is BaseNode.IIdentifier AsIdentifier && AsIdentifier.Text == FirstInstruction.Command.Path[0].Text)
                            break;

                        if (ControllerView0.FocusedCellView.Frame is IFocusKeywordFrame AsFocusableKeywordFrame && (AsFocusableKeywordFrame.Text == "deferred" || AsFocusableKeywordFrame.Text == "extern" || AsFocusableKeywordFrame.Text == "precursor"))
                            break;

                        ControllerView0.MoveFocus(+1);
                    }

                    StateView = ControllerView0.FocusedCellView.StateView;
                    CurrentState = StateView.State;
                    if (CurrentState.Node is BaseNode.IIdentifier AsStateIdentifier && AsStateIdentifier.Text == FirstInstruction.Command.Path[0].Text)
                    {
                        Assert.That(ControllerView0.IsFirstItem(StateView));

                        IFocusNodeState ParentState = CurrentState.ParentState;
                        Assert.That(ControllerView0.StateViewTable.ContainsKey(ParentState));
                        IFocusNodeStateView ParentStateView = ControllerView0.StateViewTable[ParentState];
                        Assert.That(ControllerView0.CollectionHasItems(ParentStateView, nameof(BaseNode.IQualifiedName.Path)));
                    }

                    IsItemCyclableThrough = ControllerView0.IsItemCyclableThrough(out State, out CyclePosition);
                    Assert.That(IsItemCyclableThrough);

                    CycleIndexList = State.CycleIndexList;

                    CyclePosition = (CyclePosition + 1) % CycleIndexList.Count;
                    Controller.Replace(State.ParentInner, CycleIndexList, CyclePosition, out NewItemIndex0);
                }

                for (int i = 0; i < CycleCount; i++)
                {
                    Assert.That(Controller.CanUndo);
                    Controller.Undo();
                }

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusPrune()
        {
            ControllerTools.ResetExpectedName();

            IMain MainItem = CreateRoot(ValueGuid0, Imperfections.None);
            IRoot RootNode = new Root();
            BaseNode.IDocument RootDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("root doc", Guid.NewGuid());
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(RootNode, RootDocument);
            BaseNode.IBlockList<IMain, Main> MainBlocks = BaseNodeHelper.BlockListHelper<IMain, Main>.CreateSimpleBlockList(MainItem);

            IMain UnassignedOptionalMain = CreateRoot(ValueGuid1, Imperfections.None);
            Easly.IOptionalReference<IMain> UnassignedOptional = BaseNodeHelper.OptionalReferenceHelper<IMain>.CreateReference(UnassignedOptionalMain);

            IList<ILeaf> LeafPath = new List<ILeaf>();

            BaseNodeHelper.NodeTreeHelperBlockList.SetBlockList(RootNode, nameof(IRoot.MainBlocks), (BaseNode.IBlockList)MainBlocks);
            BaseNodeHelper.NodeTreeHelperOptional.SetOptionalReference(RootNode, nameof(IRoot.UnassignedOptionalMain), (Easly.IOptionalReference)UnassignedOptional);
            BaseNodeHelper.NodeTreeHelper.SetString(RootNode, nameof(IRoot.ValueString), "root string");
            BaseNodeHelper.NodeTreeHelperList.SetChildNodeList(RootNode, nameof(IRoot.LeafPath), (IList)LeafPath);

            //System.Diagnostics.Debug.Assert(false);
            IFocusRootNodeIndex RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            using (IFocusControllerView ControllerView0 = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Assert.That(ControllerView0.Controller == Controller);

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusBlockListInner MainInner = RootState.PropertyToInner(nameof(IRoot.MainBlocks)) as IFocusBlockListInner;
                Assert.That(MainInner != null);

                IFocusBrowsingExistingBlockNodeIndex MainIndex = MainInner.IndexAt(0, 0) as IFocusBrowsingExistingBlockNodeIndex;
                Controller.Remove(MainInner, MainIndex);

                Assert.That(Controller.CanUndo);
                Controller.Undo();

                Assert.That(!Controller.CanUndo);
                Assert.That(Controller.CanRedo);

                Controller.Redo();
                Controller.Undo();

                MainIndex = MainInner.IndexAt(0, 0) as IFocusBrowsingExistingBlockNodeIndex;
                Controller.Remove(MainInner, MainIndex);

                Controller.Undo();
                Controller.Redo();
                Controller.Undo();

                Assert.That(ControllerBase.IsEqual(CompareEqual.New(), Controller));
            }
        }

        [Test]
        [Category("Coverage")]
        public static void FocusCollections()
        {
            ControllerTools.ResetExpectedName();

            IMain RootNode;
            IFocusRootNodeIndex RootIndex;
            bool IsReadOnly;
            IReadOnlyBlockState FirstBlockState;
            IReadOnlyBrowsingBlockNodeIndex FirstBlockNodeIndex;
            IReadOnlyBrowsingListNodeIndex FirstListNodeIndex;

            RootNode = CreateRoot(ValueGuid0, Imperfections.None);
            RootIndex = new FocusRootNodeIndex(RootNode);

            IFocusController ControllerBase = FocusController.Create(RootIndex);
            IFocusController Controller = FocusController.Create(RootIndex);

            IReadOnlyIndexNodeStateDictionary ControllerStateTable = DebugObjects.GetReferenceByInterface(typeof(IFocusIndexNodeStateDictionary)) as IReadOnlyIndexNodeStateDictionary;

            using (IFocusControllerView ControllerView = FocusControllerView.Create(Controller, TestDebug.CoverageFocusTemplateSet.FocusTemplateSet))
            {
                Controller.Canonicalize(out bool IsChanged);

                // IxxxBlockStateViewDictionary 

                IReadOnlyBlockStateViewDictionary ReadOnlyBlockStateViewTable = ControllerView.BlockStateViewTable;

                foreach (KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView> Entry in ReadOnlyBlockStateViewTable)
                {
                    IReadOnlyBlockStateView StateView = ReadOnlyBlockStateViewTable[Entry.Key];
                    ReadOnlyBlockStateViewTable.TryGetValue(Entry.Key, out IReadOnlyBlockStateView Value);
                    ReadOnlyBlockStateViewTable.Contains(Entry);
                    ReadOnlyBlockStateViewTable.Remove(Entry.Key);
                    ReadOnlyBlockStateViewTable.Add(Entry.Key, Entry.Value);
                    ICollection<IReadOnlyBlockState> Keys = ReadOnlyBlockStateViewTable.Keys;
                    ICollection<IReadOnlyBlockStateView> Values = ReadOnlyBlockStateViewTable.Values;

                    break;
                }

                IDictionary<IReadOnlyBlockState, IReadOnlyBlockStateView> ReadOnlyBlockStateViewTableAsDictionary = ReadOnlyBlockStateViewTable;
                foreach (KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView> Entry in ReadOnlyBlockStateViewTableAsDictionary)
                {
                    IReadOnlyBlockStateView StateView = ReadOnlyBlockStateViewTableAsDictionary[Entry.Key];
                    break;
                }

                ICollection<KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView>> ReadOnlyBlockStateViewTableAsCollection = ReadOnlyBlockStateViewTable;
                IsReadOnly = ReadOnlyBlockStateViewTableAsCollection.IsReadOnly;

                foreach (KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView> Entry in ReadOnlyBlockStateViewTableAsCollection)
                {
                    ReadOnlyBlockStateViewTableAsCollection.Contains(Entry);
                    ReadOnlyBlockStateViewTableAsCollection.Remove(Entry);
                    ReadOnlyBlockStateViewTableAsCollection.Add(Entry);
                    ReadOnlyBlockStateViewTableAsCollection.CopyTo(new KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView>[ReadOnlyBlockStateViewTableAsCollection.Count], 0);
                    break;
                }

                IEnumerable<KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView>> ReadOnlyBlockStateViewTableAsEnumerable = ReadOnlyBlockStateViewTable;
                foreach (KeyValuePair<IReadOnlyBlockState, IReadOnlyBlockStateView> Entry in ReadOnlyBlockStateViewTableAsEnumerable)
                {
                    break;
                }

                IFrameBlockStateViewDictionary FrameBlockStateViewTable = ControllerView.BlockStateViewTable;

                foreach (KeyValuePair<IFrameBlockState, IFrameBlockStateView> Entry in FrameBlockStateViewTable)
                {
                    IFrameBlockStateView StateView = FrameBlockStateViewTable[Entry.Key];
                    FrameBlockStateViewTable.TryGetValue(Entry.Key, out IFrameBlockStateView Value);
                    FrameBlockStateViewTable.Contains(Entry);
                    FrameBlockStateViewTable.Remove(Entry.Key);
                    FrameBlockStateViewTable.Add(Entry.Key, Entry.Value);
                    ICollection<IFrameBlockState> Keys = ((IDictionary<IFrameBlockState, IFrameBlockStateView>)FrameBlockStateViewTable).Keys;
                    ICollection<IFrameBlockStateView> Values = ((IDictionary<IFrameBlockState, IFrameBlockStateView>)FrameBlockStateViewTable).Values;

                    break;
                }

                IDictionary<IFrameBlockState, IFrameBlockStateView> FrameBlockStateViewTableAsDictionary = FrameBlockStateViewTable;
                foreach (KeyValuePair<IFrameBlockState, IFrameBlockStateView> Entry in FrameBlockStateViewTableAsDictionary)
                {
                    IFrameBlockStateView StateView = FrameBlockStateViewTableAsDictionary[Entry.Key];
                    break;
                }

                ICollection<KeyValuePair<IFrameBlockState, IFrameBlockStateView>> FrameBlockStateViewTableAsCollection = FrameBlockStateViewTable;
                IsReadOnly = FrameBlockStateViewTableAsCollection.IsReadOnly;

                foreach (KeyValuePair<IFrameBlockState, IFrameBlockStateView> Entry in FrameBlockStateViewTableAsCollection)
                {
                    FrameBlockStateViewTableAsCollection.Contains(Entry);
                    FrameBlockStateViewTableAsCollection.Remove(Entry);
                    FrameBlockStateViewTableAsCollection.Add(Entry);
                    FrameBlockStateViewTableAsCollection.CopyTo(new KeyValuePair<IFrameBlockState, IFrameBlockStateView>[FrameBlockStateViewTableAsCollection.Count], 0);
                    break;
                }

                IEnumerable<KeyValuePair<IFrameBlockState, IFrameBlockStateView>> FrameBlockStateViewTableAsEnumerable = FrameBlockStateViewTable;
                foreach (KeyValuePair<IFrameBlockState, IFrameBlockStateView> Entry in FrameBlockStateViewTableAsEnumerable)
                {
                    break;
                }

                // IFocusBlockStateList

                IFocusNodeState RootState = Controller.RootState;
                Assert.That(RootState != null);

                IFocusBlockListInner LeafBlocksInner = RootState.PropertyToInner(nameof(IMain.LeafBlocks)) as IFocusBlockListInner;
                Assert.That(LeafBlocksInner != null);

                IFocusListInner LeafPathInner = RootState.PropertyToInner(nameof(IMain.LeafPath)) as IFocusListInner;
                Assert.That(LeafPathInner != null);

                IFocusPlaceholderNodeState FirstNodeState = LeafBlocksInner.FirstNodeState;
                IFocusBlockStateList DebugBlockStateList = DebugObjects.GetReferenceByInterface(typeof(IFocusBlockStateList)) as IFocusBlockStateList;
                if (DebugBlockStateList != null)
                {
                    Assert.That(DebugBlockStateList.Count > 0);
                    IsReadOnly = ((ICollection<IReadOnlyBlockState>)DebugBlockStateList).IsReadOnly;
                    IsReadOnly = ((IList<IReadOnlyBlockState>)DebugBlockStateList).IsReadOnly;
                    IsReadOnly = ((ICollection<IFrameBlockState>)DebugBlockStateList).IsReadOnly;
                    IsReadOnly = ((IList<IFrameBlockState>)DebugBlockStateList).IsReadOnly;
                    FirstBlockState = DebugBlockStateList[0];
                    Assert.That(((IFrameBlockStateList)DebugBlockStateList)[0] == FirstBlockState);
                    Assert.That(DebugBlockStateList.Contains(FirstBlockState));
                    Assert.That(DebugBlockStateList.IndexOf(FirstBlockState) == 0);
                    DebugBlockStateList.Remove(FirstBlockState);
                    DebugBlockStateList.Add(FirstBlockState);
                    DebugBlockStateList.Remove(FirstBlockState);
                    DebugBlockStateList.Insert(0, FirstBlockState);
                    DebugBlockStateList.CopyTo((IReadOnlyBlockState[])(new IFocusBlockState[DebugBlockStateList.Count]), 0);
                    DebugBlockStateList.CopyTo((IFrameBlockState[])(new IFocusBlockState[DebugBlockStateList.Count]), 0);

                    IEnumerable<IReadOnlyBlockState> BlockStateListAsReadOnlyEnumerable = DebugBlockStateList;
                    foreach (IReadOnlyBlockState Item in BlockStateListAsReadOnlyEnumerable)
                    {
                        break;
                    }

                    IEnumerable<IFrameBlockState> BlockStateListAsFrameEnumerable = DebugBlockStateList;
                    foreach (IFrameBlockState Item in BlockStateListAsFrameEnumerable)
                    {
                        break;
                    }

                    IList<IReadOnlyBlockState> BlockStateListAsReadOnlyIlist = DebugBlockStateList;
                    Assert.That(BlockStateListAsReadOnlyIlist[0] == FirstBlockState);

                    IList<IFrameBlockState> BlockStateListAsFrameIList = DebugBlockStateList;
                    Assert.That(BlockStateListAsFrameIList[0] == FirstBlockState);
                    Assert.That(BlockStateListAsFrameIList.Contains((IFrameBlockState)FirstBlockState));
                    Assert.That(BlockStateListAsFrameIList.IndexOf((IFrameBlockState)FirstBlockState) == 0);

                    ICollection<IFrameBlockState> BlockStateListAsFrameICollection = DebugBlockStateList;
                    Assert.That(BlockStateListAsFrameICollection.Contains((IFrameBlockState)FirstBlockState));
                    BlockStateListAsFrameICollection.Remove((IFrameBlockState)FirstBlockState);
                    BlockStateListAsFrameICollection.Add((IFrameBlockState)FirstBlockState);
                    BlockStateListAsFrameICollection.Remove((IFrameBlockState)FirstBlockState);
                    BlockStateListAsFrameIList.Insert(0, (IFrameBlockState)FirstBlockState);

                    IReadOnlyList<IReadOnlyBlockState> BlockStateListAsReadOnlyIReadOnlylist = DebugBlockStateList;
                    Assert.That(BlockStateListAsReadOnlyIReadOnlylist[0] == FirstBlockState);

                    IReadOnlyList<IFrameBlockState> BlockStateListAsFrameIReadOnlylist = DebugBlockStateList;
                    Assert.That(BlockStateListAsFrameIReadOnlylist[0] == FirstBlockState);

                    IEnumerator<IFrameBlockState> DebugBlockStateListFrameEnumerator = ((IFrameBlockStateList)DebugBlockStateList).GetEnumerator();
                }

                IFocusBlockStateReadOnlyList FocusBlockStateList = LeafBlocksInner.BlockStateList;
                Assert.That(FocusBlockStateList.Count > 0);
                FirstBlockState = FocusBlockStateList[0];
                Assert.That(FocusBlockStateList.Contains(FirstBlockState));
                Assert.That(FocusBlockStateList.IndexOf(FirstBlockState) == 0);
                Assert.That(FocusBlockStateList.Contains((IFrameBlockState)FirstBlockState));
                Assert.That(FocusBlockStateList.IndexOf((IFrameBlockState)FirstBlockState) == 0);
                Assert.That(FocusBlockStateList.Contains((IFocusBlockState)FirstBlockState));
                Assert.That(FocusBlockStateList.IndexOf((IFocusBlockState)FirstBlockState) == 0);

                IEnumerable<IFrameBlockState> FocusBlockStateListAsIEnumerable = FocusBlockStateList;
                IEnumerator<IFrameBlockState> FocusBlockStateListAsIEnumerableEnumerator = FocusBlockStateListAsIEnumerable.GetEnumerator();

                IReadOnlyList<IFrameBlockState> FocusBlockStateListAsIReadOnlyList = FocusBlockStateList;
                Assert.That(FocusBlockStateListAsIReadOnlyList[0] == FirstBlockState);

                // IFocusBrowsingBlockNodeIndexList

                IFocusBrowsingBlockNodeIndexList BlockNodeIndexList = LeafBlocksInner.AllIndexes() as IFocusBrowsingBlockNodeIndexList;
                Assert.That(BlockNodeIndexList.Count > 0);
                IsReadOnly = ((ICollection<IReadOnlyBrowsingBlockNodeIndex>)BlockNodeIndexList).IsReadOnly;
                IsReadOnly = ((IList<IReadOnlyBrowsingBlockNodeIndex>)BlockNodeIndexList).IsReadOnly;
                IsReadOnly = ((ICollection<IFrameBrowsingBlockNodeIndex>)BlockNodeIndexList).IsReadOnly;
                IsReadOnly = ((IList<IFrameBrowsingBlockNodeIndex>)BlockNodeIndexList).IsReadOnly;
                FirstBlockNodeIndex = BlockNodeIndexList[0];
                Assert.That(((IFrameBrowsingBlockNodeIndexList)BlockNodeIndexList)[0] == FirstBlockNodeIndex);
                Assert.That(BlockNodeIndexList.Contains(FirstBlockNodeIndex));
                Assert.That(BlockNodeIndexList.IndexOf(FirstBlockNodeIndex) == 0);
                BlockNodeIndexList.Remove(FirstBlockNodeIndex);
                BlockNodeIndexList.Add(FirstBlockNodeIndex);
                BlockNodeIndexList.Remove(FirstBlockNodeIndex);
                BlockNodeIndexList.Insert(0, FirstBlockNodeIndex);
                BlockNodeIndexList.CopyTo((IReadOnlyBrowsingBlockNodeIndex[])(new IFocusBrowsingBlockNodeIndex[BlockNodeIndexList.Count]), 0);
                BlockNodeIndexList.CopyTo((IFrameBrowsingBlockNodeIndex[])(new IFocusBrowsingBlockNodeIndex[BlockNodeIndexList.Count]), 0);

                IEnumerable<IReadOnlyBrowsingBlockNodeIndex> BlockNodeIndexListAsReadOnlyEnumerable = BlockNodeIndexList;
                foreach (IReadOnlyBrowsingBlockNodeIndex Item in BlockNodeIndexListAsReadOnlyEnumerable)
                {
                    break;
                }

                IEnumerable<IFrameBrowsingBlockNodeIndex> BlockNodeIndexListAsFrameEnumerable = BlockNodeIndexList;
                foreach (IFrameBrowsingBlockNodeIndex Item in BlockNodeIndexListAsFrameEnumerable)
                {
                    break;
                }

                IList<IReadOnlyBrowsingBlockNodeIndex> BlockNodeIndexListAsReadOnlyIList = BlockNodeIndexList;
                Assert.That(BlockNodeIndexListAsReadOnlyIList[0] == FirstBlockNodeIndex);

                IList<IFrameBrowsingBlockNodeIndex> BlockNodeIndexListAsFrameIList = BlockNodeIndexList;
                Assert.That(BlockNodeIndexListAsFrameIList[0] == FirstBlockNodeIndex);
                Assert.That(BlockNodeIndexListAsFrameIList.Contains((IFrameBrowsingBlockNodeIndex)FirstBlockNodeIndex));
                Assert.That(BlockNodeIndexListAsFrameIList.IndexOf((IFrameBrowsingBlockNodeIndex)FirstBlockNodeIndex) == 0);

                ICollection<IFrameBrowsingBlockNodeIndex> BrowsingBlockNodeIndexListAsFrameICollection = BlockNodeIndexList;
                Assert.That(BrowsingBlockNodeIndexListAsFrameICollection.Contains((IFrameBrowsingBlockNodeIndex)FirstBlockNodeIndex));
                BrowsingBlockNodeIndexListAsFrameICollection.Remove((IFrameBrowsingBlockNodeIndex)FirstBlockNodeIndex);
                BrowsingBlockNodeIndexListAsFrameICollection.Add((IFrameBrowsingBlockNodeIndex)FirstBlockNodeIndex);
                BrowsingBlockNodeIndexListAsFrameICollection.Remove((IFrameBrowsingBlockNodeIndex)FirstBlockNodeIndex);
                BlockNodeIndexListAsFrameIList.Insert(0, (IFrameBrowsingBlockNodeIndex)FirstBlockNodeIndex);

                IReadOnlyList<IReadOnlyBrowsingBlockNodeIndex> BlockNodeIndexListAsReadOnlyIReadOnlylist = BlockNodeIndexList;
                Assert.That(BlockNodeIndexListAsReadOnlyIReadOnlylist[0] == FirstBlockNodeIndex);

                IReadOnlyList<IFrameBrowsingBlockNodeIndex> BlockNodeIndexListAsFrameIReadOnlylist = BlockNodeIndexList;
                Assert.That(BlockNodeIndexListAsFrameIReadOnlylist[0] == FirstBlockNodeIndex);

                IReadOnlyBrowsingBlockNodeIndexList BlockNodeIndexListAsReadOnly = BlockNodeIndexList;
                Assert.That(BlockNodeIndexListAsReadOnly[0] == FirstBlockNodeIndex);

                IEnumerator<IFrameBrowsingBlockNodeIndex> BlockNodeIndexListFrameEnumerator = ((IFrameBrowsingBlockNodeIndexList)BlockNodeIndexList).GetEnumerator();

                // IFocusBrowsingListNodeIndexList

                IFocusBrowsingListNodeIndexList ListNodeIndexList = LeafPathInner.AllIndexes() as IFocusBrowsingListNodeIndexList;
                Assert.That(ListNodeIndexList.Count > 0);
                IsReadOnly = ((ICollection<IReadOnlyBrowsingListNodeIndex>)ListNodeIndexList).IsReadOnly;
                IsReadOnly = ((IList<IReadOnlyBrowsingListNodeIndex>)ListNodeIndexList).IsReadOnly;
                IsReadOnly = ((ICollection<IFrameBrowsingListNodeIndex>)ListNodeIndexList).IsReadOnly;
                IsReadOnly = ((IList<IFrameBrowsingListNodeIndex>)ListNodeIndexList).IsReadOnly;
                FirstListNodeIndex = ListNodeIndexList[0];
                Assert.That(((IFrameBrowsingListNodeIndexList)ListNodeIndexList)[0] == FirstListNodeIndex);
                Assert.That(ListNodeIndexList.Contains(FirstListNodeIndex));
                Assert.That(ListNodeIndexList.IndexOf(FirstListNodeIndex) == 0);
                ListNodeIndexList.Remove(FirstListNodeIndex);
                ListNodeIndexList.Add(FirstListNodeIndex);
                ListNodeIndexList.Remove(FirstListNodeIndex);
                ListNodeIndexList.Insert(0, FirstListNodeIndex);
                ListNodeIndexList.CopyTo((IReadOnlyBrowsingListNodeIndex[])(new IFocusBrowsingListNodeIndex[ListNodeIndexList.Count]), 0);
                ListNodeIndexList.CopyTo((IFrameBrowsingListNodeIndex[])(new IFocusBrowsingListNodeIndex[ListNodeIndexList.Count]), 0);

                IEnumerable<IReadOnlyBrowsingListNodeIndex> ListNodeIndexListAsReadOnlyEnumerable = ListNodeIndexList;
                foreach (IReadOnlyBrowsingListNodeIndex Item in ListNodeIndexListAsReadOnlyEnumerable)
                {
                    break;
                }

                IEnumerable<IFrameBrowsingListNodeIndex> ListNodeIndexListAsFrameEnumerable = ListNodeIndexList;
                foreach (IFrameBrowsingListNodeIndex Item in ListNodeIndexListAsFrameEnumerable)
                {
                    break;
                }

                IList<IReadOnlyBrowsingListNodeIndex> ListNodeIndexListAsReadOnlyIList = ListNodeIndexList;
                Assert.That(ListNodeIndexListAsReadOnlyIList[0] == FirstListNodeIndex);

                IList<IFrameBrowsingListNodeIndex> ListNodeIndexListAsFrameIList = ListNodeIndexList;
                Assert.That(ListNodeIndexListAsFrameIList[0] == FirstListNodeIndex);
                Assert.That(ListNodeIndexListAsFrameIList.Contains((IFrameBrowsingListNodeIndex)FirstListNodeIndex));
                Assert.That(ListNodeIndexListAsFrameIList.IndexOf((IFrameBrowsingListNodeIndex)FirstListNodeIndex) == 0);

                ICollection<IFrameBrowsingListNodeIndex> BrowsingListNodeIndexListAsFrameICollection = ListNodeIndexList;
                Assert.That(BrowsingListNodeIndexListAsFrameICollection.Contains((IFrameBrowsingListNodeIndex)FirstListNodeIndex));
                BrowsingListNodeIndexListAsFrameICollection.Remove((IFrameBrowsingListNodeIndex)FirstListNodeIndex);
                BrowsingListNodeIndexListAsFrameICollection.Add((IFrameBrowsingListNodeIndex)FirstListNodeIndex);
                BrowsingListNodeIndexListAsFrameICollection.Remove((IFrameBrowsingListNodeIndex)FirstListNodeIndex);
                ListNodeIndexListAsFrameIList.Insert(0, (IFrameBrowsingListNodeIndex)FirstListNodeIndex);

                IReadOnlyList<IReadOnlyBrowsingListNodeIndex> ListNodeIndexListAsReadOnlyIReadOnlylist = ListNodeIndexList;
                Assert.That(ListNodeIndexListAsReadOnlyIReadOnlylist[0] == FirstListNodeIndex);

                IReadOnlyList<IFrameBrowsingListNodeIndex> ListNodeIndexListAsFrameIReadOnlylist = ListNodeIndexList;
                Assert.That(ListNodeIndexListAsFrameIReadOnlylist[0] == FirstListNodeIndex);

                IReadOnlyBrowsingListNodeIndexList ListNodeIndexListAsReadOnly = ListNodeIndexList;
                Assert.That(ListNodeIndexListAsReadOnly[0] == FirstListNodeIndex);

                IEnumerator<IFrameBrowsingListNodeIndex> ListNodeIndexListFrameEnumerator = ((IFrameBrowsingListNodeIndexList)ListNodeIndexList).GetEnumerator();

                // IFocusIndexNodeStateDictionary
                if (ControllerStateTable != null)
                {
                    foreach (KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState> Entry in ControllerStateTable)
                    {
                        IReadOnlyNodeState StateView = ControllerStateTable[Entry.Key];
                        ControllerStateTable.TryGetValue(Entry.Key, out IReadOnlyNodeState Value);
                        ControllerStateTable.Contains(Entry);
                        ControllerStateTable.Remove(Entry.Key);
                        ControllerStateTable.Add(Entry.Key, Entry.Value);
                        ICollection<IReadOnlyIndex> Keys = ControllerStateTable.Keys;
                        ICollection<IReadOnlyNodeState> Values = ControllerStateTable.Values;

                        break;
                    }

                    foreach (KeyValuePair<IFrameIndex, IFrameNodeState> Entry in (IFrameIndexNodeStateDictionary)ControllerStateTable)
                    {
                        break;
                    }

                    IDictionary<IReadOnlyIndex, IReadOnlyNodeState> ReadOnlyControllerStateTableAsDictionary = ControllerStateTable;
                    foreach (KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState> Entry in ReadOnlyControllerStateTableAsDictionary)
                    {
                        IReadOnlyNodeState StateView = ReadOnlyControllerStateTableAsDictionary[Entry.Key];
                        Assert.That(ReadOnlyControllerStateTableAsDictionary.ContainsKey(Entry.Key));
                        break;
                    }

                    IDictionary<IFrameIndex, IFrameNodeState> FrameControllerStateTableAsDictionary = ControllerStateTable as IDictionary<IFrameIndex, IFrameNodeState>;
                    foreach (KeyValuePair<IFrameIndex, IFrameNodeState> Entry in FrameControllerStateTableAsDictionary)
                    {
                        IFrameNodeState StateView = FrameControllerStateTableAsDictionary[Entry.Key];
                        Assert.That(FrameControllerStateTableAsDictionary.ContainsKey(Entry.Key));
                        FrameControllerStateTableAsDictionary.Remove(Entry.Key);
                        FrameControllerStateTableAsDictionary.Add(Entry.Key, Entry.Value);
                        Assert.That(FrameControllerStateTableAsDictionary.Keys != null);
                        Assert.That(FrameControllerStateTableAsDictionary.Values != null);
                        Assert.That(FrameControllerStateTableAsDictionary.TryGetValue(Entry.Key, out IFrameNodeState Value));
                        break;
                    }

                    ICollection<KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState>> ReadOnlyControllerStateTableAsCollection = ControllerStateTable;
                    IsReadOnly = ReadOnlyControllerStateTableAsCollection.IsReadOnly;
                    foreach (KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState> Entry in ReadOnlyControllerStateTableAsCollection)
                    {
                        Assert.That(ReadOnlyControllerStateTableAsCollection.Contains(Entry));
                        ReadOnlyControllerStateTableAsCollection.Remove(Entry);
                        ReadOnlyControllerStateTableAsCollection.Add(Entry);
                        ReadOnlyControllerStateTableAsCollection.CopyTo(new KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState>[ReadOnlyControllerStateTableAsCollection.Count], 0);
                        break;
                    }

                    ICollection<KeyValuePair<IFrameIndex, IFrameNodeState>> FrameControllerStateTableAsCollection = ControllerStateTable as ICollection<KeyValuePair<IFrameIndex, IFrameNodeState>>;
                    IsReadOnly = FrameControllerStateTableAsCollection.IsReadOnly;
                    foreach (KeyValuePair<IFrameIndex, IFrameNodeState> Entry in FrameControllerStateTableAsCollection)
                    {
                        Assert.That(FrameControllerStateTableAsCollection.Contains(Entry));
                        FrameControllerStateTableAsCollection.Remove(Entry);
                        FrameControllerStateTableAsCollection.Add(Entry);
                        FrameControllerStateTableAsCollection.CopyTo(new KeyValuePair<IFrameIndex, IFrameNodeState>[FrameControllerStateTableAsCollection.Count], 0);
                        break;
                    }

                    IEnumerable<KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState>> ReadOnlyControllerStateTableAsEnumerable = ControllerStateTable;
                    foreach (KeyValuePair<IReadOnlyIndex, IReadOnlyNodeState> Entry in ReadOnlyControllerStateTableAsEnumerable)
                    {
                        break;
                    }

                    IEnumerable<KeyValuePair<IFrameIndex, IFrameNodeState>> FrameControllerStateTableAsEnumerable = ControllerStateTable as IEnumerable<KeyValuePair<IFrameIndex, IFrameNodeState>>;
                    foreach (KeyValuePair<IFrameIndex, IFrameNodeState> Entry in FrameControllerStateTableAsEnumerable)
                    {
                        break;
                    }
                }

                // IFocusIndexNodeStateReadOnlyDictionary

                IReadOnlyIndexNodeStateReadOnlyDictionary ReadOnlyStateTable = Controller.StateTable;
                IFrameIndexNodeStateReadOnlyDictionary FrameStateTable = Controller.StateTable;
                Assert.That(FrameStateTable.ContainsKey(RootIndex));
                Assert.That(FrameStateTable[RootIndex] == ReadOnlyStateTable[RootIndex]);
                FrameStateTable.GetEnumerator();

                IReadOnlyDictionary<IReadOnlyIndex, IReadOnlyNodeState> ReadOnlyStateTableAsDictionary = ReadOnlyStateTable;
                Assert.That(ReadOnlyStateTable.TryGetValue(RootIndex, out IReadOnlyNodeState ReadOnlyRootStateValue) == ReadOnlyStateTableAsDictionary.TryGetValue(RootIndex, out IReadOnlyNodeState ReadOnlyRootStateValueFromDictionary) && ReadOnlyRootStateValue == ReadOnlyRootStateValueFromDictionary);
                Assert.That(ReadOnlyStateTableAsDictionary.Keys != null);
                Assert.That(ReadOnlyStateTableAsDictionary.Values != null);
                ReadOnlyStateTableAsDictionary.GetEnumerator();

                IReadOnlyDictionary<IFrameIndex, IFrameNodeState> FrameStateTableAsDictionary = ReadOnlyStateTable as IReadOnlyDictionary<IFrameIndex, IFrameNodeState>;
                Assert.That(FrameStateTable.TryGetValue(RootIndex, out IFrameNodeState FrameRootStateValue) == FrameStateTableAsDictionary.TryGetValue(RootIndex, out IFrameNodeState FrameRootStateValueFromDictionary) && FrameRootStateValue == FrameRootStateValueFromDictionary);
                Assert.That(FrameStateTableAsDictionary.ContainsKey(RootIndex));
                Assert.That(FrameStateTableAsDictionary[RootIndex] == ReadOnlyStateTable[RootIndex]);
                Assert.That(FrameStateTableAsDictionary.Keys != null);
                Assert.That(FrameStateTableAsDictionary.Values != null);

                IEnumerable<KeyValuePair<IFrameIndex, IFrameNodeState>> FrameStateTableAsEnumerable = ReadOnlyStateTable as IEnumerable<KeyValuePair<IFrameIndex, IFrameNodeState>>;
                FrameStateTableAsEnumerable.GetEnumerator();

                // IFocusInnerDictionary

                IFocusInnerDictionary<string> FocusInnerTableModify = DebugObjects.GetReferenceByInterface(typeof(IFocusInnerDictionary<string>)) as IFocusInnerDictionary<string>;
                Assert.That(FocusInnerTableModify != null);
                Assert.That(FocusInnerTableModify.Count > 0);

                IFrameInnerDictionary<string> FrameInnerTableModify = FocusInnerTableModify;
                FrameInnerTableModify.GetEnumerator();

                IDictionary<string, IReadOnlyInner> ReadOnlyInnerTableModifyAsDictionary = FocusInnerTableModify;
                Assert.That(ReadOnlyInnerTableModifyAsDictionary.Keys != null);
                Assert.That(ReadOnlyInnerTableModifyAsDictionary.Values != null);

                foreach (KeyValuePair<string, IFocusInner> Entry in FocusInnerTableModify)
                {
                    Assert.That(ReadOnlyInnerTableModifyAsDictionary.ContainsKey(Entry.Key));
                    Assert.That(ReadOnlyInnerTableModifyAsDictionary[Entry.Key] == Entry.Value);
                }

                IDictionary<string, IFrameInner> FrameInnerTableModifyAsDictionary = FocusInnerTableModify;
                Assert.That(FrameInnerTableModifyAsDictionary.Keys != null);
                Assert.That(FrameInnerTableModifyAsDictionary.Values != null);

                foreach (KeyValuePair<string, IFocusInner> Entry in FocusInnerTableModify)
                {
                    Assert.That(FrameInnerTableModifyAsDictionary.ContainsKey(Entry.Key));
                    Assert.That(FrameInnerTableModifyAsDictionary[Entry.Key] == Entry.Value);
                    FrameInnerTableModifyAsDictionary.Remove(Entry.Key);
                    FrameInnerTableModifyAsDictionary.Add(Entry.Key, Entry.Value);
                    FrameInnerTableModifyAsDictionary.TryGetValue(Entry.Key, out IFrameInner InnerValue);
                    break;
                }

                ICollection<KeyValuePair<string, IReadOnlyInner>> ReadOnlyInnerTableModifyAsCollection = FocusInnerTableModify;
                Assert.That(!ReadOnlyInnerTableModifyAsCollection.IsReadOnly);

                ICollection<KeyValuePair<string, IFrameInner>> FrameInnerTableModifyAsCollection = FocusInnerTableModify;
                Assert.That(!FrameInnerTableModifyAsCollection.IsReadOnly);
                FrameInnerTableModifyAsCollection.CopyTo(new KeyValuePair<string, IFrameInner>[FrameInnerTableModifyAsCollection.Count], 0);

                foreach (KeyValuePair<string, IFrameInner> Entry in FrameInnerTableModifyAsCollection)
                {
                    Assert.That(FrameInnerTableModifyAsCollection.Contains(Entry));
                    FrameInnerTableModifyAsCollection.Remove(Entry);
                    FrameInnerTableModifyAsCollection.Add(Entry);
                    break;
                }

                IEnumerable<KeyValuePair<string, IReadOnlyInner>> ReadOnlyInnerTableModifyAsEnumerable = FocusInnerTableModify;
                IEnumerator<KeyValuePair<string, IReadOnlyInner>> ReadOnlyInnerTableModifyAsEnumerableEnumerator = ReadOnlyInnerTableModifyAsEnumerable.GetEnumerator();

                foreach (KeyValuePair<string, IReadOnlyInner> Entry in ReadOnlyInnerTableModifyAsEnumerable)
                {
                    Assert.That(ReadOnlyInnerTableModifyAsDictionary.ContainsKey(Entry.Key));
                    Assert.That(ReadOnlyInnerTableModifyAsDictionary[Entry.Key] == Entry.Value);
                    Assert.That(FocusInnerTableModify.TryGetValue(Entry.Key, out IReadOnlyInner ReadOnlyInnerValue) == FocusInnerTableModify.TryGetValue(Entry.Key, out IFocusInner FocusInnerValue));

                    Assert.That(FocusInnerTableModify.Contains(Entry));
                    FocusInnerTableModify.Remove(Entry);
                    FocusInnerTableModify.Add(Entry);
                    FocusInnerTableModify.CopyTo(new KeyValuePair<string, IReadOnlyInner>[FocusInnerTableModify.Count], 0);
                    break;
                }

                IEnumerable<KeyValuePair<string, IFrameInner>> FrameInnerTableModifyAsEnumerable = FocusInnerTableModify;
                IEnumerator<KeyValuePair<string, IFrameInner>> FrameInnerTableModifyAsEnumerableEnumerator = FrameInnerTableModifyAsEnumerable.GetEnumerator();

                // IFocusInnerReadOnlyDictionary

                IFocusInnerReadOnlyDictionary<string> FocusInnerTable = RootState.InnerTable;
                IFrameInnerReadOnlyDictionary<string> FrameInnerTable = RootState.InnerTable;

                IReadOnlyDictionary<string, IReadOnlyInner> ReadOnlyInnerTableAsDictionary = FocusInnerTable;
                Assert.That(ReadOnlyInnerTableAsDictionary.Keys != null);
                Assert.That(ReadOnlyInnerTableAsDictionary.Values != null);

                IReadOnlyDictionary<string, IFrameInner> FrameInnerTableAsDictionary = FocusInnerTable;
                Assert.That(FrameInnerTableAsDictionary.Keys != null);
                Assert.That(FrameInnerTableAsDictionary.Values != null);

                IEnumerable<KeyValuePair<string, IFrameInner>> FrameInnerTableAsIEnumerable = FocusInnerTable;
                FrameInnerTableAsIEnumerable.GetEnumerator();

                foreach (KeyValuePair<string, IFocusInner> Entry in FocusInnerTable)
                {
                    Assert.That(FrameInnerTableAsDictionary[Entry.Key] == Entry.Value);
                    Assert.That(FocusInnerTable.TryGetValue(Entry.Key, out IReadOnlyInner ReadOnlyInnerValue) == FocusInnerTable.TryGetValue(Entry.Key, out IFocusInner FocusInnerValue));
                    Assert.That(FocusInnerTable.TryGetValue(Entry.Key, out IFrameInner FrameInnerValue) == FocusInnerTable.TryGetValue(Entry.Key, out FocusInnerValue));
                    break;
                }

                // FocusNodeStateList

                FirstNodeState = LeafPathInner.FirstNodeState;
                Assert.That(FirstNodeState != null);

                IFocusNodeStateList FocusNodeStateListModify = DebugObjects.GetReferenceByInterface(typeof(IFocusNodeStateList)) as IFocusNodeStateList;
                Assert.That(FocusNodeStateListModify != null);
                Assert.That(FocusNodeStateListModify.Count > 0);
                FirstNodeState = FocusNodeStateListModify[0] as IFocusPlaceholderNodeState;
                Assert.That(FocusNodeStateListModify.Contains((IReadOnlyNodeState)FirstNodeState));
                Assert.That(FocusNodeStateListModify.IndexOf((IReadOnlyNodeState)FirstNodeState) == 0);

                FocusNodeStateListModify.Remove((IReadOnlyNodeState)FirstNodeState);
                FocusNodeStateListModify.Insert(0, (IReadOnlyNodeState)FirstNodeState);
                FocusNodeStateListModify.CopyTo((IReadOnlyNodeState[])(new IFocusNodeState[FocusNodeStateListModify.Count]), 0);

                IReadOnlyNodeStateList FocusNodeStateListModifyAsReadOnly = FocusNodeStateListModify as IReadOnlyNodeStateList;
                Assert.That(FocusNodeStateListModifyAsReadOnly != null);
                Assert.That(FocusNodeStateListModifyAsReadOnly[0] == FocusNodeStateListModify[0]);

                IFrameNodeStateList FocusNodeStateListModifyAsFrame = FocusNodeStateListModify as IFrameNodeStateList;
                Assert.That(FocusNodeStateListModifyAsFrame != null);
                Assert.That(FocusNodeStateListModifyAsFrame[0] == FocusNodeStateListModify[0]);
                FocusNodeStateListModifyAsFrame.GetEnumerator();

                IList<IReadOnlyNodeState> ReadOnlyNodeStateListModifyAsIList = FocusNodeStateListModify as IList<IReadOnlyNodeState>;
                Assert.That(ReadOnlyNodeStateListModifyAsIList != null);
                Assert.That(ReadOnlyNodeStateListModifyAsIList[0] == FocusNodeStateListModify[0]);

                IList<IFrameNodeState> FrameNodeStateListModifyAsIList = FocusNodeStateListModify as IList<IFrameNodeState>;
                Assert.That(FrameNodeStateListModifyAsIList != null);
                Assert.That(FrameNodeStateListModifyAsIList[0] == FocusNodeStateListModify[0]);
                Assert.That(FrameNodeStateListModifyAsIList.IndexOf(FirstNodeState) == 0);
                FrameNodeStateListModifyAsIList.Remove(FirstNodeState);
                FrameNodeStateListModifyAsIList.Insert(0, FirstNodeState);

                IReadOnlyList<IReadOnlyNodeState> ReadOnlyNodeStateListModifyAsIReadOnlyList = FocusNodeStateListModify as IReadOnlyList<IReadOnlyNodeState>;
                Assert.That(ReadOnlyNodeStateListModifyAsIReadOnlyList != null);
                Assert.That(ReadOnlyNodeStateListModifyAsIReadOnlyList[0] == FocusNodeStateListModify[0]);

                IReadOnlyList<IFrameNodeState> FrameNodeStateListModifyAsIReadOnlyList = FocusNodeStateListModify as IReadOnlyList<IFrameNodeState>;
                Assert.That(FrameNodeStateListModifyAsIReadOnlyList != null);
                Assert.That(FrameNodeStateListModifyAsIReadOnlyList[0] == FocusNodeStateListModify[0]);

                ICollection<IReadOnlyNodeState> ReadOnlyNodeStateListModifyAsCollection = FocusNodeStateListModify as ICollection<IReadOnlyNodeState>;
                Assert.That(ReadOnlyNodeStateListModifyAsCollection != null);
                Assert.That(!ReadOnlyNodeStateListModifyAsCollection.IsReadOnly);

                ICollection<IFrameNodeState> FrameNodeStateListModifyAsCollection = FocusNodeStateListModify as ICollection<IFrameNodeState>;
                Assert.That(FrameNodeStateListModifyAsCollection != null);
                Assert.That(!FrameNodeStateListModifyAsCollection.IsReadOnly);
                Assert.That(FrameNodeStateListModifyAsCollection.Contains(FirstNodeState));
                FrameNodeStateListModifyAsCollection.Remove(FirstNodeState);
                FrameNodeStateListModifyAsCollection.Add(FirstNodeState);
                FrameNodeStateListModifyAsCollection.Remove(FirstNodeState);
                FocusNodeStateListModify.Insert(0, FirstNodeState);
                FrameNodeStateListModifyAsCollection.CopyTo(new IFocusNodeState[FrameNodeStateListModifyAsCollection.Count], 0);

                IEnumerable<IReadOnlyNodeState> ReadOnlyNodeStateListModifyAsEnumerable = FocusNodeStateListModify as IEnumerable<IReadOnlyNodeState>;
                Assert.That(ReadOnlyNodeStateListModifyAsEnumerable != null);
                Assert.That(ReadOnlyNodeStateListModifyAsEnumerable.GetEnumerator() != null);

                IEnumerable<IFrameNodeState> FrameNodeStateListModifyAsEnumerable = FocusNodeStateListModify as IEnumerable<IFrameNodeState>;
                Assert.That(FrameNodeStateListModifyAsEnumerable != null);
                Assert.That(FrameNodeStateListModifyAsEnumerable.GetEnumerator() != null);

                // FocusNodeStateReadOnlyList

                IFocusNodeStateReadOnlyList FocusNodeStateList = FocusNodeStateListModify.ToReadOnly() as IFocusNodeStateReadOnlyList;
                Assert.That(FocusNodeStateList != null);
                Assert.That(FocusNodeStateList.Count > 0);
                FirstNodeState = FocusNodeStateList[0] as IFocusPlaceholderNodeState;
                Assert.That(FocusNodeStateList.Contains((IReadOnlyNodeState)FirstNodeState));
                Assert.That(FocusNodeStateList.IndexOf((IReadOnlyNodeState)FirstNodeState) == 0);

                IFrameNodeStateReadOnlyList FrameNodeStateList = FocusNodeStateList;
                Assert.That(FrameNodeStateList.Contains(FirstNodeState));
                Assert.That(FrameNodeStateList.IndexOf(FirstNodeState) == 0);
                Assert.That(FrameNodeStateList[0] == FocusNodeStateList[0]);
                FrameNodeStateList.GetEnumerator();

                IReadOnlyList<IReadOnlyNodeState> ReadOnlyNodeStateListAsIReadOnlyList = FocusNodeStateList as IReadOnlyList<IReadOnlyNodeState>;
                Assert.That(ReadOnlyNodeStateListAsIReadOnlyList[0] == FirstNodeState);

                IReadOnlyList<IFrameNodeState> FrameNodeStateListAsIReadOnlyList = FocusNodeStateList as IReadOnlyList<IFrameNodeState>;
                Assert.That(FrameNodeStateListAsIReadOnlyList[0] == FirstNodeState);

                IEnumerable<IReadOnlyNodeState> ReadOnlyNodeStateListAsEnumerable = FocusNodeStateList as IEnumerable<IReadOnlyNodeState>;
                Assert.That(ReadOnlyNodeStateListAsEnumerable != null);
                Assert.That(ReadOnlyNodeStateListAsEnumerable.GetEnumerator() != null);

                IEnumerable<IFrameNodeState> FrameNodeStateListAsEnumerable = FocusNodeStateList as IEnumerable<IFrameNodeState>;
                Assert.That(FrameNodeStateListAsEnumerable != null);
                Assert.That(FrameNodeStateListAsEnumerable.GetEnumerator() != null);

                // IFocusOperationGroupList

                IFocusOperationGroupReadOnlyList FocusOperationStack = Controller.OperationStack;

                Assert.That(FocusOperationStack.Count > 0);
                IFocusOperationGroup FirstOperationGroup = FocusOperationStack[0];

                IFocusOperationGroupList FocusOperationGroupList = DebugObjects.GetReferenceByInterface(typeof(IFocusOperationGroupList)) as IFocusOperationGroupList;
                if (FocusOperationGroupList != null)
                {
                    IFrameOperationGroupList FrameOperationGroupList = FocusOperationGroupList;
                    Assert.That(FrameOperationGroupList.Count > 0);
                    Assert.That(FrameOperationGroupList[0] == FirstOperationGroup);

                    IList<IFrameOperationGroup> FrameOperationGroupAsIList = FrameOperationGroupList;
                    Assert.That(FrameOperationGroupAsIList.Count > 0);
                    Assert.That(FrameOperationGroupAsIList[0] == FirstOperationGroup);
                    Assert.That(FrameOperationGroupAsIList.IndexOf(FirstOperationGroup) == 0);
                    FrameOperationGroupAsIList.Remove(FirstOperationGroup);
                    FrameOperationGroupAsIList.Insert(0, FirstOperationGroup);

                    ICollection<IFrameOperationGroup> FrameOperationGroupAsICollection = FrameOperationGroupList;
                    Assert.That(FrameOperationGroupAsICollection.Count > 0);
                    Assert.That(!FrameOperationGroupAsICollection.IsReadOnly);
                    Assert.That(FrameOperationGroupAsICollection.Contains(FirstOperationGroup));
                    FrameOperationGroupAsICollection.Remove(FirstOperationGroup);
                    FrameOperationGroupAsICollection.Add(FirstOperationGroup);
                    FrameOperationGroupAsICollection.Remove(FirstOperationGroup);
                    FrameOperationGroupAsIList.Insert(0, FirstOperationGroup);
                    FrameOperationGroupAsICollection.CopyTo(new IFocusOperationGroup[FrameOperationGroupAsICollection.Count], 0);

                    IEnumerable<IFrameOperationGroup> FrameOperationGroupAsIEnumerable = FrameOperationGroupList;
                    FrameOperationGroupAsIEnumerable.GetEnumerator();

                    IReadOnlyList<IFrameOperationGroup> FrameOperationGroupAsIReadOnlyList = FrameOperationGroupList;
                    Assert.That(FrameOperationGroupAsIReadOnlyList.Count > 0);
                    Assert.That(FrameOperationGroupAsIReadOnlyList[0] == FirstOperationGroup);
                }

                // IFocusOperationGroupReadOnlyList

                IFrameOperationGroupReadOnlyList FrameOperationStack = FocusOperationStack;
                Assert.That(FrameOperationStack.Contains(FirstOperationGroup));
                Assert.That(FrameOperationStack.IndexOf(FirstOperationGroup) == 0);

                IEnumerable<IFrameOperationGroup> FrameOperationStackAsIEnumerable = FrameOperationStack;
                FrameOperationStackAsIEnumerable.GetEnumerator();

                // IFocusOperationList

                IFocusOperationReadOnlyList FocusOperationReadOnlyList = FirstOperationGroup.OperationList;

                Assert.That(FocusOperationReadOnlyList.Count > 0);
                IFocusOperation FirstOperation = FocusOperationReadOnlyList[0];

                IFocusOperationList FocusOperationList = DebugObjects.GetReferenceByInterface(typeof(IFocusOperationList)) as IFocusOperationList;
                if (FocusOperationList != null)
                {
                    IFrameOperationList FrameOperationList = FocusOperationList;
                    Assert.That(FrameOperationList.Count > 0);
                    Assert.That(FrameOperationList[0] == FirstOperation);

                    IList<IFrameOperation> FrameOperationAsIList = FrameOperationList;
                    Assert.That(FrameOperationAsIList.Count > 0);
                    Assert.That(FrameOperationAsIList[0] == FirstOperation);
                    Assert.That(FrameOperationAsIList.IndexOf(FirstOperation) == 0);
                    FrameOperationAsIList.Remove(FirstOperation);
                    FrameOperationAsIList.Insert(0, FirstOperation);

                    ICollection<IFrameOperation> FrameOperationAsICollection = FrameOperationList;
                    Assert.That(FrameOperationAsICollection.Count > 0);
                    Assert.That(!FrameOperationAsICollection.IsReadOnly);
                    Assert.That(FrameOperationAsICollection.Contains(FirstOperation));
                    FrameOperationAsICollection.Remove(FirstOperation);
                    FrameOperationAsICollection.Add(FirstOperation);
                    FrameOperationAsICollection.Remove(FirstOperation);
                    FrameOperationAsIList.Insert(0, FirstOperation);
                    FrameOperationAsICollection.CopyTo(new IFocusOperation[FrameOperationAsICollection.Count], 0);

                    IEnumerable<IFrameOperation> FrameOperationAsIEnumerable = FrameOperationList;
                    FrameOperationAsIEnumerable.GetEnumerator();

                    IReadOnlyList<IFrameOperation> FrameOperationAsIReadOnlyList = FrameOperationList;
                    Assert.That(FrameOperationAsIReadOnlyList.Count > 0);
                    Assert.That(FrameOperationAsIReadOnlyList[0] == FirstOperation);
                }

                // IFocusOperationReadOnlyList
                IFrameOperationReadOnlyList FrameOperationReadOnlyList = FocusOperationReadOnlyList;
                Assert.That(FrameOperationReadOnlyList.Contains(FirstOperation));
                Assert.That(FrameOperationReadOnlyList.IndexOf(FirstOperation) == 0);

                IEnumerable<IFrameOperation> FrameOperationReadOnlyListAsIEnumerable = FrameOperationReadOnlyList;
                FrameOperationReadOnlyListAsIEnumerable.GetEnumerator();

                // FocusPlaceholderNodeStateList

                //System.Diagnostics.Debug.Assert(false);
                FirstNodeState = LeafPathInner.FirstNodeState;
                Assert.That(FirstNodeState != null);

                IFocusPlaceholderNodeStateList FocusPlaceholderNodeStateListModify = DebugObjects.GetReferenceByInterface(typeof(IFocusPlaceholderNodeStateList)) as IFocusPlaceholderNodeStateList;
                if (FocusPlaceholderNodeStateListModify != null)
                {
                    Assert.That(FocusPlaceholderNodeStateListModify.Count > 0);
                    FirstNodeState = FocusPlaceholderNodeStateListModify[0] as IFocusPlaceholderNodeState;
                    Assert.That(FocusPlaceholderNodeStateListModify.Contains((IReadOnlyPlaceholderNodeState)FirstNodeState));
                    Assert.That(FocusPlaceholderNodeStateListModify.IndexOf((IReadOnlyPlaceholderNodeState)FirstNodeState) == 0);

                    FocusPlaceholderNodeStateListModify.Remove((IReadOnlyPlaceholderNodeState)FirstNodeState);
                    FocusPlaceholderNodeStateListModify.Insert(0, (IReadOnlyPlaceholderNodeState)FirstNodeState);
                    FocusPlaceholderNodeStateListModify.CopyTo((IReadOnlyPlaceholderNodeState[])(new IFocusPlaceholderNodeState[FocusPlaceholderNodeStateListModify.Count]), 0);

                    IReadOnlyPlaceholderNodeStateList FocusPlaceholderNodeStateListModifyAsReadOnly = FocusPlaceholderNodeStateListModify as IReadOnlyPlaceholderNodeStateList;
                    Assert.That(FocusPlaceholderNodeStateListModifyAsReadOnly != null);
                    Assert.That(FocusPlaceholderNodeStateListModifyAsReadOnly[0] == FocusPlaceholderNodeStateListModify[0]);

                    IFramePlaceholderNodeStateList FocusPlaceholderNodeStateListModifyAsFrame = FocusPlaceholderNodeStateListModify as IFramePlaceholderNodeStateList;
                    Assert.That(FocusPlaceholderNodeStateListModifyAsFrame != null);
                    Assert.That(FocusPlaceholderNodeStateListModifyAsFrame[0] == FocusPlaceholderNodeStateListModify[0]);
                    FocusPlaceholderNodeStateListModifyAsFrame.GetEnumerator();

                    IList<IReadOnlyPlaceholderNodeState> ReadOnlyPlaceholderNodeStateListModifyAsIList = FocusPlaceholderNodeStateListModify as IList<IReadOnlyPlaceholderNodeState>;
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsIList != null);
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsIList[0] == FocusPlaceholderNodeStateListModify[0]);

                    IList<IFramePlaceholderNodeState> FramePlaceholderNodeStateListModifyAsIList = FocusPlaceholderNodeStateListModify as IList<IFramePlaceholderNodeState>;
                    Assert.That(FramePlaceholderNodeStateListModifyAsIList != null);
                    Assert.That(FramePlaceholderNodeStateListModifyAsIList[0] == FocusPlaceholderNodeStateListModify[0]);
                    Assert.That(FramePlaceholderNodeStateListModifyAsIList.IndexOf(FirstNodeState) == 0);
                    FramePlaceholderNodeStateListModifyAsIList.Remove(FirstNodeState);
                    FramePlaceholderNodeStateListModifyAsIList.Insert(0, FirstNodeState);

                    IReadOnlyList<IReadOnlyPlaceholderNodeState> ReadOnlyPlaceholderNodeStateListModifyAsIReadOnlyList = FocusPlaceholderNodeStateListModify as IReadOnlyList<IReadOnlyPlaceholderNodeState>;
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsIReadOnlyList != null);
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsIReadOnlyList[0] == FocusPlaceholderNodeStateListModify[0]);

                    IReadOnlyList<IFramePlaceholderNodeState> FramePlaceholderNodeStateListModifyAsIReadOnlyList = FocusPlaceholderNodeStateListModify as IReadOnlyList<IFramePlaceholderNodeState>;
                    Assert.That(FramePlaceholderNodeStateListModifyAsIReadOnlyList != null);
                    Assert.That(FramePlaceholderNodeStateListModifyAsIReadOnlyList[0] == FocusPlaceholderNodeStateListModify[0]);

                    ICollection<IReadOnlyPlaceholderNodeState> ReadOnlyPlaceholderNodeStateListModifyAsCollection = FocusPlaceholderNodeStateListModify as ICollection<IReadOnlyPlaceholderNodeState>;
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsCollection != null);
                    Assert.That(!ReadOnlyPlaceholderNodeStateListModifyAsCollection.IsReadOnly);
                    ReadOnlyPlaceholderNodeStateListModifyAsCollection.Remove(FirstNodeState);
                    ReadOnlyPlaceholderNodeStateListModifyAsCollection.Add(FirstNodeState);
                    ReadOnlyPlaceholderNodeStateListModifyAsCollection.Remove(FirstNodeState);
                    FramePlaceholderNodeStateListModifyAsIList.Insert(0, FirstNodeState);

                    ICollection<IFramePlaceholderNodeState> FramePlaceholderNodeStateListModifyAsCollection = FocusPlaceholderNodeStateListModify as ICollection<IFramePlaceholderNodeState>;
                    Assert.That(FramePlaceholderNodeStateListModifyAsCollection != null);
                    Assert.That(!FramePlaceholderNodeStateListModifyAsCollection.IsReadOnly);
                    Assert.That(FramePlaceholderNodeStateListModifyAsCollection.Contains(FirstNodeState));
                    FramePlaceholderNodeStateListModifyAsCollection.Remove(FirstNodeState);
                    FramePlaceholderNodeStateListModifyAsCollection.Add(FirstNodeState);
                    FramePlaceholderNodeStateListModifyAsCollection.Remove(FirstNodeState);
                    FocusPlaceholderNodeStateListModify.Insert(0, FirstNodeState);
                    FramePlaceholderNodeStateListModifyAsCollection.CopyTo(new IFocusPlaceholderNodeState[FramePlaceholderNodeStateListModifyAsCollection.Count], 0);

                    IEnumerable<IReadOnlyPlaceholderNodeState> ReadOnlyPlaceholderNodeStateListModifyAsEnumerable = FocusPlaceholderNodeStateListModify as IEnumerable<IReadOnlyPlaceholderNodeState>;
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsEnumerable != null);
                    Assert.That(ReadOnlyPlaceholderNodeStateListModifyAsEnumerable.GetEnumerator() != null);

                    IEnumerable<IFramePlaceholderNodeState> FramePlaceholderNodeStateListModifyAsEnumerable = FocusPlaceholderNodeStateListModify as IEnumerable<IFramePlaceholderNodeState>;
                    Assert.That(FramePlaceholderNodeStateListModifyAsEnumerable != null);
                    Assert.That(FramePlaceholderNodeStateListModifyAsEnumerable.GetEnumerator() != null);
                }

                // FocusPlaceholderNodeStateReadOnlyList

                IFocusPlaceholderNodeStateReadOnlyList FocusPlaceholderNodeStateList = FocusPlaceholderNodeStateListModify != null ? FocusPlaceholderNodeStateListModify.ToReadOnly() as IFocusPlaceholderNodeStateReadOnlyList : null;

                if (FocusPlaceholderNodeStateList != null)
                {
                    Assert.That(FocusPlaceholderNodeStateList.Count > 0);
                    FirstNodeState = FocusPlaceholderNodeStateList[0] as IFocusPlaceholderNodeState;
                    Assert.That(FocusPlaceholderNodeStateList.Contains((IReadOnlyPlaceholderNodeState)FirstNodeState));
                    Assert.That(FocusPlaceholderNodeStateList.IndexOf((IReadOnlyPlaceholderNodeState)FirstNodeState) == 0);

                    IFramePlaceholderNodeStateReadOnlyList FramePlaceholderNodeStateList = FocusPlaceholderNodeStateList;
                    Assert.That(FramePlaceholderNodeStateList.Contains(FirstNodeState));
                    Assert.That(FramePlaceholderNodeStateList.IndexOf(FirstNodeState) == 0);
                    Assert.That(FramePlaceholderNodeStateList[0] == FocusPlaceholderNodeStateList[0]);
                    FramePlaceholderNodeStateList.GetEnumerator();

                    IReadOnlyList<IReadOnlyPlaceholderNodeState> ReadOnlyPlaceholderNodeStateListAsIReadOnlyList = FocusPlaceholderNodeStateList as IReadOnlyList<IReadOnlyPlaceholderNodeState>;
                    Assert.That(ReadOnlyPlaceholderNodeStateListAsIReadOnlyList[0] == FirstNodeState);

                    IReadOnlyList<IFramePlaceholderNodeState> FramePlaceholderNodeStateListAsIReadOnlyList = FocusPlaceholderNodeStateList as IReadOnlyList<IFramePlaceholderNodeState>;
                    Assert.That(FramePlaceholderNodeStateListAsIReadOnlyList[0] == FirstNodeState);

                    IEnumerable<IReadOnlyPlaceholderNodeState> ReadOnlyPlaceholderNodeStateListAsEnumerable = FocusPlaceholderNodeStateList as IEnumerable<IReadOnlyPlaceholderNodeState>;
                    Assert.That(ReadOnlyPlaceholderNodeStateListAsEnumerable != null);
                    Assert.That(ReadOnlyPlaceholderNodeStateListAsEnumerable.GetEnumerator() != null);

                    IEnumerable<IFramePlaceholderNodeState> FramePlaceholderNodeStateListAsEnumerable = FocusPlaceholderNodeStateList as IEnumerable<IFramePlaceholderNodeState>;
                    Assert.That(FramePlaceholderNodeStateListAsEnumerable != null);
                    Assert.That(FramePlaceholderNodeStateListAsEnumerable.GetEnumerator() != null);
                }

                // IFocusStateViewDictionary
                IFocusStateViewDictionary FocusStateViewTable = ControllerView.StateViewTable;
                IFrameStateViewDictionary FrameStateViewTable = ControllerView.StateViewTable;
                FrameStateViewTable.GetEnumerator();

                IDictionary<IReadOnlyNodeState, IReadOnlyNodeStateView> ReadOnlyStateViewTableAsDictionary = FocusStateViewTable;
                Assert.That(ReadOnlyStateViewTableAsDictionary != null);
                Assert.That(ReadOnlyStateViewTableAsDictionary.TryGetValue(RootState, out IReadOnlyNodeStateView StateViewTableAsDictionaryValue) == FocusStateViewTable.TryGetValue(RootState, out IReadOnlyNodeStateView StateViewTableValue));
                Assert.That(ReadOnlyStateViewTableAsDictionary.Keys != null);
                Assert.That(ReadOnlyStateViewTableAsDictionary.Values != null);

                ICollection<KeyValuePair<IReadOnlyNodeState, IReadOnlyNodeStateView>> ReadOnlyStateViewTableAsCollection = FocusStateViewTable;
                Assert.That(!ReadOnlyStateViewTableAsCollection.IsReadOnly);

                ICollection<KeyValuePair<IFrameNodeState, IFrameNodeStateView>> FrameStateViewTableAsCollection = FocusStateViewTable;
                Assert.That(!FrameStateViewTableAsCollection.IsReadOnly);

                IDictionary<IFrameNodeState, IFrameNodeStateView> FrameStateViewTableAsDictionary = FocusStateViewTable;
                Assert.That(FrameStateViewTableAsDictionary != null);
                Assert.That(FrameStateViewTableAsDictionary.TryGetValue(RootState, out IFrameNodeStateView FrameStateViewTableAsDictionaryValue) == FocusStateViewTable.TryGetValue(RootState, out StateViewTableValue));
                Assert.That(FrameStateViewTableAsDictionary.Keys != null);
                Assert.That(FrameStateViewTableAsDictionary.Values != null);

                foreach (KeyValuePair<IReadOnlyNodeState, IReadOnlyNodeStateView> Entry in ReadOnlyStateViewTableAsCollection)
                {
                    Assert.That(ReadOnlyStateViewTableAsCollection.Contains(Entry));
                    ReadOnlyStateViewTableAsCollection.Remove(Entry);
                    ReadOnlyStateViewTableAsCollection.Add(Entry);
                    ReadOnlyStateViewTableAsCollection.CopyTo(new KeyValuePair<IReadOnlyNodeState, IReadOnlyNodeStateView>[FocusStateViewTable.Count], 0);

                    Assert.That(FrameStateViewTableAsDictionary.ContainsKey((IFrameNodeState)Entry.Key));
                    FrameStateViewTableAsDictionary.Remove((IFrameNodeState)Entry.Key);
                    FrameStateViewTableAsDictionary.Add((IFrameNodeState)Entry.Key, (IFrameNodeStateView)Entry.Value);

                    break;
                }

                foreach (KeyValuePair<IFrameNodeState, IFrameNodeStateView> Entry in FrameStateViewTableAsCollection)
                {
                    Assert.That(FrameStateViewTableAsDictionary.ContainsKey(Entry.Key));
                    Assert.That(FrameStateViewTableAsDictionary[Entry.Key] == Entry.Value);
                    FrameStateViewTableAsDictionary.Remove(Entry.Key);
                    FrameStateViewTableAsDictionary.Add(Entry.Key, Entry.Value);

                    Assert.That(FrameStateViewTableAsCollection.Contains(Entry));
                    FrameStateViewTableAsCollection.Remove(Entry);
                    FrameStateViewTableAsCollection.Add(Entry);
                    FrameStateViewTableAsCollection.CopyTo(new KeyValuePair<IFrameNodeState, IFrameNodeStateView>[FocusStateViewTable.Count], 0);

                    break;
                }

                IEnumerable<KeyValuePair<IFrameNodeState, IFrameNodeStateView>> FrameStateViewTableAsEnumerable = FocusStateViewTable;
                FrameStateViewTableAsEnumerable.GetEnumerator();

            }
        }
        #endregion
    }
}
