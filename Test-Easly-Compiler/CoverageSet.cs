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
            foreach (IError Error in compiler.ErrorList)
                Result += $"{NL}{Error}: {Error.Message} from {Error.Location}";

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

            string TestFileName = $"{RootPath}/coverage/coverage.easly";

            Exception ex;
            string NullString = null;
            ex = Assert.Throws<ArgumentNullException>(() => Compiler.Compile(NullString));
            Assert.That(ex.Message == $"Value cannot be null.{NL}Parameter name: fileName", ex.Message);

            Compiler.Compile("notfound.easly");
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorInputFileNotFound AsInputFileNotFound && AsInputFileNotFound.Message == "File not found: 'notfound.easly'.", ErrorListToString(Compiler));

            using (FileStream fs = new FileStream(TestFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                Compiler.Compile(TestFileName);
                Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorInputFileInvalid, ErrorListToString(Compiler));
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
                Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorInputFileInvalid, ErrorListToString(Compiler));
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
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorInputRootInvalid, ErrorListToString(Compiler));

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

            string TestFileName = $"{RootPath}/coverage/coverage replication.easly";

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

            string TestFileName = $"{RootPath}coverage/coverage invalid 0.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorEmptyClassPath && Compiler.ErrorList[0].Location.Node is IClass, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid1()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 1.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorWhiteSpaceNotAllowed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid2()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 2.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorEmptyString, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid3()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 3.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorIllFormedString, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid4()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 4.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid5()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 5.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorPatternAlreadyUsed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid6()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 6.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid7()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 7.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorInvalidCharacter, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid8()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 8.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count > 0 && Compiler.ErrorList[0] is IErrorSourceRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid9()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 9.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorCyclicDependency, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid10()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 10.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid11()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 11.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid12()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 12.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid13()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 13.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid14()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 14.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid15()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 15.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid16()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 16.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorDuplicateImport, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid17()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 17.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorNameChanged, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid18()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 18.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorImportTypeConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid19()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 19.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorNameAlreadyUsed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid20()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 20.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorImportTypeConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid21()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 21.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorImportTypeConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid22()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 22.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorClassAlreadyImported, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid23()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 23.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid24()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 24.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid25()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 25.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid26()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 26.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid27()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 27.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorDuplicateImport, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid28()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 28.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid29()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 29.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid30()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 30.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid31()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 31.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid32()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 32.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid33()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 33.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid34()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 34.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorNameUnchanged, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid35()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 35.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid36()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 36.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorDoubleRename, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid37()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 37.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid38()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 38.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplicationInvalid39()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 39.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }
        #endregion
    }
}
