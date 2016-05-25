using FelicidApp.Model;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelicidApp.Services
{
    public class TelemetryService
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
        public class Config
        {
            public string IotHubUri { get; set; }

            static Config _config;
            public static Config Default
            {
                get
                {
                    if (_config == null)
                    {
                        try {
                            _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
                        }
                        catch
                        {
                            //default config by now
                            _config = new Config
                            {
                                //Id,PrimaryKey,SecondaryKey,ConnectionString,ConnectionState,LastActivityTime,LastConnectionStateUpdatedTime,LastStateUpdatedTime,MessageCount,State,SuspensionReason
                                IotHubUri = "FeliciHub.azure-devices.net"
                            };
                        }
                    }
                    return _config;
                }
            }
        }

        public class MessageEventArgs : EventArgs
        {
            public string Message { get; set; }
            public MessageEventArgs(string message)
            {
                Message = message;
            }
        }

        DeviceClient deviceClient;
        public event EventHandler<MessageEventArgs> MessageReceived;

        public TelemetryService()
        {
            var key = AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(ConfigurationService.DeviceId, ConfigurationService.DeviceKey);
            deviceClient = DeviceClient.Create(Config.Default.IotHubUri, key, TransportType.Http1);
            receiveCommands();
        }

        static TelemetryService _default = new TelemetryService();
        public static TelemetryService Default => _default;

        private async void receiveCommands()
        {
            Message receivedMessage;
            string messageData;
            int recoverTimeout = 1000;
            while (true)
            {
                try
                {
                    receivedMessage = await deviceClient.ReceiveAsync();
                    if (receivedMessage != null)
                    {
                        messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                        //Logger.LogInfo($"\t> Received message: {messageData}");
                        if (MessageReceived != null)
                        {
                            MessageReceived(this, new MessageEventArgs(messageData));
                        }
                        await deviceClient.CompleteAsync(receivedMessage);
                    }
                    recoverTimeout = 1000;
                }
                catch (Exception ex)
                {
                    //Logger.LogException(ex);
                    Debug.WriteLine(ex.Message);
                    await Task.Delay(recoverTimeout);
                    recoverTimeout *= 10; // increment timeout for connection recovery
                    if (recoverTimeout > 600000)//set a maximum timeout
                    {
                        recoverTimeout = 600000;
                    }
                }
            }
        }

        public async Task SendAsync<T>(T value) where T:Data
        {
            value.DeviceId = ConfigurationService.DeviceId;
            var info = JsonConvert.SerializeObject(value);
            System.Diagnostics.Debug.WriteLine(info);
            var message = new Message(Encoding.UTF8.GetBytes(info));
            await deviceClient.SendEventAsync(message);
        }
    }
}
