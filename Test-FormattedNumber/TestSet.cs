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
        public static void TestDigits()
        {
            int Value;
            Assert.That(IntegerBase.Binary.IsValidDigit('0', out Value) && Value == 0);
            Assert.That(IntegerBase.Binary.IsValidDigit('1', out Value) && Value == 1);
            Assert.That(!IntegerBase.Binary.IsValidDigit('2', out Value));
            Assert.That(IntegerBase.Octal.IsValidDigit('0', out Value) && Value == 0);
            Assert.That(IntegerBase.Octal.IsValidDigit('1', out Value) && Value == 1);
            Assert.That(IntegerBase.Octal.IsValidDigit('2', out Value) && Value == 2);
            Assert.That(IntegerBase.Octal.IsValidDigit('3', out Value) && Value == 3);
            Assert.That(IntegerBase.Octal.IsValidDigit('4', out Value) && Value == 4);
            Assert.That(IntegerBase.Octal.IsValidDigit('5', out Value) && Value == 5);
            Assert.That(IntegerBase.Octal.IsValidDigit('6', out Value) && Value == 6);
            Assert.That(IntegerBase.Octal.IsValidDigit('7', out Value) && Value == 7);
            Assert.That(!IntegerBase.Octal.IsValidDigit('8', out Value));
            Assert.That(IntegerBase.Decimal.IsValidDigit('0', out Value) && Value == 0);
            Assert.That(IntegerBase.Decimal.IsValidDigit('1', out Value) && Value == 1);
            Assert.That(IntegerBase.Decimal.IsValidDigit('2', out Value) && Value == 2);
            Assert.That(IntegerBase.Decimal.IsValidDigit('3', out Value) && Value == 3);
            Assert.That(IntegerBase.Decimal.IsValidDigit('4', out Value) && Value == 4);
            Assert.That(IntegerBase.Decimal.IsValidDigit('5', out Value) && Value == 5);
            Assert.That(IntegerBase.Decimal.IsValidDigit('6', out Value) && Value == 6);
            Assert.That(IntegerBase.Decimal.IsValidDigit('7', out Value) && Value == 7);
            Assert.That(IntegerBase.Decimal.IsValidDigit('8', out Value) && Value == 8);
            Assert.That(IntegerBase.Decimal.IsValidDigit('9', out Value) && Value == 9);
            Assert.That(!IntegerBase.Decimal.IsValidDigit('a', out Value));
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('0', out Value) && Value == 0);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('1', out Value) && Value == 1);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('2', out Value) && Value == 2);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('3', out Value) && Value == 3);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('4', out Value) && Value == 4);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('5', out Value) && Value == 5);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('6', out Value) && Value == 6);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('7', out Value) && Value == 7);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('8', out Value) && Value == 8);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('9', out Value) && Value == 9);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('a', out Value) && Value == 10);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('b', out Value) && Value == 11);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('c', out Value) && Value == 12);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('d', out Value) && Value == 13);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('e', out Value) && Value == 14);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('f', out Value) && Value == 15);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('A', out Value) && Value == 10);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('B', out Value) && Value == 11);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('C', out Value) && Value == 12);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('D', out Value) && Value == 13);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('E', out Value) && Value == 14);
            Assert.That(IntegerBase.Hexadecimal.IsValidDigit('F', out Value) && Value == 15);
            Assert.That(!IntegerBase.Hexadecimal.IsValidDigit('g', out Value));
            Assert.That(!IntegerBase.Hexadecimal.IsValidDigit('G', out Value));

            Assert.That(IntegerBase.Binary.ToDigit(0) == '0');
            Assert.That(IntegerBase.Binary.ToDigit(1) == '1');

            Assert.That(IntegerBase.Octal.ToDigit(0) == '0');
            Assert.That(IntegerBase.Octal.ToDigit(1) == '1');
            Assert.That(IntegerBase.Octal.ToDigit(2) == '2');
            Assert.That(IntegerBase.Octal.ToDigit(3) == '3');
            Assert.That(IntegerBase.Octal.ToDigit(4) == '4');
            Assert.That(IntegerBase.Octal.ToDigit(5) == '5');
            Assert.That(IntegerBase.Octal.ToDigit(6) == '6');
            Assert.That(IntegerBase.Octal.ToDigit(7) == '7');

            Assert.That(IntegerBase.Decimal.ToDigit(0) == '0');
            Assert.That(IntegerBase.Decimal.ToDigit(1) == '1');
            Assert.That(IntegerBase.Decimal.ToDigit(2) == '2');
            Assert.That(IntegerBase.Decimal.ToDigit(3) == '3');
            Assert.That(IntegerBase.Decimal.ToDigit(4) == '4');
            Assert.That(IntegerBase.Decimal.ToDigit(5) == '5');
            Assert.That(IntegerBase.Decimal.ToDigit(6) == '6');
            Assert.That(IntegerBase.Decimal.ToDigit(7) == '7');
            Assert.That(IntegerBase.Decimal.ToDigit(8) == '8');
            Assert.That(IntegerBase.Decimal.ToDigit(9) == '9');

            Assert.That(IntegerBase.Hexadecimal.ToDigit(0) == '0');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(1) == '1');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(2) == '2');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(3) == '3');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(4) == '4');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(5) == '5');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(6) == '6');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(7) == '7');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(8) == '8');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(9) == '9');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(10) == 'A');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(11) == 'B');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(12) == 'C');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(13) == 'D');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(14) == 'E');
            Assert.That(IntegerBase.Hexadecimal.ToDigit(15) == 'F');
        }

        [Test]
        public static void TestBinary()
        {
            Assert.That(!IntegerBase.Binary.IsValidNumber("", false));
            Assert.That(IntegerBase.Binary.IsValidNumber("0", false));
            Assert.That(IntegerBase.Binary.IsValidNumber("1", false));
            Assert.That(!IntegerBase.Binary.IsValidNumber("2", false));
            Assert.That(!IntegerBase.Binary.IsValidNumber("0120", false));
            Assert.That(!IntegerBase.Binary.IsValidNumber("010", false));
            Assert.That(!IntegerBase.Binary.IsValidNumber("011", false));
            Assert.That(IntegerBase.Binary.IsValidNumber("111", false));
            Assert.That(IntegerBase.Binary.IsValidNumber("110", false));
            Assert.That(IntegerBase.Binary.IsValidSignificand("111"));
            Assert.That(!IntegerBase.Binary.IsValidSignificand("110"));
        }

        [Test]
        public static void TestOctal()
        {
            Assert.That(!IntegerBase.Octal.IsValidNumber("", false));
            Assert.That(IntegerBase.Octal.IsValidNumber("0", false));
            Assert.That(IntegerBase.Octal.IsValidNumber("1", false));
            Assert.That(!IntegerBase.Octal.IsValidNumber("8", false));
            Assert.That(!IntegerBase.Octal.IsValidNumber("0180", false));
            Assert.That(!IntegerBase.Octal.IsValidNumber("010", false));
            Assert.That(!IntegerBase.Octal.IsValidNumber("011", false));
            Assert.That(IntegerBase.Octal.IsValidNumber("111", false));
            Assert.That(IntegerBase.Octal.IsValidNumber("110", false));
            Assert.That(IntegerBase.Octal.IsValidSignificand("111"));
            Assert.That(!IntegerBase.Octal.IsValidSignificand("110"));
        }

        [Test]
        public static void TestDecimal()
        {
            Assert.That(!IntegerBase.Decimal.IsValidNumber("", false));
            Assert.That(IntegerBase.Decimal.IsValidNumber("0", false));
            Assert.That(IntegerBase.Decimal.IsValidNumber("1", false));
            Assert.That(!IntegerBase.Decimal.IsValidNumber("a", false));
            Assert.That(!IntegerBase.Decimal.IsValidNumber("01a0", false));
            Assert.That(!IntegerBase.Decimal.IsValidNumber("010", false));
            Assert.That(!IntegerBase.Decimal.IsValidNumber("011", false));
            Assert.That(IntegerBase.Decimal.IsValidNumber("111", false));
            Assert.That(IntegerBase.Decimal.IsValidNumber("110", false));
            Assert.That(IntegerBase.Decimal.IsValidSignificand("111"));
            Assert.That(!IntegerBase.Decimal.IsValidSignificand("110"));
        }

        [Test]
        public static void TestHexadecimal()
        {
            Assert.That(!IntegerBase.Hexadecimal.IsValidNumber("", false));
            Assert.That(IntegerBase.Hexadecimal.IsValidNumber("0", false));
            Assert.That(IntegerBase.Hexadecimal.IsValidNumber("1", false));
            Assert.That(!IntegerBase.Hexadecimal.IsValidNumber("g", false));
            Assert.That(!IntegerBase.Hexadecimal.IsValidNumber("01g0", false));
            Assert.That(!IntegerBase.Hexadecimal.IsValidNumber("010", false));
            Assert.That(!IntegerBase.Hexadecimal.IsValidNumber("011", false));
            Assert.That(IntegerBase.Hexadecimal.IsValidNumber("111", false));
            Assert.That(IntegerBase.Hexadecimal.IsValidNumber("110", false));
            Assert.That(IntegerBase.Hexadecimal.IsValidSignificand("111"));
            Assert.That(!IntegerBase.Hexadecimal.IsValidSignificand("110"));
        }

        [Test]
        public static void TestDivide()
        {
            bool HasCarry;

            Assert.That(IntegerBase.Binary.DividedByTwo("0", out HasCarry) == "0" && !HasCarry);
            Assert.That(IntegerBase.Binary.DividedByTwo("1", out HasCarry) == "0" && HasCarry);
            Assert.That(IntegerBase.Binary.DividedByTwo("10", out HasCarry) == "1" && !HasCarry);
            Assert.That(IntegerBase.Binary.DividedByTwo("11", out HasCarry) == "1" && HasCarry);
            Assert.That(IntegerBase.Binary.DividedByTwo("100", out HasCarry) == "10" && !HasCarry);
            Assert.That(IntegerBase.Binary.DividedByTwo("101", out HasCarry) == "10" && HasCarry);
            Assert.That(IntegerBase.Binary.DividedByTwo("110", out HasCarry) == "11" && !HasCarry);
            Assert.That(IntegerBase.Binary.DividedByTwo("111", out HasCarry) == "11" && HasCarry);

            Assert.That(IntegerBase.Octal.DividedByTwo("0", out HasCarry) == "0" && !HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("1", out HasCarry) == "0" && HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("2", out HasCarry) == "1" && !HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("3", out HasCarry) == "1" && HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("4", out HasCarry) == "2" && !HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("5", out HasCarry) == "2" && HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("6", out HasCarry) == "3" && !HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("7", out HasCarry) == "3" && HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("10", out HasCarry) == "4" && !HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("11", out HasCarry) == "4" && HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("12", out HasCarry) == "5" && !HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("13", out HasCarry) == "5" && HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("14", out HasCarry) == "6" && !HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("15", out HasCarry) == "6" && HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("16", out HasCarry) == "7" && !HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("17", out HasCarry) == "7" && HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("20", out HasCarry) == "10" && !HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("21", out HasCarry) == "10" && HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("22", out HasCarry) == "11" && !HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("23", out HasCarry) == "11" && HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("24", out HasCarry) == "12" && !HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("25", out HasCarry) == "12" && HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("26", out HasCarry) == "13" && !HasCarry);
            Assert.That(IntegerBase.Octal.DividedByTwo("27", out HasCarry) == "13" && HasCarry);
        }

        [Test]
        public static void TestMultiply()
        {
            Assert.That(IntegerBase.Binary.MultipliedByTwo("0", false) == "0");
            Assert.That(IntegerBase.Binary.MultipliedByTwo("0", true) == "1");
            Assert.That(IntegerBase.Binary.MultipliedByTwo("1", false) == "10");
            Assert.That(IntegerBase.Binary.MultipliedByTwo("1", true) == "11");
            Assert.That(IntegerBase.Binary.MultipliedByTwo("10", false) == "100");
            Assert.That(IntegerBase.Binary.MultipliedByTwo("10", true) == "101");
            Assert.That(IntegerBase.Binary.MultipliedByTwo("11", false) == "110");
            Assert.That(IntegerBase.Binary.MultipliedByTwo("11", true) == "111");
        }

        [Test]
        public static void TestRandom()
        {
            Random Rand = new Random(0);
            int MaxDigits = 30;

            bool HasCarry;

            string BinaryNumber = "";
            string OctalNumber = "";
            string DecimalNumber = "";
            string HexadecimalNumber = "";

            for (int i = 0; i < MaxDigits; i++)
            {
                int BinaryValue = Rand.Next(IntegerBase.Binary.Radix);
                do BinaryValue = Rand.Next(IntegerBase.Binary.Radix); while (i + 1 == MaxDigits && BinaryValue == 0);
                char BinaryDigit = IntegerBase.Binary.ToDigit(BinaryValue);
                BinaryNumber += BinaryDigit;

                int OctalValue = Rand.Next(IntegerBase.Octal.Radix);
                do OctalValue = Rand.Next(IntegerBase.Octal.Radix); while (i + 1 == MaxDigits && OctalValue == 0);
                char OctalDigit = IntegerBase.Octal.ToDigit(OctalValue);
                OctalNumber += OctalDigit;

                int DecimalValue = Rand.Next(IntegerBase.Decimal.Radix);
                do DecimalValue = Rand.Next(IntegerBase.Decimal.Radix); while (i + 1 == MaxDigits && DecimalValue == 0);
                char DecimalDigit = IntegerBase.Decimal.ToDigit(DecimalValue);
                DecimalNumber += DecimalDigit;

                int HexadecimalValue = Rand.Next(IntegerBase.Hexadecimal.Radix);
                do HexadecimalValue = Rand.Next(IntegerBase.Hexadecimal.Radix); while (i + 1 == MaxDigits && HexadecimalValue == 0);
                char HexadecimalDigit = IntegerBase.Hexadecimal.ToDigit(HexadecimalValue);
                HexadecimalNumber += HexadecimalDigit;
            }

            string BinaryNumberDivided = IntegerBase.Binary.DividedByTwo(BinaryNumber, out HasCarry);
            string BinaryNumberRemultiplied = IntegerBase.Binary.MultipliedByTwo(BinaryNumberDivided, HasCarry);
            Assert.That(BinaryNumberRemultiplied == BinaryNumber);

            string OctalNumberDivided = IntegerBase.Octal.DividedByTwo(OctalNumber, out HasCarry);
            string OctalNumberRemultiplied = IntegerBase.Octal.MultipliedByTwo(OctalNumberDivided, HasCarry);
            Assert.That(OctalNumberRemultiplied == OctalNumber);

            string DecimalNumberDivided = IntegerBase.Decimal.DividedByTwo(DecimalNumber, out HasCarry);
            string DecimalNumberRemultiplied = IntegerBase.Decimal.MultipliedByTwo(DecimalNumberDivided, HasCarry);
            Assert.That(DecimalNumberRemultiplied == DecimalNumber);

            string HexadecimalNumberDivided = IntegerBase.Hexadecimal.DividedByTwo(HexadecimalNumber, out HasCarry);
            string HexadecimalNumberRemultiplied = IntegerBase.Hexadecimal.MultipliedByTwo(HexadecimalNumberDivided, HasCarry);
            Assert.That(HexadecimalNumberRemultiplied == HexadecimalNumber);

            Assert.That(IntegerBase.Convert(IntegerBase.Convert(BinaryNumber, IntegerBase.Binary, IntegerBase.Binary), IntegerBase.Binary, IntegerBase.Binary) == BinaryNumber);
            Assert.That(IntegerBase.Convert(IntegerBase.Convert(BinaryNumber, IntegerBase.Binary, IntegerBase.Octal), IntegerBase.Octal, IntegerBase.Binary) == BinaryNumber);
            Assert.That(IntegerBase.Convert(IntegerBase.Convert(BinaryNumber, IntegerBase.Binary, IntegerBase.Decimal), IntegerBase.Decimal, IntegerBase.Binary) == BinaryNumber);
            Assert.That(IntegerBase.Convert(IntegerBase.Convert(BinaryNumber, IntegerBase.Binary, IntegerBase.Hexadecimal), IntegerBase.Hexadecimal, IntegerBase.Binary) == BinaryNumber);

            Assert.That(IntegerBase.Convert(IntegerBase.Convert(OctalNumber, IntegerBase.Octal, IntegerBase.Binary), IntegerBase.Binary, IntegerBase.Octal) == OctalNumber);
            Assert.That(IntegerBase.Convert(IntegerBase.Convert(OctalNumber, IntegerBase.Octal, IntegerBase.Octal), IntegerBase.Octal, IntegerBase.Octal) == OctalNumber);
            Assert.That(IntegerBase.Convert(IntegerBase.Convert(OctalNumber, IntegerBase.Octal, IntegerBase.Decimal), IntegerBase.Decimal, IntegerBase.Octal) == OctalNumber);
            Assert.That(IntegerBase.Convert(IntegerBase.Convert(OctalNumber, IntegerBase.Octal, IntegerBase.Hexadecimal), IntegerBase.Hexadecimal, IntegerBase.Octal) == OctalNumber);

            Assert.That(IntegerBase.Convert(IntegerBase.Convert(DecimalNumber, IntegerBase.Decimal, IntegerBase.Binary), IntegerBase.Binary, IntegerBase.Decimal) == DecimalNumber);
            Assert.That(IntegerBase.Convert(IntegerBase.Convert(DecimalNumber, IntegerBase.Decimal, IntegerBase.Octal), IntegerBase.Octal, IntegerBase.Decimal) == DecimalNumber);
            Assert.That(IntegerBase.Convert(IntegerBase.Convert(DecimalNumber, IntegerBase.Decimal, IntegerBase.Decimal), IntegerBase.Decimal, IntegerBase.Decimal) == DecimalNumber);
            Assert.That(IntegerBase.Convert(IntegerBase.Convert(DecimalNumber, IntegerBase.Decimal, IntegerBase.Hexadecimal), IntegerBase.Hexadecimal, IntegerBase.Decimal) == DecimalNumber);

            Assert.That(IntegerBase.Convert(IntegerBase.Convert(HexadecimalNumber, IntegerBase.Hexadecimal, IntegerBase.Binary), IntegerBase.Binary, IntegerBase.Hexadecimal) == HexadecimalNumber);
            Assert.That(IntegerBase.Convert(IntegerBase.Convert(HexadecimalNumber, IntegerBase.Hexadecimal, IntegerBase.Octal), IntegerBase.Octal, IntegerBase.Hexadecimal) == HexadecimalNumber);
            Assert.That(IntegerBase.Convert(IntegerBase.Convert(HexadecimalNumber, IntegerBase.Hexadecimal, IntegerBase.Decimal), IntegerBase.Decimal, IntegerBase.Hexadecimal) == HexadecimalNumber);
            Assert.That(IntegerBase.Convert(IntegerBase.Convert(HexadecimalNumber, IntegerBase.Hexadecimal, IntegerBase.Hexadecimal), IntegerBase.Hexadecimal, IntegerBase.Hexadecimal) == HexadecimalNumber);
        }

        [Test]
        public static void SimpleParse()
        {
            IFormattedNumber Number;

            Number = FormattedNumber.FormattedNumber.Parse("", false);
            Number = FormattedNumber.FormattedNumber.Parse("0", false);
            Number = FormattedNumber.FormattedNumber.Parse("0:B", false);
            Number = FormattedNumber.FormattedNumber.Parse("0:O", false);
            Number = FormattedNumber.FormattedNumber.Parse("0:H", false);
            Number = FormattedNumber.FormattedNumber.Parse("5", false);
            Number = FormattedNumber.FormattedNumber.Parse("1:B", false);
            Number = FormattedNumber.FormattedNumber.Parse("5:O", false);
            Number = FormattedNumber.FormattedNumber.Parse("F:H", false);
            Number = FormattedNumber.FormattedNumber.Parse("468F3ECF:H", false);
            Number = FormattedNumber.FormattedNumber.Parse("468F3xECF:H", false);
        }

        [Test]
        public static void FullParse()
        {
            IFormattedNumber Number;

            string Charset = "01.e-+";
            long N = Charset.Length;
            long T = N * N * N * N * N * N * N * N * N * N;
            //Debug.WriteLine($"T = {T}");
            double Percent = 1.0;
            for (long n = 0; n < T; n++)
            {
                string s = GenerateNumber(Charset, n);
                Number = FormattedNumber.FormattedNumber.Parse(s, false);

                double d = (100.0 * ((double)n)) / ((double)T);
                if (d >= Percent)
                {
                    //Debug.WriteLine(((int)Percent).ToString() + "%");
                    Percent += 1.0;
                }
            }
        }

        private static string GenerateNumber(string charset, long pattern)
        {
            if (pattern == 0)
                return "";

            string s = charset.Substring((int)(pattern % charset.Length), 1);
            s += GenerateNumber(charset, pattern / charset.Length);

            return s;
        }
        #endregion
    }
}
