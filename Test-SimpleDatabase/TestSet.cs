using Database;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

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

            Assembly SimpleDatabaseAssembly;

            try
            {
                SimpleDatabaseAssembly = Assembly.Load("SimpleDatabase");
            }
            catch
            {
                SimpleDatabaseAssembly = null;
            }
            Assume.That(SimpleDatabaseAssembly != null);

            if (File.Exists("passwords.txt"))
            {
                using (FileStream fs = new FileStream("passwords.txt", FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        RootId = sr.ReadLine();
                        RootPassword = sr.ReadLine();
                    }
                }
            }
            else
            {
                RootId = "root";
                RootPassword = "";
            }

            try
            {
                TestSchema = new TestSchema();
                ISimpleDatabase Database = new SimpleDatabase();
                ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
                Database.DeleteTables(Credential);
                Database.DeleteCredential(RootId, RootPassword, Credential);
            }
            catch
            {
            }
        }

        private static string RootId;
        private static string RootPassword;
        private static string Server = "localhost";
        private static string UserId = "test";
        private static string UserPassword = "test";
        private static TestSchema TestSchema;
        #endregion

        #region Init
        [Test]
        public static void TestInitCredential()
        {
            ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
            Assert.That(Credential != null, "Init Credential 0");
            Assert.That(Credential.Server == Server, "Init Credential 1");
            Assert.That(Credential.UserId == UserId, "Init Credential 2");
            Assert.That(Credential.Password == UserPassword, "Init Credential 3");
            Assert.That(Credential.Schema == TestSchema, "Init Credential 4");
        }

        [Test]
        public static void TestInitDatabase()
        {
            ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
            Assert.That(Credential != null, "Init Database 0");

            ISimpleDatabase Database = new SimpleDatabase();
            Assert.That(Database != null, "Init Database 1");

            ConnectorType ConnectorType = ConnectorType.MySql;
            ConnectionOption ConnectionOption = ConnectionOption.KeepAlive;

            Database.Initialize(ConnectorType, ConnectionOption);
            Assert.That(Database.ConnectorType == ConnectorType, "Init Database 2");
            Assert.That(Database.ConnectionOption == ConnectionOption, "Init Database 3");
        }

        [Test]
        public static void TestVerifyCredential()
        {
            ConnectorType ConnectorType = ConnectorType.MySql;
            ConnectionOption ConnectionOption = ConnectionOption.KeepAlive;

            ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
            Assert.That(Credential != null, "Verify Credential 0");

            ISimpleDatabase Database = new SimpleDatabase();
            Assert.That(Database != null, "Verify Credential 1");

            Database.Initialize(ConnectorType, ConnectionOption);

            Assert.That(!Database.IsCredentialValid(Credential), "Verify Credential 2");
            Assert.That(Database.CreateCredential(RootId, RootPassword, Credential), "Verify Credential 3");
            Assert.That(Database.IsCredentialValid(Credential), "Verify Credential  4");

            Database.DeleteCredential(RootId, RootPassword, Credential);

            Assert.That(!Database.IsCredentialValid(Credential), "Verify Credential 5");
        }

        [Test]
        public static void TestCreateTables()
        {
            ConnectorType ConnectorType = ConnectorType.MySql;
            ConnectionOption ConnectionOption = ConnectionOption.KeepAlive;

            ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
            Assert.That(Credential != null, "Create Tables 0");

            ISimpleDatabase Database = new SimpleDatabase();
            Assert.That(Database != null, "Create Tables 1");

            Database.Initialize(ConnectorType, ConnectionOption);
            Assert.That(Database.CreateCredential(RootId, RootPassword, Credential), "Create Tables 2");
            Assert.That(Database.CreateTables(Credential), "Create Tables 3");

            Database.DeleteTables(Credential);
            Database.DeleteCredential(RootId, RootPassword, Credential);

            Assert.That(!Database.IsCredentialValid(Credential), "Create Tables 7");
        }

        [Test]
        public static void TestOpen()
        {
            ConnectorType ConnectorType = ConnectorType.MySql;
            ConnectionOption ConnectionOption = ConnectionOption.KeepAlive;

            ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
            Assert.That(Credential != null, "Open 0");

            ISimpleDatabase Database = new SimpleDatabase();
            Assert.That(Database != null, "Open 1");

            Database.Initialize(ConnectorType, ConnectionOption);
            Assert.That(Database.CreateCredential(RootId, RootPassword, Credential), "Open 2");
            Assert.That(Database.CreateTables(Credential), "Open 3");

            Assert.That(!Database.IsOpen, "Open 4");
            Assert.That(Database.Open(Credential), "Open 5");
            Assert.That(Database.IsOpen, "Open 6");

            Database.Close();

            Assert.That(!Database.IsOpen, "Open 7");

            Database.DeleteTables(Credential);
            Database.DeleteCredential(RootId, RootPassword, Credential);

            Assert.That(!Database.IsCredentialValid(Credential), "Open 8");
        }

        [Test]
        public static void TestDeleteNonEmpty()
        {
            ConnectorType ConnectorType = ConnectorType.MySql;
            ConnectionOption ConnectionOption = ConnectionOption.KeepAlive;

            ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
            Assert.That(Credential != null, "Delete Non Empty 0");

            ISimpleDatabase Database = new SimpleDatabase();
            Assert.That(Database != null, "Delete Non Empty 1");

            Database.Initialize(ConnectorType, ConnectionOption);
            Assert.That(Database.CreateCredential(RootId, RootPassword, Credential), "Delete Non Empty 2");
            Assert.That(Database.CreateTables(Credential), "Delete Non Empty 3");
            Assert.That(Database.Open(Credential), "Delete Non Empty 4");

            ISingleInsertResult InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test0, new List<ColumnValuePair<Guid>>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, Guid.Empty) }));
            Assert.That(InsertResult.Success, "Delete Non Empty 5");

            Database.Close();
            Database.DeleteTables(Credential);
            Database.DeleteCredential(RootId, RootPassword, Credential);

            Assert.That(Database.IsCredentialValid(Credential), "Delete Non Empty 6");

            Assert.That(Database.Open(Credential), "Delete Non Empty 7");

            ISingleRowDeleteResult DeleteResult = Database.Run(new SingleRowDeleteContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, Guid.Empty)));
            Assert.That(DeleteResult.Success, "Delete Non Empty 8");

            Database.Close();
            Database.DeleteTables(Credential);
            Database.DeleteCredential(RootId, RootPassword, Credential);

            Assert.That(!Database.IsCredentialValid(Credential), "Delete Non Empty 9");
        }
        #endregion

        #region Queries
        private static Guid guidKey0 = new Guid("{1BA0D7E9-039F-44E6-A966-CC67AC01A65D}");
        private static Guid guidKey1 = new Guid("{2FA55A73-0311-4818-8B34-1492308ADBF1}");
        private static Guid guidKey2 = new Guid("{16DC914E-CDED-41DD-AE23-43B62676159D}");

        private static void InstallDatabase(string testName, out ICredential credential, out ISimpleDatabase database)
        {
            ConnectorType ConnectorType = ConnectorType.MySql;
            ConnectionOption ConnectionOption = ConnectionOption.KeepAlive;

            credential = new Credential(Server, UserId, UserPassword, TestSchema);
            Assert.That(credential != null, $"{testName} - Create Credential Object");

            database = new SimpleDatabase();
            Assert.That(database != null, $"{testName} - Create Database Object");

            database.Initialize(ConnectorType, ConnectionOption);
            Assert.That(!database.IsCredentialValid(credential), $"{testName} - Verify Credential Invalid");
            Assert.That(database.CreateCredential(RootId, RootPassword, credential), $"{testName} - Create Credential");
            Assert.That(database.IsCredentialValid(credential), $"{testName} - Verify Credential Valid");
            Assert.That(database.CreateTables(credential), $"{testName} - Create Tables");
            Assert.That(database.Open(credential), $"{testName} - Open");
        }

        private static void UninstallDatabase(string testName, ref ICredential credential, ref ISimpleDatabase database)
        {
            IMultiRowDeleteResult DeleteResult;
            DeleteResult = database.Run(new MultiRowDeleteContext(TestSchema.Test0, 0));
            DeleteResult = database.Run(new MultiRowDeleteContext(TestSchema.Test1, 0));

            database.Close();
            database.DeleteTables(credential);
            database.DeleteCredential(RootId, RootPassword, credential);

            Assert.That(!database.IsCredentialValid(credential), $"{testName} - Verify Credential Invalid (after close)");
        }
