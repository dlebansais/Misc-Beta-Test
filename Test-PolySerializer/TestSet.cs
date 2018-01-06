using PolySerializer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.IO;
using DeepEqual.Syntax;

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
        }

        #region Basic Tests
        [Test]
        public static void TestBasic0()
        {
            Serializer s = new Serializer();

            ParentA parentA0 = new ParentA();
            parentA0.Test = "test";

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, parentA0);
            }

            ParentA parentA1;

            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                parentA1 = s.Deserialize(fs) as ParentA;
            }

            Assert.That(parentA0.IsDeepEqual(parentA1), "Basic serializing");
        }

        [Test]
        public static void TestBasic1()
        {
            Serializer s = new Serializer();

            ChildAA childAA0 = new ChildAA();
            childAA0.Test = "test";

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, childAA0);
            }

            ChildAA childAA1;

            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                childAA1 = s.Deserialize(fs) as ChildAA;
            }

            Assert.That(childAA0.IsDeepEqual(childAA1), "Basic serializing of parent");
        }

        [Test]
        public static void TestBasic2()
        {
            Serializer s = new Serializer();

            ChildAA childAA = new ChildAA();
            childAA.Test = "test";

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, childAA);
            }

            s.TypeOverrideTable = new Dictionary<Type, Type>() { { typeof(ChildAA), typeof(ChildAB) } };

            ChildAB childAB;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                childAB = s.Deserialize(fs) as ChildAB;
            }

            Assert.That(childAA.IsDeepEqual(childAB), "Basic polymorphic serializing");
        }

        [Test]
        public static void TestBasic3()
        {
            Serializer s = new Serializer();

            ChildAA childAA = new ChildAA();
            childAA.Test = "test";

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, childAA);
            }

            s.TypeOverrideTable = new Dictionary<Type, Type>() { { typeof(ChildAA), typeof(ParentA) } };

            ParentA parentA;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                parentA = s.Deserialize(fs) as ParentA;
            }

            Assert.That(childAA.IsDeepEqual(parentA), "Basic polymorphic serializing child to parent");
        }

        [Test]
        public static void TestBasic4()
        {
            Serializer s = new Serializer();

            ParentA parentA = new ParentA();
            parentA.Test = "test";

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, parentA);
            }

            s.TypeOverrideTable = new Dictionary<Type, Type>() { { typeof(ParentA), typeof(ChildAA) } };

            ChildAA childAA;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                childAA = s.Deserialize(fs) as ChildAA;
            }

            Assert.That(parentA.IsDeepEqual(childAA), "Basic polymorphic serializing parent to child");
        }

        [Test]
        public static void TestBasic5()
        {
            Serializer s = new Serializer();

            GrandChildAA grandChildAA0 = new GrandChildAA();
            grandChildAA0.Test = "test";

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, grandChildAA0);
            }

            GrandChildAA grandChildAA1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                grandChildAA1 = s.Deserialize(fs) as GrandChildAA;
            }

            Assert.That(grandChildAA0.IsDeepEqual(grandChildAA1), "Basic deep serializing of parent");
        }

        [Test]
        public static void TestBasic6()
        {
            Serializer s = new Serializer();

            GrandChildAA grandChildAA = new GrandChildAA();
            grandChildAA.Test = "test";

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, grandChildAA);
            }

            s.TypeOverrideTable = new Dictionary<Type, Type>() { { typeof(GrandChildAA), typeof(GrandChildAB) } };

            GrandChildAB grandChildAB;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                grandChildAB = s.Deserialize(fs) as GrandChildAB;
            }

            Assert.That(grandChildAA.IsDeepEqual(grandChildAB), "Basic deep polymorphic serializing");
        }

        [Test]
        public static void TestBasic7()
        {
            Serializer s = new Serializer();

            GrandChildAA grandChildAA = new GrandChildAA();
            grandChildAA.Test = "test";

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, grandChildAA);
            }

            s.TypeOverrideTable = new Dictionary<Type, Type>() { { typeof(GrandChildAA), typeof(ParentA) } };

            ParentA parentA;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                parentA = s.Deserialize(fs) as ParentA;
            }

            Assert.That(grandChildAA.IsDeepEqual(parentA), "Basic deep polymorphic serializing child to parent");
        }

        [Test]
        public static void TestBasic8()
        {
            Serializer s = new Serializer();

            ParentA parentA = new ParentA();
            parentA.Test = "test";

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, parentA);
            }

            s.TypeOverrideTable = new Dictionary<Type, Type>() { { typeof(ParentA), typeof(GrandChildAA) } };

            GrandChildAA grandChildAA;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                grandChildAA = s.Deserialize(fs) as GrandChildAA;
            }

            Assert.That(parentA.IsDeepEqual(grandChildAA), "Basic deep polymorphic serializing parent to child");
        }

        [Test]
        public static void TestBasic9()
        {
            Serializer s = new Serializer();

            ParentB parentB0 = new ParentB();
            parentB0.Init();

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, parentB0);
            }

            ParentB parentB1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                parentB1 = s.Deserialize(fs) as ParentB;
            }

            parentB0.ShouldDeepEqual(parentB1);
            Assert.That(parentB0.IsDeepEqual(parentB1), "Basic serializing of built-in types");
        }

        [Test]
        public static void TestBasic10()
        {
            Serializer s = new Serializer();

            ParentC parentC0 = new ParentC();
            parentC0.InitInt(50);
            parentC0.InitString("60");
            parentC0.InitObject(new ParentA());

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, parentC0);
            }

            ParentC parentC1 = new ParentC();
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                parentC1 = s.Deserialize(fs) as ParentC;
            }

            Assert.That(!(parentC0.IsDeepEqual(parentC1)), "Basic serializing of readonly properties (should fail)");
        }
        #endregion

        #region Enum Tests
        public enum Enum0
        {
            test0,
            test1,
            test2,
        }

        [Test]
        public static void TestEnum0()
        {
            Serializer s = new Serializer();

            Enum0 test0 = Enum0.test1;

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, test0);
            }

            Enum0 test1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                test1 = (Enum0)s.Deserialize(fs);
            }

            Assert.That(test0.IsDeepEqual(test1), "Basic serializing of enum type");
        }

        public enum Enum1
        {
            test0 = 3,
            test1 = 4,
            test2 = 5,
        }

        [Test]
        public static void TestEnum1()
        {
            Serializer s = new Serializer();

            Enum1 test0 = Enum1.test1;

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, test0);
            }

            Enum1 test1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                test1 = (Enum1)s.Deserialize(fs);
            }

            Assert.That(test0.IsDeepEqual(test1), "Basic serializing of enum type with value");
        }

        [Flags]
        public enum Enum2
        {
            test0 = 0x01,
            test1 = 0x02,
            test2 = 0x04,
        }

        [Test]
        public static void TestEnum2()
        {
            Serializer s = new Serializer();

            Enum2 test0 = Enum2.test1;

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, test0);
            }

            Enum2 test1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                test1 = (Enum2)s.Deserialize(fs);
            }

            Assert.That(test0.IsDeepEqual(test1), "Basic serializing of enum type with flag value");
        }

        public enum Enum3 : byte
        {
            test0,
            test1,
            test2,
        }

        [Test]
        public static void TestEnum3()
        {
            Serializer s = new Serializer();

            Enum3 test0 = Enum3.test1;

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, test0);
            }

            Enum3 test1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                test1 = (Enum3)s.Deserialize(fs);
            }

            Assert.That(test0.IsDeepEqual(test1), "Basic serializing of enum type (byte)");
        }

        public enum Enum4 : sbyte
        {
            test0,
            test1,
            test2,
        }

        [Test]
        public static void TestEnum4()
        {
            Serializer s = new Serializer();

            Enum4 test0 = Enum4.test1;

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, test0);
            }

            Enum4 test1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                test1 = (Enum4)s.Deserialize(fs);
            }

            Assert.That(test0.IsDeepEqual(test1), "Basic serializing of enum type (sbyte)");
        }

        public enum Enum5 : short
        {
            test0,
            test1,
            test2,
        }

        [Test]
        public static void TestEnum5()
        {
            Serializer s = new Serializer();

            Enum5 test0 = Enum5.test1;

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, test0);
            }

            Enum5 test1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                test1 = (Enum5)s.Deserialize(fs);
            }

            Assert.That(test0.IsDeepEqual(test1), "Basic serializing of enum type (short)");
        }

        public enum Enum6 : ushort
        {
            test0,
            test1,
            test2,
        }

        [Test]
        public static void TestEnum6()
        {
            Serializer s = new Serializer();

            Enum6 test0 = Enum6.test1;

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, test0);
            }

            Enum6 test1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                test1 = (Enum6)s.Deserialize(fs);
            }

            Assert.That(test0.IsDeepEqual(test1), "Basic serializing of enum type (ushort)");
        }

        public enum Enum7 : int
        {
            test0,
            test1,
            test2,
        }

        [Test]
        public static void TestEnum7()
        {
            Serializer s = new Serializer();

            Enum7 test0 = Enum7.test1;

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, test0);
            }

            Enum7 test1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                test1 = (Enum7)s.Deserialize(fs);
            }

            Assert.That(test0.IsDeepEqual(test1), "Basic serializing of enum type (int)");
        }

        public enum Enum8 : uint
        {
            test0,
            test1,
            test2,
        }

        [Test]
        public static void TestEnum8()
        {
            Serializer s = new Serializer();

            Enum8 test0 = Enum8.test1;

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, test0);
            }

            Enum8 test1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                test1 = (Enum8)s.Deserialize(fs);
            }

            Assert.That(test0.IsDeepEqual(test1), "Basic serializing of enum type (uint)");
        }

        public enum Enum10 : long
        {
            test0,
            test1,
            test2,
        }

        [Test]
        public static void TestEnum10()
        {
            Serializer s = new Serializer();

            Enum10 test0 = Enum10.test1;

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, test0);
            }

            Enum10 test1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                test1 = (Enum10)s.Deserialize(fs);
            }

            Assert.That(test0.IsDeepEqual(test1), "Basic serializing of enum type (long)");
        }

        public enum Enum11 : ulong
        {
            test0,
            test1,
            test2,
        }

        [Test]
        public static void TestEnum11()
        {
            Serializer s = new Serializer();

            Enum11 test0 = Enum11.test1;

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, test0);
            }

            Enum11 test1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                test1 = (Enum11)s.Deserialize(fs);
            }

            Assert.That(test0.IsDeepEqual(test1), "Basic serializing of enum type (ulong)");
        }
        #endregion

        #region Test Struct
        [System.Serializable]
        public struct Struct0
        {
        }

        [Test]
        public static void TestStruct0()
        {
            Serializer s = new Serializer();

            Struct0 test0 = new Struct0();

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, test0);
            }

            Struct0 test1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                test1 = (Struct0)s.Deserialize(fs);
            }

            Assert.That(test0.IsDeepEqual(test1), "Basic serializing of empty struct");
        }

        [System.Serializable]
        public struct Struct1
        {
            public bool field0;
            public byte field1;
            public sbyte field2;
            public char field3;
            public decimal field4;
            public double field5;
            public float field6;
            public int field7;
            public uint field8;
            public long field9;
            public ulong field10;
            public object field11;
            public short field12;
            public ushort field13;
            public string field14;
        }

        [Test]
        public static void TestStruct1()
        {
            Serializer s = new Serializer();

            Struct1 test0 = new Struct1();

            using (FileStream fs = new FileStream("test.log", FileMode.Create, FileAccess.Write))
            {
                s.Serialize(fs, test0);
            }

            Struct1 test1;
            using (FileStream fs = new FileStream("test.log", FileMode.Open, FileAccess.Read))
            {
                test1 = (Struct1)s.Deserialize(fs);
            }

            Assert.That(test0.IsDeepEqual(test1), "Basic serializing of empty struct");
        }
        #endregion
    }
}
