using mekg.DeviceSimulation.Clients;
using mekg.DeviceSimulation.Clients.Interfaces;
using mekg.DeviceSimulation.Configuration;
using mekg.DeviceSimulation.Models;
using mekg.DeviceSimulation.Models.Interfaces;
using mekg.DeviceSimulation.Services;
using mekg.DeviceSimulation.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace mekg.DeviceSimulation.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create service collectioon
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            //Create service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            //Enable log into program
            var logger = serviceProvider.GetService<ILogger<Program>>();
            try
            {
                logger.LogInformation("[Program.cs] Started.");

                //Run app
                serviceProvider.GetService<App>().Run().GetAwaiter().GetResult();
                logger.LogInformation("[Program.cs]. Finished.");
            }
            catch (Exception ex)
            {
                logger.LogError($"[Program.cs] Something went wrong: {ex.Message}");
                logger.LogError($"[Program.cs] Exception: {ex}");

                throw;
            }
        }

        static void ConfigureServices(IServiceCollection services)
        {
            //Add logging
            services.AddLogging(configure => configure.AddSerilog()
                                                      //.AddConsole()
                                                      );

            //Initialize serilog logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logging.log")
                .CreateLogger();

            //Build configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            //Add configuration
            services.AddOptions();
            services.Configure<IoTHubConfiguration>(configuration.GetSection("AppSettings"));
            services.Configure<MeasurementConfiguration>(configuration.GetSection("Measurement"));

            //Register DI
            services.AddScoped<IIoTHubClient, IoTHubClient>();
            services.AddScoped<IIoTDeviceService, IoTDeviceService>();
            services.AddScoped<IDevice, Device>();
            services.AddScoped<ISession, Session>();

            //Add app
            services.AddTransient<App>();
        }
    }
}
