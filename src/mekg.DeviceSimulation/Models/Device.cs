using mekg.DeviceSimulation.Helpers;
using mekg.DeviceSimulation.Models.DTO;
using mekg.DeviceSimulation.Models.Enums;
using mekg.DeviceSimulation.Models.Interfaces;
using mekg.DeviceSimulation.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using mekg.DeviceSimulation.Configuration;
using Microsoft.Extensions.Options;

namespace mekg.DeviceSimulation.Models
{
    public class Device : IDevice, INotifyPropertyChanged
    {
        //Events 
        public event PropertyChangedEventHandler PropertyChanged;

        //Services
        private readonly ILogger<Device> _logger;
        private readonly IIoTDeviceService _iotDeviceService;
        private readonly MeasurementConfiguration _config;

        private static System.Timers.Timer _timer;
        private static MeasurementModel _model;

        private DeviceState _deviceState;
        public DeviceState State
        {
            get { return _deviceState; }
            private set
            {
                _deviceState = value;
                OnPropertyChanged("State");
            }
        }
        public List<double> EcgData { get; private set; }
        public List<double> IrData { get; private set; }
        public List<double> RData { get; private set; }
        //public List<double> PpgData { get; set; }
        public List<DateTime> SessionStarts { get; private set; }

        public Device(ILogger<Device> logger, IIoTDeviceService ioTDeviceService, IOptions<MeasurementConfiguration> config)
        {
            _logger = logger;
            _iotDeviceService = ioTDeviceService;
            _config = config.Value;

            _deviceState = DeviceState.Listen;
            PropertyChanged += ChangeDeviceState;
        }

        #region IDevice implementation

        public async Task DeviceStateMachine()
        {
            switch (State)
            {
                case DeviceState.Listen:
                    await Listen();
                    break;

                case DeviceState.WaitForMeasurementStart:
                    PrepareMeasurement();
                    break;

                case DeviceState.PerformMeasurement:
                    Logger.WriteInfo(_logger, "Measurement performed", ConsoleColor.Red);
                    await Measure();
                    State = DeviceState.Listen;
                    break;

                case DeviceState.Idle:
                    break;
            }
        }

        public async Task Listen()
        {
            Logger.WriteInfo(_logger, "Listening for measurement request..");

            _model = await _iotDeviceService.ReceiveCloudToDeviceMessageAsync<MeasurementModel>();

            Logger.WriteInfo(_logger, "Message received", ConsoleColor.Green);
            Logger.WriteInfo(_logger, $"Message body: {JsonConvert.SerializeObject(_model)}", ConsoleColor.Green);

            State = DeviceState.WaitForMeasurementStart;
        }

        public void PrepareMeasurement()
        {
            var startDate = Converter.UnixEpochToDateTime(_model.StartDate);

            if (startDate < DateTime.UtcNow)
            {
                Logger.WriteError(_logger, $"Date is smaller that current UTC date. Cannot proceed measurement.");

                State = DeviceState.Listen;
                return;
            }

            LoadDeviceData();
            SessionStarts = CalculateSessionsStartTime(startDate);

            Logger.WriteInfo(_logger, $"Waiting for measurement start. Start date: {startDate}");

            CountToMeasurementStart(startDate);

            State = DeviceState.Idle;
        }

        public async Task Measure()
        {
            //_model = new MeasurementModel
            //{
            //    Id = 1, 
            //    Mode = MeasurementMode.Active,
            //    Type = MeasurementType.ECG,
            //    Frequency = 30,
            //    Duration = 20,
            //    Length = 2,
            //    StartDate = 123132131
            //};

            //var startDate = Converter.UnixEpochToDateTime(_model.StartDate);
            //LoadDeviceData();
            //SessionStarts = CalculateSessionsStartTime(startDate);


            await PerformMeasurement();

            State = DeviceState.Listen;

        }

        #endregion

        #region Internal private functions

        private async Task PerformMeasurement()
        {
            var sessionSamples = _model.Length * _config.SampleRate;

            for (var i = 0; i < SessionStarts.Count; ++i)
            {
                if (_model.Type == MeasurementType.ECG)
                {
                    var session = new Session(_iotDeviceService, _logger, _config.Samples)
                    {
                        SessionData = EcgData.Take(sessionSamples).Skip(i * sessionSamples).ToList(),
                        Id = i + 1,
                        MeasurementId = _model.Id,
                        Type = MessageType.Ecg
                    };

                    await session.SendPackages();
                }

                var sleepTime = (SessionStarts.ElementAt(i + 1) - DateTime.UtcNow).Milliseconds;
                Thread.Sleep(sleepTime);
            }

        }

        private void LoadDeviceData()
        {
            EcgData = Converter.SampleDataConverter(Resources.ecg);
        }

        private List<DateTime> CalculateSessionsStartTime(DateTime startDate)
        {
            var sessions = new List<DateTime>();
            var sessionEnd = startDate.AddHours(_model.Duration)
                                      .AddMinutes(-_model.Frequency)
                                      .AddSeconds(-_model.Length);
            DateTime nextSession = new DateTime();

            while (sessionEnd > nextSession)
            {
                if (nextSession == DateTime.MinValue)
                {
                    nextSession = startDate.AddMinutes(_model.Frequency).AddSeconds(_model.Duration);
                    sessions.Add(nextSession);
                }
                else
                {
                    nextSession = nextSession.AddMinutes(_model.Frequency).AddSeconds(_model.Duration);
                    if (sessionEnd > nextSession) sessions.Add(nextSession);
                }
            }

            return sessions;
        }

        #endregion

        #region Events 

        private void LaunchMeasurement(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            Logger.WriteInfo(_logger, "Measurement launched from event.", ConsoleColor.Green);

            State = DeviceState.PerformMeasurement;
        }

        private void ChangeDeviceState(object sender, PropertyChangedEventArgs e)
        {
            DeviceStateMachine().Wait();
        }

        #endregion

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }

        private void CountToMeasurementStart(DateTime startDate)
        {
            var startTime = (startDate - DateTime.UtcNow).TotalMilliseconds;

            _timer = new System.Timers.Timer(startTime);

            _timer.Elapsed += new ElapsedEventHandler(LaunchMeasurement);
            _timer.Start();
        }
    }
}
