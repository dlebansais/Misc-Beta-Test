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

            Compiler.InferenceRetries = -1;
            Compiler.Compile(CoverageNode as IRoot);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorInternal, ErrorListToString(Compiler));

            Compiler.InferenceRetries = 10;

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
        public static void TestInvalid0000_Replication()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 00-00.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorEmptyClassPath && Compiler.ErrorList[0].Location.Node is IClass, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0001_Replication()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 00-01.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorWhiteSpaceNotAllowed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0002_Replication()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 00-02.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorEmptyString, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0003_Replication()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 00-03.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorIllFormedString, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0004_Replication()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 00-04.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0005_Replication()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 00-05.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorPatternAlreadyUsed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0006_Replication()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 00-06.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0007_Replication()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 00-07.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorInvalidCharacter, ErrorListToString(Compiler));
        }
        #endregion

        #region Class and library
        [Test]
        [Category("Coverage")]
        public static void TestInvalid0100_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-00.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count > 0 && Compiler.ErrorList[0] is IErrorSourceRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0101_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-01.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorCyclicDependency, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0102_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-02.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0103_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-03.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0104_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-04.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0105_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-05.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0106_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-06.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0107_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-07.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0108_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-08.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorDuplicateImport, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0109_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-09.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorNameChanged, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0110_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-10.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorImportTypeConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0111_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-11.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorNameAlreadyUsed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0112_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-12.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorImportTypeConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0113_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-13.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorImportTypeConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0114_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-14.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorClassAlreadyImported, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0115_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-15.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0116_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-16.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0117_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-17.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0118_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-18.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0119_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-19.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorDuplicateImport, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0120_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-20.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0121_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-21.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0122_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-22.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0123_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-23.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0124_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-24.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0125_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-25.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0126_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-26.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorNameUnchanged, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0127_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-27.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0128_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-28.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorDoubleRename, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0129_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-29.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0130_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-30.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0131_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-31.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }
        #endregion

        #region Inference: Identifiers
        [Test]
        [Category("Coverage")]
        public static void TestInvalid0200_Identifiers()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 02-00.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0201_Identifiers()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 02-01.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0202_Identifiers()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 02-02.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0203_Identifiers()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 02-03.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0204_Identifiers()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 02-04.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorInvalidManifestChraracter, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0205_Identifiers()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 02-05.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorInvalidManifestNumber, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0206_Identifiers()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 02-06.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0207_Identifiers()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 02-07.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorInvalidManifestNumber, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0208_Identifiers()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 02-08.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0209_Identifiers()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 02-09.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0210_Identifiers()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 02-10.easly";

            Compiler.OutputRootFolder = "./";
            Compiler.Namespace = "Coverage";
            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is IErrorNameUnchanged, ErrorListToString(Compiler));
        }
        #endregion

        #region Inference: types
        #endregion
    }
}
