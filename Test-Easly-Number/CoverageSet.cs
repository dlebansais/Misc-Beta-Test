namespace Coverage
{
    using EaslyNumber;
    using NUnit.Framework;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;

    [TestFixture]
    public class CoverageSet
    {
        #region Setup
        [OneTimeSetUp]
        public static void InitTestSession()
        {
            CultureInfo frFR = CultureInfo.CreateSpecificCulture("fr-FR");
            CultureInfo.DefaultThreadCurrentCulture = frFR;
            CultureInfo.DefaultThreadCurrentUICulture = frFR;
            Thread.CurrentThread.CurrentCulture = frFR;
            Thread.CurrentThread.CurrentUICulture = frFR;

            Assembly NumberAssembly;

            try
            {
                NumberAssembly = Assembly.Load("Easly-Number");
            }
            catch
            {
                NumberAssembly = null;
            }
            Assume.That(NumberAssembly != null);
        }

        static string NL = Environment.NewLine;
        #endregion

        #region BitField
        [Test]
        [Category("Coverage")]
        public void TestBitField()
        {
            long BitIndex;

            string IntegerString = "123456789012345678901234567890";
            BitField IntegerField = new BitField();
            BitIndex = 0;

            IntegerField.SetZero();
            IntegerField = new BitField();

            do
            {
                IntegerString = NumberTextPartition.DividedByTwo(IntegerString, 10, IsValidDecimalDigit, ToDecimalDigit, out bool HasCarry);
                IntegerField.SetBit(BitIndex, HasCarry);
                Assert.That(IntegerField.GetBit(BitIndex) == HasCarry);
                BitIndex++;
            }
            while (IntegerString != "0");

            for (int i = 0; i < BitIndex; i++)
                IntegerField.ShiftRight();

            Assert.That(IntegerField.SignificantBits == 0);
            Assert.That(IntegerField.ShiftBits == BitIndex);
        }

        [Test]
        [Category("Coverage")]
        public void TestBitField_byte()
        {
            long BitIndex;

            string IntegerString = "123456789012345678901234567890";
            BitField_byte IntegerField = new BitField_byte();
            BitIndex = 0;

            IntegerField.SetZero();
            IntegerField = new BitField_byte();

            do
            {
                IntegerString = NumberTextPartition.DividedByTwo(IntegerString, 10, IsValidDecimalDigit, ToDecimalDigit, out bool HasCarry);
                IntegerField.SetBit(BitIndex, HasCarry);
                Assert.That(IntegerField.GetBit(BitIndex) == HasCarry);
                BitIndex++;
            }
            while (IntegerString != "0");

            for (int i = 0; i < BitIndex; i++)
                IntegerField.ShiftRight();

            Assert.That(IntegerField.SignificantBits == 0);
            Assert.That(IntegerField.ShiftBits == BitIndex);
        }

        [Test]
        [Category("Coverage")]
        public void TestBitField_uint()
        {
            long BitIndex;

            string IntegerString = "123456789012345678901234567890";
            BitField_uint IntegerField = new BitField_uint();
            BitIndex = 0;

            IntegerField.SetZero();
            IntegerField = new BitField_uint();

            do
            {
                IntegerString = NumberTextPartition.DividedByTwo(IntegerString, 10, IsValidDecimalDigit, ToDecimalDigit, out bool HasCarry);
                IntegerField.SetBit(BitIndex, HasCarry);
                Assert.That(IntegerField.GetBit(BitIndex) == HasCarry);
                BitIndex++;
            }
            while (IntegerString != "0");

            for (int i = 0; i < BitIndex; i++)
                IntegerField.ShiftRight();

            Assert.That(IntegerField.SignificantBits == 0);
            Assert.That(IntegerField.ShiftBits == BitIndex);
        }

        public static bool IsValidDecimalDigit(char digit, out int value)
        {
            if (digit >= '0' && digit <= '9')
            {
                value = digit - '0';
                return true;
            }
            else
            {
                value = -1;
                return false;
            }
        }

        public static char ToDecimalDigit(int value)
        {
            if (value >= 0 && value < 10)
                return (char)('0' + value);
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }
        #endregion

        #region Rounding
        [Test]
        [Category("Coverage")]
        public void TestRounding()
        {
            double[] TestArray = new double[]
            {
                1.2547856e2,
                1.2547856,
                1.2547857,
                1.2547858,
                1.2547859,
                9.9999,
                1.2547850,
                1.0000001,
            };

            //Debug.Assert(false);

            for (int i = 0; i < TestArray.Length; i++)
            {
                double d = TestArray[i];
                string Text = d.ToString();

                FormattedNumber FormattedNumber = new FormattedNumber(Text);

                Number Value = FormattedNumber.Value;
                Assert.That(Value.ToString() == d.ToString(), $"Result #{i}={Value}, Expected={d}");
            }
        }

        [Test]
        [Category("Coverage")]
        public void TestRoundingText()
        {
            string[] TestArray = new string[]
            {
                "2547856",
                "254785600",
                "2547857",
                "254785700",
                "2547858",
                "254785800",
                "2547859",
                "254785900",
                "9999",
                "999900",
                "2547850",
                "254785000",
                "0000001",
                "0000001000",
            };

            //Debug.Assert(false);

            string FractionalString;
            foreach (string s in TestArray)
            {
                FractionalString = RealTextPartition.RoundedToNearest(s, 10, IsValidDecimalDigit, ToDecimalDigit, false);
                FractionalString = RealTextPartition.RoundedToNearest(s, 10, IsValidDecimalDigit, ToDecimalDigit, true);
            }
        }
        #endregion

        #region Precision
        [Test]
        [Category("Coverage")]
        public void TestPrecision()
        {
            double[] TestArray = new double[]
            {
                1.2547856e2,
                1.2547856,
                1.2547857,
                1.2547858,
                1.2547859,
                9.9999,
                1.2547850,
                1.0000001,
            };

            //Debug.Assert(false);

            Assert.That(Arithmetic.SignificandPrecision == Arithmetic.DefaultSignificandPrecision);
            Arithmetic.SignificandPrecision = Arithmetic.DefaultSignificandPrecision;

            Assert.That(Arithmetic.ExponentPrecision == Arithmetic.DefaultExponentPrecision);
            Arithmetic.ExponentPrecision = Arithmetic.DefaultExponentPrecision;

            Assert.That(Arithmetic.Rounding == Rounding.ToNearest);
            Arithmetic.Rounding = Rounding.ToNearest;

            Assert.That(!Arithmetic.EnableInfinitePrecision);
            Arithmetic.EnableInfinitePrecision = false;

            for (int i = 0; i < TestArray.Length; i++)
            {
                double d = TestArray[i];
                string Text = d.ToString();

                FormattedNumber FormattedNumber = new FormattedNumber(Text);

                Number Value = FormattedNumber.Value;
                Assert.That(Value.ToString() == d.ToString(), $"Result #{i}={Value}, Expected={d}");
            }

            Flags Flags = Arithmetic.Flags;
            Assert.That(!Flags.DivideByZero, $"Flags.DivideByZero={Flags.DivideByZero}, Expected=false");
            Assert.That(!Flags.Inexact, $"Flags.Inexact={Flags.Inexact}, Expected=false");

            Exception ex;

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => Arithmetic.SignificandPrecision = 0);
            Assert.That(ex.Message == $"Specified argument was out of the range of valid values.{NL}Parameter name: value", ex.Message);

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => Arithmetic.ExponentPrecision = 0);
            Assert.That(ex.Message == $"Specified argument was out of the range of valid values.{NL}Parameter name: value", ex.Message);
        }
        #endregion
    }
}
