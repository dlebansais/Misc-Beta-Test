using System.Collections.Generic;

namespace Coverage
{
    public interface IRoot : BaseNode.INode
    {
        BaseNode.IBlockList<IMain, Main> MainBlocks { get; }
        Easly.IOptionalReference<IMain> UnassignedOptionalMain { get; }
        string ValueString { get; }
        IList<ILeaf> LeafPath { get; }
    }

    [System.Serializable]
    public class Root : BaseNode.Node, IRoot
    {
        public virtual BaseNode.IBlockList<IMain, Main> MainBlocks { get; set; }
        public virtual Easly.IOptionalReference<IMain> UnassignedOptionalMain { get; set; }
        public virtual string ValueString { get; set; }
        public virtual IList<ILeaf> LeafPath { get; set; }
    }
}
