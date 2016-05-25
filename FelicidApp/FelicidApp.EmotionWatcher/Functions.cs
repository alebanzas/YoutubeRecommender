using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelicidApp.EmotionWatcher
{
    public class Functions
    {
        const string EmotionsTableName = "emotions";
        public static void ReadTable(
            [TimerTrigger("00:00:15", RunOnStartup = true)] TimerInfo timerInfo,
            [Table(EmotionsTableName)] IQueryable<Emotion> tableBinding,
            TextWriter logger)
        {
            var query = from p in tableBinding
                        where p.done == "false"
                        select p;

            var query2 = query.ToList().OrderBy(p=> p.Timestamp);

            var tmpArray = new List<string>();

            foreach (var emotion in query2)
            {
                if (!tmpArray.Contains(emotion.deviceid))
                {
                    tmpArray.Add(emotion.deviceid);
                    EmotionLogger(logger, emotion);
                    SendCloudToDeviceMessageAsync(emotion).Wait();
                }
            }

            Debug.WriteLine("================");
            UpdateTableEntries(query);
        }

        private static void EmotionLogger(TextWriter logger, Emotion emotion)
        {
            var msg = $"Name: {emotion.PartitionKey } - Date: {emotion.RowKey} - Mood: {emotion.mood} - HeartRate: {emotion.heartrate}";
            //logger.WriteLine(msg);
            logger.WriteLine("================");
            logger.Write("=   NEW DATA   =");
            logger.WriteLine(msg);
        }

        private static void UpdateTableEntries
            (IQueryable<Emotion> emotionData)
        {
            var emotionTable = CloudStorageAccount.CreateCloudTableClient()
                                  .GetTableReference(EmotionsTableName);

            foreach (var emotion in emotionData)
            {
                emotion.done = "true";
                TableOperation updateOperation = TableOperation.Replace(emotion);
                emotionTable.Execute(updateOperation);
            }
        }

        readonly static JobHostConfiguration HostConfig;
        readonly static CloudStorageAccount CloudStorageAccount;
        readonly static string IoTHubConnectionString;
        readonly static ServiceClient ServiceClient;
        readonly static CloudTable EmotionTable;

        private async static Task SendCloudToDeviceMessageAsync(Emotion emotion)
        {
            var emotionChange = JsonConvert.SerializeObject(emotion);
            var commandMessage = new Message(Encoding.ASCII.GetBytes(emotionChange));
            await ServiceClient.SendAsync(emotion.deviceid, commandMessage);
        }

        static Functions()
        {
            HostConfig = new JobHostConfiguration();
            CloudStorageAccount = CloudStorageAccount.Parse(HostConfig.StorageConnectionString);
            EmotionTable = CloudStorageAccount.CreateCloudTableClient()
                                              .GetTableReference(nameof(Emotion));
            IoTHubConnectionString = ConfigurationManager.ConnectionStrings["AzureiOTHub"].ConnectionString;
            ServiceClient = ServiceClient.CreateFromConnectionString(IoTHubConnectionString);
        }
    }
}
