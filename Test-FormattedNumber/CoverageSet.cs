using System;
using System.Diagnostics;
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

        #region Tools
        #endregion

        #region Basic Contract
        [Test]
        [Category("Coverage")]
        public static void BasicContractInteger()
        {
            FormattedNumber.FormattedNumber Number;
            CanonicalNumber Canonical = CanonicalNumber.FromEFloat(PeterO.Numbers.EFloat.FromInt32(1));
            Exception LastException = null;

            //Debug.Assert(false);

            try
            {
                LastException = null;
                Number = new FormattedInteger(IntegerBase.Decimal, OptionalSign.None, 0, null, string.Empty, Canonical);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentNullException);
            Assert.That(LastException.Message == "Value cannot be null.\r\nParameter name: integerText");

            try
            {
                LastException = null;
                Number = new FormattedInteger(IntegerBase.Decimal, OptionalSign.None, 0, "1", null, Canonical);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentNullException);
            Assert.That(LastException.Message == "Value cannot be null.\r\nParameter name: invalidText");

            try
            {
                LastException = null;
                Number = new FormattedInteger(IntegerBase.Decimal, OptionalSign.None, 0, "1", string.Empty, null);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentNullException);
            Assert.That(LastException.Message == "Value cannot be null.\r\nParameter name: canonical");

            try
            {
                LastException = null;
                Number = new FormattedInteger(IntegerBase.Decimal, OptionalSign.None, -1, "1", string.Empty, Canonical);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentOutOfRangeException);
            Assert.That(LastException.Message == "Specified argument was out of the range of valid values.\r\nParameter name: leadingZeroCount");

            Number = new FormattedInteger(IntegerBase.Decimal, OptionalSign.None, 0, "1", string.Empty, Canonical);
            Assert.That(Number != null);
            Assert.That(Number is FormattedInteger AsInteger && AsInteger.IsValid && AsInteger.ExponentPart.Length == 0);
            string Diagnostic = Number.Diagnostic;
            Assert.That(Diagnostic != null);
        }

        [Test]
        [Category("Coverage")]
        public static void BasicContractInvalid()
        {
            FormattedNumber.FormattedNumber Number;
            CanonicalNumber Canonical = CanonicalNumber.NaN;
            Exception LastException = null;

            //Debug.Assert(false);

            try
            {
                LastException = null;
                Number = new FormattedInvalid(null, Canonical);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentNullException);
            Assert.That(LastException.Message == "Value cannot be null.\r\nParameter name: invalidText");

            try
            {
                LastException = null;
                Number = new FormattedInvalid(string.Empty, null);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentNullException);
            Assert.That(LastException.Message == "Value cannot be null.\r\nParameter name: canonical");

            Number = new FormattedInvalid(string.Empty, Canonical);
            Assert.That(Number != null);
            Assert.That(Number is FormattedInvalid AsInvalid && !AsInvalid.IsValid && AsInvalid.SignificandPart.Length == 0 && AsInvalid.ExponentPart.Length == 0);
            string Diagnostic = Number.Diagnostic;
            Assert.That(Diagnostic != null);
        }

        [Test]
        [Category("Coverage")]
        public static void BasicContractReal()
        {
            FormattedNumber.FormattedNumber Number;
            CanonicalNumber Canonical = CanonicalNumber.FromEFloat(PeterO.Numbers.EFloat.FromDouble(1.1));
            Exception LastException = null;

            //Debug.Assert(false);

            try
            {
                LastException = null;
                Number = new FormattedReal(OptionalSign.None, 0, null, Parser.NoSeparator, string.Empty, 'e', OptionalSign.None, "0", string.Empty, Canonical);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentNullException);
            Assert.That(LastException.Message == "Value cannot be null.\r\nParameter name: integerText");

            try
            {
                LastException = null;
                Number = new FormattedReal(OptionalSign.None, 0, "1", Parser.NoSeparator, null, 'e', OptionalSign.None, "0", string.Empty, Canonical);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentNullException);
            Assert.That(LastException.Message == "Value cannot be null.\r\nParameter name: fractionalText");

            try
            {
                LastException = null;
                Number = new FormattedReal(OptionalSign.None, 0, "1", Parser.NoSeparator, string.Empty, 'e', OptionalSign.None, null, string.Empty, Canonical);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentNullException);
            Assert.That(LastException.Message == "Value cannot be null.\r\nParameter name: exponentText");

            try
            {
                LastException = null;
                Number = new FormattedReal(OptionalSign.None, 0, "1", Parser.NoSeparator, string.Empty, 'e', OptionalSign.None, "0", null, Canonical);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentNullException);
            Assert.That(LastException.Message == "Value cannot be null.\r\nParameter name: invalidText");

            try
            {
                LastException = null;
                Number = new FormattedReal(OptionalSign.None, 0, "1", Parser.NoSeparator, string.Empty, 'e', OptionalSign.None, "0", string.Empty, null);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentNullException);
            Assert.That(LastException.Message == "Value cannot be null.\r\nParameter name: canonical");

            try
            {
                LastException = null;
                Number = new FormattedReal(OptionalSign.None, -1, "1", Parser.NoSeparator, string.Empty, 'e', OptionalSign.None, "0", string.Empty, Canonical);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentOutOfRangeException);
            Assert.That(LastException.Message == "Specified argument was out of the range of valid values.\r\nParameter name: leadingZeroCount");

            try
            {
                LastException = null;
                Number = new FormattedReal(OptionalSign.None, 0, string.Empty, Parser.NeutralDecimalSeparator, string.Empty, 'e', OptionalSign.None, "0", string.Empty, Canonical);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentException);
            Assert.That(LastException.Message == "Either integerText or fractionalText must not be empty.");

            try
            {
                LastException = null;
                Number = new FormattedReal(OptionalSign.None, 0, string.Empty, Parser.NoSeparator, string.Empty, 'e', OptionalSign.None, "0", string.Empty, Canonical);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentException);
            Assert.That(LastException.Message == "integerText must not be empty.");

            try
            {
                LastException = null;
                Number = new FormattedReal(OptionalSign.None, 0, "1", Parser.NoSeparator, string.Empty, 'e', OptionalSign.None, string.Empty, string.Empty, Canonical);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentException);
            Assert.That(LastException.Message == "exponentText must not be empty.");

            Number = new FormattedReal(OptionalSign.None, 0, "1", Parser.NeutralDecimalSeparator, "1", 'e', OptionalSign.None, "0", string.Empty, Canonical);
            Assert.That(Number != null);
            Assert.That(Number is FormattedReal AsReal && AsReal.IsValid);
            string Diagnostic = Number.Diagnostic;
            Assert.That(Diagnostic != null);
        }

        [Test]
        [Category("Coverage")]
        public static void BasicContractNumber()
        {
            FormattedNumber.FormattedNumber Number;
            CanonicalNumber Canonical = CanonicalNumber.FromEFloat(PeterO.Numbers.EFloat.FromInt32(1));
            Exception LastException = null;

            try
            {
                LastException = null;
                Number = FormattedNumber.FormattedNumber.FromCanonical(null);
            }
            catch (Exception e)
            {
                LastException = e;
                Number = null;
            }

            Assert.That(Number == null);
            Assert.That(LastException is ArgumentNullException);
            Assert.That(LastException.Message == "Value cannot be null.\r\nParameter name: canonical");

            Number = FormattedNumber.FormattedNumber.FromCanonical(CanonicalNumber.NaN);
            Assert.That(Number == FormattedNumber.FormattedNumber.NaN);

            Number = FormattedNumber.FormattedNumber.FromCanonical(CanonicalNumber.PositiveInfinity);
            Assert.That(Number == FormattedNumber.FormattedNumber.PositiveInfinity);

            Number = FormattedNumber.FormattedNumber.FromCanonical(CanonicalNumber.NegativeInfinity);
            Assert.That(Number == FormattedNumber.FormattedNumber.NegativeInfinity);

            //Debug.Assert(false);
            Number = FormattedNumber.FormattedNumber.FromCanonical(Canonical);
            Assert.That(Number != null);
            Assert.That(Number is FormattedReal AsReal && AsReal.IsValid);
            string Diagnostic = Number.Diagnostic;
            Assert.That(Diagnostic != null);
        }

        [Test]
        [Category("Coverage")]
        public static void BasicContractArithmetic()
        {
            Assert.That(Arithmetic.Precision > 0);

            try
            {
                Arithmetic.Precision = -1;
            }
            catch (Exception e)
            {
                Assert.That(e is ArgumentOutOfRangeException && e.Message == "Specified argument was out of the range of valid values.\r\nParameter name: value");
            }
        }
        #endregion

        #region Basic
        [Test]
        [Category("Coverage")]
        public static void Basic()
        {
            TestParser("");
            TestParser("+");
            TestParser("-");
            TestParser(".");
            TestParser(",");
            TestParser("e");
            TestParser("E");
            TestParser("++");
            TestParser("--");
            TestParser("+-");
            TestParser("-+");
            TestParser("+.");
            TestParser("-.");
            TestParser("+,");
            TestParser("-,");
            TestParser("+e");
            TestParser("-e");
            TestParser("+E");
            TestParser("-E");
            TestParser("$^)=%*");

            Assert.That(!IntegerBase.Hexadecimal.IsValidNumber(string.Empty));
            Assert.That(IntegerBase.Hexadecimal.IsValidNumber("0"));
            Assert.That(!IntegerBase.Hexadecimal.IsValidNumber("x"));
            Assert.That(IntegerBase.Hexadecimal.IsValidNumber("00"));
            Assert.That(!IntegerBase.Hexadecimal.IsValidNumber("00", supportLeadingZeroes: false));

            Assert.That(!IntegerBase.Hexadecimal.IsValidSignificand(string.Empty));
            Assert.That(IntegerBase.Hexadecimal.IsValidSignificand("0"));
            Assert.That(!IntegerBase.Hexadecimal.IsValidSignificand("x"));
            Assert.That(!IntegerBase.Hexadecimal.IsValidSignificand("00"));
        }

        [Test]
        [Category("Coverage")]
        public static void BasicDecimal()
        {
            TestParser("0");
            TestParser("+0");
            TestParser("-0");
            TestParser("00");
            TestParser("+00");
            TestParser("-00");
            TestParser("09");
            TestParser("+09");
            TestParser("-09");
            TestParser("9");
            TestParser("+9");
            TestParser("-9");
            TestParser("90");
            TestParser("+90");
            TestParser("-90");
            TestParser("99");
            TestParser("+99");
            TestParser("-99");
            TestParser("0e0");
            TestParser("0e-0");
            TestParser("0e+0");
            TestParser("0e9");
            TestParser("0e+9");
            TestParser("0e-9");
            TestParser("0e09");
            TestParser("0e+09");
            TestParser("0e-09");
            TestParser("0e90");
            TestParser("0e+90");
            TestParser("0e-90");
            TestParser("0e99");
            TestParser("0e+99");
            TestParser("0e-99");
            //Debug.Assert(false);
            TestParser("0123456789");
            TestParser("012345678x9");

            Assert.That(IntegerBase.Decimal.ToDigit(0) == '0');
            Assert.That(IntegerBase.Decimal.ToDigit(9) == '9');
            Assert.That(IntegerBase.Decimal.ToValue('0') == 0);
            Assert.That(IntegerBase.Decimal.ToValue('9') == 9);
        }

        [Test]
        [Category("Coverage")]
        public static void BasicBinary()
        {
            TestParser("0");
            TestParser("0:");
            TestParser("0:B");
            TestParser("+0:");
            TestParser("-0:");
            TestParser("+0:B");
            TestParser("-0:B");
            TestParser("00");
            TestParser("00:");
            TestParser("00:B");
            TestParser("+00");
            TestParser("+00:");
            TestParser("+00:B");
            TestParser("-00");
            TestParser("-00:");
            TestParser("-00:B");
            TestParser("01");
            TestParser("01:B");
            TestParser("01:B");
            TestParser("+01:B");
            TestParser("-01");
            TestParser("1:B");
            TestParser("+1:B");
            TestParser("-1:B");
            TestParser("10:B");
            TestParser("+10:B");
            TestParser("-10:B");
            TestParser("11:B");
            TestParser("+11:B");
            TestParser("-11:B");
            TestParser("01111:B");
            TestParser("0111x1:B");
            TestParser("1x1111");
            //Debug.Assert(false);
            TestParser("01111:Bx");

            Assert.That(IntegerBase.Binary.ToDigit(0) == '0');
            Assert.That(IntegerBase.Binary.ToDigit(1) == '1');
            Assert.That(IntegerBase.Binary.ToValue('0') == 0);
            Assert.That(IntegerBase.Binary.ToValue('1') == 1);
        }

        [Test]
        [Category("Coverage")]
        public static void BasicOctal()
        {
            TestParser("0");
            TestParser("0:");
            TestParser("0:O");
            TestParser("+0:");
            TestParser("-0:");
            TestParser("+0:O");
            TestParser("-0:O");
            TestParser("00");
            TestParser("00:");
            TestParser("00:O");
            TestParser("+00");
            TestParser("+00:");
            TestParser("+00:O");
            TestParser("-00");
            TestParser("-00:");
            TestParser("-00:O");
            TestParser("07");
            TestParser("07:O");
            TestParser("07:O");
            TestParser("+07:O");
            TestParser("-07");
            TestParser("7:O");
            TestParser("+7:O");
            TestParser("-7:O");
            TestParser("70:O");
            TestParser("+70:O");
            TestParser("-70:O");
            TestParser("77:O");
            TestParser("+77:O");
            TestParser("-77:O");
            TestParser("01234567:O");
            TestParser("0123456x7:O");
            TestParser("0123456x7:Ox");

            Assert.That(IntegerBase.Octal.ToDigit(0) == '0');
            Assert.That(IntegerBase.Octal.ToDigit(7) == '7');
            Assert.That(IntegerBase.Octal.ToValue('0') == 0);
            Assert.That(IntegerBase.Octal.ToValue('7') == 7);
        }

        [Test]
        [Category("Coverage")]
        public static void BasicHexadecimal()
        {
            TestParser("0");
            TestParser("0:");
            TestParser("0:H");
            TestParser("+0:");
            TestParser("-0:");
            TestParser("+0:H");
            TestParser("-0:H");
            TestParser("00");
            TestParser("00:");
            TestParser("00:H");
            TestParser("+00");
            TestParser("+00:");
            TestParser("+00:H");
            TestParser("-00");
            TestParser("-00:");
            TestParser("-00:H");
            TestParser("0f");
            TestParser("0f:H");
            TestParser("0f:H");
            TestParser("+0f:H");
            TestParser("-0f");
            TestParser("f:H");
            TestParser("+f:H");
            TestParser("-f:H");
            TestParser("f0:H");
            TestParser("+f0:H");
            TestParser("-f0:H");
            TestParser("ff:H");
            TestParser("+ff:H");
            TestParser("-ff:H");
            TestParser("0F");
            TestParser("0F:H");
            TestParser("0F:H");
            TestParser("+0F:H");
            TestParser("-0F");
            TestParser("F:H");
            TestParser("+F:H");
            TestParser("-F:H");
            TestParser("F0:H");
            TestParser("+F0:H");
            TestParser("-F0:H");
            TestParser("FF:H");
            TestParser("+FF:H");
            TestParser("-FF:H");
            TestParser("0123456789abcdef:H");
            TestParser("0123456789ABCDEF:H");
            TestParser("0123456789abcdexf:H");
            TestParser("0123456789ABCDExF:H");
            TestParser("0123456789abcdef:Hx");
            TestParser("0123456789ABCDEF:Hx");

            Assert.That(IntegerBase.Hexadecimal.ToDigit(0) == '0');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(15) == 'F');
            Assert.That(IntegerBase.Hexadecimal.ToValue('0') == 0);
            Assert.That(IntegerBase.Hexadecimal.ToValue('F') == 15);
            Assert.That(IntegerBase.Hexadecimal.ToValue('f') == 15);
        }

        [Test]
        [Category("Coverage")]
        public static void BasicReal()
        {
            TestParser("0");
            TestParser("+0");
            TestParser("-0");
            TestParser("00");
            TestParser("+00");
            TestParser("-00");
            TestParser("01");
            TestParser("+01");
            TestParser("-01");
            TestParser("1");
            TestParser("+1");
            TestParser("-1");
            TestParser("10");
            TestParser("+10");
            TestParser("-10");
            TestParser("11");
            TestParser("+11");
            TestParser("-11");

            TestParser("0.");
            TestParser("+0.");
            TestParser("-0.");
            TestParser("00.");
            TestParser("+00.");
            TestParser("-00.");
            TestParser("01.");
            TestParser("+01.");
            TestParser("-01.");
            TestParser("1.");
            TestParser("+1.");
            TestParser("-1.");
            TestParser("10.");
            TestParser("+10.");
            TestParser("-10.");
            TestParser("11.");
            TestParser("+11.");
            TestParser("-11.");

            TestParser(".0");
            TestParser("+.0");
            TestParser("-.0");
            TestParser("0.0");
            TestParser("+0.0");
            TestParser("-0.0");
            TestParser("0.1");
            TestParser("+0.1");
            TestParser("-0.1");
            TestParser(".1");
            TestParser("+.1");
            TestParser("-.1");
            TestParser("1.0");
            TestParser("+1.0");
            TestParser("-1.0");
            TestParser("1.1");
            TestParser("+1.1");
            TestParser("-1.1");

            TestParser("0.0");
            TestParser("+0.0");
            TestParser("-0.0");
            TestParser("00.0");
            TestParser("+00.0");
            TestParser("-00.0");
            TestParser("01.0");
            TestParser("+01.0");
            TestParser("-01.0");
            TestParser("1.0");
            TestParser("+1.0");
            TestParser("-1.0");
            TestParser("10.0");
            TestParser("+10.0");
            TestParser("-10.0");
            TestParser("11.0");
            TestParser("+11.0");
            TestParser("-11.0");

            TestParser("0.9");
            TestParser("+0.9");
            TestParser("-0.9");
            TestParser("00.9");
            TestParser("+00.9");
            TestParser("-00.9");
            TestParser("01.9");
            TestParser("+01.9");
            TestParser("-01.9");
            TestParser("1.9");
            TestParser("+1.9");
            TestParser("-1.9");
            TestParser("10.9");
            TestParser("+10.9");
            TestParser("-10.9");
            TestParser("11.9");
            TestParser("+11.9");
            TestParser("-11.9");

            TestParser("0.09");
            TestParser("+0.09");
            TestParser("-0.09");
            TestParser("00.09");
            TestParser("+00.09");
            TestParser("-00.09");
            TestParser("01.09");
            TestParser("+01.09");
            TestParser("-01.09");
            TestParser("1.09");
            TestParser("+1.09");
            TestParser("-1.09");
            TestParser("10.09");
            TestParser("+10.09");
            TestParser("-10.09");
            TestParser("11.09");
            TestParser("+11.09");
            TestParser("-11.09");

            TestParser("0.99");
            TestParser("+0.99");
            TestParser("-0.99");
            TestParser("00.99");
            TestParser("+00.99");
            TestParser("-00.99");
            TestParser("01.99");
            TestParser("+01.99");
            TestParser("-01.99");
            TestParser("1.99");
            TestParser("+1.99");
            TestParser("-1.99");
            TestParser("10.99");
            TestParser("+10.99");
            TestParser("-10.99");
            TestParser("11.99");
            TestParser("+11.99");
            TestParser("-11.99");

            TestParser("0.90");
            TestParser("+0.90");
            TestParser("-0.90");
            TestParser("00.90");
            TestParser("+00.90");
            TestParser("-00.90");
            TestParser("01.90");
            TestParser("+01.90");
            TestParser("-01.90");
            TestParser("1.90");
            TestParser("+1.90");
            TestParser("-1.90");
            TestParser("10.90");
            TestParser("+10.90");
            TestParser("-10.90");
            TestParser("11.90");
            TestParser("+11.90");
            TestParser("-11.90");

            TestParser("0e0");
            TestParser("0e-0");
            TestParser("0e+0");
            TestParser("0e9");
            TestParser("0e+9");
            TestParser("0e-9");
            TestParser("0e09");
            TestParser("0e+09");
            TestParser("0e-09");
            TestParser("0e90");
            TestParser("0e+90");
            TestParser("0e-90");
            TestParser("0e99");
            TestParser("0e+99");
            TestParser("0e-99");

            TestParser("0.e0");
            TestParser("0.e-0");
            TestParser("0.e+0");
            TestParser("0.e9");
            TestParser("0.e+9");
            TestParser("0.e-9");
            TestParser("0.e09");
            TestParser("0.e+09");
            TestParser("0.e-09");
            TestParser("0.e90");
            TestParser("0.e+90");
            TestParser("0.e-90");
            TestParser("0.e99");
            TestParser("0.e+99");
            TestParser("0.e-99");

            TestParser("9.e0");
            TestParser("9.e-0");
            TestParser("9.e+0");
            TestParser("9.e9");
            TestParser("9.e+9");
            TestParser("9.e-9");
            TestParser("9.e09");
            TestParser("9.e+09");
            TestParser("9.e-09");
            TestParser("9.e90");
            TestParser("9.e+90");
            TestParser("9.e-90");
            TestParser("9.e99");
            TestParser("9.e+99");
            TestParser("9.e-99");

            TestParser("0.0e0");
            TestParser("0.0e-0");
            TestParser("0.0e+0");
            TestParser("0.0e9");
            TestParser("0.0e+9");
            TestParser("0.0e-9");
            TestParser("0.0e09");
            TestParser("0.0e+09");
            TestParser("0.0e-09");
            TestParser("0.0e90");
            TestParser("0.0e+90");
            TestParser("0.0e-90");
            TestParser("0.0e99");
            TestParser("0.0e+99");
            TestParser("0.0e-99");

            TestParser("0.9e0");
            TestParser("0.9e-0");
            TestParser("0.9e+0");
            TestParser("0.9e9");
            TestParser("0.9e+9");
            TestParser("0.9e-9");
            TestParser("0.9e09");
            TestParser("0.9e+09");
            TestParser("0.9e-09");
            TestParser("0.9e90");
            TestParser("0.9e+90");
            TestParser("0.9e-90");
            TestParser("0.9e99");
            TestParser("0.9e+99");
            TestParser("0.9e-99");

            TestParser("0.09e0");
            TestParser("0.09e-0");
            TestParser("0.09e+0");
            TestParser("0.09e9");
            TestParser("0.09e+9");
            TestParser("0.09e-9");
            TestParser("0.09e09");
            TestParser("0.09e+09");
            TestParser("0.09e-09");
            TestParser("0.09e90");
            TestParser("0.09e+90");
            TestParser("0.09e-90");
            TestParser("0.09e99");
            TestParser("0.09e+99");
            TestParser("0.09e-99");

            TestParser("00.09e0");
            TestParser("00.09e-0");
            TestParser("00.09e+0");
            TestParser("00.09e9");
            TestParser("00.09e+9");
            TestParser("00.09e-9");
            TestParser("00.09e09");
            TestParser("00.09e+09");
            TestParser("00.09e-09");
            TestParser("00.09e90");
            TestParser("00.09e+90");
            TestParser("00.09e-90");
            TestParser("00.09e99");
            TestParser("00.09e+99");
            TestParser("00.09e-99");

            TestParser("+00.09e0");
            TestParser("+00.09e-0");
            TestParser("+00.09e+0");
            TestParser("+00.09e9");
            TestParser("+00.09e+9");
            TestParser("+00.09e-9");
            TestParser("+00.09e09");
            TestParser("+00.09e+09");
            TestParser("+00.09e-09");
            TestParser("+00.09e90");
            TestParser("+00.09e+90");
            TestParser("+00.09e-90");
            TestParser("+00.09e99");
            TestParser("+00.09e+99");
            TestParser("+00.09e-99");

            TestParser("-00.09e0");
            TestParser("-00.09e-0");
            TestParser("-00.09e+0");
            TestParser("-00.09e9");
            TestParser("-00.09e+9");
            TestParser("-00.09e-9");
            TestParser("-00.09e09");
            TestParser("-00.09e+09");
            TestParser("-00.09e-09");
            TestParser("-00.09e90");
            TestParser("-00.09e+90");
            TestParser("-00.09e-90");
            TestParser("-00.09e99");
            TestParser("-00.09e+99");
            TestParser("-00.09e-99");
            TestParser("123.456e789");
            TestParser("123.x456e789");
            TestParser("123.456xe789");
            TestParser("123.456ex789");
            TestParser("123.456e78x9");
            TestParser("123.456e789x");

            TestParser("90e0");
            TestParser("90e-0");
            TestParser("90e+0");
            TestParser("90e9");
            TestParser("90e+9");
            TestParser("90e-9");
            TestParser("90e09");
            TestParser("90e+09");
            TestParser("90e-09");
            TestParser("90e90");
            TestParser("90e+90");
            TestParser("90e-90");
            TestParser("90e99");
            TestParser("90e+99");
            TestParser("90e-99");
            TestParser("90e99x");
        }

        private static void TestParser(string text)
        {
            FormattedNumber.FormattedNumber Number = Parser.Parse(text);
            Assert.That(Number != null, $"Number == null for '{text}'");

            string NumberText = Number.ToString();
            Assert.That(NumberText == text, $"n give '{NumberText}' but '{text}' expected.");
        }

        [Test]
        [Category("Coverage")]
        public static void BasicCanonical()
        {
            CanonicalNumber n0 = new CanonicalNumber(0);
            string n0Text = n0.ToString();
            CanonicalNumber n1 = new CanonicalNumber(-1);
            CanonicalNumber n2 = new CanonicalNumber(2);
            CanonicalNumber n3 = CanonicalNumber.FromEFloat(n1.NumberFloat);
            CanonicalNumber n4 = CanonicalNumber.FromEFloat(n2.NumberFloat);
            CanonicalNumber n5 = CanonicalNumber.FromEFloat(CanonicalNumber.NaN.NumberFloat);
            CanonicalNumber n6 = CanonicalNumber.FromEFloat(CanonicalNumber.PositiveInfinity.NumberFloat);
            CanonicalNumber n7 = CanonicalNumber.FromEFloat(CanonicalNumber.NegativeInfinity.NumberFloat);

            Assert.That(n0.IsEqual(n0));
            Assert.That(n1.IsEqual(n1));
            Assert.That(n2.IsEqual(n2));
            Assert.That(n3.IsEqual(n3));
            Assert.That(n4.IsEqual(n4));

            CanonicalNumber n8 = -n1;
            CanonicalNumber n9 = -n2;

            Assert.That(n0 < n2);
            Assert.That(n2 > n0);

            Assert.That(n0.TryParseInt(out int n0int));
            Assert.That(n1.TryParseInt(out int n1int));
            Assert.That(n2.TryParseInt(out int n2int));
            Assert.That(n3.TryParseInt(out int n3int));
            Assert.That(n4.TryParseInt(out int n4int));
            Assert.That(n8.TryParseInt(out int n8int));
            Assert.That(n9.TryParseInt(out int n9int));

            CheckNotInt(1e-2);
            CheckNotInt(1e20);
            CheckNotInt(123456e123);
        }

        private static void CheckNotInt(double d)
        {
            PeterO.Numbers.EFloat f;
            CanonicalNumber n;
            int nAsInt;

            f = PeterO.Numbers.EFloat.FromDouble(d);
            n = CanonicalNumber.FromEFloat(f);
            Assert.That(!n.TryParseInt(out nAsInt));
        }
        #endregion

        #region Arithmetic
        [Test]
        [Category("Coverage")]
        public static void Add0()
        {
            double d1 = 1.2547856e2;
            double d2 = 5.478231405e-3;

            string Text1 = d1.ToString();
            string Text2 = d2.ToString();

            FormattedNumber.FormattedNumber Number1 = Parser.Parse(Text1);
            FormattedNumber.FormattedNumber Number2 = Parser.Parse(Text2);

            //Debug.Assert(false);
            FormattedNumber.FormattedNumber Result = Number1 + Number2;

            double d = d1 + d2;

            string ExpectedText = d.ToString();

            if (ExpectedText.Length > 4)
                ExpectedText = ExpectedText.Substring(0, ExpectedText.Length - 1);

            string ResultText = Result.ToString();
            if (ResultText.Length > ExpectedText.Length)
                ResultText = ResultText.Substring(0, ExpectedText.Length);

            Assert.That(ResultText == ExpectedText, $"Result={ResultText}, Expected={ExpectedText}");
        }

        [Test]
        [Category("Coverage")]
        public static void Subtract0()
        {
            double d1 = 1.2547856e2;
            double d2 = 5.478231405e-3;

            string Text1 = d1.ToString();
            string Text2 = d2.ToString();

            FormattedNumber.FormattedNumber Number1 = Parser.Parse(Text1);
            FormattedNumber.FormattedNumber Number2 = Parser.Parse(Text2);

            //Debug.Assert(false);
            FormattedNumber.FormattedNumber Result = Number1 - Number2;

            double d = d1 - d2;

            string ExpectedText = d.ToString();

            if (ExpectedText.Length > 4)
                ExpectedText = ExpectedText.Substring(0, ExpectedText.Length - 1);

            string ResultText = Result.ToString();
            if (ResultText.Length > ExpectedText.Length)
                ResultText = ResultText.Substring(0, ExpectedText.Length);

            Assert.That(ResultText == ExpectedText, $"Result={ResultText}, Expected={ExpectedText}");
        }

        [Test]
        [Category("Coverage")]
        public static void Multiply0()
        {
            double d1 = 1.2547856e2;
            double d2 = 5.478231405e-3;

            string Text1 = d1.ToString();
            string Text2 = d2.ToString();

            FormattedNumber.FormattedNumber Number1 = Parser.Parse(Text1);
            FormattedNumber.FormattedNumber Number2 = Parser.Parse(Text2);

            //Debug.Assert(false);
            FormattedNumber.FormattedNumber Result = Number1 * Number2;

            double d = d1 * d2;

            string ExpectedText = d.ToString();

            if (ExpectedText.Length > 4)
                ExpectedText = ExpectedText.Substring(0, ExpectedText.Length - 1);

            string ResultText = Result.ToString();
            if (ResultText.Length > ExpectedText.Length)
                ResultText = ResultText.Substring(0, ExpectedText.Length);

            Assert.That(ResultText == ExpectedText, $"Result={ResultText}, Expected={ExpectedText}");
        }

        [Test]
        [Category("Coverage")]
        public static void Divide0()
        {
            double d1 = -1.2547856e2;
            double d2 = 5.478231405e-3;

            string Text1 = d1.ToString();
            string Text2 = d2.ToString();

            FormattedNumber.FormattedNumber Number1 = Parser.Parse(Text1);
            FormattedNumber.FormattedNumber Number2 = Parser.Parse(Text2);

            //Debug.Assert(false);
            FormattedNumber.FormattedNumber Result = Number1 / Number2;
            Flags Flags = Arithmetic.Flags;
            bool DivideByZero = Flags.DivideByZero;
            Flags.Clear();

            Assert.That(!DivideByZero);

            double d = d1 / d2;
            string ExpectedText = d.ToString();

            if (ExpectedText.Length > 4)
                ExpectedText = ExpectedText.Substring(0, ExpectedText.Length - 1);

            string ResultText = Result.ToString();
            if (ResultText.Length > ExpectedText.Length)
                ResultText = ResultText.Substring(0, ExpectedText.Length);

            Assert.That(ResultText == ExpectedText, $"Result={ResultText}, Expected={ExpectedText}");
        }

        [Test]
        [Category("Coverage")]
        public static void Divide1()
        {
            double d1 = -1.2547856e2;
            double d2 = 0;

            string Text1 = d1.ToString();
            string Text2 = d2.ToString();

            FormattedNumber.FormattedNumber Number1 = Parser.Parse(Text1);
            FormattedNumber.FormattedNumber Number2 = Parser.Parse(Text2);

            //Debug.Assert(false);
            FormattedNumber.FormattedNumber Result = Number1 / Number2;
            Flags Flags = Arithmetic.Flags;
            bool DivideByZero = Flags.DivideByZero;
            Flags.Clear();

            Assert.That(DivideByZero);

            double d = d1 / d2;
            string ExpectedText = d.ToString();

            if (ExpectedText.Length > 4)
                ExpectedText = ExpectedText.Substring(0, ExpectedText.Length - 1);

            string ResultText = Result.ToString();
            if (ResultText.Length > ExpectedText.Length)
                ResultText = ResultText.Substring(0, ExpectedText.Length);

            Assert.That(ResultText == ExpectedText, $"Result={ResultText}, Expected={ExpectedText}");
        }

        [Test]
        [Category("Coverage")]
        public static void Negate0()
        {
            double d1 = -1.2547856e2;

            string Text1 = d1.ToString();

            FormattedNumber.FormattedNumber Number1 = Parser.Parse(Text1);

            //Debug.Assert(false);
            FormattedNumber.FormattedNumber Result = -Number1;
            Flags Flags = Arithmetic.Flags;
            bool DivideByZero = Flags.DivideByZero;
            Flags.Clear();

            Assert.That(!DivideByZero);

            double d = -d1;
            string ExpectedText = d.ToString();

            if (ExpectedText.Length > 4)
                ExpectedText = ExpectedText.Substring(0, ExpectedText.Length - 1);

            string ResultText = Result.ToString();
            if (ResultText.Length > ExpectedText.Length)
                ResultText = ResultText.Substring(0, ExpectedText.Length);

            Assert.That(ResultText == ExpectedText, $"Result={ResultText}, Expected={ExpectedText}");
        }

        [Test]
        [Category("Coverage")]
        public static void Abs0()
        {
            double d1 = -1.2547856e2;

            string Text1 = d1.ToString();

            FormattedNumber.FormattedNumber Number1 = Parser.Parse(Text1);

            //Debug.Assert(false);
            FormattedNumber.FormattedNumber Result = Number1.Abs();
            Flags Flags = Arithmetic.Flags;
            bool DivideByZero = Flags.DivideByZero;
            Flags.Clear();

            Assert.That(!DivideByZero);

            double d = Math.Abs(d1);
            string ExpectedText = d.ToString();

            if (ExpectedText.Length > 4)
                ExpectedText = ExpectedText.Substring(0, ExpectedText.Length - 1);

            string ResultText = Result.ToString();
            if (ResultText.Length > ExpectedText.Length)
                ResultText = ResultText.Substring(0, ExpectedText.Length);

            Assert.That(ResultText == ExpectedText, $"Result={ResultText}, Expected={ExpectedText}");
        }

        [Test]
        [Category("Coverage")]
        public static void Exp0()
        {
            double d1 = -1.2547856;

            string Text1 = d1.ToString();

            FormattedNumber.FormattedNumber Number1 = Parser.Parse(Text1);

            //Debug.Assert(false);
            FormattedNumber.FormattedNumber Result = Number1.Exp();
            Flags Flags = Arithmetic.Flags;
            bool DivideByZero = Flags.DivideByZero;
            Flags.Clear();

            Assert.That(!DivideByZero);

            double d = Math.Pow(Math.E, d1);
            string ExpectedText = d.ToString();

            if (ExpectedText.Length > 4)
                ExpectedText = ExpectedText.Substring(0, ExpectedText.Length - 1);

            string ResultText = Result.ToString();
            if (ResultText.Length > ExpectedText.Length)
                ResultText = ResultText.Substring(0, ExpectedText.Length);

            Assert.That(ResultText == ExpectedText, $"Result={ResultText}, Expected={ExpectedText}");
        }

        [Test]
        [Category("Coverage")]
        public static void Log0()
        {
            double d1 = 1.2547856e2;

            string Text1 = d1.ToString();

            FormattedNumber.FormattedNumber Number1 = Parser.Parse(Text1);

            //Debug.Assert(false);
            FormattedNumber.FormattedNumber Result = Number1.Log();
            Flags Flags = Arithmetic.Flags;
            bool DivideByZero = Flags.DivideByZero;
            Flags.Clear();

            Assert.That(!DivideByZero);

            double d = Math.Log(d1);
            string ExpectedText = d.ToString();

            if (ExpectedText.Length > 4)
                ExpectedText = ExpectedText.Substring(0, ExpectedText.Length - 1);

            string ResultText = Result.ToString();
            if (ResultText.Length > ExpectedText.Length)
                ResultText = ResultText.Substring(0, ExpectedText.Length);

            Assert.That(ResultText == ExpectedText, $"Result={ResultText}, Expected={ExpectedText}");
        }

        [Test]
        [Category("Coverage")]
        public static void Log10_0()
        {
            double d1 = 1.2547856e2;

            string Text1 = d1.ToString();

            FormattedNumber.FormattedNumber Number1 = Parser.Parse(Text1);

            //Debug.Assert(false);
            FormattedNumber.FormattedNumber Result = Number1.Log10();
            Flags Flags = Arithmetic.Flags;
            bool DivideByZero = Flags.DivideByZero;
            Flags.Clear();

            Assert.That(!DivideByZero);

            double d = Math.Log10(d1);
            string ExpectedText = d.ToString();

            if (ExpectedText.Length > 4)
                ExpectedText = ExpectedText.Substring(0, ExpectedText.Length - 1);

            string ResultText = Result.ToString();
            if (ResultText.Length > ExpectedText.Length)
                ResultText = ResultText.Substring(0, ExpectedText.Length);

            Assert.That(ResultText == ExpectedText, $"Result={ResultText}, Expected={ExpectedText}");
        }

        [Test]
        [Category("Coverage")]
        public static void Pow0()
        {
            double d1 = -1.2547856e2;
            double d2 = 5.478231405e-3;

            string Text1 = d1.ToString();
            string Text2 = d2.ToString();

            FormattedNumber.FormattedNumber Number1 = Parser.Parse(Text1);
            FormattedNumber.FormattedNumber Number2 = Parser.Parse(Text2);

            //Debug.Assert(false);
            FormattedNumber.FormattedNumber Result = Number1.Pow(Number2);
            Flags Flags = Arithmetic.Flags;
            bool DivideByZero = Flags.DivideByZero;
            Flags.Clear();

            Assert.That(!DivideByZero);

            double d = Math.Pow(d1, d2);
            string ExpectedText = d.ToString();

            if (ExpectedText.Length > 4)
                ExpectedText = ExpectedText.Substring(0, ExpectedText.Length - 1);

            string ResultText = Result.ToString();
            if (ResultText.Length > ExpectedText.Length)
                ResultText = ResultText.Substring(0, ExpectedText.Length);

            Assert.That(ResultText == ExpectedText, $"Result={ResultText}, Expected={ExpectedText}");
        }

        [Test]
        [Category("Coverage")]
        public static void Sqrt0()
        {
            double d1 = 1.2547856e2;

            string Text1 = d1.ToString();

            FormattedNumber.FormattedNumber Number1 = Parser.Parse(Text1);

            //Debug.Assert(false);
            FormattedNumber.FormattedNumber Result = Number1.Sqrt();
            Flags Flags = Arithmetic.Flags;
            bool DivideByZero = Flags.DivideByZero;
            Flags.Clear();

            Assert.That(!DivideByZero);

            double d = Math.Sqrt(d1);
            string ExpectedText = d.ToString();

            if (ExpectedText.Length > 4)
                ExpectedText = ExpectedText.Substring(0, ExpectedText.Length - 1);

            string ResultText = Result.ToString();
            if (ResultText.Length > ExpectedText.Length)
                ResultText = ResultText.Substring(0, ExpectedText.Length);

            Assert.That(ResultText == ExpectedText, $"Result={ResultText}, Expected={ExpectedText}");
        }
        #endregion
    }
}
