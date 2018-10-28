using mekg.DeviceSimulation.Clients.Interfaces;
using mekg.DeviceSimulation.Services.Interfaces;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace mekg.DeviceSimulation.Services
{
    public class IoTDeviceService : IIoTDeviceService
    {
        private readonly IIoTHubClient _iotHubClient;

        public IoTDeviceService(IIoTHubClient client)
        {
            _iotHubClient = client;
        }

        public async Task SendDeviceToCloudMessageAsync<TModel>(TModel model)
        {
            var jsonModel = JsonConvert.SerializeObject(model);

            var message = new Message(Encoding.ASCII.GetBytes(jsonModel));

            await _iotHubClient.SendD2CAsync(message);
        }

        public async Task<TResult> ReceiveCloudToDeviceMessageAsync<TResult>()
        {
            while (true)
            {
                var message = await _iotHubClient.ReceiveC2DAsync();

                if (message == null) continue;

                return ConvertToTResultObject<TResult>(message);
            }
        }

        private TResult ConvertToTResultObject<TResult>(Message message)
        {
            var jsonString = Encoding.ASCII.GetString(message.GetBytes());

            var result = JsonConvert.DeserializeObject<TResult>(jsonString);

            return result;
        }
    }
}
