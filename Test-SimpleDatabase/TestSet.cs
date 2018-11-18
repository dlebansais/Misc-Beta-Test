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
                RootPassword = "root";
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
    }
}
