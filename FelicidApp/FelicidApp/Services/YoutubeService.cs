using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FelicidApp.Services
{
    public static class YoutubeService
    {
        public static async Task<List<string>> GetYoutubeURI()
        {
            var r = await GetPrediction();

            return r;
        }

        private static async Task<List<string>> GetPrediction()
        {
            return new List<string>
            {
                "https://www.youtube.com/watch?v=AVytYWzRcv8",
                "https://www.youtube.com/watch?v=k4V3Mo61fJM",
                "https://www.youtube.com/watch?v=iAP9AF6DCu4",
                "http://www.youtube.com/watch?v=Fwsl1dUcgwM",
                "https://www.youtube.com/watch?v=epYKVcHrVr0",
                "https://www.youtube.com/watch?v=NUTGr5t3MoY",
                "https://www.youtube.com/watch?v=RkZkekS8NQU",
                "https://www.youtube.com/watch?v=MPZEEPQJ10A",
                "https://www.youtube.com/watch?v=SWSz_PAfgNc",
                "https://www.youtube.com/watch?v=6hzrDeceEKc",
            };



            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"Id", "Date", "Gender", "Age", "anger", "contempt", "disgust", "fear", "happiness", "neutral", "sadness", "surprise", "HR", "Media Name"},
                                Values = new string[,] {  { "125", "", "value", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "value" },  { "0", "", "value", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "value" },  }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                string apiKey = Config.Default.MachineLearningKey; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/1de8c8f49ef643cd98ed40f86bd31b96/services/c67f29c3826345b5a14d84575536533d/execute?api-version=2.0&details=true");

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine("Result: {0}", result);

                    return new List<string> {result};
                }
                else
                {
                    Debug.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Debug.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(responseContent);

                    return new List<string> {responseContent};
                }
            }
        }
    }
    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }
}
