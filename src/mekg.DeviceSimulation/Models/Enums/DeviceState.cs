using System;
using System.Collections.Generic;
using System.Text;

namespace mekg.DeviceSimulation.Models.Enums
{
    public enum DeviceState
    {
        Listen, 
        WaitForMeasurementStart, 
        PerformMeasurement,
        Idle

    }
}
