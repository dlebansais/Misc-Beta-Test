using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using BaseNode;
using EaslyCompiler;
using NUnit.Framework;
using PolySerializer;

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

            Assembly EaslyCompilerAssembly;

            try
            {
                EaslyCompilerAssembly = Assembly.Load("Easly-Compiler");
            }
            catch
            {
                EaslyCompilerAssembly = null;
            }
            Assume.That(EaslyCompilerAssembly != null);

            string RootPath;
            if (File.Exists("./Easly-Compiler/bin/x64/Travis/test.easly"))
                RootPath = "./Easly-Compiler/bin/x64/Travis/";
            else
                RootPath = "./";

            FileNameTable = new List<string>();
            FirstRootNode = null;
            AddEaslyFiles(RootPath);
        }

        static void AddEaslyFiles(string path)
        {
            foreach (string FileName in Directory.GetFiles(path, "*.easly"))
            {
                FileNameTable.Add(FileName.Replace("\\", "/"));

                if (FirstRootNode == null)
                {
                    using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                    {
                        Serializer Serializer = new Serializer();
                        INode RootNode = Serializer.Deserialize(fs) as INode;

                        FirstRootNode = RootNode;
                    }
                }
            }

            foreach (string Folder in Directory.GetDirectories(path))
                AddEaslyFiles(Folder);
        }

        static IEnumerable<int> FileIndexRange()
        {
            for (int i = 0; i < 1; i++)
                yield return i;
        }

        static int RandValue = 0;

        static void SeedRand(int seed)
        {
            RandValue = seed;
        }

        static int RandNext(int maxValue)
        {
            RandValue = (int)(5478541UL + (ulong)RandValue * 872143693217UL);
            if (RandValue < 0)
                RandValue = -RandValue;

            return RandValue % maxValue;
        }

        static List<string> FileNameTable;
        static INode FirstRootNode;
        #endregion

        static bool TestOff = false;
        const int TestRepeatCount = 5;

        #region Sanity Check
        [Test]
        public static void TestInit()
        {
            if (TestOff)
                return;

            Compiler Compiler = new Compiler();

            Assert.That(Compiler != null, "Sanity Check #0");
        }
        #endregion
    }
}
