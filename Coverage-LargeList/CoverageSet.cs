namespace Coverage
{
    using LargeList;
    using Xunit;

    public class CoverageSet
    {
        public static void TestSessionInteger_collection()
        {
            LargeList<int> List = new LargeList<int>();
            List.Add(0);
            Assert.True(List.Count > 0);
        }
    }
}
