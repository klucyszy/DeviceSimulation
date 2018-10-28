using mekg.DeviceSimulation.Models;
using mekg.DeviceSimulation.Models.Interfaces;
using mekg.DeviceSimulation.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using mekg.DeviceSimulation.Configuration;
using Microsoft.Extensions.Options;

namespace mekg.DeviceSimulation.ConsoleApp
{
    public class App
    {
        private readonly ILogger<App> _appLogger;
        private readonly ILogger<Device> _deviceLogger;
        private readonly IIoTDeviceService _iotDeviceService;
        private readonly IDevice _device;
        private readonly IOptions<MeasurementConfiguration> _config;

        public App(ILogger<App> appLogger, ILogger<Device> deviceLogger, IIoTDeviceService ioTDeviceService,
                    IOptions<MeasurementConfiguration> config)
        {
            _appLogger = appLogger;
            _deviceLogger = deviceLogger;
            _iotDeviceService = ioTDeviceService;

            _device = new Device(_deviceLogger, _iotDeviceService, config); 
        }

        public async Task Run()
        {
            await _device.DeviceStateMachine();

            Console.ReadLine();
        }

    }
}
