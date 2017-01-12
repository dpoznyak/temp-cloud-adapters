using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace LegacyConfigged
{
    class Program
    {
        static void Main(string[] args)
        {
            var configString = new Dictionary<string,string>( )
            {
                ["KEY"] = "special value",
                ["KEY2"] = "special value",
                ["ConnectionStrings:myConnectionString2"] = "server=overriden"
            };

            ConfigurationBuilder configurationBuilder =
    new ConfigurationBuilder();
            configurationBuilder
                .AddInMemoryCollection(configString)
                //.AddJsonFile("Config.json",
                //  true) // Bool indicates file is optional
                //        // "EssentialDotNetConfiguartion" is an optional prefix for all
                //        // environment configuration keys, but once used,
                //        // only environment variables with that prefix will be found        
                //.AddEnvironmentVariables("EssentialDotNetConfiguration")
                //.AddCommandLine(
                //  args, GetSwitchMappings(DefaultConfigurationStrings));
                ;

            var root = configurationBuilder.Build();

            root.GetConnectionString("myConnectionString2").Should().Be("server=overriden", "//new config");

            foreach (var v in root.AsEnumerable())
            {
                ConfigurationManager.AppSettings[v.Key] = v.Value;
            }


            var connStrings = root.GetSection("ConnectionStrings").GetChildren().AsEnumerable().ToList();
            if (connStrings.Any())
            {
                ConfigurationManager.
                foreach (var cs in connStrings)
                {
                  //  ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings(cs.Key, cs.Value));
                    ConfigurationManager.ConnectionStrings[cs.Key].ConnectionString = cs.Value;
                }
                //ConfigurationManager.ConnectionStrings.LockItem = true;

            }

            ConfigurationManager.AppSettings["KEY"].Should().Be("special value");
            ConfigurationManager.AppSettings["KEY2"].Should().Be("special value");

            ConfigurationManager.ConnectionStrings["myConnectionString1"].ConnectionString.Should().Be("server=test");
            ConfigurationManager.ConnectionStrings["myConnectionString2"].ConnectionString.Should().Be("server=overriden");
            Console.WriteLine("All fine");
        }
    }
}
