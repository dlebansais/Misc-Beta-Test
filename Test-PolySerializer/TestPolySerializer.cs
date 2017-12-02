using PolySerializer;
using System.IO;

namespace Test
{
    public class TestPolySerializer
    {
        public static void Init()
        {
        }

        public static bool Test()
        {
            Serializer s = new Serializer();

            ChildAA childAA = new ChildAA();
            MemoryStream ms = new MemoryStream();

            s.Serialize(ms, childAA);
            s.RootType = typeof(ChildAB);

            ChildAB childAB = s.Deserialize(ms) as ChildAB;

            return childAB != null;
        }
    }
}
