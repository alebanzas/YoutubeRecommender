using System.IO;
using Newtonsoft.Json;

namespace FelicidApp.Services
{
    /// <summary>
    /// A Configuration class, just fill the config.json file
    /// with the form:
    /// 
    /// {
    ///  "IotHubUri": "[hubname].azure-devices.net",
    ///  "DeviceName": "[registeredname]",
    ///  "DeviceKey": "[registeredkey]"
    /// }
    /// </summary>
    public partial class Config
    {
        public string IotHubUri { get; set; }
        public string MachineLearningKey { get; set; }

        static Config _config;
        public static Config Default
        {
            get
            {
                if (_config == null)
                {
                    try
                    {
                        _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
                    }
                    catch
                    {
                        //default config by now
                        _config = new Config
                        {
                            //Id,PrimaryKey,SecondaryKey,ConnectionString,ConnectionState,LastActivityTime,LastConnectionStateUpdatedTime,LastStateUpdatedTime,MessageCount,State,SuspensionReason
                            IotHubUri = "FeliciHub.azure-devices.net",
                            MachineLearningKey = "123",
                        };
                    }
                }
                return _config;
            }
        }
    }
}