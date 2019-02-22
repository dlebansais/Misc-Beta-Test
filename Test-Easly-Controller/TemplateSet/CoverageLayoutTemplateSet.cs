﻿using EaslyController.Layout;
using System.IO;
using System.Text;
using System.Windows.Markup;

namespace TestDebug
{
    public class CoverageLayoutTemplateSet
    {
        #region Init
#if !TRAVIS
        static CoverageLayoutTemplateSet()
        {
            NodeTemplateDictionary = LoadTemplate(LayoutTemplateListString);
            ILayoutTemplateReadOnlyDictionary LayoutCustomNodeTemplates = NodeTemplateDictionary.ToReadOnly() as ILayoutTemplateReadOnlyDictionary;
            BlockTemplateDictionary = LoadTemplate(LayoutBlockTemplateString);
            ILayoutTemplateReadOnlyDictionary LayoutCustomBlockTemplates = BlockTemplateDictionary.ToReadOnly() as ILayoutTemplateReadOnlyDictionary;
            LayoutTemplateSet = new LayoutTemplateSet(LayoutCustomNodeTemplates, LayoutCustomBlockTemplates);
        }

        private static ILayoutTemplateDictionary LoadTemplate(string s)
        {
            byte[] ByteArray = Encoding.UTF8.GetBytes(s);
            using (MemoryStream ms = new MemoryStream(ByteArray))
            {
                Templates = XamlReader.Parse(s) as ILayoutTemplateList;

                LayoutTemplateDictionary TemplateDictionary = new LayoutTemplateDictionary();
                foreach (ILayoutTemplate Item in Templates)
                {
                    Item.Root.UpdateParent(Item, LayoutFrame.LayoutRoot);
                    RecursivelyCheckParent(Item.Root, Item.Root);
                    TemplateDictionary.Add(Item.NodeType, Item);
                }

                return TemplateDictionary;
            }
        }

        private CoverageLayoutTemplateSet()
        {
        }

        private static void RecursivelyCheckParent(ILayoutFrame rootFrame, ILayoutFrame frame)
        {
            EaslyController.Frame.IFrameFrame AsFrameRootFrame = rootFrame;
            EaslyController.Frame.IFrameFrame AsFrameFrame = frame;
            EaslyController.Focus.IFocusFrame AsFocusRootFrame = rootFrame;
            EaslyController.Focus.IFocusFrame AsFocusFrame = frame;

            //System.Diagnostics.Debug.Assert(false);
            System.Diagnostics.Debug.Assert(rootFrame.ParentTemplate == frame.ParentTemplate);
            System.Diagnostics.Debug.Assert(AsFrameRootFrame.ParentTemplate == AsFrameFrame.ParentTemplate);
            System.Diagnostics.Debug.Assert(AsFocusRootFrame.ParentTemplate == AsFocusFrame.ParentTemplate);

            System.Diagnostics.Debug.Assert(frame.ParentFrame != null || frame == rootFrame);

            if (frame is ILayoutBlockFrameWithVisibility AsBlockFrame)
            {
                ILayoutBlockFrameVisibility BlockVisibility = AsBlockFrame.BlockVisibility;
            }

            if (frame is ILayoutNodeFrameWithVisibility AsNodeFrameWithVisibility)
            {
                ILayoutNodeFrameVisibility Visibility = AsNodeFrameWithVisibility.Visibility;
            }

            if (frame is ILayoutPanelFrame AsPanelFrame)
            {
                foreach (ILayoutFrame Item in AsPanelFrame.Items)
                {
                    EaslyController.Frame.IFrameFrame AsFrameItem = Item;

                    System.Diagnostics.Debug.Assert(Item.ParentFrame == AsPanelFrame);
                    System.Diagnostics.Debug.Assert(AsFrameItem.ParentFrame == AsPanelFrame);

                    RecursivelyCheckParent(rootFrame, Item);
                }
            }

            else if (frame is ILayoutSelectionFrame AsSelectionFrame)
            {
                foreach (ILayoutFrame Item in AsSelectionFrame.Items)
                {
                    EaslyController.Frame.IFrameFrame AsFrameItem = Item;

                    System.Diagnostics.Debug.Assert(Item.ParentFrame == AsSelectionFrame);
                    System.Diagnostics.Debug.Assert(AsFrameItem.ParentFrame == AsSelectionFrame);

                    RecursivelyCheckParent(rootFrame, Item);
                }
            }

            else if (frame is ILayoutDiscreteFrame AsDiscreteFrame)
            {
                foreach (ILayoutKeywordFrame Item in AsDiscreteFrame.Items)
                {
                    EaslyController.Frame.IFrameFrame AsFrameItem = Item;

                    System.Diagnostics.Debug.Assert(Item.ParentFrame == AsDiscreteFrame);
                    System.Diagnostics.Debug.Assert(AsFrameItem.ParentFrame == AsDiscreteFrame);

                    RecursivelyCheckParent(rootFrame, Item);
                }
            }
        }

#endif
        #endregion

        #region Properties
        public static ILayoutTemplateDictionary NodeTemplateDictionary { get; private set; }
        public static ILayoutTemplateDictionary BlockTemplateDictionary { get; private set; }
        public static ILayoutTemplateSet LayoutTemplateSet { get; private set; }
        public static ILayoutTemplateList Templates { get; private set; }
        #endregion

