using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SWTI.DbUpgrade
{
    public class SqlUpgradeSystemSteps
    {

        public SqlUpgradeSystemSteps()
        {

        }
        public IEnumerable<Step> List()
        {
            return
                Push("Upgrade for system store procedure, trigger, view, function...", () => new List<SqlStatement>()
                {
                  
                })
                .Yield();
        }

        private Pusher Push(string name, Expression<Func<IEnumerable<SqlStatement>>> expr)
        {
            return new Pusher(name, expr);
        }

        class Pusher
        {
            readonly Stack<Step> steps = new Stack<Step>();

            public Pusher(string name, Expression<Func<IEnumerable<SqlStatement>>> expr)
            {
                Push(name, expr);
            }

            public Pusher Push(string name, Expression<Func<IEnumerable<SqlStatement>>> expr)
            {
                steps.Push(new Step { Name = name, Sql = expr.Compile()() });

                return this;
            }

            public IEnumerable<Step> Yield()
            {
                return steps;
            }
        }
    }
}
