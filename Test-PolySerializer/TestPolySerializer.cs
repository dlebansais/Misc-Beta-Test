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

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, childAA);
            }

            s.RootType = typeof(ChildAB);

            ChildAB childAB;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                childAB = s.Deserialize(fs) as ChildAB;
            }

            return childAB != null;
        }
    }
}
