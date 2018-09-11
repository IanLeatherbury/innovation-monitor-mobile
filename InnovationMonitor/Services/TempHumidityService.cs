using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using InnovationMonitor.Models;

namespace InnovationMonitor.Services
{
    public class TempHumidityService
    {
        // TODO: 
        string _baseUrl = "https://your-backend-service.azurewebsites.net/api{0}";

        public async Task<List<TempHumidityModel>> GetTempHumidity()
        {
            var tempHumidityModels = new List<TempHumidityModel>();

            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage(HttpMethod.Get, Constants.Constants.GetTempHumidityURL);

            try
            {
                HttpResponseMessage requestResult = await client.SendAsync(request);

                var responseText = await requestResult.Content.ReadAsStringAsync();

                tempHumidityModels = DeserializeResponse<List<TempHumidityModel>>(responseText);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
            }

            return tempHumidityModels;
        }

        public async Task TakeTempHumidity()
        {
            var request = WebRequest.Create(Constants.Constants.TakeTempHumidityURL);
            request.ContentType = "application/xml";
            request.Method = "GET";

            using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                    return;
                }
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        Debug.WriteLine("Response contained empty body...");
                    }
                    else
                    {
                        Debug.WriteLine("Response Body: \r\n {0}", content);
                    }
                }
            }

        }

        /// <summary>
        /// Deserializes the response.
        /// </summary>
        /// <returns>The response.</returns>
        /// <param name="jsonResponse">Json response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private static T DeserializeResponse<T>(string jsonResponse)
        {
            return DeserializeResponse<T>(jsonResponse, string.Empty);
        }

        /// <summary>
        /// Accepts a JSON string and deserializes it to a given object of type T
        /// </summary>
        /// <typeparam name="T">Type of the parameter to add</typeparam>
        /// <param name="jsonResponse">JSON data to deserialize</param>
        /// <param name="rootNode">Name of the root node (if any) to grab the data to deserialize</param>
        /// <returns></returns>
        private static T DeserializeResponse<T>(string jsonResponse, string rootNode)
        {
            var returnObject = Activator.CreateInstance<T>();

            if (!string.IsNullOrEmpty(rootNode)) jsonResponse = JObject.Parse(jsonResponse)[rootNode].ToString();
            returnObject = JsonConvert.DeserializeObject<T>(jsonResponse);

            return returnObject;
        }
    }
}
