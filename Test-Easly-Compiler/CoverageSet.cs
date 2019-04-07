using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using BaseNode;
using BaseNodeHelper;
using EaslyCompiler;
using NUnit.Framework;
using PolySerializer;

namespace Coverage
{
    [TestFixture]
    public class CoverageSet
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

            if (File.Exists("./Easly-Compiler/bin/x64/Travis/test.easly"))
                RootPath = "./Easly-Compiler/bin/x64/Travis/";
            else if (File.Exists("./Test/test.easly"))
                RootPath = "./Test/";
            else
                RootPath = "./";

            FileNameTable = new List<string>();
            CoverageNode = null;
            AddEaslyFiles(RootPath);
        }

        static void AddEaslyFiles(string path)
        {
            foreach (string FileName in Directory.GetFiles(path, "*.easly"))
            {
                FileNameTable.Add(FileName.Replace("\\", "/"));

                if (FileName.EndsWith("coverage.easly"))
                {
                    using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                    {
                        Serializer Serializer = new Serializer();
                        INode RootNode = Serializer.Deserialize(fs) as INode;

                        CoverageNode = RootNode;
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
        static INode CoverageNode;
        static string RootPath;
        static string NL = Environment.NewLine;
        #endregion

        #region Tools
        private static string ErrorListToString(Compiler compiler)
        {
            string Result = "";

            Result += $"{compiler.ErrorList.Count} error(s).";
            foreach (Error Error in compiler.ErrorList)
                Result += $"{NL}{Error}: {Error.Message}";

            return Result;
        }
        #endregion

        #region Compilation calls
        [Test]
        [Category("Coverage")]
        public static void TestCompilationCalls()
        {
            Compiler Compiler = new Compiler();

            Assert.That(Compiler != null, "Sanity Check #0");

            string TestFileName = $"{RootPath}coverage.easly";

            Exception ex;
            string NullString = null;
            ex = Assert.Throws<ArgumentNullException>(() => Compiler.Compile(NullString));
            Assert.That(ex.Message == $"Value cannot be null.{NL}Parameter name: fileName", ex.Message);

            Compiler.Compile("notfound.easly");
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is ErrorInputFileNotFound AsInputFileNotFound && AsInputFileNotFound.Message == "File not found: 'notfound.easly'.", ErrorListToString(Compiler));

            using (FileStream fs = new FileStream(TestFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                Compiler.Compile(TestFileName);
                Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is ErrorInputFileInvalid, ErrorListToString(Compiler));
            }

            Stream NullStream = null;
            ex = Assert.Throws<ArgumentNullException>(() => Compiler.Compile(NullStream));
            Assert.That(ex.Message == $"Value cannot be null.{NL}Parameter name: stream", ex.Message);

            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 0, ErrorListToString(Compiler));

            string InvalidFile = File.Exists($"{RootPath}Test-Easly-Compiler.dll") ? $"{RootPath}Test-Easly-Compiler.dll" : $"{RootPath}Test-Easly-Compiler.csproj";
            using (FileStream fs = new FileStream(InvalidFile, FileMode.Open, FileAccess.Read))
            {
                Compiler.Compile(fs);
                Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is ErrorInputFileInvalid, ErrorListToString(Compiler));
            }

            IRoot NullRoot = null;
            ex = Assert.Throws<ArgumentNullException>(() => Compiler.Compile(NullRoot));
            Assert.That(ex.Message == $"Value cannot be null.{NL}Parameter name: root", ex.Message);

            using (FileStream fs = new FileStream(TestFileName, FileMode.Open, FileAccess.Read))
            {
                Compiler.Compile(fs);
                Assert.That(Compiler.ErrorList.Count == 0, ErrorListToString(Compiler));
            }

            IRoot ClonedRoot = NodeHelper.DeepCloneNode(CoverageNode, cloneCommentGuid: true) as IRoot;
            NodeTreeHelper.SetGuidProperty(ClonedRoot.ClassBlocks.NodeBlockList[0].NodeList[0], nameof(IClass.ClassGuid), Guid.Empty);
            Assert.That(!NodeTreeDiagnostic.IsValid(ClonedRoot, assertValid: false));

            Compiler.Compile(ClonedRoot);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is ErrorInputRootInvalid, ErrorListToString(Compiler));

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(CoverageNode as IRoot);
            Assert.That(Compiler.ErrorList.Count == 0, ErrorListToString(Compiler));

            Assert.That(Compiler.OutputRootFolder == "./");
            Assert.That(Compiler.Namespace == "Coverage");
            Assert.That(Compiler.ActivateVerification == false);
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplication()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage replication.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 0);
        }
        #endregion

        #region Replication Invalid
        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid0()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage invalid 0.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is ErrorEmptyClassPath && Compiler.ErrorList[0].Location.Node is IClass, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid1()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage invalid 1.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is ErrorWhiteSpaceNotAllowed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid2()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage invalid 2.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is ErrorEmptyString, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid3()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage invalid 3.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is ErrorIllFormedString, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid4()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage invalid 4.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is ErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid5()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage invalid 5.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is ErrorPatternAlreadyUsed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid6()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage invalid 6.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is ErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid7()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage invalid 7.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is ErrorInvalidCharacter, ErrorListToString(Compiler));
        }
        #endregion
    }
}
