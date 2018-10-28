using mekg.DeviceSimulation.Models.Enums;
using System.Threading.Tasks;

namespace mekg.DeviceSimulation.Models.Interfaces
{
    public interface IDevice
    {
        DeviceState State { get; }

        Task DeviceStateMachine();
        Task Listen();
        void PrepareMeasurement();
        Task Measure();

    }
}
