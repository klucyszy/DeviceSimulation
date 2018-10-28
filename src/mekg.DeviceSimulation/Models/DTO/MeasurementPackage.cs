using mekg.DeviceSimulation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace mekg.DeviceSimulation.Models.DTO
{
    public class MeasurementPackage
    {
        public int MeasurementId { get; set; }
        public MessageType Type { get; set; }
        public int SessionId { get; set; }
        public int PackageId { get; set; }
        public bool LastPackage { get; set; }
        public List<double>  Data { get; set; }

        public MeasurementPackage(int measurementId, MessageType type, int sessionId)
        {
            MeasurementId = measurementId;
            Type = type;
            SessionId = sessionId;
        }
    }
}
