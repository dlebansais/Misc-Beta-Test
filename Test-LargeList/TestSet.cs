using LargeList;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace Test
{
    [TestFixture]
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

            TestLargeList<int>.Init(IsStrict, DefaultMaxSegmentCapacity);
            TestLargeList<string>.Init(IsStrict, DefaultMaxSegmentCapacity);
            TestLargeList<TestClass>.Init(IsStrict, DefaultMaxSegmentCapacity);
        }

        [Test]
        public static void TestSessionInteger()
        {
            TestStatus Status;

            Status = TestLargeList<int>.TestAll(CreateInt);
            Assert.That(Status.Succeeded, Status.Name);
        }

        private static int CreateInt(Random rand, int MaxIntValue)
        {
            return rand.Next(MaxIntValue);
        }

        [Test]
        public static void TestSessionString()
        {
            TestStatus Status;

            Status = TestLargeList<string>.TestAll(CreateString);
            Assert.That(Status.Succeeded, Status.Name);
        }

        private static string CreateString(Random rand, int MaxIntValue)
        {
            return rand.Next(MaxIntValue).ToString();
        }

        [Test]
        public static void TestSessionGeneric()
        {
            TestStatus Status;

            Status = TestLargeList<TestClass>.TestAll(CreateTestClass);
            Assert.That(Status.Succeeded, Status.Name);
        }

        private static TestClass CreateTestClass(Random rand, int MaxIntValue)
        {
            int IntegerValue = rand.Next(MaxIntValue);
            string StringValue = rand.Next(MaxIntValue).ToString();
            return new TestClass(IntegerValue, StringValue);
        }
    }
}
