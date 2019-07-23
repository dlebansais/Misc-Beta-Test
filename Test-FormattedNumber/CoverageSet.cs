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
            CultureInfo frFR = CultureInfo.CreateSpecificCulture("fr-FR");
            CultureInfo.DefaultThreadCurrentCulture = frFR;
            CultureInfo.DefaultThreadCurrentUICulture = frFR;
            Thread.CurrentThread.CurrentCulture = frFR;
            Thread.CurrentThread.CurrentUICulture = frFR;

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