/*
        [Test]
        public static void TestSingleInsert()
        {
            string TestName = "Single Insert";

            InstallDatabase(TestName, out ICredential Credential, out ISimpleDatabase Database);

            ISingleInsertResult InsertResult;
            IMultiQueryResult SelectResult;
            List<IResultRow> RowList;

            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey0) }));
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert first key");

            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 1) }));
            Assert.That(!InsertResult.Success, $"{TestName} - 0: Insert with no key (must fail)");

            ((SimpleDatabase)Database).IgnoreErrorCode = 1062;
            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey0) }));
            Assert.That(!InsertResult.Success, $"{TestName} - 0: Insert same key (must fail)");

            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new ColumnValuePair<int>(TestSchema.Test0_Int, 1) }));
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert new key and int");

            SelectResult = Database.Run(new MultiQueryContext(TestSchema.Test0.All));
            Assert.That(SelectResult.Success, $"{TestName} - 0: Read table");
            Assert.That(SelectResult.RowList != null, $"{TestName} - 0: Read table result");

            RowList = new List<IResultRow>(SelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 2, $"{TestName} - 0: Count rows");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[0], out Guid Test0_Row_0_0) && Test0_Row_0_0 == guidKey0, $"{TestName} - 0: Check row 0, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[0], out int Test0_Row_0_1), $"{TestName} - 0: Check row 0, column 1");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[1], out Guid Test0_Row_1_0) && Test0_Row_1_0 == guidKey1, $"{TestName} - 0: Check row 1, column 0");
            Assert.That(TestSchema.Test0_Int.TryParseRow(RowList[1], out int Test0_Row_1_1) && Test0_Row_1_1 == 1, $"{TestName} - 0: Check row 1, column 1");

            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<string>(TestSchema.Test1_String, "row 0") }));
            Assert.That(InsertResult.Success, $"{TestName} - 1: Insert first row");

            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<string>(TestSchema.Test1_String, "row 1") }));
            Assert.That(InsertResult.Success, $"{TestName} - 1: Insert second row");

            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test1_Int, 1), new ColumnValuePair<string>(TestSchema.Test1_String, "row 2") }));
            Assert.That(!InsertResult.Success, $"{TestName} - 1: Insert with key (must fail)");

            SelectResult = Database.Run(new MultiQueryContext(TestSchema.Test1.All));
            Assert.That(SelectResult.Success, $"{TestName} - 1: Read table");
            Assert.That(SelectResult.RowList != null, $"{TestName} - 1: Read table result");

            RowList = new List<IResultRow>(SelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 2, $"{TestName} - 1: Count rows");
            Assert.That(TestSchema.Test1_Int.TryParseRow(RowList[0], out int Test1_Row_0_0) && Test1_Row_0_0 == 1, $"{TestName} - 1: Check row 0, column 0");
            Assert.That(TestSchema.Test1_String.TryParseRow(RowList[0], out string Test1_Row_0_1) && Test1_Row_0_1 == "row 0", $"{TestName} - 1: Check row 0, column 1");
            Assert.That(TestSchema.Test1_Int.TryParseRow(RowList[1], out int Test1_Row_1_0) && Test1_Row_1_0 == 2, $"{TestName} - 1: Check row 1, column 0");
            Assert.That(TestSchema.Test1_String.TryParseRow(RowList[1], out string Test1_Row_1_1) && Test1_Row_1_1 == "row 1", $"{TestName} - 1: Check row 1, column 1");

            UninstallDatabase(TestName, ref Credential, ref Database);
        }

        [Test]
        public static void TestMultiInsert()
        {
            string TestName = "Multi Insert";

            InstallDatabase(TestName, out ICredential Credential, out ISimpleDatabase Database);

            IMultiInsertResult InsertResult;
            IMultiQueryResult SelectResult;
            List<IResultRow> RowList;

            InsertResult = Database.Run(new MultiInsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }), }));
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert first 3 keys");

            SelectResult = Database.Run(new MultiQueryContext(TestSchema.Test0.All));
            Assert.That(SelectResult.Success, $"{TestName} - 0: Read table");
            Assert.That(SelectResult.RowList != null, $"{TestName} - 0: Read table result");

            // Columns are reordered by Guid
            RowList = new List<IResultRow>(SelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 3, $"{TestName} - 0: Count rows");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[0], out Guid Test0_Row_0_0) && Test0_Row_0_0 == guidKey2, $"{TestName} - 0: Check row 0, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[0], out int Test0_Row_0_1), $"{TestName} - 0: Check row 0, column 1");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[1], out Guid Test0_Row_1_0) && Test0_Row_1_0 == guidKey0, $"{TestName} - 0: Check row 1, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[1], out int Test0_Row_1_1), $"{TestName} - 0: Check row 1, column 1");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[2], out Guid Test0_Row_2_0) && Test0_Row_2_0 == guidKey1, $"{TestName} - 0: Check row 2, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[2], out int Test0_Row_2_1), $"{TestName} - 0: Check row 2, column 1");

            InsertResult = Database.Run(new MultiInsertContext(TestSchema.Test1, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<string>(TestSchema.Test1_String, new List<string>() { "row 0", "row 1", "row 2" }) }));
            Assert.That(InsertResult.Success, $"{TestName} - 1: Insert first row");

            SelectResult = Database.Run(new MultiQueryContext(TestSchema.Test1.All));
            Assert.That(SelectResult.Success, $"{TestName} - 1: Read table");
            Assert.That(SelectResult.RowList != null, $"{TestName} - 1: Read table result");

            RowList = new List<IResultRow>(SelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 3, $"{TestName} - 1: Count rows");
            Assert.That(TestSchema.Test1_Int.TryParseRow(RowList[0], out int Test1_Row_0_0) && Test1_Row_0_0 == 1, $"{TestName} - 1: Check row 0, column 0");
            Assert.That(TestSchema.Test1_String.TryParseRow(RowList[0], out string Test1_Row_0_1) && Test1_Row_0_1 == "row 0", $"{TestName} - 1: Check row 0, column 1");
            Assert.That(TestSchema.Test1_Int.TryParseRow(RowList[1], out int Test1_Row_1_0) && Test1_Row_1_0 == 2, $"{TestName} - 1: Check row 1, column 0");
            Assert.That(TestSchema.Test1_String.TryParseRow(RowList[1], out string Test1_Row_1_1) && Test1_Row_1_1 == "row 1", $"{TestName} - 1: Check row 1, column 1");
            Assert.That(TestSchema.Test1_Int.TryParseRow(RowList[2], out int Test1_Row_2_0) && Test1_Row_2_0 == 3, $"{TestName} - 2: Check row 2, column 0");
            Assert.That(TestSchema.Test1_String.TryParseRow(RowList[2], out string Test1_Row_2_1) && Test1_Row_2_1 == "row 2", $"{TestName} - 2: Check row 2, column 2");

            UninstallDatabase(TestName, ref Credential, ref Database);
        }

        [Test]
        public static void TestUpdate()
        {
            string TestName = "Update";

            InstallDatabase(TestName, out ICredential Credential, out ISimpleDatabase Database);

            IMultiInsertResult InsertResult;
            IUpdateResult UpdateResult;
            IMultiQueryResult SelectResult;
            List<IResultRow> RowList;

            InsertResult = Database.Run(new MultiInsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }), }));
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert first 3 keys");

            UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 10) }));
            Assert.That(UpdateResult.Success, $"{TestName} - 0: Update third keys");

            UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new ColumnValuePair<int>(TestSchema.Test0_Int, 10) }, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 20) }));
            Assert.That(UpdateResult.Success, $"{TestName} - 0: Update second and third keys");

            SelectResult = Database.Run(new MultiQueryContext(TestSchema.Test0.All));
            Assert.That(SelectResult.Success, $"{TestName} - 0: Read table");
            Assert.That(SelectResult.RowList != null, $"{TestName} - 0: Read table result");

            // Columns are reordered by Guid
            RowList = new List<IResultRow>(SelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 3, $"{TestName} - 0: Count rows");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[0], out Guid Test0_Row_0_0) && Test0_Row_0_0 == guidKey2, $"{TestName} - 0: Check row 0, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[0], out int Test0_Row_0_1), $"{TestName} - 0: Check row 0, column 1");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[1], out Guid Test0_Row_1_0) && Test0_Row_1_0 == guidKey0, $"{TestName} - 0: Check row 1, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[1], out int Test0_Row_1_1), $"{TestName} - 0: Check row 1, column 1");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[2], out Guid Test0_Row_2_0) && Test0_Row_2_0 == guidKey1, $"{TestName} - 0: Check row 2, column 0");
            Assert.That(TestSchema.Test0_Int.TryParseRow(RowList[2], out int Test0_Row_2_1) && Test0_Row_2_1 == 20, $"{TestName} - 0: Check row 2, column 1");

            UninstallDatabase(TestName, ref Credential, ref Database);
        }

        [Test]
        public static void TestSingleDelete()
        {
            string TestName = "Single Delete";

            InstallDatabase(TestName, out ICredential Credential, out ISimpleDatabase Database);

            IMultiInsertResult InsertResult;
            ISingleRowDeleteResult DeleteResult;
            IMultiQueryResult SelectResult;
            List<IResultRow> RowList;

            InsertResult = Database.Run(new MultiInsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }), }));
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert first 3 keys");

            DeleteResult = Database.Run(new SingleRowDeleteContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey2) }));
            Assert.That(DeleteResult.Success, $"{TestName} - 0: Delete first key");

            SelectResult = Database.Run(new MultiQueryContext(TestSchema.Test0.All));
            Assert.That(SelectResult.Success, $"{TestName} - 0: Read table");
            Assert.That(SelectResult.RowList != null, $"{TestName} - 0: Read table result");

            RowList = new List<IResultRow>(SelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 2, $"{TestName} - 0: Count rows");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[0], out Guid Test0_Row_0_0) && Test0_Row_0_0 == guidKey0, $"{TestName} - 0: Check row 0, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[0], out int Test0_Row_0_1), $"{TestName} - 0: Check row 0, column 1");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[1], out Guid Test0_Row_1_0) && Test0_Row_1_0 == guidKey1, $"{TestName} - 0: Check row 1, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[1], out int Test0_Row_1_1), $"{TestName} - 0: Check row 1, column 1");

            UninstallDatabase(TestName, ref Credential, ref Database);
        }

        [Test]
        public static void TestMultiDelete()
        {
            string TestName = "Multi Delete";

            InstallDatabase(TestName, out ICredential Credential, out ISimpleDatabase Database);

            IMultiInsertResult InsertResult;
            IMultiRowDeleteResult DeleteResult;
            IMultiQueryResult SelectResult;
            List<IResultRow> RowList;

            InsertResult = Database.Run(new MultiInsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }), }));
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert first 3 keys");

            DeleteResult = Database.Run(new MultiRowDeleteContext(TestSchema.Test0, new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1 }), 2));
            Assert.That(DeleteResult.Success, $"{TestName} - 0: Delete first 3 keys");

            SelectResult = Database.Run(new MultiQueryContext(TestSchema.Test0.All));
            Assert.That(SelectResult.Success, $"{TestName} - 0: Read table");
            Assert.That(SelectResult.RowList != null, $"{TestName} - 0: Read table result");

            // Columns are reordered by Guid
            RowList = new List<IResultRow>(SelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 1, $"{TestName} - 0: Count rows");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[0], out Guid Test0_Row_0_0) && Test0_Row_0_0 == guidKey2, $"{TestName} - 0: Check row 0, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[0], out int Test0_Row_0_1), $"{TestName} - 0: Check row 0, column 1");

            UninstallDatabase(TestName, ref Credential, ref Database);
        }*/
        #endregion
    }
}
