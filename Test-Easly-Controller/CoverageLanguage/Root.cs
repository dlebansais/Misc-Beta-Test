namespace Coverage
{
    public interface IRoot : BaseNode.INode
    {
        BaseNode.IBlockList<IMain, Main> MainBlocks { get; }
        Easly.IOptionalReference<IMain> UnassignedOptionalMain { get; }
    }

    [System.Serializable]
    public class Root : BaseNode.Node, IRoot
    {
        public virtual BaseNode.IBlockList<IMain, Main> MainBlocks { get; set; }
        public virtual Easly.IOptionalReference<IMain> UnassignedOptionalMain { get; set; }
    }
}
