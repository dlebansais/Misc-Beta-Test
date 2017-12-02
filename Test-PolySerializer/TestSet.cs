using PolySerializer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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

            Assembly PolySerializerAssembly;

            try
            {
                PolySerializerAssembly = Assembly.Load("PolySerializer");
            }
            catch
            {
                PolySerializerAssembly = null;
            }
            Assume.That(PolySerializerAssembly != null);

            TestPolySerializer.Init();
        }

        #region Test
        [Test]
        public static void TestSession()
        {
            bool Status;

            Status = TestPolySerializer.Test();
            Assert.That(Status, "Simple test");
        }
        #endregion
    }
}
