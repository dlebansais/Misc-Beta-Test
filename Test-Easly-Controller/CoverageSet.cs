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

        private static IMain CreateRoot(Imperfections imperfection)
        {
            Guid LeafGuid0 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887270}");
            Guid LeafGuid1 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887271}");
            Guid LeafGuid2 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887272}");
            Guid LeafGuid3 = new Guid("{B18935CD-AC21-4905-9901-9F25CD887273}");
            Guid MainGuid = new Guid("{EDBDC354-C70B-4BAF-AE1B-C342CD9BFADE}");

            Leaf Placeholder = new Leaf();

            BaseNode.IDocument PlaceholderDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", imperfection == Imperfections.BadGuid ? MainGuid : LeafGuid0);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(Placeholder, PlaceholderDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(Placeholder, nameof(ILeaf.Text), "placeholder");

            Leaf OptionalLeaf = new Leaf();

            BaseNode.IDocument OptionalLeafDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", LeafGuid1);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(OptionalLeaf, OptionalLeafDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(OptionalLeaf, nameof(ILeaf.Text), "optional");

            Easly.IOptionalReference<ILeaf> Optional = BaseNodeHelper.OptionalReferenceHelper<ILeaf>.CreateReference(OptionalLeaf);

            Leaf FirstChild = new Leaf();

            BaseNode.IDocument FirstChildDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", LeafGuid2);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(FirstChild, FirstChildDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(FirstChild, nameof(ILeaf.Text), "first child");

            BaseNode.IBlockList<ILeaf, Leaf> LeafBlocks = BaseNodeHelper.BlockListHelper<ILeaf, Leaf>.CreateSimpleBlockList(FirstChild);

            Leaf FirstPath = new Leaf();

            BaseNode.IDocument FirstPathDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("leaf doc", LeafGuid3);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(FirstPath, FirstPathDocument);
            BaseNodeHelper.NodeTreeHelper.SetString(FirstPath, nameof(ILeaf.Text), "first child");

            IList<ILeaf> LeafPath = new List<ILeaf>();
            LeafPath.Add(FirstPath);

            Main Root = new Main();

            BaseNode.IDocument RootDocument = BaseNodeHelper.NodeHelper.CreateSimpleDocumentation("main doc", MainGuid);
            BaseNodeHelper.NodeTreeHelper.SetDocumentation(Root, RootDocument);
            BaseNodeHelper.NodeTreeHelperChild.SetChildNode(Root, nameof(IMain.Placeholder), Placeholder);
            BaseNodeHelper.NodeTreeHelperOptional.SetOptionalReference(Root, nameof(IMain.Optional), (Easly.IOptionalReference)Optional);
            BaseNodeHelper.NodeTreeHelperBlockList.SetBlockList(Root, nameof(IMain.LeafBlocks), (BaseNode.IBlockList)LeafBlocks);
            BaseNodeHelper.NodeTreeHelperList.SetChildNodeList(Root, nameof(IMain.LeafPath), (IList)LeafPath);

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

            RootIndex1 = new ReadOnlyRootNodeIndex(RootNode);
            Assert.That(RootIndex1.Node == RootNode);

            Assert.That(RootIndex0.IsEqual(CompareEqual.New(), RootIndex0));

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

            //System.Diagnostics.Debug.Assert(false);
            Assert.That(RootState.InnerTable.Count == 4);
            Assert.That(RootState.InnerTable.ContainsKey(nameof(IMain.Placeholder)));
            Assert.That(RootState.InnerTable.ContainsKey(nameof(IMain.Optional)));
            Assert.That(RootState.InnerTable.ContainsKey(nameof(IMain.LeafBlocks)));
            Assert.That(RootState.InnerTable.ContainsKey(nameof(IMain.LeafPath)));
            
            IReadOnlyPlaceholderInner<IReadOnlyBrowsingPlaceholderNodeIndex> PlaceholderInner = RootState.InnerTable[nameof(IMain.Placeholder)] as IReadOnlyPlaceholderInner<IReadOnlyBrowsingPlaceholderNodeIndex>;
            Assert.That(PlaceholderInner != null);

            IReadOnlyBrowsingPlaceholderNodeIndex PlaceholderNodeIndex = PlaceholderInner.ChildState.ParentIndex as IReadOnlyBrowsingPlaceholderNodeIndex;
            Assert.That(PlaceholderNodeIndex != null);
            Assert.That(Controller0.Contains(PlaceholderNodeIndex));

            IReadOnlyOptionalInner<IReadOnlyBrowsingOptionalNodeIndex> OptionalInner = RootState.InnerTable[nameof(IMain.Optional)] as IReadOnlyOptionalInner<IReadOnlyBrowsingOptionalNodeIndex>;
            Assert.That(OptionalInner != null);

            IReadOnlyBrowsingOptionalNodeIndex OptionalNodeIndex = OptionalInner.ChildState.ParentIndex;
            Assert.That(OptionalNodeIndex != null);
            Assert.That(Controller0.Contains(OptionalNodeIndex));
            Assert.That(Controller0.IsAssigned(OptionalNodeIndex) == false);

            Assert.That(Controller0.GetDiscreteValue(RootIndex0, nameof(IMain.CopySpecification)) == 0);

            IReadOnlyController Controller1 = ReadOnlyController.Create(RootIndex0);
            Assert.That(Controller0.IsEqual(CompareEqual.New(), Controller0));
            Assert.That(Controller0.IsEqual(CompareEqual.New(), Controller1));
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
                    Assert.That(ControllerView0.IsEqual(CompareEqual.New(), ControllerView1));
                }
            }
        }
        #endregion
    }
}
