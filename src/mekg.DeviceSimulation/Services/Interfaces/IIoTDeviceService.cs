using mekg.DeviceSimulation.Models;
using System.Threading.Tasks;

namespace mekg.DeviceSimulation.Services.Interfaces
{
    public interface IIoTDeviceService
    {
        Task SendDeviceToCloudMessageAsync<TModel>(TModel model);
        Task<TModel> ReceiveCloudToDeviceMessageAsync<TModel>();
    }
}
