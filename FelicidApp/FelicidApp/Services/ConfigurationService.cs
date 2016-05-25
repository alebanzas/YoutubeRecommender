using Windows.Storage;

namespace FelicidApp.Services
{
    public static class ConfigurationService
    {
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
