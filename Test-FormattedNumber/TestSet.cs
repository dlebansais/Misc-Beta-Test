using FormattedNumber;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.IO;

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

            Assembly FormattedNumberAssembly;

            try
            {
                FormattedNumberAssembly = Assembly.Load("FormattedNumber");
            }
            catch
            {
                FormattedNumberAssembly = null;
            }
            Assume.That(FormattedNumberAssembly != null);
        }

        #region Basic Tests
        [Test]
        public static void Test0()
        {
        }
        #endregion
    }
}
