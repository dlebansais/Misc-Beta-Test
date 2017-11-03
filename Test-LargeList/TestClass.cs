using System;

namespace Test
{
    public class TestClass : IComparable
    {
        public TestClass(int IntegerValue, string StringValue)
        {
            this.IntegerValue = IntegerValue;
            this.StringValue = StringValue;
        }

        public int IntegerValue { get; private set; }
        public string StringValue { get; private set; }

        public int CompareTo(object obj)
        {
            TestClass Other = obj as TestClass;

            if (Other == null)
                return 1;

            else if (IntegerValue > Other.IntegerValue)
                return 1;

            else if (IntegerValue < Other.IntegerValue)
                return -1;

            else
                return StringValue.CompareTo(Other.StringValue);
        }
    }
}