        #region Node Templates
        static string LayoutTemplateListString =
@"<LayoutTemplateList
    xmlns=""clr-namespace:EaslyController.Layout;assembly=Easly-Controller""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:xaml=""clr-namespace:EaslyController.Xaml;assembly=Easly-Controller""
    xmlns:easly=""clr-namespace:BaseNode;assembly=Easly-Language""
    xmlns:cov=""clr-namespace:Coverage;assembly=Test-Easly-Controller""
    xmlns:const=""clr-namespace:EaslyController.Constants;assembly=Easly-Controller"">
    <LayoutNodeTemplate NodeType=""{xaml:Type cov:ILeaf}"" IsComplex=""True"" IsSimple=""True"">
        <LayoutVerticalPanelFrame>
            <LayoutCommentFrame/>
            <LayoutTextValueFrame PropertyName=""Text""/>
            <LayoutKeywordFrame Text=""first"">
                <LayoutKeywordFrame.Visibility>
                    <LayoutNotFirstItemFrameVisibility/>
                </LayoutKeywordFrame.Visibility>
            </LayoutKeywordFrame>
            <LayoutKeywordFrame Text=""not first"">
                <LayoutKeywordFrame.Visibility>
                    <LayoutNotFirstItemFrameVisibility/>
                </LayoutKeywordFrame.Visibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type cov:ITree}"">
        <LayoutVerticalPanelFrame>
            <LayoutCommentFrame/>
            <LayoutPlaceholderFrame PropertyName=""Placeholder""/>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.Dot}"">
                <LayoutSymbolFrame.Visibility>
                    <LayoutComplexFrameVisibility PropertyName=""Placeholder""/>
                </LayoutSymbolFrame.Visibility>
            </LayoutSymbolFrame>
            <LayoutDiscreteFrame PropertyName=""ValueBoolean"">
                <LayoutKeywordFrame>True</LayoutKeywordFrame>
                <LayoutKeywordFrame>False</LayoutKeywordFrame>
            </LayoutDiscreteFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.Dot}""/>
            <LayoutDiscreteFrame PropertyName=""ValueEnum"">
                <LayoutKeywordFrame>Any</LayoutKeywordFrame>
                <LayoutKeywordFrame>Reference</LayoutKeywordFrame>
                <LayoutKeywordFrame>Value</LayoutKeywordFrame>
            </LayoutDiscreteFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type cov:IMain}"">
        <LayoutVerticalPanelFrame>
            <LayoutCommentFrame/>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.HorizontalLine}""/>
            <LayoutVerticalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.HorizontalLine}""/>
            </LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}""/>
                <LayoutHorizontalPanelFrame>
                    <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}""/>
                </LayoutHorizontalPanelFrame>
                <LayoutPlaceholderFrame PropertyName=""PlaceholderTree""/>
                <LayoutPlaceholderFrame PropertyName=""PlaceholderLeaf""/>
                <LayoutOptionalFrame PropertyName=""UnassignedOptionalLeaf"" />
                <LayoutOptionalFrame PropertyName=""AssignedOptionalTree"" />
                <LayoutOptionalFrame PropertyName=""AssignedOptionalLeaf"" />
                <LayoutInsertFrame CollectionName=""LeafBlocks""/>
                <LayoutHorizontalBlockListFrame PropertyName=""LeafBlocks"" Separator=""Comma"">
                    <LayoutHorizontalBlockListFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""LeafPath""/>
                    </LayoutHorizontalBlockListFrame.Visibility>
                    <LayoutHorizontalBlockListFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutHorizontalBlockListFrame.Selectors>
                </LayoutHorizontalBlockListFrame>
                <LayoutVerticalListFrame PropertyName=""LeafPath"" IsPreferred=""True"">
                    <LayoutVerticalListFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""LeafBlocks""/>
                    </LayoutVerticalListFrame.Visibility>
                </LayoutVerticalListFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""LeafBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutDiscreteFrame PropertyName=""ValueBoolean"">
                        <LayoutKeywordFrame>True</LayoutKeywordFrame>
                        <LayoutKeywordFrame>False</LayoutKeywordFrame>
                    </LayoutDiscreteFrame>
                </LayoutVerticalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""ValueEnum"">
                    <LayoutKeywordFrame>Any</LayoutKeywordFrame>
                    <LayoutKeywordFrame>Reference</LayoutKeywordFrame>
                    <LayoutKeywordFrame>Value</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutTextValueFrame PropertyName=""ValueString""/>
                <LayoutCharacterFrame PropertyName=""ValueString"">
                    <LayoutCharacterFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""PlaceholderTree""/>
                    </LayoutCharacterFrame.Visibility>
                </LayoutCharacterFrame>
                <LayoutCharacterFrame PropertyName=""ValueString""/>
                <LayoutNumberFrame PropertyName=""ValueString"">
                    <LayoutNumberFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""PlaceholderTree""/>
                    </LayoutNumberFrame.Visibility>
                </LayoutNumberFrame>
                <LayoutNumberFrame PropertyName=""ValueString""/>
                <LayoutKeywordFrame Text=""end"">
                    <LayoutKeywordFrame.Visibility>
                        <LayoutMixedFrameVisibility>
                            <LayoutCountFrameVisibility PropertyName=""LeafBlocks""/>
                            <LayoutCountFrameVisibility PropertyName=""LeafPath""/>
                            <LayoutOptionalFrameVisibility PropertyName=""AssignedOptionalTree""/>
                            <LayoutOptionalFrameVisibility PropertyName=""UnassignedOptionalLeaf""/>
                        </LayoutMixedFrameVisibility>
                    </LayoutKeywordFrame.Visibility>
                </LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type cov:IRoot}"">
        <LayoutHorizontalPanelFrame>
            <LayoutCommentFrame/>
            <LayoutHorizontalBlockListFrame PropertyName=""MainBlocksH"">
                <LayoutHorizontalBlockListFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""LeafPathH""/>
                </LayoutHorizontalBlockListFrame.Visibility>
                <LayoutHorizontalBlockListFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IDeferredBody}"" SelectorName=""Overload""/>
                </LayoutHorizontalBlockListFrame.Selectors>
            </LayoutHorizontalBlockListFrame>
            <LayoutVerticalBlockListFrame PropertyName=""MainBlocksV"" HasTabulationMargin=""True"" Separator=""Line"">
                <LayoutVerticalBlockListFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""LeafPathV""/>
                </LayoutVerticalBlockListFrame.Visibility>
                <LayoutVerticalBlockListFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IDeferredBody}"" SelectorName=""Overload""/>
                </LayoutVerticalBlockListFrame.Selectors>
            </LayoutVerticalBlockListFrame>
            <LayoutInsertFrame CollectionName=""UnassignedOptionalMain.LeafBlocks""/>
            <LayoutOptionalFrame PropertyName=""UnassignedOptionalMain"" />
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""LeafPathH""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutTextValueFrame PropertyName=""ValueString""/>
            </LayoutVerticalPanelFrame>
            <LayoutHorizontalListFrame PropertyName=""LeafPathH"" Separator=""Comma"">
                <LayoutHorizontalListFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""MainBlocksH""/>
                </LayoutHorizontalListFrame.Visibility>
                <LayoutHorizontalListFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IDeferredBody}"" SelectorName=""Overload""/>
                </LayoutHorizontalListFrame.Selectors>
            </LayoutHorizontalListFrame>
            <LayoutVerticalListFrame PropertyName=""LeafPathV"" Separator=""Line"">
                <LayoutVerticalListFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""MainBlocksV""/>
                </LayoutVerticalListFrame.Visibility>
                <LayoutVerticalListFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IDeferredBody}"" SelectorName=""Overload""/>
                </LayoutVerticalListFrame.Selectors>
            </LayoutVerticalListFrame >
            <LayoutOptionalFrame PropertyName=""UnassignedOptionalLeaf"">
                <LayoutOptionalFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""MainBlocksV""/>
                </LayoutOptionalFrame.Visibility>
            </LayoutOptionalFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IAssertion}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutOptionalFrame PropertyName=""Tag"" />
                <LayoutKeywordFrame>:</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""BooleanExpression"" />
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IAttachment}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame Text=""else"">
                    <LayoutKeywordFrame.Visibility>
                        <LayoutNotFirstItemFrameVisibility/>
                    </LayoutKeywordFrame.Visibility>
                </LayoutKeywordFrame>
                <LayoutKeywordFrame>as</LayoutKeywordFrame>
                <LayoutHorizontalBlockListFrame PropertyName=""AttachTypeBlocks""/>
                <LayoutInsertFrame CollectionName=""Instructions.InstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
            </LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""Instructions"" />
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IClass}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""CopySpecification"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""CopySpecification"" DefaultValue=""1""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>any</LayoutKeywordFrame>
                    <LayoutKeywordFrame>reference</LayoutKeywordFrame>
                    <LayoutKeywordFrame>value</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutDiscreteFrame PropertyName=""Cloneable"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""Cloneable""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>cloneable</LayoutKeywordFrame>
                    <LayoutKeywordFrame>single</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutDiscreteFrame PropertyName=""Comparable"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""Comparable""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>comparable</LayoutKeywordFrame>
                    <LayoutKeywordFrame>incomparable</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutDiscreteFrame PropertyName=""IsAbstract"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""IsAbstract""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>instanceable</LayoutKeywordFrame>
                    <LayoutKeywordFrame>abstract</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutKeywordFrame>class</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""EntityName"" IsPreferred=""True""/>
                <LayoutHorizontalPanelFrame>
                    <LayoutHorizontalPanelFrame.Visibility>
                        <LayoutOptionalFrameVisibility PropertyName=""FromIdentifier""/>
                    </LayoutHorizontalPanelFrame.Visibility>
                    <LayoutKeywordFrame>from</LayoutKeywordFrame>
                    <LayoutOptionalFrame PropertyName=""FromIdentifier"">
                        <LayoutOptionalFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                        </LayoutOptionalFrame.Selectors>
                    </LayoutOptionalFrame>
                </LayoutHorizontalPanelFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ImportBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>import</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""ImportBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""ImportBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""GenericBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>generic</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""GenericBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""GenericBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ExportBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>export</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""ExportBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""ExportBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""TypedefBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>typedef</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""TypedefBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""TypedefBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""InheritanceBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>inheritance</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""InheritanceBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""InheritanceBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""DiscreteBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>discrete</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""DiscreteBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""DiscreteBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ClassReplicateBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>replicate</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""ClassReplicateBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""ClassReplicateBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""FeatureBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>feature</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""FeatureBlocks"" ItemType=""{xaml:Type easly:AttributeFeature}""/>
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""FeatureBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ConversionBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>conversion</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""ConversionBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""ConversionBlocks"">
                    <LayoutVerticalBlockListFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                    </LayoutVerticalBlockListFrame.Selectors>
                </LayoutVerticalBlockListFrame>
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""InvariantBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>invariant</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""InvariantBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""InvariantBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.Visibility>
                    <LayoutMixedFrameVisibility>
                        <LayoutCountFrameVisibility PropertyName=""ImportBlocks""/>
                        <LayoutCountFrameVisibility PropertyName=""GenericBlocks""/>
                        <LayoutCountFrameVisibility PropertyName=""ExportBlocks""/>
                        <LayoutCountFrameVisibility PropertyName=""TypedefBlocks""/>
                        <LayoutCountFrameVisibility PropertyName=""InheritanceBlocks""/>
                        <LayoutCountFrameVisibility PropertyName=""DiscreteBlocks""/>
                        <LayoutCountFrameVisibility PropertyName=""ClassReplicateBlocks""/>
                        <LayoutCountFrameVisibility PropertyName=""FeatureBlocks""/>
                        <LayoutCountFrameVisibility PropertyName=""ConversionBlocks""/>
                        <LayoutCountFrameVisibility PropertyName=""InvariantBlocks""/>
                    </LayoutMixedFrameVisibility>
                </LayoutKeywordFrame.Visibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IClassReplicate}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""ReplicateName"" />
            <LayoutKeywordFrame>to</LayoutKeywordFrame>
            <LayoutHorizontalBlockListFrame PropertyName=""PatternBlocks""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:ICommandOverload}"">
        <LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ParameterBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>parameter</LayoutKeywordFrame>
                    <LayoutDiscreteFrame PropertyName=""ParameterEnd"">
                        <LayoutDiscreteFrame.Visibility>
                            <LayoutDefaultDiscreteFrameVisibility PropertyName=""ParameterEnd""/>
                        </LayoutDiscreteFrame.Visibility>
                        <LayoutKeywordFrame>closed</LayoutKeywordFrame>
                        <LayoutKeywordFrame>open</LayoutKeywordFrame>
                    </LayoutDiscreteFrame>
                    <LayoutInsertFrame CollectionName=""ParameterBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""ParameterBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""CommandBody"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IDeferredBody}"" SelectorName=""Overload""/>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IEffectiveBody}"" SelectorName=""Overload""/>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IExternBody}"" SelectorName=""Overload""/>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IPrecursorBody}"" SelectorName=""Overload""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:ICommandOverloadType}"">
        <LayoutHorizontalPanelFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}""/>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>parameter</LayoutKeywordFrame>
                        <LayoutDiscreteFrame PropertyName=""ParameterEnd"">
                            <LayoutDiscreteFrame.Visibility>
                                <LayoutDefaultDiscreteFrameVisibility PropertyName=""ParameterEnd""/>
                            </LayoutDiscreteFrame.Visibility>
                            <LayoutKeywordFrame>closed</LayoutKeywordFrame>
                            <LayoutKeywordFrame>open</LayoutKeywordFrame>
                        </LayoutDiscreteFrame>
                        <LayoutInsertFrame CollectionName=""ParameterBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""ParameterBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>require</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>exception</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                        <LayoutVerticalBlockListFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                        </LayoutVerticalBlockListFrame.Selectors>
                    </LayoutVerticalBlockListFrame>
                </LayoutVerticalPanelFrame>
                <LayoutKeywordFrame>end</LayoutKeywordFrame>
            </LayoutVerticalPanelFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightBracket}""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IConditional}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame Text=""else"">
                    <LayoutKeywordFrame.Visibility>
                        <LayoutNotFirstItemFrameVisibility/>
                    </LayoutKeywordFrame.Visibility>
                </LayoutKeywordFrame>
                <LayoutKeywordFrame>if</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""BooleanExpression""/>
                <LayoutKeywordFrame>then</LayoutKeywordFrame>
                <LayoutInsertFrame CollectionName=""Instructions.InstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
            </LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""Instructions"" />
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IConstraint}"">
        <LayoutVerticalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""ParentType"" />
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""RenameBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>rename</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""RenameBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""RenameBlocks"" />
            </LayoutVerticalPanelFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IContinuation}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame>execute</LayoutKeywordFrame>
                <LayoutInsertFrame CollectionName=""Instructions.InstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutPlaceholderFrame PropertyName=""Instructions"" />
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""CleanupBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>cleanup</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""CleanupBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""CleanupBlocks"" />
                </LayoutVerticalPanelFrame>
            </LayoutVerticalPanelFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IDiscrete}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""EntityName"" />
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.Visibility>
                    <LayoutOptionalFrameVisibility PropertyName=""NumericValue""/>
                </LayoutHorizontalPanelFrame.Visibility>
                <LayoutKeywordFrame>=</LayoutKeywordFrame>
                <LayoutOptionalFrame PropertyName=""NumericValue"" />
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IEntityDeclaration}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""EntityName"" />
            <LayoutKeywordFrame>:</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""EntityType"" />
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.Visibility>
                    <LayoutOptionalFrameVisibility PropertyName=""DefaultValue""/>
                </LayoutHorizontalPanelFrame.Visibility>
                <LayoutKeywordFrame>=</LayoutKeywordFrame>
                <LayoutOptionalFrame PropertyName=""DefaultValue"" />
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IExceptionHandler}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame>catch</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ExceptionIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutInsertFrame CollectionName=""Instructions.InstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
            </LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""Instructions"" />
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IExport}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""EntityName"" />
            <LayoutKeywordFrame>to</LayoutKeywordFrame>
            <LayoutHorizontalBlockListFrame PropertyName=""ClassIdentifierBlocks"">
                <LayoutHorizontalBlockListFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""ClassOrExport""/>
                </LayoutHorizontalBlockListFrame.Selectors>
            </LayoutHorizontalBlockListFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IExportChange}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""ExportIdentifier"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Export""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
            <LayoutKeywordFrame>to</LayoutKeywordFrame>
            <LayoutHorizontalBlockListFrame PropertyName=""IdentifierBlocks"">
                <LayoutHorizontalBlockListFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                </LayoutHorizontalBlockListFrame.Selectors>
            </LayoutHorizontalBlockListFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IGeneric}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutPlaceholderFrame PropertyName=""EntityName"" />
                <LayoutHorizontalPanelFrame>
                    <LayoutHorizontalPanelFrame.Visibility>
                        <LayoutOptionalFrameVisibility PropertyName=""DefaultValue""/>
                    </LayoutHorizontalPanelFrame.Visibility>
                    <LayoutKeywordFrame>=</LayoutKeywordFrame>
                    <LayoutOptionalFrame PropertyName=""DefaultValue"" />
                </LayoutHorizontalPanelFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ConstraintBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>conform to</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""ConstraintBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""ConstraintBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.Visibility>
                    <LayoutMixedFrameVisibility>
                        <LayoutCountFrameVisibility PropertyName=""ConstraintBlocks""/>
                    </LayoutMixedFrameVisibility>
                </LayoutKeywordFrame.Visibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IGlobalReplicate}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""ReplicateName""/>
            <LayoutKeywordFrame>to</LayoutKeywordFrame>
            <LayoutHorizontalListFrame PropertyName=""Patterns""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IImport}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""Type"">
                    <LayoutKeywordFrame>latest</LayoutKeywordFrame>
                    <LayoutKeywordFrame>strict</LayoutKeywordFrame>
                    <LayoutKeywordFrame>stable</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutPlaceholderFrame PropertyName=""LibraryIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Library""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutHorizontalPanelFrame.Visibility>
                        <LayoutOptionalFrameVisibility PropertyName=""FromIdentifier""/>
                    </LayoutHorizontalPanelFrame.Visibility>
                    <LayoutKeywordFrame>from</LayoutKeywordFrame>
                    <LayoutOptionalFrame PropertyName=""FromIdentifier"">
                        <LayoutOptionalFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Source""/>
                        </LayoutOptionalFrame.Selectors>
                    </LayoutOptionalFrame>
                </LayoutHorizontalPanelFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""RenameBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>rename</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""RenameBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""RenameBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutMixedFrameVisibility>
                        <LayoutCountFrameVisibility PropertyName=""RenameBlocks""/>
                    </LayoutMixedFrameVisibility>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutKeywordFrame>end</LayoutKeywordFrame>
            </LayoutVerticalPanelFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IInheritance}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""Conformance"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""Conformance""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>conformant</LayoutKeywordFrame>
                    <LayoutKeywordFrame>non-conformant</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutPlaceholderFrame PropertyName=""ParentType"" />
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""RenameBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>rename</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""RenameBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""RenameBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""ForgetBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>forget</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""ForgetBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""ForgetBlocks"">
                        <LayoutVerticalBlockListFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                        </LayoutVerticalBlockListFrame.Selectors>
                    </LayoutVerticalBlockListFrame>
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""KeepBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>keep</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""KeepBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""KeepBlocks"">
                        <LayoutVerticalBlockListFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                        </LayoutVerticalBlockListFrame.Selectors>
                    </LayoutVerticalBlockListFrame>
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""DiscontinueBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>discontinue</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""DiscontinueBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""DiscontinueBlocks"">
                        <LayoutVerticalBlockListFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                        </LayoutVerticalBlockListFrame.Selectors>
                    </LayoutVerticalBlockListFrame>
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""ExportChangeBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>export</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""ExportChangeBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""ExportChangeBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""ForgetIndexer"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""ForgetIndexer""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>ignore indexer</LayoutKeywordFrame>
                    <LayoutKeywordFrame>forget indexer</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutDiscreteFrame PropertyName=""KeepIndexer"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""KeepIndexer""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>ignore indexer</LayoutKeywordFrame>
                    <LayoutKeywordFrame>keep indexer</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutDiscreteFrame PropertyName=""DiscontinueIndexer"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""DiscontinueIndexer""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>ignore indexer</LayoutKeywordFrame>
                    <LayoutKeywordFrame>discontinue indexer</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutKeywordFrame Text=""end"">
                    <LayoutKeywordFrame.Visibility>
                        <LayoutMixedFrameVisibility>
                            <LayoutCountFrameVisibility PropertyName=""RenameBlocks""/>
                            <LayoutCountFrameVisibility PropertyName=""ForgetBlocks""/>
                            <LayoutCountFrameVisibility PropertyName=""KeepBlocks""/>
                            <LayoutCountFrameVisibility PropertyName=""DiscontinueBlocks""/>
                            <LayoutDefaultDiscreteFrameVisibility PropertyName=""ForgetIndexer""/>
                            <LayoutDefaultDiscreteFrameVisibility PropertyName=""KeepIndexer""/>
                            <LayoutDefaultDiscreteFrameVisibility PropertyName=""DiscontinueIndexer""/>
                        </LayoutMixedFrameVisibility>
                    </LayoutKeywordFrame.Visibility>
                </LayoutKeywordFrame>
            </LayoutVerticalPanelFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:ILibrary}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame>library</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""EntityName""/>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>from</LayoutKeywordFrame>
                    <LayoutOptionalFrame PropertyName=""FromIdentifier"">
                        <LayoutOptionalFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                        </LayoutOptionalFrame.Selectors>
                    </LayoutOptionalFrame>
                </LayoutHorizontalPanelFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ImportBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>import</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""ImportBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""ImportBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ClassIdentifierBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>class</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""ClassIdentifierBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""ClassIdentifierBlocks"">
                    <LayoutVerticalBlockListFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                    </LayoutVerticalBlockListFrame.Selectors>
                </LayoutVerticalBlockListFrame>
            </LayoutVerticalPanelFrame>
                <LayoutKeywordFrame Text=""end"">
                    <LayoutKeywordFrame.Visibility>
                        <LayoutMixedFrameVisibility>
                            <LayoutCountFrameVisibility PropertyName=""ImportBlocks""/>
                            <LayoutCountFrameVisibility PropertyName=""ClassIdentifierBlocks""/>
                        </LayoutMixedFrameVisibility>
                    </LayoutKeywordFrame.Visibility>
                </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IName}"" IsSimple=""True"">
        <LayoutTextValueFrame PropertyName=""Text""/>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IPattern}"" IsSimple=""True"">
        <LayoutTextValueFrame PropertyName=""Text""/>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IQualifiedName}"">
        <LayoutHorizontalListFrame PropertyName=""Path"">
            <LayoutHorizontalListFrame.Selectors>
                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
            </LayoutHorizontalListFrame.Selectors>
        </LayoutHorizontalListFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IQueryOverload}"">
        <LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ParameterBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>parameter</LayoutKeywordFrame>
                    <LayoutDiscreteFrame PropertyName=""ParameterEnd"">
                        <LayoutKeywordFrame>closed</LayoutKeywordFrame>
                        <LayoutKeywordFrame>open</LayoutKeywordFrame>
                    </LayoutDiscreteFrame>
                    <LayoutInsertFrame CollectionName=""ParameterBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""ParameterBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ResultBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>result</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""ResultBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""ResultBlocks""/>
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ModifiedQueryBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>modified</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""ModifiedQueryBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""ModifiedQueryBlocks"">
                    <LayoutVerticalBlockListFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                    </LayoutVerticalBlockListFrame.Selectors>
                </LayoutVerticalBlockListFrame>
            </LayoutVerticalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""QueryBody"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IDeferredBody}"" SelectorName=""Overload""/>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IEffectiveBody}"" SelectorName=""Overload""/>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IExternBody}"" SelectorName=""Overload""/>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IPrecursorBody}"" SelectorName=""Overload""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame>variant</LayoutKeywordFrame>
                <LayoutOptionalFrame PropertyName=""Variant"" />
            </LayoutHorizontalPanelFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IQueryOverloadType}"">
        <LayoutHorizontalPanelFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}""/>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>parameter</LayoutKeywordFrame>
                        <LayoutDiscreteFrame PropertyName=""ParameterEnd"">
                            <LayoutDiscreteFrame.Visibility>
                                <LayoutDefaultDiscreteFrameVisibility PropertyName=""ParameterEnd""/>
                            </LayoutDiscreteFrame.Visibility>
                            <LayoutKeywordFrame>closed</LayoutKeywordFrame>
                            <LayoutKeywordFrame>open</LayoutKeywordFrame>
                        </LayoutDiscreteFrame>
                        <LayoutInsertFrame CollectionName=""ParameterBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""ParameterBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>return</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""ResultBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""ResultBlocks""/>
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>require</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>exception</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                        <LayoutVerticalBlockListFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                        </LayoutVerticalBlockListFrame.Selectors>
                    </LayoutVerticalBlockListFrame>
                </LayoutVerticalPanelFrame>
                <LayoutKeywordFrame>end</LayoutKeywordFrame>
            </LayoutVerticalPanelFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightBracket}""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IRange}"">
        <LayoutHorizontalPanelFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}"">
                <LayoutSymbolFrame.Visibility>
                    <LayoutOptionalFrameVisibility PropertyName=""RightExpression""/>
                </LayoutSymbolFrame.Visibility>
            </LayoutSymbolFrame>
            <LayoutPlaceholderFrame PropertyName=""LeftExpression"" />
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.Visibility>
                    <LayoutOptionalFrameVisibility PropertyName=""RightExpression""/>
                </LayoutHorizontalPanelFrame.Visibility>
                <LayoutKeywordFrame>to</LayoutKeywordFrame>
                <LayoutOptionalFrame PropertyName=""RightExpression"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightBracket}""/>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IRename}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
            <LayoutKeywordFrame>to</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""DestinationIdentifier"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IRoot}"">
        <LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>libraries</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""LibraryBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""LibraryBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>classes</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""ClassBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""ClassBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>replicates</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""Replicates"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalListFrame PropertyName=""Replicates"" />
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame>end</LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IScope}"">
        <LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""EntityDeclarationBlocks""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>local</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""EntityDeclarationBlocks"" />
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""EntityDeclarationBlocks"" />
            </LayoutVerticalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>do</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""InstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""InstructionBlocks"" />
            </LayoutVerticalPanelFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:ITypedef}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""EntityName"" />
            <LayoutKeywordFrame>is</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""DefinedType"" />
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IWith}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame>case</LayoutKeywordFrame>
                <LayoutHorizontalBlockListFrame PropertyName=""RangeBlocks""/>
                <LayoutInsertFrame CollectionName=""RangeBlocks""/>
            </LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""Instructions""/>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IAssignmentArgument}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalBlockListFrame PropertyName=""ParameterBlocks"">
                <LayoutHorizontalBlockListFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                </LayoutHorizontalBlockListFrame.Selectors>
            </LayoutHorizontalBlockListFrame>
            <LayoutPlaceholderFrame PropertyName=""Source""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IPositionalArgument}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""Source""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IDeferredBody}"">
        <LayoutSelectionFrame>
            <LayoutSelectableFrame Name=""Overload"">
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>require</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>throw</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame IsFocusable=""true"">deferred</LayoutKeywordFrame>
                        </LayoutHorizontalPanelFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Getter"">
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>require</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>throw</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>getter</LayoutKeywordFrame>
                            <LayoutKeywordFrame IsFocusable=""true"">deferred</LayoutKeywordFrame>
                        </LayoutHorizontalPanelFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Setter"">
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>require</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>throw</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>setter</LayoutKeywordFrame>
                            <LayoutKeywordFrame IsFocusable=""true"">deferred</LayoutKeywordFrame>
                        </LayoutHorizontalPanelFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
            </LayoutSelectableFrame>
        </LayoutSelectionFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IEffectiveBody}"">
        <LayoutSelectionFrame>
            <LayoutSelectableFrame Name=""Overload"">
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>require</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>throw</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EntityDeclarationBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>local</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EntityDeclarationBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EntityDeclarationBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""BodyInstructionBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame IsFocusable=""true"">do</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""BodyInstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""BodyInstructionBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionHandlerBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>exception</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionHandlerBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""ExceptionHandlerBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Getter"">
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>require</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>throw</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EntityDeclarationBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>local</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EntityDeclarationBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EntityDeclarationBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""BodyInstructionBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame IsFocusable=""true"">getter</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""BodyInstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""BodyInstructionBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionHandlerBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>exception</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionHandlerBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""ExceptionHandlerBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Setter"">
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>require</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>throw</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EntityDeclarationBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>local</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EntityDeclarationBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EntityDeclarationBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""BodyInstructionBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame IsFocusable=""true"">setter</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""BodyInstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""BodyInstructionBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionHandlerBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>exception</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionHandlerBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""ExceptionHandlerBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
            </LayoutSelectableFrame>
        </LayoutSelectionFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IExternBody}"">
        <LayoutSelectionFrame>
            <LayoutSelectableFrame Name=""Overload"">
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>require</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>throw</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame IsFocusable=""true"">extern</LayoutKeywordFrame>
                        </LayoutHorizontalPanelFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Getter"">
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>require</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>throw</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>getter</LayoutKeywordFrame>
                            <LayoutKeywordFrame IsFocusable=""true"">extern</LayoutKeywordFrame>
                        </LayoutHorizontalPanelFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Setter"">
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>require</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>throw</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>setter</LayoutKeywordFrame>
                            <LayoutKeywordFrame IsFocusable=""true"">extern</LayoutKeywordFrame>
                        </LayoutHorizontalPanelFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
            </LayoutSelectableFrame>
        </LayoutSelectionFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IPrecursorBody}"">
        <LayoutSelectionFrame>
            <LayoutSelectableFrame Name=""Overload"">
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>require</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>throw</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame IsFocusable=""true"">precursor</LayoutKeywordFrame>
                        </LayoutHorizontalPanelFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Getter"">
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>require</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>throw</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>getter</LayoutKeywordFrame>
                            <LayoutKeywordFrame IsFocusable=""true"">precursor</LayoutKeywordFrame>
                        </LayoutHorizontalPanelFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Setter"">
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""RequireBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>require</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""RequireBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""RequireBlocks"" />
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""ExceptionIdentifierBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>throw</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""ExceptionIdentifierBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ExceptionIdentifierBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>setter</LayoutKeywordFrame>
                            <LayoutKeywordFrame IsFocusable=""true"">precursor</LayoutKeywordFrame>
                        </LayoutHorizontalPanelFrame>
                    </LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutVerticalPanelFrame.Visibility>
                            <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                        </LayoutVerticalPanelFrame.Visibility>
                        <LayoutHorizontalPanelFrame>
                            <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                            <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                        </LayoutHorizontalPanelFrame>
                        <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
            </LayoutSelectableFrame>
        </LayoutSelectionFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IAgentExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>agent</LayoutKeywordFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.Visibility>
                    <LayoutOptionalFrameVisibility PropertyName=""BaseType""/>
                </LayoutHorizontalPanelFrame.Visibility>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftCurlyBracket}""/>
                <LayoutOptionalFrame PropertyName=""BaseType"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightCurlyBracket}""/>
            </LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""Delegated"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IAssertionTagExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>tag</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""TagIdentifier"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IBinaryConditionalExpression}"" IsComplex=""True"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""LeftExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutPlaceholderFrame PropertyName=""LeftExpression"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""LeftExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutDiscreteFrame PropertyName=""Conditional"">
                <LayoutKeywordFrame>and</LayoutKeywordFrame>
                <LayoutKeywordFrame>or</LayoutKeywordFrame>
            </LayoutDiscreteFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""RightExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutPlaceholderFrame PropertyName=""RightExpression"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""RightExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IBinaryOperatorExpression}"" IsComplex=""True"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""LeftExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutPlaceholderFrame PropertyName=""LeftExpression"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""LeftExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""Operator"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""RightExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutPlaceholderFrame PropertyName=""RightExpression"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""RightExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IClassConstantExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftCurlyBracket}""/>
            <LayoutPlaceholderFrame PropertyName=""ClassIdentifier"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Class""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightCurlyBracket}""/>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.Dot}""/>
            <LayoutPlaceholderFrame PropertyName=""ConstantIdentifier"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:ICloneOfExpression}"" IsComplex=""True"">
        <LayoutHorizontalPanelFrame>
            <LayoutDiscreteFrame PropertyName=""Type"">
                <LayoutDiscreteFrame.Visibility>
                    <LayoutDefaultDiscreteFrameVisibility PropertyName=""Type""/>
                </LayoutDiscreteFrame.Visibility>
                <LayoutKeywordFrame>shallow</LayoutKeywordFrame>
                <LayoutKeywordFrame>deep</LayoutKeywordFrame>
            </LayoutDiscreteFrame>
            <LayoutKeywordFrame IsFocusable=""true"">clone of</LayoutKeywordFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""Source""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutPlaceholderFrame PropertyName=""Source"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""Source""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IEntityExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>entity</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""Query""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IEqualityExpression}"" IsComplex=""True"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""LeftExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutPlaceholderFrame PropertyName=""LeftExpression"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""LeftExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutDiscreteFrame PropertyName=""Comparison"">
                <LayoutKeywordFrame>=</LayoutKeywordFrame>
                <LayoutKeywordFrame>!=</LayoutKeywordFrame>
            </LayoutDiscreteFrame>
            <LayoutDiscreteFrame PropertyName=""Equality"">
                <LayoutDiscreteFrame.Visibility>
                    <LayoutDefaultDiscreteFrameVisibility PropertyName=""Equality""/>
                </LayoutDiscreteFrame.Visibility>
                <LayoutKeywordFrame>phys</LayoutKeywordFrame>
                <LayoutKeywordFrame>deep</LayoutKeywordFrame>
            </LayoutDiscreteFrame>
            <LayoutKeywordFrame Text="" ""/>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""RightExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutPlaceholderFrame PropertyName=""RightExpression"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""RightExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IIndexQueryExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""IndexedExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutPlaceholderFrame PropertyName=""IndexedExpression"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""IndexedExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}""/>
            <LayoutHorizontalBlockListFrame PropertyName=""ArgumentBlocks""/>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightBracket}""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IInitializedObjectExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""ClassIdentifier"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Type""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}""/>
            <LayoutVerticalBlockListFrame PropertyName=""AssignmentBlocks"" />
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightBracket}""/>
            <LayoutInsertFrame CollectionName=""AssignmentBlocks"" />
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IKeywordExpression}"">
        <LayoutDiscreteFrame PropertyName=""Value"">
            <LayoutKeywordFrame>True</LayoutKeywordFrame>
            <LayoutKeywordFrame>False</LayoutKeywordFrame>
            <LayoutKeywordFrame>Current</LayoutKeywordFrame>
            <LayoutKeywordFrame>Value</LayoutKeywordFrame>
            <LayoutKeywordFrame>Result</LayoutKeywordFrame>
            <LayoutKeywordFrame>Retry</LayoutKeywordFrame>
            <LayoutKeywordFrame>Exception</LayoutKeywordFrame>
        </LayoutDiscreteFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IManifestCharacterExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>'</LayoutKeywordFrame>
            <LayoutCharacterFrame PropertyName=""Text""/>
            <LayoutKeywordFrame>'</LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IManifestNumberExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutNumberFrame PropertyName=""Text""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IManifestStringExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>""</LayoutKeywordFrame>
            <LayoutTextValueFrame PropertyName=""Text""/>
            <LayoutKeywordFrame>""</LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:INewExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>new</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""Object"" />
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IOldExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>old</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""Query"" />
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IPrecursorExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame IsFocusable=""true"">precursor</LayoutKeywordFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.Visibility>
                    <LayoutOptionalFrameVisibility PropertyName=""AncestorType""/>
                </LayoutHorizontalPanelFrame.Visibility>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftCurlyBracket}""/>
                <LayoutOptionalFrame PropertyName=""AncestorType"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightCurlyBracket}""/>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""ArgumentBlocks""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutHorizontalBlockListFrame PropertyName=""ArgumentBlocks"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""ArgumentBlocks""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IPrecursorIndexExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame IsFocusable=""true"">precursor</LayoutKeywordFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.Visibility>
                    <LayoutOptionalFrameVisibility PropertyName=""AncestorType""/>
                </LayoutHorizontalPanelFrame.Visibility>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftCurlyBracket}""/>
                <LayoutOptionalFrame PropertyName=""AncestorType"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightCurlyBracket}""/>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}""/>
                <LayoutHorizontalBlockListFrame PropertyName=""ArgumentBlocks""/>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightBracket}""/>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IPreprocessorExpression}"">
        <LayoutDiscreteFrame PropertyName=""Value"">
            <LayoutKeywordFrame>DateAndTime</LayoutKeywordFrame>
            <LayoutKeywordFrame>CompilationDiscreteIdentifier</LayoutKeywordFrame>
            <LayoutKeywordFrame>ClassPath</LayoutKeywordFrame>
            <LayoutKeywordFrame>CompilerVersion</LayoutKeywordFrame>
            <LayoutKeywordFrame>ConformanceToStandard</LayoutKeywordFrame>
            <LayoutKeywordFrame>DiscreteClassIdentifier</LayoutKeywordFrame>
            <LayoutKeywordFrame>Counter</LayoutKeywordFrame>
            <LayoutKeywordFrame>Debugging</LayoutKeywordFrame>
            <LayoutKeywordFrame>RandomInteger</LayoutKeywordFrame>
        </LayoutDiscreteFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IQueryExpression}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""Query"" />
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""ArgumentBlocks""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutHorizontalBlockListFrame PropertyName=""ArgumentBlocks"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""ArgumentBlocks""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IResultOfExpression}"" IsComplex=""True"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>result of</LayoutKeywordFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""Source""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutPlaceholderFrame PropertyName=""Source"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""Source""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IUnaryNotExpression}"" IsComplex=""True"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>not</LayoutKeywordFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""RightExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutPlaceholderFrame PropertyName=""RightExpression"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""RightExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IUnaryOperatorExpression}"" IsComplex=""True"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""Operator"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""RightExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutPlaceholderFrame PropertyName=""RightExpression"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutComplexFrameVisibility PropertyName=""RightExpression""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IAttributeFeature}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""Export"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""Export""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>exported</LayoutKeywordFrame>
                    <LayoutKeywordFrame>private</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutPlaceholderFrame PropertyName=""EntityName"" />
                <LayoutKeywordFrame>:</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""EntityType""/>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>ensure</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""EnsureBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""EnsureBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutHorizontalPanelFrame.Visibility>
                        <LayoutTextMatchFrameVisibility PropertyName=""ExportIdentifier"" TextPattern=""All""/>
                    </LayoutHorizontalPanelFrame.Visibility>
                    <LayoutKeywordFrame>export to</LayoutKeywordFrame>
                    <LayoutPlaceholderFrame PropertyName=""ExportIdentifier"">
                        <LayoutPlaceholderFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Export""/>
                        </LayoutPlaceholderFrame.Selectors>
                    </LayoutPlaceholderFrame>
                </LayoutHorizontalPanelFrame>
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.Visibility>
                    <LayoutMixedFrameVisibility>
                        <LayoutCountFrameVisibility PropertyName=""EnsureBlocks""/>
                        <LayoutTextMatchFrameVisibility PropertyName=""ExportIdentifier"" TextPattern=""All""/>
                    </LayoutMixedFrameVisibility>
                </LayoutKeywordFrame.Visibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IConstantFeature}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""Export"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""Export""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>exported</LayoutKeywordFrame>
                    <LayoutKeywordFrame>private</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutPlaceholderFrame PropertyName=""EntityName"" />
                <LayoutKeywordFrame>:</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""EntityType""/>
                <LayoutKeywordFrame>=</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ConstantValue""/>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutHorizontalPanelFrame.Visibility>
                        <LayoutTextMatchFrameVisibility PropertyName=""ExportIdentifier"" TextPattern=""All""/>
                    </LayoutHorizontalPanelFrame.Visibility>
                    <LayoutKeywordFrame>export to</LayoutKeywordFrame>
                    <LayoutPlaceholderFrame PropertyName=""ExportIdentifier"">
                        <LayoutPlaceholderFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Export""/>
                        </LayoutPlaceholderFrame.Selectors>
                    </LayoutPlaceholderFrame>
                </LayoutHorizontalPanelFrame>
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.Visibility>
                    <LayoutMixedFrameVisibility>
                        <LayoutTextMatchFrameVisibility PropertyName=""ExportIdentifier"" TextPattern=""All""/>
                    </LayoutMixedFrameVisibility>
                </LayoutKeywordFrame.Visibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:ICreationFeature}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""Export"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""Export""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>exported</LayoutKeywordFrame>
                    <LayoutKeywordFrame>private</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutPlaceholderFrame PropertyName=""EntityName"" />
                <LayoutKeywordFrame>creation</LayoutKeywordFrame>
                <LayoutInsertFrame CollectionName=""OverloadBlocks"" />
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""OverloadBlocks""/>
                <LayoutHorizontalPanelFrame>
                    <LayoutHorizontalPanelFrame.Visibility>
                        <LayoutTextMatchFrameVisibility PropertyName=""ExportIdentifier"" TextPattern=""All""/>
                    </LayoutHorizontalPanelFrame.Visibility>
                    <LayoutKeywordFrame>export to</LayoutKeywordFrame>
                    <LayoutPlaceholderFrame PropertyName=""ExportIdentifier"">
                        <LayoutPlaceholderFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Export""/>
                        </LayoutPlaceholderFrame.Selectors>
                    </LayoutPlaceholderFrame>
                </LayoutHorizontalPanelFrame>
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame>end</LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IFunctionFeature}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""Export"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""Export""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>exported</LayoutKeywordFrame>
                    <LayoutKeywordFrame>private</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutPlaceholderFrame PropertyName=""EntityName"" />
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>once per</LayoutKeywordFrame>
                    <LayoutDiscreteFrame PropertyName=""Once"">
                        <LayoutDiscreteFrame.Visibility>
                            <LayoutDefaultDiscreteFrameVisibility PropertyName=""Once""/>
                        </LayoutDiscreteFrame.Visibility>
                        <LayoutKeywordFrame>normal</LayoutKeywordFrame>
                        <LayoutKeywordFrame>object</LayoutKeywordFrame>
                        <LayoutKeywordFrame>processor</LayoutKeywordFrame>
                        <LayoutKeywordFrame>process</LayoutKeywordFrame>
                    </LayoutDiscreteFrame>
                </LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame>function</LayoutKeywordFrame>
                <LayoutInsertFrame CollectionName=""OverloadBlocks"" />
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""OverloadBlocks""/>
                <LayoutHorizontalPanelFrame>
                    <LayoutHorizontalPanelFrame.Visibility>
                        <LayoutTextMatchFrameVisibility PropertyName=""ExportIdentifier"" TextPattern=""All""/>
                    </LayoutHorizontalPanelFrame.Visibility>
                    <LayoutKeywordFrame>export to</LayoutKeywordFrame>
                    <LayoutPlaceholderFrame PropertyName=""ExportIdentifier"">
                        <LayoutPlaceholderFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Export""/>
                        </LayoutPlaceholderFrame.Selectors>
                    </LayoutPlaceholderFrame>
                </LayoutHorizontalPanelFrame>
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame>end</LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IIndexerFeature}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""Export"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""Export""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>exported</LayoutKeywordFrame>
                    <LayoutKeywordFrame>private</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutKeywordFrame IsFocusable=""true"">indexer</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""EntityType""/>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""IndexParameterBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutDiscreteFrame PropertyName=""ParameterEnd"">
                            <LayoutDiscreteFrame.Visibility>
                                <LayoutDefaultDiscreteFrameVisibility PropertyName=""ParameterEnd""/>
                            </LayoutDiscreteFrame.Visibility>
                            <LayoutKeywordFrame>closed</LayoutKeywordFrame>
                            <LayoutKeywordFrame>open</LayoutKeywordFrame>
                        </LayoutDiscreteFrame>
                        <LayoutKeywordFrame>parameter</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""IndexParameterBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""IndexParameterBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""ModifiedQueryBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>modify</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""ModifiedQueryBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ModifiedQueryBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutOptionalFrame PropertyName=""GetterBody"">
                        <LayoutOptionalFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IDeferredBody}"" SelectorName=""Getter""/>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IEffectiveBody}"" SelectorName=""Getter""/>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IExternBody}"" SelectorName=""Getter""/>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IPrecursorBody}"" SelectorName=""Getter""/>
                        </LayoutOptionalFrame.Selectors>
                    </LayoutOptionalFrame>
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutOptionalFrame PropertyName=""SetterBody"">
                        <LayoutOptionalFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IDeferredBody}"" SelectorName=""Setter""/>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IEffectiveBody}"" SelectorName=""Setter""/>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IExternBody}"" SelectorName=""Setter""/>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IPrecursorBody}"" SelectorName=""Setter""/>
                        </LayoutOptionalFrame.Selectors>
                    </LayoutOptionalFrame>
                </LayoutVerticalPanelFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutHorizontalPanelFrame.Visibility>
                        <LayoutTextMatchFrameVisibility PropertyName=""ExportIdentifier"" TextPattern=""All""/>
                    </LayoutHorizontalPanelFrame.Visibility>
                    <LayoutKeywordFrame>export to</LayoutKeywordFrame>
                    <LayoutPlaceholderFrame PropertyName=""ExportIdentifier"">
                        <LayoutPlaceholderFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Export""/>
                        </LayoutPlaceholderFrame.Selectors>
                    </LayoutPlaceholderFrame>
                </LayoutHorizontalPanelFrame>
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame>end</LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IProcedureFeature}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""Export"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""Export""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>exported</LayoutKeywordFrame>
                    <LayoutKeywordFrame>private</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutPlaceholderFrame PropertyName=""EntityName"" />
                <LayoutKeywordFrame>procedure</LayoutKeywordFrame>
                <LayoutInsertFrame CollectionName=""OverloadBlocks"" />
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""OverloadBlocks""/>
                <LayoutHorizontalPanelFrame>
                    <LayoutHorizontalPanelFrame.Visibility>
                        <LayoutTextMatchFrameVisibility PropertyName=""ExportIdentifier"" TextPattern=""All""/>
                    </LayoutHorizontalPanelFrame.Visibility>
                    <LayoutKeywordFrame>export to</LayoutKeywordFrame>
                    <LayoutPlaceholderFrame PropertyName=""ExportIdentifier"">
                        <LayoutPlaceholderFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Export""/>
                        </LayoutPlaceholderFrame.Selectors>
                    </LayoutPlaceholderFrame>
                </LayoutHorizontalPanelFrame>
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame>end</LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IPropertyFeature}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""Export"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""Export""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>exported</LayoutKeywordFrame>
                    <LayoutKeywordFrame>private</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutPlaceholderFrame PropertyName=""EntityName"" />
                <LayoutKeywordFrame>is</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""EntityType""/>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""ModifiedQueryBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>modify</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""ModifiedQueryBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalPanelFrame>
                        <LayoutHorizontalBlockListFrame PropertyName=""ModifiedQueryBlocks"">
                            <LayoutHorizontalBlockListFrame.Selectors>
                                <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                            </LayoutHorizontalBlockListFrame.Selectors>
                        </LayoutHorizontalBlockListFrame>
                    </LayoutVerticalPanelFrame>
                </LayoutVerticalPanelFrame>
                    <LayoutOptionalFrame PropertyName=""GetterBody"">
                        <LayoutOptionalFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IDeferredBody}"" SelectorName=""Getter""/>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IEffectiveBody}"" SelectorName=""Getter""/>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IExternBody}"" SelectorName=""Getter""/>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IPrecursorBody}"" SelectorName=""Getter""/>
                        </LayoutOptionalFrame.Selectors>
                    </LayoutOptionalFrame>
                    <LayoutOptionalFrame PropertyName=""SetterBody"">
                        <LayoutOptionalFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IDeferredBody}"" SelectorName=""Setter""/>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IEffectiveBody}"" SelectorName=""Setter""/>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IExternBody}"" SelectorName=""Setter""/>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IPrecursorBody}"" SelectorName=""Setter""/>
                        </LayoutOptionalFrame.Selectors>
                    </LayoutOptionalFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutHorizontalPanelFrame.Visibility>
                        <LayoutTextMatchFrameVisibility PropertyName=""ExportIdentifier"" TextPattern=""All""/>
                    </LayoutHorizontalPanelFrame.Visibility>
                    <LayoutKeywordFrame>export to</LayoutKeywordFrame>
                    <LayoutPlaceholderFrame PropertyName=""ExportIdentifier"">
                        <LayoutPlaceholderFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Export""/>
                        </LayoutPlaceholderFrame.Selectors>
                    </LayoutPlaceholderFrame>
                </LayoutHorizontalPanelFrame>
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.Visibility>
                    <LayoutMixedFrameVisibility>
                        <LayoutCountFrameVisibility PropertyName=""ModifiedQueryBlocks""/>
                        <LayoutOptionalFrameVisibility PropertyName=""GetterBody""/>
                        <LayoutOptionalFrameVisibility PropertyName=""SetterBody""/>
                        <LayoutTextMatchFrameVisibility PropertyName=""ExportIdentifier"" TextPattern=""All""/>
                    </LayoutMixedFrameVisibility>
                </LayoutKeywordFrame.Visibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IIdentifier}"" IsSimple=""True"">
        <LayoutSelectionFrame>
            <LayoutSelectableFrame Name=""Identifier"">
                <LayoutTextValueFrame PropertyName=""Text""/>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Feature"">
                <LayoutTextValueFrame PropertyName=""Text""/>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Class"">
                <LayoutTextValueFrame PropertyName=""Text""/>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""ClassOrExport"">
                <LayoutTextValueFrame PropertyName=""Text""/>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Export"">
                <LayoutTextValueFrame PropertyName=""Text""/>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Library"">
                <LayoutTextValueFrame PropertyName=""Text""/>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Source"">
                <LayoutTextValueFrame PropertyName=""Text""/>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Type"">
                <LayoutTextValueFrame PropertyName=""Text""/>
            </LayoutSelectableFrame>
            <LayoutSelectableFrame Name=""Pattern"">
                <LayoutTextValueFrame PropertyName=""Text""/>
            </LayoutSelectableFrame>
        </LayoutSelectionFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IAsLongAsInstruction}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame>as long as</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ContinueCondition""/>
                <LayoutInsertFrame CollectionName=""ContinuationBlocks""/>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""ContinuationBlocks""/>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutOptionalFrameVisibility PropertyName=""ElseInstructions""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>else</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""ElseInstructions.InstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
                    </LayoutHorizontalPanelFrame>
                    <LayoutOptionalFrame PropertyName=""ElseInstructions"" />
                </LayoutVerticalPanelFrame>
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame>end</LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IAssignmentInstruction}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalBlockListFrame PropertyName=""DestinationBlocks""/>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftArrow}""/>
            <LayoutPlaceholderFrame PropertyName=""Source"" />
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IAttachmentInstruction}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame>attach</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""Source"" />
                <LayoutKeywordFrame>to</LayoutKeywordFrame>
                <LayoutHorizontalBlockListFrame PropertyName=""EntityNameBlocks""/>
                <LayoutInsertFrame CollectionName=""AttachmentBlocks"" />
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""AttachmentBlocks""/>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutOptionalFrameVisibility PropertyName=""ElseInstructions""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>else</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""ElseInstructions.InstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
                    </LayoutHorizontalPanelFrame>
                    <LayoutOptionalFrame PropertyName=""ElseInstructions"" />
                </LayoutVerticalPanelFrame>
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame>end</LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:ICheckInstruction}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>check</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""BooleanExpression"" />
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:ICommandInstruction}"">
        <LayoutHorizontalPanelFrame>
            <LayoutInsertFrame CollectionName=""Command.Path""/>
            <LayoutPlaceholderFrame PropertyName=""Command"" />
            <LayoutHorizontalPanelFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""ArgumentBlocks""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
                <LayoutHorizontalBlockListFrame PropertyName=""ArgumentBlocks"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}"">
                    <LayoutSymbolFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""ArgumentBlocks""/>
                    </LayoutSymbolFrame.Visibility>
                </LayoutSymbolFrame>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:ICreateInstruction}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>create</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""EntityIdentifier"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame>with</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""CreationRoutineIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}""/>
                    <LayoutHorizontalBlockListFrame PropertyName=""ArgumentBlocks"" />
                    <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}""/>
                </LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutHorizontalPanelFrame.Visibility>
                        <LayoutOptionalFrameVisibility PropertyName=""Processor""/>
                    </LayoutHorizontalPanelFrame.Visibility>
                    <LayoutKeywordFrame>same processor as</LayoutKeywordFrame>
                    <LayoutOptionalFrame PropertyName=""Processor"" />
                </LayoutHorizontalPanelFrame>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IDebugInstruction}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame>debug</LayoutKeywordFrame>
                <LayoutInsertFrame CollectionName=""Instructions.InstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
            </LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""Instructions"" />
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame>end</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IForLoopInstruction}"">
        <LayoutVerticalPanelFrame>
            <LayoutKeywordFrame>loop</LayoutKeywordFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""EntityDeclarationBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>local</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""EntityDeclarationBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""EntityDeclarationBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""InitInstructionBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>init</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""InitInstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""InitInstructionBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>while</LayoutKeywordFrame>
                    <LayoutPlaceholderFrame PropertyName=""WhileCondition""/>
                    <LayoutInsertFrame CollectionName=""LoopInstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""LoopInstructionBlocks"" />
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""IterationInstructionBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>iterate</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""IterationInstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""IterationInstructionBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""InvariantBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>invariant</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""InvariantBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""InvariantBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutHorizontalPanelFrame.Visibility>
                        <LayoutOptionalFrameVisibility PropertyName=""Variant""/>
                    </LayoutHorizontalPanelFrame.Visibility>
                    <LayoutKeywordFrame>variant</LayoutKeywordFrame>
                    <LayoutOptionalFrame PropertyName=""Variant"" />
                </LayoutHorizontalPanelFrame>
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame>end</LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IIfThenElseInstruction}"">
        <LayoutVerticalPanelFrame>
            <LayoutVerticalBlockListFrame PropertyName=""ConditionalBlocks""/>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.Visibility>
                    <LayoutOptionalFrameVisibility PropertyName=""ElseInstructions""/>
                </LayoutVerticalPanelFrame.Visibility>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>else</LayoutKeywordFrame>
                    <LayoutInsertFrame CollectionName=""ElseInstructions.InstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
                </LayoutHorizontalPanelFrame>
                <LayoutOptionalFrame PropertyName=""ElseInstructions"" />
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame>end</LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IIndexAssignmentInstruction}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""Destination"" />
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ArgumentBlocks""/>
                </LayoutHorizontalPanelFrame.Visibility>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}""/>
                <LayoutHorizontalBlockListFrame PropertyName=""ArgumentBlocks"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightBracket}""/>
            </LayoutHorizontalPanelFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftArrow}""/>
            <LayoutPlaceholderFrame PropertyName=""Source"" />
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IInspectInstruction}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame>inspect</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""Source"" />
                <LayoutInsertFrame CollectionName=""WithBlocks"" />
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalBlockListFrame PropertyName=""WithBlocks""/>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutOptionalFrameVisibility PropertyName=""ElseInstructions""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>else</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""ElseInstructions.InstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
                    </LayoutHorizontalPanelFrame>
                    <LayoutOptionalFrame PropertyName=""ElseInstructions"" />
                </LayoutVerticalPanelFrame>
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame>end</LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IKeywordAssignmentInstruction}"">
        <LayoutHorizontalPanelFrame>
            <LayoutDiscreteFrame PropertyName=""Destination"">
                <LayoutKeywordFrame>True</LayoutKeywordFrame>
                <LayoutKeywordFrame>False</LayoutKeywordFrame>
                <LayoutKeywordFrame>Current</LayoutKeywordFrame>
                <LayoutKeywordFrame>Value</LayoutKeywordFrame>
                <LayoutKeywordFrame>Result</LayoutKeywordFrame>
                <LayoutKeywordFrame>Retry</LayoutKeywordFrame>
                <LayoutKeywordFrame>Exception</LayoutKeywordFrame>
            </LayoutDiscreteFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftArrow}""/>
            <LayoutPlaceholderFrame PropertyName=""Source"" />
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IOverLoopInstruction}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutKeywordFrame>over</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""OverList"" />
                <LayoutKeywordFrame>for each</LayoutKeywordFrame>
                <LayoutHorizontalBlockListFrame PropertyName=""IndexerBlocks""/>
                <LayoutDiscreteFrame PropertyName=""Iteration"">
                    <LayoutDiscreteFrame.Visibility>
                        <LayoutDefaultDiscreteFrameVisibility PropertyName=""Iteration""/>
                    </LayoutDiscreteFrame.Visibility>
                    <LayoutKeywordFrame>Single</LayoutKeywordFrame>
                    <LayoutKeywordFrame>Nested</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutInsertFrame CollectionName=""LoopInstructions.InstructionBlocks"" ItemType=""{xaml:Type easly:CommandInstruction}""/>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalPanelFrame>
                <LayoutPlaceholderFrame PropertyName=""LoopInstructions"" />
                <LayoutHorizontalPanelFrame>
                    <LayoutHorizontalPanelFrame.Visibility>
                        <LayoutOptionalFrameVisibility PropertyName=""ExitEntityName""/>
                    </LayoutHorizontalPanelFrame.Visibility>
                    <LayoutKeywordFrame>exit if</LayoutKeywordFrame>
                    <LayoutOptionalFrame PropertyName=""ExitEntityName"">
                        <LayoutOptionalFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                        </LayoutOptionalFrame.Selectors>
                    </LayoutOptionalFrame>
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""InvariantBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>invariant</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""InvariantBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""InvariantBlocks"" />
                </LayoutVerticalPanelFrame>
            </LayoutVerticalPanelFrame>
            <LayoutKeywordFrame>end</LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IPrecursorIndexAssignmentInstruction}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>precursor</LayoutKeywordFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.Visibility>
                    <LayoutOptionalFrameVisibility PropertyName=""AncestorType""/>
                </LayoutHorizontalPanelFrame.Visibility>
                <LayoutKeywordFrame>from</LayoutKeywordFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftCurlyBracket}""/>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightBracket}""/>
                <LayoutOptionalFrame PropertyName=""AncestorType"" />
            </LayoutHorizontalPanelFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}""/>
            <LayoutHorizontalBlockListFrame PropertyName=""ArgumentBlocks""/>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightBracket}""/>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftArrow}""/>
            <LayoutPlaceholderFrame PropertyName=""Source"" />
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IPrecursorInstruction}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame IsFocusable=""true"">precursor</LayoutKeywordFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.Visibility>
                    <LayoutOptionalFrameVisibility PropertyName=""AncestorType""/>
                </LayoutHorizontalPanelFrame.Visibility>
                <LayoutKeywordFrame>from</LayoutKeywordFrame>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftCurlyBracket}""/>
                <LayoutOptionalFrame PropertyName=""AncestorType"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightCurlyBracket}""/>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ArgumentBlocks""/>
                </LayoutHorizontalPanelFrame.Visibility>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}""/>
                <LayoutHorizontalBlockListFrame PropertyName=""ArgumentBlocks"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}""/>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IRaiseEventInstruction}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>raise</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""QueryIdentifier"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
            <LayoutDiscreteFrame PropertyName=""Event"">
                <LayoutDiscreteFrame.Visibility>
                    <LayoutDefaultDiscreteFrameVisibility PropertyName=""Event""/>
                </LayoutDiscreteFrame.Visibility>
                <LayoutKeywordFrame>once</LayoutKeywordFrame>
                <LayoutKeywordFrame>forever</LayoutKeywordFrame>
            </LayoutDiscreteFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IReleaseInstruction}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>release</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""EntityName""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IThrowInstruction}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>throw</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""ExceptionType"" />
            <LayoutKeywordFrame>with</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""CreationRoutine"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.Visibility>
                    <LayoutCountFrameVisibility PropertyName=""ArgumentBlocks""/>
                </LayoutHorizontalPanelFrame.Visibility>
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftParenthesis}""/>
                <LayoutHorizontalBlockListFrame PropertyName=""ArgumentBlocks"" />
                <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightParenthesis}""/>
            </LayoutHorizontalPanelFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IAnchoredType}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>like</LayoutKeywordFrame>
            <LayoutDiscreteFrame PropertyName=""AnchorKind"">
                <LayoutDiscreteFrame.Visibility>
                    <LayoutDefaultDiscreteFrameVisibility PropertyName=""AnchorKind""/>
                </LayoutDiscreteFrame.Visibility>
                <LayoutKeywordFrame>declaration</LayoutKeywordFrame>
                <LayoutKeywordFrame>creation</LayoutKeywordFrame>
            </LayoutDiscreteFrame>
            <LayoutPlaceholderFrame PropertyName=""AnchoredName"" />
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IFunctionType}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>function</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""BaseType"" />
            <LayoutHorizontalBlockListFrame PropertyName=""OverloadBlocks""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IGenericType}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""ClassIdentifier"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}""/>
            <LayoutHorizontalBlockListFrame PropertyName=""TypeArgumentBlocks""/>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightBracket}""/>
            <LayoutDiscreteFrame PropertyName=""Sharing"">
                <LayoutDiscreteFrame.Visibility>
                    <LayoutDefaultDiscreteFrameVisibility PropertyName=""Sharing""/>
                </LayoutDiscreteFrame.Visibility>
                <LayoutKeywordFrame>not shared</LayoutKeywordFrame>
                <LayoutKeywordFrame>readwrite</LayoutKeywordFrame>
                <LayoutKeywordFrame>read-only</LayoutKeywordFrame>
                <LayoutKeywordFrame>write-only</LayoutKeywordFrame>
            </LayoutDiscreteFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IIndexerType}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""BaseType"" />
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}""/>
            <LayoutVerticalPanelFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>indexer</LayoutKeywordFrame>
                    <LayoutPlaceholderFrame PropertyName=""EntityType""/>
                </LayoutHorizontalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""IndexParameterBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>parameter</LayoutKeywordFrame>
                        <LayoutDiscreteFrame PropertyName=""ParameterEnd"">
                            <LayoutDiscreteFrame.Visibility>
                                <LayoutDefaultDiscreteFrameVisibility PropertyName=""ParameterEnd""/>
                            </LayoutDiscreteFrame.Visibility>
                            <LayoutKeywordFrame>closed</LayoutKeywordFrame>
                            <LayoutKeywordFrame>open</LayoutKeywordFrame>
                        </LayoutDiscreteFrame>
                        <LayoutInsertFrame CollectionName=""IndexParameterBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""IndexParameterBlocks""/>
                </LayoutVerticalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""IndexerKind"">
                    <LayoutKeywordFrame>read-only</LayoutKeywordFrame>
                    <LayoutKeywordFrame>write-only</LayoutKeywordFrame>
                    <LayoutKeywordFrame>readwrite</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""GetRequireBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>getter require</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""GetRequireBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""GetRequireBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""GetEnsureBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>getter ensure</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""GetEnsureBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""GetEnsureBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""GetExceptionIdentifierBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>getter exception</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""GetExceptionIdentifierBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""GetExceptionIdentifierBlocks"">
                        <LayoutVerticalBlockListFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                        </LayoutVerticalBlockListFrame.Selectors>
                    </LayoutVerticalBlockListFrame>
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""SetRequireBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>setter require</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""SetRequireBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""SetRequireBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""SetEnsureBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>setter ensure</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""SetEnsureBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""SetEnsureBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""SetExceptionIdentifierBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>setter exception</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""SetExceptionIdentifierBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""SetExceptionIdentifierBlocks"">
                        <LayoutVerticalBlockListFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                        </LayoutVerticalBlockListFrame.Selectors>
                    </LayoutVerticalBlockListFrame>
                </LayoutVerticalPanelFrame>
                <LayoutKeywordFrame>end</LayoutKeywordFrame>
            </LayoutVerticalPanelFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightBracket}""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IKeywordAnchoredType}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>like</LayoutKeywordFrame>
            <LayoutDiscreteFrame PropertyName=""Anchor"">
                <LayoutKeywordFrame>True</LayoutKeywordFrame>
                <LayoutKeywordFrame>False</LayoutKeywordFrame>
                <LayoutKeywordFrame>Current</LayoutKeywordFrame>
                <LayoutKeywordFrame>Value</LayoutKeywordFrame>
                <LayoutKeywordFrame>Result</LayoutKeywordFrame>
                <LayoutKeywordFrame>Retry</LayoutKeywordFrame>
                <LayoutKeywordFrame>Exception</LayoutKeywordFrame>
            </LayoutDiscreteFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IProcedureType}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>procedure</LayoutKeywordFrame>
            <LayoutPlaceholderFrame PropertyName=""BaseType"" />
            <LayoutHorizontalBlockListFrame PropertyName=""OverloadBlocks""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IPropertyType}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""BaseType"" />
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}""/>
            <LayoutVerticalPanelFrame>
                <LayoutHorizontalPanelFrame>
                    <LayoutKeywordFrame>is</LayoutKeywordFrame>
                    <LayoutPlaceholderFrame PropertyName=""EntityType""/>
                </LayoutHorizontalPanelFrame>
                <LayoutDiscreteFrame PropertyName=""PropertyKind"">
                    <LayoutKeywordFrame>read-only</LayoutKeywordFrame>
                    <LayoutKeywordFrame>write-only</LayoutKeywordFrame>
                    <LayoutKeywordFrame>readwrite</LayoutKeywordFrame>
                </LayoutDiscreteFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""GetEnsureBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>getter ensure</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""GetEnsureBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""GetEnsureBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""GetExceptionIdentifierBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>getter exception</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""GetExceptionIdentifierBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""GetExceptionIdentifierBlocks"">
                        <LayoutVerticalBlockListFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                        </LayoutVerticalBlockListFrame.Selectors>
                    </LayoutVerticalBlockListFrame>
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""SetRequireBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>setter require</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""SetRequireBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""SetRequireBlocks"" />
                </LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame>
                    <LayoutVerticalPanelFrame.Visibility>
                        <LayoutCountFrameVisibility PropertyName=""SetExceptionIdentifierBlocks""/>
                    </LayoutVerticalPanelFrame.Visibility>
                    <LayoutHorizontalPanelFrame>
                        <LayoutKeywordFrame>setter exception</LayoutKeywordFrame>
                        <LayoutInsertFrame CollectionName=""SetExceptionIdentifierBlocks"" />
                    </LayoutHorizontalPanelFrame>
                    <LayoutVerticalBlockListFrame PropertyName=""SetExceptionIdentifierBlocks"">
                        <LayoutVerticalBlockListFrame.Selectors>
                            <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                        </LayoutVerticalBlockListFrame.Selectors>
                    </LayoutVerticalBlockListFrame>
                </LayoutVerticalPanelFrame>
                <LayoutKeywordFrame>end</LayoutKeywordFrame>
            </LayoutVerticalPanelFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightBracket}""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:ISimpleType}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""ClassIdentifier"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Type""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
            <LayoutDiscreteFrame PropertyName=""Sharing"">
                <LayoutDiscreteFrame.Visibility>
                    <LayoutDefaultDiscreteFrameVisibility PropertyName=""Sharing""/>
                </LayoutDiscreteFrame.Visibility>
                <LayoutKeywordFrame>not shared</LayoutKeywordFrame>
                <LayoutKeywordFrame>readwrite</LayoutKeywordFrame>
                <LayoutKeywordFrame>read-only</LayoutKeywordFrame>
                <LayoutKeywordFrame>write-only</LayoutKeywordFrame>
            </LayoutDiscreteFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:ITupleType}"">
        <LayoutHorizontalPanelFrame>
            <LayoutKeywordFrame>tuple</LayoutKeywordFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftBracket}""/>
            <LayoutVerticalBlockListFrame PropertyName=""EntityDeclarationBlocks""/>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.RightBracket}""/>
            <LayoutDiscreteFrame PropertyName=""Sharing"">
                <LayoutDiscreteFrame.Visibility>
                    <LayoutDefaultDiscreteFrameVisibility PropertyName=""Sharing""/>
                </LayoutDiscreteFrame.Visibility>
                <LayoutKeywordFrame>not shared</LayoutKeywordFrame>
                <LayoutKeywordFrame>readwrite</LayoutKeywordFrame>
                <LayoutKeywordFrame>read-only</LayoutKeywordFrame>
                <LayoutKeywordFrame>write-only</LayoutKeywordFrame>
            </LayoutDiscreteFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IAssignmentTypeArgument}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""ParameterIdentifier"">
                <LayoutPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Feature""/>
                </LayoutPlaceholderFrame.Selectors>
            </LayoutPlaceholderFrame>
            <LayoutSymbolFrame Symbol=""{x:Static const:Symbols.LeftArrow}""/>
            <LayoutPlaceholderFrame PropertyName=""Source"" />
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
    <LayoutNodeTemplate NodeType=""{xaml:Type easly:IPositionalTypeArgument}"">
        <LayoutHorizontalPanelFrame>
            <LayoutPlaceholderFrame PropertyName=""Source""/>
        </LayoutHorizontalPanelFrame>
    </LayoutNodeTemplate>
