using System.Collections.Generic;

namespace Coverage
{
    public interface IMain : BaseNode.INode
    {
        ILeaf Placeholder { get; }
        Easly.IOptionalReference<ILeaf> Optional { get; }
        BaseNode.CopySemantic CopySpecification { get; }
        BaseNode.IBlockList<ILeaf, Leaf> LeafBlocks { get; }
        IList<ILeaf> LeafPath { get; }
    }

    [System.Serializable]
    public class Main : BaseNode.Node, IMain
    {
        public virtual ILeaf Placeholder { get; set; }
        public virtual Easly.IOptionalReference<ILeaf> Optional { get; set; }
        public virtual BaseNode.CopySemantic CopySpecification { get; set; }
        public virtual BaseNode.IBlockList<ILeaf, Leaf> LeafBlocks { get; set; }
        public virtual IList<ILeaf> LeafPath { get; set; }
    }
}
