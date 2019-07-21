using System.Globalization;
using System.Reflection;
using System.Threading;
using FormattedNumber;
using NUnit.Framework;

namespace Coverage
{
    [TestFixture]
    public class CoverageSet
    {
        #region Setup
        [OneTimeSetUp]
        public static void InitCoverageSession()
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
        #endregion

        #region Basic
        [Test]
        [Category("Coverage")]
        public static void Creation0()
        {
            FormattedNumber.FormattedNumber n = Parser.Parse("0");
            Assert.That(n != null, "Sanity Check #0");
        }
        #endregion
    }
}
