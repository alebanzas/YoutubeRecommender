using FelicidApp.Model;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Band;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static FelicidApp.Utils.Extensions.FunctionalExtensions;

namespace FelicidApp.Services
{
    public static class BandService
    {
        private static IBandClient bandClient;
        private static IBandInfo bandInfo;

        public static async Task InitializeAsync()
        {
            if (bandClient != null)
            {
                return;
            }

            var bands = await BandClientManager.Instance.GetBandsAsync();
            bandInfo = bands.First();

            bandClient = await BandClientManager.Instance.ConnectAsync(bandInfo);

            var uc = bandClient.SensorManager.HeartRate.GetCurrentUserConsent();
            bool isConsented = false;
            if (uc == UserConsent.NotSpecified)
            {
                isConsented = await bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
            }

            if (isConsented || uc == UserConsent.Granted)
            {
                bandClient.SensorManager.HeartRate.ReadingChanged += (obj, ev) =>
                {
                    int heartRate = ev.SensorReading.HeartRate;
                    Debug.WriteLine($"Heart rate = {heartRate}");
                    DispatchAsync(() => Messenger.Default.Send(
                        new BandData(ConfigurationService.UserName, DateTime.Now, heartRate)));
                };
                await bandClient.SensorManager.HeartRate.StartReadingsAsync();
            }
        }
    }
}