</LayoutTemplateList>";
        #endregion

        #region Block Templates
        static string LayoutBlockTemplateString =
@"<LayoutTemplateList
    xmlns=""clr-namespace:EaslyController.Layout;assembly=Easly-Controller""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:xaml=""clr-namespace:EaslyController.Xaml;assembly=Easly-Controller""
    xmlns:easly=""clr-namespace:BaseNode;assembly=Easly-Language""
    xmlns:cov=""clr-namespace:Coverage;assembly=Test-Easly-Controller""
    xmlns:const=""clr-namespace:EaslyController.Constants;assembly=Easly-Controller"">
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,cov:ILeaf,cov:Leaf}"">
        <LayoutVerticalPanelFrame>
            <LayoutCommentFrame/>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,cov:ITree,cov:Tree}"">
        <LayoutVerticalPanelFrame>
            <LayoutCommentFrame/>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame>
                <LayoutVerticalCollectionPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                </LayoutVerticalCollectionPlaceholderFrame.Selectors>
            </LayoutVerticalCollectionPlaceholderFrame>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,cov:IMain,cov:Main}"">
        <LayoutVerticalPanelFrame>
            <LayoutCommentFrame/>
            <LayoutVerticalPanelFrame>
                <LayoutVerticalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutVerticalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutVerticalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IArgument,easly:Argument}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IAssertion,easly:Assertion}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IAssignmentArgument,easly:AssignmentArgument}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IAttachment,easly:Attachment}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IClass,easly:Class}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IClassReplicate,easly:ClassReplicate}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:ICommandOverload,easly:CommandOverload}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:ICommandOverloadType,easly:CommandOverloadType}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IConditional,easly:Conditional}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IConstraint,easly:Constraint}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IContinuation,easly:Continuation}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IDiscrete,easly:Discrete}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IEntityDeclaration,easly:EntityDeclaration}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IExceptionHandler,easly:ExceptionHandler}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IExport,easly:Export}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IExportChange,easly:ExportChange}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IFeature,easly:Feature}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IGeneric,easly:Generic}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IIdentifier,easly:Identifier}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame>
                <LayoutHorizontalCollectionPlaceholderFrame.Selectors>
                    <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                </LayoutHorizontalCollectionPlaceholderFrame.Selectors>
            </LayoutHorizontalCollectionPlaceholderFrame>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IImport,easly:Import}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IInheritance,easly:Inheritance}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IInstruction,easly:Instruction}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:ILibrary,easly:Library}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IName,easly:Name}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IObjectType,easly:ObjectType}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IPattern,easly:Pattern}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IQualifiedName,easly:QualifiedName}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IQueryOverload,easly:QueryOverload}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IQueryOverloadType,easly:QueryOverloadType}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IRange,easly:Range}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IRename,easly:Rename}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:ITypeArgument,easly:TypeArgument}"">
        <LayoutHorizontalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutHorizontalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutHorizontalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:ITypedef,easly:Typedef}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
    <LayoutBlockTemplate NodeType=""{xaml:Type easly:IBlock,easly:IWith,easly:With}"">
        <LayoutVerticalPanelFrame>
            <LayoutHorizontalPanelFrame>
                <LayoutHorizontalPanelFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutHorizontalPanelFrame.BlockVisibility>
                <LayoutKeywordFrame>Replicate</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""ReplicationPattern""/>
                <LayoutKeywordFrame>From</LayoutKeywordFrame>
                <LayoutPlaceholderFrame PropertyName=""SourceIdentifier"">
                    <LayoutPlaceholderFrame.Selectors>
                        <LayoutFrameSelector SelectorType=""{xaml:Type easly:IIdentifier}"" SelectorName=""Identifier""/>
                    </LayoutPlaceholderFrame.Selectors>
                </LayoutPlaceholderFrame>
                <LayoutKeywordFrame>All</LayoutKeywordFrame>
            </LayoutHorizontalPanelFrame>
            <LayoutVerticalCollectionPlaceholderFrame/>
            <LayoutKeywordFrame Text=""end"">
                <LayoutKeywordFrame.BlockVisibility>
                    <LayoutReplicationFrameVisibility/>
                </LayoutKeywordFrame.BlockVisibility>
            </LayoutKeywordFrame>
        </LayoutVerticalPanelFrame>
    </LayoutBlockTemplate>
</LayoutTemplateList>
";
        #endregion
    }
}
