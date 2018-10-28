using mekg.DeviceSimulation.Models.Enums;

namespace mekg.DeviceSimulation.Models.DTO
{
    public class MeasurementModel
    {
        public int Id{ get; set; }
        public MeasurementMode Mode { get; set; }
        public MeasurementType Type { get; set; }
        public int Frequency { get; set; }
        public int Duration { get; set; }
        public int Length { get; set; }
        public long StartDate { get; set; } // -> converted to seconds in UTC - from 01.01.1970
    }
}

/*
{ "Id": 1, "Mode": 0, "Type": 0, "Frequency":30, "Duration": 2, "Length": 60, "StartDate": 123123 }


{
	"Id": 1,
	"Mode": 0,
	"Type": 0,
	"Frequency": 30,
	"Duration": 2, 
	"Lenght": 60,
	"StartDate":  1540024697
}

*/
