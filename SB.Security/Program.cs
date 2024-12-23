using SB.Security.Helper;
using SB.Security.Models.Configuration;
using SB.Security.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Security
{
    public class Program
    {

        public static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile(ConstantSupplier.APP_SETTINGS_FILE_NAME)
                  .Build();


            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .CreateLogger();

            try
            {


                Log.Information(ConstantSupplier.LOG_INFO_APP_START_MSG);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_FIRST);
                sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_SECOND_GATEWAY);
                sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_THIRD_VERSION);
                sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_FOURTH_COPYRIGHT);
                sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_END);
                Log.Logger.Information(sb.ToString());

                // It aggregates all of the middlewares on functionality and puts into the applications
                CreateHostBuilder(args).Build().Run();
                
                //BuildWebHost(args).Run();
            }
            catch (Exception Ex)
            {
                Log.Fatal(Ex, ConstantSupplier.LOG_FATAL_APP_FAILED_MSG);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureKestrel(serverOptions =>
            {
            })
            .UseStartup<Startup>()
            .UseContentRoot(Directory.GetCurrentDirectory());
        }).UseSerilog();

    }
}

