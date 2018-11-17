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

            Assembly SimpleDatabaseAssembly;

            try
            {
                SimpleDatabaseAssembly = Assembly.Load("SimpleDatabase");
            }
            catch
            {
                SimpleDatabaseAssembly = null;
            }
            Assume.That(SimpleDatabaseAssembly != null);
        }

        #region Init
        [Test]
        public static void TestInit()
        {
            Assert.That(true, "Init");
        }
        #endregion
    }
}
