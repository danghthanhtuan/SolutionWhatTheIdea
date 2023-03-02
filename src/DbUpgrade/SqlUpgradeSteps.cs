using SWTI.DbUpgrade.RunOnceScripts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SWTI.DbUpgrade
{
    public class SqlStatement
    {
        public string Sql { get; set; }
        public string SqlName { get; set; }
        public bool IsRollback { get; set; }
        public SqlStatement(string sqlName, string sql, bool isRollback)
        {
            Sql = sql;
            SqlName = sqlName;
            IsRollback = isRollback;
        }
    }

    public class Step
    {
        public string Name { get; set; }

        public IEnumerable<SqlStatement> Sql { get; set; }

        public DbVersion Version { get; set; }
    }
    public class SqlUpgraderStep
    {

        public IEnumerable<Step> List()
        {
            return
                Push(new DbVersion(1, 0, 1), "Initial_Database", () => new List<SqlStatement>()
                {
                    new SqlStatement("_0001_Initial_Database", RunOnce._0001_Initial_Database, false),
                })
                .Yield();
        }

        private Pusher Push(DbVersion version, string name, Expression<Func<IEnumerable<SqlStatement>>> expr)
        {
            return new Pusher(version, name, expr);
        }

        class Pusher
        {
            readonly Stack<Step> steps = new Stack<Step>();

            public Pusher(DbVersion version, string name, Expression<Func<IEnumerable<SqlStatement>>> expr)
            {
                Push(version, name, expr);
            }

            public Pusher Push(DbVersion version, string name, Expression<Func<IEnumerable<SqlStatement>>> expr)
            {
                steps.Push(new Step { Version = version, Name = name, Sql = expr.Compile()() });

                return this;
            }

            public IEnumerable<Step> Yield()
            {
                return steps;
            }
        }
    }
}
