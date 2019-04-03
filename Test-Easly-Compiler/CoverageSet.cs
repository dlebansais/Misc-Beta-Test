using System;
using System.Collections.Generic;
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

            string TestFileName = $"{RootPath}test.easly";

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

            IRoot ClonedRoot = NodeHelper.DeepCloneNode(FirstRootNode, cloneCommentGuid: true) as IRoot;
            NodeTreeHelper.SetGuidProperty(ClonedRoot.ClassBlocks.NodeBlockList[0].NodeList[0], nameof(IClass.ClassGuid), Guid.Empty);
            Assert.That(!NodeTreeDiagnostic.IsValid(ClonedRoot, assertValid: false));

            Compiler.Compile(ClonedRoot);
            Assert.That(Compiler.ErrorList.Count == 1 && Compiler.ErrorList[0] is ErrorInputRootInvalid, ErrorListToString(Compiler));

            Compiler.Compile(FirstRootNode as IRoot);
            Assert.That(Compiler.ErrorList.Count == 0, ErrorListToString(Compiler));
        }
        #endregion
    }
}
