using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SWTI.DbUpgrade
{
    public class DbExecutorBuilder
    {
        private readonly string _connectionString;
        private SqlConnection provider { get; set; }
        private SqlTransaction transaction { get; set; }
        private DbVersion FromVersion { get; set; }
        private DbVersion LatestVersion { get; set; }
        public DbExecutorBuilder(string connectionString)
        {
            _connectionString = connectionString;
            provider = new SqlConnection(connectionString);
            LatestVersion = DbVersion.Default();
        }
        public DbExecutorBuilder OpenTransaction()
        {
            Console.WriteLine("Open transaction");
            provider.Open();
            transaction = provider.BeginTransaction();
            return this;
        }
        public void CommitTransaction()
        {
            try
            {
                Console.WriteLine("Commit transaction");
                transaction.Commit();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Rollback transaction");
                transaction.Rollback();
            }
            transaction.Dispose();
            provider.Close();
        }

        public DbExecutorBuilder EnsureDatabaseExists()
        {
            var dbName = provider.Database;
            var masterConnectionString = provider.ConnectionString.Replace(dbName, "master");
            using (var conn = new SqlConnection(masterConnectionString))
            {
                var sqlStatement = string.Format("SELECT NAME FROM sys.databases WHERE name = N'{0}'", dbName);
                conn.Open();
                var result = conn.ExecuteScalar<string>(sqlStatement);

                if (string.IsNullOrEmpty(result))
                {
                    Console.WriteLine("Initial database " + dbName);
                    conn.Execute(string.Format("IF  NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'{0}') BEGIN CREATE DATABASE {0} END; ", dbName));
                }
            }
            return this;
        }

        public DbExecutorBuilder SetupDbUpgradeStepTable()
        {
            var versionTableName = GetVersionTableName();
            var upgradeStepTableName = GetUpgradeStepTableName();
            var dbName = provider.Database;
            var executeResult = 0;
            using (SqlCommand command = new SqlCommand(string.Format(@"SELECT TOP (1) * FROM information_schema.TABLES WHERE (TABLE_CATALOG = '{0}') AND (TABLE_NAME = '{1}')", dbName, versionTableName), provider, transaction))
            {
                var reader = command.ExecuteScalar();
                if (reader == null)
                {
                    //var query = string.Format("IF  NOT EXISTS (SELECT * FROM [sys].[databases] WHERE name = N'{0}') BEGIN CREATE DATABASE [{0}] END; ", dbName);
                    using (SqlCommand commandVersion = new SqlCommand($"CREATE TABLE [{versionTableName}] ([Value] VARCHAR(20) NOT NULL )", provider, transaction))
                    {
                        executeResult = commandVersion.ExecuteNonQuery();
                        if (executeResult < 0)
                        {
                            var dbVersion = new DbVersion(1, 0, 0);
                            using (SqlCommand commandInsertVersion = new SqlCommand($"INSERT INTO [{versionTableName}] (Value) VALUES ('" + dbVersion.ToString() + "')", provider, transaction))
                            {
                                executeResult = commandInsertVersion.ExecuteNonQuery();

                            }
                        }
                    }
                }
            }

            using (SqlCommand commandUpgradeStep = new SqlCommand(string.Format(@"SELECT TOP (1) * FROM information_schema.TABLES WHERE (TABLE_CATALOG = '{0}') AND (TABLE_NAME = '{1}')", dbName, upgradeStepTableName), provider, transaction))
            {
                var readerUpgrade = commandUpgradeStep.ExecuteScalar();
                if (readerUpgrade == null)
                {
                    using (SqlCommand commandVersion = new SqlCommand($"CREATE TABLE [{upgradeStepTableName}] (Version VARCHAR(20) NOT NULL, SqlStatement NVARCHAR(4000) NOT NULL, CreatedDate DATETIME NOT NULL )", provider, transaction))
                    {
                        executeResult = commandVersion.ExecuteNonQuery();
                    }
                }
            }
            return this;
        }
        public DbExecutorBuilder GetVersion()
        {
            var versionTableName = GetVersionTableName();
            using (SqlCommand command = new SqlCommand($"SELECT TOP 1 * FROM [{versionTableName}]", provider, transaction))
            {
                var version = ((string)command.ExecuteScalar()).Split('.');
                FromVersion = new DbVersion(int.Parse(version[0]), int.Parse(version[1]), int.Parse(version[2]));
                LatestVersion = FromVersion;
            }
            return this;
        }

        public DbExecutorBuilder RunSteps(IEnumerable<Step> steps)
        {
            var upgradeSteps = steps.Where(x => x.Version.Compare(FromVersion) > 0).OrderBy(x => x.Version.ToString());
            var upgradeStepTableName = GetUpgradeStepTableName();

            if (upgradeSteps.Any())
            {
                foreach (var step in upgradeSteps)
                {
                    foreach (var sql in step.Sql.Where(x => !x.IsRollback))
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            var scripts = Regex.Split(sql.Sql, @"^GO\r\n|go\r\n|go \r\n", RegexOptions.Multiline);
                            command.Connection = provider;
                            command.Transaction = transaction;
                            foreach (var splitScript in scripts)
                            {
                                if (string.IsNullOrEmpty(splitScript))
                                    continue;
                                Console.WriteLine("Running: " + sql.SqlName);
                                command.CommandText = splitScript;
                                command.CommandTimeout = 300;
                                command.ExecuteNonQuery();
                            }
                            using (SqlCommand commandChildren = new SqlCommand($"INSERT INTO [{upgradeStepTableName}] (Version,SqlStatement,CreatedDate) VALUES ('" + step.Version.ToString() + "','" + step.Name + " - " + sql.SqlName + "',GETDATE())", provider, transaction))
                            {
                                commandChildren.ExecuteNonQuery();
                            }
                        };
                    }
                }
                LatestVersion = upgradeSteps.Last().Version;
            }
            return this;
        }
        public DbExecutorBuilder Do(IEnumerable<Step> steps, bool isSplit = false)
        {
            var upgradeSteps = steps.ToList();
            var upgradeStepTableName = GetUpgradeStepTableName();
            if (upgradeSteps.Any())
            {
                foreach (var step in upgradeSteps)
                {
                    foreach (var sql in step.Sql.Where(x => !x.IsRollback))
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            var scripts = Regex.Split(sql.Sql, @"^GO\r\n|go\r\n|go \r\n", RegexOptions.Multiline);
                            command.Connection = provider;
                            command.Transaction = transaction;

                            if (isSplit)
                            {
                                foreach (var splitScript in scripts)
                                {
                                    Console.WriteLine("Running: " + sql.SqlName);
                                    command.CommandText = splitScript;
                                    command.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                Console.WriteLine("Running: " + sql.SqlName);
                                command.CommandText = sql.Sql;
                                command.ExecuteNonQuery();
                            }

                            using (SqlCommand commandChildren = new SqlCommand($"INSERT INTO [{upgradeStepTableName}] (Version,SqlStatement,CreatedDate) VALUES ('Translations','" + step.Name + " - " + sql.SqlName + "',GETDATE())", provider, transaction))
                            {
                                commandChildren.ExecuteNonQuery();
                            }
                        };
                    }
                }
            }
            return this;
        }
        public DbExecutorBuilder UpdateDatabaseVersion()
        {
            var version = LatestVersion.ToString();
            if (!string.IsNullOrEmpty(version))
            {
                var versionTableName = GetVersionTableName();
                using (SqlCommand command = new SqlCommand($"UPDATE {versionTableName} SET Value = '" + LatestVersion.ToString() + "'", provider, transaction))
                {
                    var result = command.ExecuteNonQuery();
                };
            }
            return this;
        }
        private string GetUpgradeStepTableName()
        {
            var upgradeStepTableName = "upgradestep";
            return upgradeStepTableName;
        }

        private string GetVersionTableName()
        {
            var versionTableName = "version";
            return versionTableName;
        }

    }
}
