using System.Collections.Generic;
using Windows.Storage;

namespace FelicidApp.Services
{
    public static class ConfigurationService
    {
        private static string _motion;
        public static string Motion
        {
            get { return _motion ?? "Happiness"; }
            set { _motion = value; }
        }

        public static List<string> Playlist { get; set; }

        public static string UserName
        {
            get
            {
                return ApplicationData.Current.RoamingSettings.Values[nameof(UserName)] as string ?? "";
            }

            set
            {
                ApplicationData.Current.RoamingSettings.Values[nameof(UserName)] = value;
            }
        }

        public static string AppointmentId
        {
            get
            {
                return ApplicationData.Current.RoamingSettings.Values[nameof(AppointmentId)] as string ?? "";
            }

            set
            {
                ApplicationData.Current.RoamingSettings.Values[nameof(AppointmentId)] = value;
            }
        }

        public static string DeviceId
        {
            get
            {
                return ApplicationData.Current.RoamingSettings.Values[nameof(DeviceId)] as string ?? "";
            }

            set
            {
                ApplicationData.Current.RoamingSettings.Values[nameof(DeviceId)] = value;
            }
        }
        public static string DeviceKey
        {
            get
            {
                return ApplicationData.Current.RoamingSettings.Values[nameof(DeviceKey)] as string ?? "";
            }

            set
            {
                ApplicationData.Current.RoamingSettings.Values[nameof(DeviceKey)] = value;
            }
        }

    }
}
