using System;
using System.Collections.Generic;
using System.Text;

namespace SWTI.DbUpgrade
{
    public class SqlUpgrader : IDbUpgrade
    {
        private readonly string _connectionString;
        public SqlUpgrader(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void Run()
        {
            try
            {
                new DbExecutorBuilder(_connectionString)
             .EnsureDatabaseExists()
             .OpenTransaction()
             .SetupDbUpgradeStepTable()
             .GetVersion()
             //.Do(new SqlUpgradeSystemSteps().List(), true)
             .RunSteps(new SqlUpgraderStep().List())
             .UpdateDatabaseVersion()
             .CommitTransaction();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
    }
}
