using mekg.DeviceSimulation.Clients.Interfaces;
using mekg.DeviceSimulation.Configuration;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using MAD = Microsoft.Azure.Devices;
using MADC = Microsoft.Azure.Devices.Client;

namespace mekg.DeviceSimulation.Clients
{
    public class IoTHubClient : IIoTHubClient, IDisposable
    {
        private readonly IoTHubConfiguration _config;
        private readonly DeviceClient _iotDeviceClient;

        public IoTHubClient(IOptions<IoTHubConfiguration> config)
        {
            _config = config.Value;

            _iotDeviceClient = DeviceClient.CreateFromConnectionString(_config.IoTDeviceConnectionString);
        }

        public async Task SendD2CAsync(MADC.Message message)
        {
            await _iotDeviceClient.SendEventAsync(message);
        }

        public async Task<MADC.Message> ReceiveC2DAsync()
        {
            var message =  await _iotDeviceClient.ReceiveAsync();

            await _iotDeviceClient.CompleteAsync(message);

            return message;
        }

        public void Dispose()
        {
            _iotDeviceClient.Dispose();
        }
    }
}
