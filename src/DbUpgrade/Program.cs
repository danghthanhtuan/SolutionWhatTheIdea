using Microsoft.Extensions.Configuration;
using System;

namespace SWTI.DbUpgrade
{
    class Program
    {
        static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();
            Console.WriteLine(configuration["ConnectionString:DefaultConnection"]);
            Console.WriteLine("====================Poc - start to run db scripts=====================");
            var connectionString = configuration["ConnectionString:DefaultConnection"];
            IDbUpgrade db = new SqlUpgrader(connectionString);
            db.Run();
            Console.WriteLine("====================End=====================");
        }
    }
}
