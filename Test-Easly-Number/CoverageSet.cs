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
        static string SP = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        #endregion

        #region BitField
        [Test]
        [Category("Coverage")]
        public void TestBitField()
        {
            BitField IntegerField = new BitField();
            IntegerField.SetZero();
            Assert.That(IntegerField.IsZero);

            IntegerField = CreateBitFieldFromString("123456789012345678901234567890", out long BitIndex);

            Assert.That(IntegerField.Equals(IntegerField));

            BitField OtherField = new BitField();
            Assert.That(OtherField.Equals(OtherField));
            Assert.That(!IntegerField.Equals(OtherField));
            Assert.That(!OtherField.Equals(IntegerField));

            OtherField = (BitField)IntegerField.Clone();
            Assert.That(IntegerField.Equals(OtherField));
            Assert.That(!IntegerField.Equals(null));
            Assert.That(!(IntegerField == null));
            Assert.That(IntegerField != null);

            bool OldBit = OtherField.GetBit(0);
            Assert.That(OldBit == false);

            OtherField.SetBit(0, !OldBit);
            Assert.That(!IntegerField.Equals(OtherField));

            int HashCode1 = IntegerField.GetHashCode();
            int HashCode2 = OtherField.GetHashCode();
            Assert.That(HashCode1 != HashCode2);

            Assert.That(IntegerField < OtherField);
            Assert.That(OtherField > IntegerField);

            for (int i = 0; i < BitIndex; i++)
                IntegerField.DecreasePrecision();

            Assert.That(IntegerField.SignificantBits == 0);
            Assert.That(IntegerField.ShiftBits == BitIndex);
            Assert.That(IntegerField.GetBit(0) == false);
        }

        [Test]
        [Category("Coverage")]
        public void TestBitFieldComparison()
        {
            BitField IntegerField1 = CreateBitFieldFromString("123456789012345678901234567890", out long BitIndex1);
            BitField IntegerField2 = CreateBitFieldFromString("1234567890123456789012345678", out long BitIndex2);

            Assert.That(!IntegerField1.Equals(IntegerField2));
            Assert.That(IntegerField1 > IntegerField2);
            Assert.That(IntegerField2 < IntegerField1);

            BitField IntegerField3 = new BitField();
            BitField IntegerField4 = new BitField();

            Assert.That(IntegerField1 > IntegerField3);
            Assert.That(IntegerField3 < IntegerField1);
            Assert.That(IntegerField2 > IntegerField3);
            Assert.That(IntegerField3 < IntegerField2);
            Assert.That(!(IntegerField1 < IntegerField3));
            Assert.That(!(IntegerField3 > IntegerField1));
            Assert.That(!(IntegerField3 < IntegerField4));
            Assert.That(!(IntegerField3 > IntegerField4));
            Assert.That(!(IntegerField4 < IntegerField3));
            Assert.That(!(IntegerField4 > IntegerField3));

            BitField IntegerField5 = CreateBitFieldFromString("123456789012345678901234567891", out long BitIndex5);
            Assert.That(IntegerField1 < IntegerField5);
            Assert.That(IntegerField5 > IntegerField1);
            Assert.That(!(IntegerField1 > IntegerField5));
            Assert.That(!(IntegerField5 < IntegerField1));

            BitField IntegerField6 = CreateBitFieldFromString("123456789012345678901234567890", out long BitIndex6);

            Assert.That(!(IntegerField1 < IntegerField6));
            Assert.That(!(IntegerField1 > IntegerField6));
            Assert.That(!(IntegerField6 < IntegerField1));
            Assert.That(!(IntegerField6 > IntegerField1));

            BitField IntegerField7 = new BitField();
            IntegerField7.SetZero();
            BitField IntegerField8 = new BitField();
            IntegerField8.SetZero();
            Assert.That(!(IntegerField7 < IntegerField8));
            Assert.That(!(IntegerField7 > IntegerField8));
        }

        private BitField CreateBitFieldFromString(string integerString, out long bitIndex)
        {
            BitField IntegerField = new BitField();
            bitIndex = 0;

            do
            {
                integerString = NumberTextPartition.DividedByTwo(integerString, 10, IsValidDecimalDigit, ToDecimalDigit, out bool HasCarry);
                IntegerField.SetBit(bitIndex, HasCarry);
                Assert.That(IntegerField.GetBit(bitIndex) == HasCarry);
                bitIndex++;
            }
            while (integerString != "0");

            return IntegerField;
        }

        [Test]
        [Category("Coverage")]
        public void TestBitField_byte()
        {
            BitField_byte IntegerField = new BitField_byte();
            IntegerField.SetZero();
            Assert.That(IntegerField.IsZero);

            IntegerField = CreateBitField_byteFromString("123456789012345678901234567890", out long BitIndex);

            Assert.That(IntegerField.Equals(IntegerField));

            BitField_byte OtherField = new BitField_byte();
            Assert.That(OtherField.Equals(OtherField));
            Assert.That(!IntegerField.Equals(OtherField));
            Assert.That(!OtherField.Equals(IntegerField));

            OtherField = (BitField_byte)IntegerField.Clone();
            Assert.That(IntegerField.Equals(OtherField));
            Assert.That(!IntegerField.Equals(null));
            Assert.That(!(IntegerField == null));
            Assert.That(IntegerField != null);

            bool OldBit = OtherField.GetBit(0);
            Assert.That(OldBit == false);

            OtherField.SetBit(0, !OldBit);
            Assert.That(!IntegerField.Equals(OtherField));

            int HashCode1 = IntegerField.GetHashCode();
            int HashCode2 = OtherField.GetHashCode();
            Assert.That(HashCode1 != HashCode2);

            Assert.That(IntegerField < OtherField);
            Assert.That(OtherField > IntegerField);

            for (int i = 0; i < BitIndex; i++)
                IntegerField.DecreasePrecision();

            Assert.That(IntegerField.SignificantBits == 0);
            Assert.That(IntegerField.ShiftBits == BitIndex);
            Assert.That(IntegerField.GetBit(0) == false);
        }

        [Test]
        [Category("Coverage")]
        public void TestBitField_byteComparison()
        {
            BitField_byte IntegerField1 = CreateBitField_byteFromString("123456789012345678901234567890", out long BitIndex1);
            BitField_byte IntegerField2 = CreateBitField_byteFromString("1234567890123456789012345678", out long BitIndex2);

            Assert.That(!IntegerField1.Equals(IntegerField2));
            Assert.That(IntegerField1 > IntegerField2);
            Assert.That(IntegerField2 < IntegerField1);

            BitField_byte IntegerField3 = new BitField_byte();
            BitField_byte IntegerField4 = new BitField_byte();

            Assert.That(IntegerField1 > IntegerField3);
            Assert.That(IntegerField3 < IntegerField1);
            Assert.That(IntegerField2 > IntegerField3);
            Assert.That(IntegerField3 < IntegerField2);
            Assert.That(!(IntegerField1 < IntegerField3));
            Assert.That(!(IntegerField3 > IntegerField1));
            Assert.That(!(IntegerField3 < IntegerField4));
            Assert.That(!(IntegerField3 > IntegerField4));
            Assert.That(!(IntegerField4 < IntegerField3));
            Assert.That(!(IntegerField4 > IntegerField3));

            BitField_byte IntegerField5 = CreateBitField_byteFromString("123456789012345678901234567891", out long BitIndex5);
            Assert.That(IntegerField1 < IntegerField5);
            Assert.That(IntegerField5 > IntegerField1);
            Assert.That(!(IntegerField1 > IntegerField5));
            Assert.That(!(IntegerField5 < IntegerField1));

            BitField_byte IntegerField6 = CreateBitField_byteFromString("123456789012345678901234567890", out long BitIndex6);

            Assert.That(!(IntegerField1 < IntegerField6));
            Assert.That(!(IntegerField1 > IntegerField6));
            Assert.That(!(IntegerField6 < IntegerField1));
            Assert.That(!(IntegerField6 > IntegerField1));

            BitField_byte IntegerField7 = new BitField_byte();
            IntegerField7.SetZero();
            BitField_byte IntegerField8 = new BitField_byte();
            IntegerField8.SetZero();
            Assert.That(!(IntegerField7 < IntegerField8));
            Assert.That(!(IntegerField7 > IntegerField8));
        }

        private BitField_byte CreateBitField_byteFromString(string integerString, out long bitIndex)
        {
            BitField_byte IntegerField = new BitField_byte();
            bitIndex = 0;

            do
            {
                integerString = NumberTextPartition.DividedByTwo(integerString, 10, IsValidDecimalDigit, ToDecimalDigit, out bool HasCarry);
                IntegerField.SetBit(bitIndex, HasCarry);
                Assert.That(IntegerField.GetBit(bitIndex) == HasCarry);
                bitIndex++;
            }
            while (integerString != "0");

            return IntegerField;
        }

        [Test]
        [Category("Coverage")]
        public void TestBitField_uint()
        {
            BitField_uint IntegerField = new BitField_uint();
            IntegerField.SetZero();
            Assert.That(IntegerField.IsZero);

            IntegerField = CreateBitField_uintFromString("123456789012345678901234567890", out long BitIndex);

            Assert.That(IntegerField.Equals(IntegerField));

            BitField_uint OtherField = new BitField_uint();
            Assert.That(OtherField.Equals(OtherField));
            Assert.That(!IntegerField.Equals(OtherField));
            Assert.That(!OtherField.Equals(IntegerField));

            OtherField = (BitField_uint)IntegerField.Clone();
            Assert.That(IntegerField.Equals(OtherField));
            Assert.That(!IntegerField.Equals(null));
            Assert.That(!(IntegerField == null));
            Assert.That(IntegerField != null);

            bool OldBit = OtherField.GetBit(0);
            Assert.That(OldBit == false);

            OtherField.SetBit(0, !OldBit);
            Assert.That(!IntegerField.Equals(OtherField));

            int HashCode1 = IntegerField.GetHashCode();
            int HashCode2 = OtherField.GetHashCode();
            Assert.That(HashCode1 != HashCode2);

            Assert.That(IntegerField < OtherField);
            Assert.That(OtherField > IntegerField);

            for (int i = 0; i < BitIndex; i++)
                IntegerField.DecreasePrecision();

            Assert.That(IntegerField.SignificantBits == 0);
            Assert.That(IntegerField.ShiftBits == BitIndex);
            Assert.That(IntegerField.GetBit(0) == false);
        }

        [Test]
        [Category("Coverage")]
        public void TestBitField_uintComparison()
        {
            BitField_uint IntegerField1 = CreateBitField_uintFromString("123456789012345678901234567890", out long BitIndex1);
            BitField_uint IntegerField2 = CreateBitField_uintFromString("1234567890123456789012345678", out long BitIndex2);

            Assert.That(!IntegerField1.Equals(IntegerField2));
            Assert.That(IntegerField1 > IntegerField2);
            Assert.That(IntegerField2 < IntegerField1);

            BitField_uint IntegerField3 = new BitField_uint();
            BitField_uint IntegerField4 = new BitField_uint();

            Assert.That(IntegerField1 > IntegerField3);
            Assert.That(IntegerField3 < IntegerField1);
            Assert.That(IntegerField2 > IntegerField3);
            Assert.That(IntegerField3 < IntegerField2);
            Assert.That(!(IntegerField1 < IntegerField3));
            Assert.That(!(IntegerField3 > IntegerField1));
            Assert.That(!(IntegerField3 < IntegerField4));
            Assert.That(!(IntegerField3 > IntegerField4));
            Assert.That(!(IntegerField4 < IntegerField3));
            Assert.That(!(IntegerField4 > IntegerField3));

            BitField_uint IntegerField5 = CreateBitField_uintFromString("123456789012345678901234567891", out long BitIndex5);
            Assert.That(IntegerField1 < IntegerField5);
            Assert.That(IntegerField5 > IntegerField1);
            Assert.That(!(IntegerField1 > IntegerField5));
            Assert.That(!(IntegerField5 < IntegerField1));

            BitField_uint IntegerField6 = CreateBitField_uintFromString("123456789012345678901234567890", out long BitIndex6);

            Assert.That(!(IntegerField1 < IntegerField6));
            Assert.That(!(IntegerField1 > IntegerField6));
            Assert.That(!(IntegerField6 < IntegerField1));
            Assert.That(!(IntegerField6 > IntegerField1));

            BitField_uint IntegerField7 = new BitField_uint();
            IntegerField7.SetZero();
            BitField_uint IntegerField8 = new BitField_uint();
            IntegerField8.SetZero();
            Assert.That(!(IntegerField7 < IntegerField8));
            Assert.That(!(IntegerField7 > IntegerField8));
        }

        private BitField_uint CreateBitField_uintFromString(string integerString, out long bitIndex)
        {
            BitField_uint IntegerField = new BitField_uint();
            bitIndex = 0;

            do
            {
                integerString = NumberTextPartition.DividedByTwo(integerString, 10, IsValidDecimalDigit, ToDecimalDigit, out bool HasCarry);
                IntegerField.SetBit(bitIndex, HasCarry);
                Assert.That(IntegerField.GetBit(bitIndex) == HasCarry);
                bitIndex++;
            }
            while (integerString != "0");

            return IntegerField;
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

            Flags.SetDivideByZero();
            Assert.That(Flags.DivideByZero);

            Flags.SetInexact();
            Assert.That(Flags.Inexact);

            Flags.Clear();
            Assert.That(!Flags.DivideByZero);
            Assert.That(!Flags.Inexact);

            Exception ex;

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => Arithmetic.SignificandPrecision = 0);
            Assert.That(ex.Message == $"Specified argument was out of the range of valid values.{NL}Parameter name: value", ex.Message);

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => Arithmetic.ExponentPrecision = 0);
            Assert.That(ex.Message == $"Specified argument was out of the range of valid values.{NL}Parameter name: value", ex.Message);
        }
        #endregion

        #region Misc
        [Test]
        [Category("Coverage")]
        public void TestTryParse()
        {
            Assert.That(!Number.TryParse("", out Number n1));
            //Debug.Assert(false);
            Assert.That(Number.TryParse("0", out Number n2) && n2.IsZero && n2.ToString() == "0", $"Result: {n2}, expected: 0");
            Assert.That(Number.TryParse("1", out Number n3) && !n3.IsZero && n3.ToString() == "1", $"Result: {n3}, expected: 1");
        }

        [Test]
        [Category("Coverage")]
        public void TestCreate()
        {
            //Debug.Assert(false);
            Exception ex;

            Number n1;

            ex = Assert.Throws<ArgumentException>(() => n1 = new Number(""));
            Assert.That(ex.Message == $"Value does not fall within the expected range.", ex.Message);

            Number n2 = new Number("0");
            Assert.That(n2.IsZero && n2.ToString() == "0", $"Result: {n2}, expected: 0");

            Number n3 = new Number("1");
            Assert.That(!n3.IsZero && n3.ToString() == "1", $"Result: {n3}, expected: 1");

            Number n4 = new Number("NaN");
            Assert.That(n4.IsNaN && n4.ToString() == "NaN", $"Result: {n4}, expected: NaN");

            Number n5 = new Number("∞");
            Assert.That(n5.IsPositiveInfinity && n5.ToString() == "∞", $"Result: {n5}, expected: ∞");

            Number n6 = new Number("+∞");
            Assert.That(n6.IsPositiveInfinity && n6.ToString() == "∞", $"Result: {n6}, expected: ∞");

            Number n7 = new Number("-∞");
            Assert.That(n7.IsNegativeInfinity && n7.ToString() == "-∞", $"Result: {n7}, expected: -∞");

            ex = Assert.Throws<ArgumentException>(() => n4 = new Number(" NaN"));
            Assert.That(ex.Message == $"Value does not fall within the expected range.", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => n4 = new Number("NaNx"));
            Assert.That(ex.Message == $"Value does not fall within the expected range.", ex.Message);

            Number n8 = new Number("0xFF");
            Assert.That(!n8.IsZero && n8.ToString() == "255", $"Result: {n8}, expected: 255");

            ex = Assert.Throws<ArgumentException>(() => n8 = new Number(" 0xFF"));
            Assert.That(ex.Message == $"Value does not fall within the expected range.", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => n8 = new Number("0xFFx"));
            Assert.That(ex.Message == $"Value does not fall within the expected range.", ex.Message);

            Number n9 = new Number("FF:H");
            Assert.That(!n9.IsZero && n9.ToString() == "255", $"Result: {n9}, expected: 255");

            ex = Assert.Throws<ArgumentException>(() => n9 = new Number(" FF:H"));
            Assert.That(ex.Message == $"Value does not fall within the expected range.", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => n9 = new Number("FF:Hx"));
            Assert.That(ex.Message == $"Value does not fall within the expected range.", ex.Message);

            //Debug.Assert(false);
            Number n10 = new Number("1.2e3");
            Assert.That(n10.ToString() == $"1{SP}2e3", $"Result: {n10}, expected: 1{SP}2e3");

            n10 = new Number("1.2E3");
            Assert.That(n10.ToString() == $"1{SP}2e3", $"Result: {n10}, expected: 1{SP}2e3");

            ex = Assert.Throws<ArgumentException>(() => n10 = new Number(" 1.2e3"));
            Assert.That(ex.Message == $"Value does not fall within the expected range.", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => n10 = new Number("1.2e3x"));
            Assert.That(ex.Message == $"Value does not fall within the expected range.", ex.Message);
        }

        [Test]
        [Category("Coverage")]
        public void TestConversion()
        {
            //Debug.Assert(false);
            Number n1 = new Number(1.0F);
            Assert.That(n1.ToString() == "1", $"Result: {n1}, expected: 1");

            Number n2 = new Number(2.0);
            Assert.That(n2.ToString() == "2", $"Result: {n2}, expected: 2");

            Number n3 = new Number(3.0M);
            Assert.That(n3.ToString() == "3", $"Result: {n3}, expected: 3");

            Number n4 = new Number(4);
            Assert.That(n4.ToString() == "4", $"Result: {n4}, expected: 4");

            Number n5 = new Number(5U);
            Assert.That(n5.ToString() == "5", $"Result: {n5}, expected: 5");

            Number n6 = new Number(6L);
            Assert.That(n6.ToString() == "6", $"Result: {n6}, expected: 6");

            Number n7 = new Number(7UL);
            Assert.That(n7.ToString() == "7", $"Result: {n7}, expected: 7");
        }
        #endregion

        #region Comparison
        [Test]
        [Category("Coverage")]
        public void TestEqual()
        {
            CheckSameEqual(0);
            CheckSameEqual(1);
            CheckSameEqual(-1);
            //Debug.Assert(false);
            CheckSameEqual(double.NaN);
            CheckSameEqual(double.PositiveInfinity);
            CheckSameEqual(double.NegativeInfinity);

            Number n1, n2;

            n1 = new Number(double.NaN);
            n2 = new Number(double.PositiveInfinity);
            Assert.That(!n1.Equals(n2));
            Assert.That(!n2.Equals(n1));

            n1 = new Number(double.NaN);
            n2 = new Number(double.NegativeInfinity);
            Assert.That(!n1.Equals(n2));
            Assert.That(!n2.Equals(n1));

            n1 = new Number(double.PositiveInfinity);
            n2 = new Number(double.NegativeInfinity);
            Assert.That(!n1.Equals(n2));
            Assert.That(!n2.Equals(n1));

            n1 = new Number(0);
            n2 = new Number(double.NaN);
            Assert.That(!n1.Equals(n2));
            Assert.That(!n2.Equals(n1));

            n1 = new Number(0);
            n2 = new Number(double.PositiveInfinity);
            Assert.That(!n1.Equals(n2));
            Assert.That(!n2.Equals(n1));

            n1 = new Number(0);
            n2 = new Number(double.NegativeInfinity);
            Assert.That(!n1.Equals(n2));
            Assert.That(!n2.Equals(n1));

            n1 = new Number(0);
            n2 = new Number(1);
            Assert.That(!n1.Equals(n2));
            Assert.That(!n2.Equals(n1));

            n1 = new Number(1);
            n2 = new Number(1);
            Assert.That(n1.Equals(n2));
            Assert.That(n2.Equals(n1));

            n1 = new Number(1.0F);
            n2 = new Number(1.0F);
            Assert.That(n1 == n2);
            Assert.That(n1.Equals(n2));
            Assert.That(n1.GetHashCode() == n2.GetHashCode());
            Assert.That(!n1.Equals(null));
            Assert.That(n1 != Number.PositiveInfinity);

            n2 = new Number(-1.0F);
            Assert.That(n1 != n2);
        }

        private void CheckSameEqual(double d)
        {
            double d1 = d;
            double d2 = d;
            Number n1 = new Number(d1);
            Number n2 = new Number(d2);

#pragma warning disable CS1718
            bool EqualsNumber = n1.Equals(n1);
            bool EqualsDouble = d1.Equals(d1);
            bool IdenticalNumber = n1 == n1;
            bool IdenticalDouble = d1 == d1;
#pragma warning restore CS1718

            Assert.That(EqualsNumber == EqualsDouble);
            Assert.That(IdenticalNumber == IdenticalDouble);

            EqualsNumber = n1.Equals(n2);
            EqualsDouble = d1.Equals(d2);
            IdenticalNumber = n1 == n2;
            IdenticalDouble = d1 == d2;

            Assert.That(EqualsNumber == EqualsDouble);
            Assert.That(IdenticalNumber == IdenticalDouble);
        }

        [Test]
        [Category("Coverage")]
        public void TestLower()
        {
            Number n1, n2;

            n1 = new Number(double.NaN);
            n2 = new Number(double.PositiveInfinity);
            Assert.That(!(n1 < n2));
            Assert.That(!(n1 > n2));

            n1 = new Number(1.0F);
            n2 = new Number(-1.0F);
            Assert.That(n1 < Number.PositiveInfinity);
            Assert.That(n1 > Number.NegativeInfinity);
            Assert.That(!(n1 > Number.PositiveInfinity));
            Assert.That(!(n1 < Number.NegativeInfinity));
            Assert.That(Number.NegativeInfinity < n1);
            Assert.That(Number.PositiveInfinity > n1);
            Assert.That(!(Number.NegativeInfinity > n1));
            Assert.That(!(Number.PositiveInfinity < n1));

            Assert.That(n1 > Number.Zero);
            Assert.That(n2 < Number.Zero);
            Assert.That(n2 < n1);
            Assert.That(!(n2 > n1));
            Assert.That(n1 > n2);
            Assert.That(!(n1 < n2));

            n1 = new Number(2.0F);
            n2 = new Number(1.0F);
            Assert.That(n2 < n1);

            n1 = new Number(1.2F);
            n2 = new Number(1.1F);
            Assert.That(n2 < n1);

            n1 = new Number("1.0e2");
            n2 = new Number("1.0e1");
            Assert.That(n2 < n1);
            Assert.That(n1 > n2);
            Assert.That(!(n2 > n1));
            Assert.That(!(n1 < n2));

            n1 = new Number("1.0");
            n2 = new Number("1.0");
            Assert.That(!(n2 < n1));
            Assert.That(!(n2 > n1));
        }
        #endregion
    }
}
