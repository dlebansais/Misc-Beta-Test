using BaseNode;
using BaseNodeHelper;
using EaslyController.ReadOnly;
using NUnit.Framework;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace Test
{
    [TestFixture]
    public class TestSet
    {
        #region Setup
        [OneTimeSetUp]
        public static void InitTestSession()
        {
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.DefaultThreadCurrentCulture = enUS;
            CultureInfo.DefaultThreadCurrentUICulture = enUS;
            Thread.CurrentThread.CurrentCulture = enUS;
            Thread.CurrentThread.CurrentUICulture = enUS;

            Assembly EaslyControllerAssembly;

            try
            {
                EaslyControllerAssembly = Assembly.Load("Easly-Controller");
            }
            catch
            {
                EaslyControllerAssembly = null;
            }
            Assume.That(EaslyControllerAssembly != null);
        }
        #endregion

        #region Init
        [Test]
        public static void TestInit()
        {
            INode RootNode = NodeHelper.CreateEmptyName();
            IReadOnlyRootNodeIndex RootIndex = new ReadOnlyRootNodeIndex(RootNode);
            IReadOnlyController Controller = ReadOnlyController.Create(RootIndex);
            Assert.That(Controller != null, "Init Controller");
        }
        #endregion
    }
}
