using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using mekg.DeviceSimulation.Helpers;
using mekg.DeviceSimulation.Models.DTO;
using mekg.DeviceSimulation.Models.Enums;
using mekg.DeviceSimulation.Models.Interfaces;
using mekg.DeviceSimulation.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace mekg.DeviceSimulation.Models
{
    public class Session : ISession
    {
        public int Id { get; set; }
        public int MeasurementId { get; set; }
        public MessageType Type {get; set; }
        public List<double> SessionData { get; set; }
        public int Packages { get; set; }
        public int Pause { get; set; }


        private readonly IIoTDeviceService _deviceService;
        private readonly ILogger<Device> _logger;
        private readonly int _samples;


        public Session(IIoTDeviceService deviceService, ILogger<Device> logger, int samples)
        {
            _deviceService = deviceService;
            _logger = logger;
            
            _samples = samples;                         // number of samples in one package
        }

        public async Task SendPackages()
        {
            CalculatePackagesAndPause();

            for (var i = 0; i < Packages; ++i)
            {
                var model = new MeasurementPackage(MeasurementId, Type, Id)
                {
                    PackageId = i + 1,
                    Data = SessionData.Take(_samples).Skip(i * _samples).ToList(),
                    LastPackage = i + 1 == Packages
                };

                await _deviceService.SendDeviceToCloudMessageAsync(model);

                Logger.WriteInfo(_logger, $"MeasurementId: {MeasurementId}, " +
                                          $"SessionId: {Id},  " +
                                          $"PackageId: {model.PackageId} sent. " +
                                          $"Body: {model}");

                Thread.Sleep(Pause);
            }
        }

        private void CalculatePackagesAndPause()
        {
            Packages = SessionData.Count / _samples;   // number of packages in one session
            Pause = _samples * 10;                     // (ms) pause between every package send
        }
    }
}
