using FelicidApp.Services;
using FelicidApp.ViewModel.Base;
using static FelicidApp.Utils.Extensions.FunctionalExtensions;
using GalaSoft.MvvmLight.Messaging;
using FelicidApp.Model;
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Windows.ApplicationModel.Appointments;
using System.Threading.Tasks;

namespace FelicidApp.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private bool busy;
        public bool Busy { get { return busy; } set { Set(ref busy, value); } }

        private int heartRate;
        public int HeartRate { get { return heartRate; } set { Set(ref heartRate, value); } }

        private DateTime heartRateTimestamp;
        public DateTime HeartRateTimestamp { get { return heartRateTimestamp; } set { Set(ref heartRateTimestamp, value); } }

        private string emotion;
        public string Emotion { get { return emotion; } set { Set(ref emotion, value); } }

        private DateTime emotionTimestamp;
        public DateTime EmotionTimestamp { get { return emotionTimestamp; } set { Set(ref emotionTimestamp, value); } }

        private string averageEmotion;
        public string AverageEmotion { get { return averageEmotion; } set { Set(ref averageEmotion, value); } }

        private DateTime averageTimestamp;
        public DateTime AverageTimestamp { get { return averageTimestamp; } set { Set(ref averageTimestamp, value); } }

        public MainViewModel()
        {
            Messenger.Default.Register<BandData>(this, OnBandDataReceived);
            Messenger.Default.Register<EmotionData>(this, OnEmotionDataReceived);
        }

        private void WorkStarting() => Busy = true;

        private bool IsNotBusy() => !Busy;

        private void WorkDone() => Busy = false;

        public async override void OnPageLoaded()
            => await WardAsync(
                WorkStarting,
                InitializeAsync,
                WorkDone,
                "Error connecting to MS Band!");

        private async Task InitializeAsync()
        {
            TelemetryService.Default.MessageReceived += OnMessageReceived;
            await BandService.InitializeAsync();
        }

        private async void OnBandDataReceived(BandData data)
            => await WardAsync(async () => 
                {
                    await TelemetryService.Default.SendAsync(data);
                    HeartRate = data.HeartRate;
                    HeartRateTimestamp = data.Timestamp;
                },
                "Error sending heart rate to telemetry service!");

        private async void OnEmotionDataReceived(EmotionData data)
            => await WardAsync(async () =>
                {
                    await TelemetryService.Default.SendAsync(data);
                    Emotion = data.Emotion;
                    EmotionTimestamp = data.Timestamp;
                },
                "Error sending emotion to telemetry service!");

        private async void OnMessageReceived(object sender, TelemetryService.MessageEventArgs e)
            => await WardAsync(async () =>
            {
                var message = e.Message;
                Debug.WriteLine(message);
                var summary = JsonConvert.DeserializeObject<SummaryData>(message);
                AverageEmotion = summary.Mood;
                AverageTimestamp = DateTime.Now;
                RegisterStatusInOutlook(summary.Mood);
            },
            "Error getting status from telemetry service!");

        private async void RegisterStatusInOutlook(string emotion)
            => await WardAsync(async () =>
            {
                Debug.WriteLine(ConfigurationService.AppointmentId);

                var id = ConfigurationService.AppointmentId;

                if (emotion == "Neutral")
                {
                    var appointment = new Appointment();
                    appointment.StartTime = DateTime.Now;
                    appointment.Duration = TimeSpan.FromHours(2);
                    appointment.Subject = "I'm working. Do not disturb!";
                    appointment.Location = "My office";
                    appointment.Sensitivity = AppointmentSensitivity.Private;
                    appointment.BusyStatus = AppointmentBusyStatus.Busy;
                    appointment.Details = "I'm in the middle of a burst of creativity. Just let me work for a while. Thx!";

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        var newId = await AppointmentManager.ShowAddAppointmentAsync(appointment, new Windows.Foundation.Rect(100, 100, 1, 1));
                        ConfigurationService.AppointmentId = newId;
                    }
                    //else
                    //{
                    //    var newId = await AppointmentManager.ShowReplaceAppointmentAsync(id, appointment, new Windows.Foundation.Rect(100, 100, 1, 1));
                    //    ConfigurationService.AppointmentId = newId;
                    //}

                    //http://graph.microsoft.io/en-us/docs/platform/uwp 
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        await AppointmentManager.ShowRemoveAppointmentAsync(id, new Windows.Foundation.Rect(100, 100, 1, 1));
                        ConfigurationService.AppointmentId = "";
                    }
                }
            },
            "Error creating calendar event!");
    }
}
