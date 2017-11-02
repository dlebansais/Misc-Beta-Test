using LargeList;
using NUnit.Framework;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace Test
{
    [TestFixture]
    //[Culture("en-US")]
    public class TestSet
    {
        [OneTimeSetUp]
        public static void InitTestSession()
        {
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.DefaultThreadCurrentCulture = enUS;
            CultureInfo.DefaultThreadCurrentUICulture = enUS;
            Thread.CurrentThread.CurrentCulture = enUS;
            Thread.CurrentThread.CurrentUICulture = enUS;

            Assembly LargeListAssembly;

            try
            {
                LargeListAssembly = Assembly.Load("LargeList");
            }
            catch
            {
                LargeListAssembly = null;
            }
            Assume.That(LargeListAssembly != null);

            LargeListAssemblyAttribute Attribute = LargeListAssembly.GetCustomAttribute(typeof(LargeListAssemblyAttribute)) as LargeListAssemblyAttribute;
            Assume.That(Attribute != null);

            bool IsStrict = Attribute.IsStrict;
            int DefaultMaxSegmentCapacity = Attribute.DefaultMaxSegmentCapacity;
            TestLargeList.Init(IsStrict, DefaultMaxSegmentCapacity);
        }

        [Test]
        public static void TestSession()
        {
            TestStatus Status;

            Status = TestLargeList.TestAll();
            Assert.That(Status.Succeeded, Status.Name);
        }
    }
}
