using System.Threading.Tasks;
using MADC = Microsoft.Azure.Devices.Client;

namespace mekg.DeviceSimulation.Clients.Interfaces
{
    public interface IIoTHubClient
    {
        Task SendD2CAsync(MADC.Message message);
        Task<MADC.Message> ReceiveC2DAsync();

    }
}
