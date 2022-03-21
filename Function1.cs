using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventHubs;

namespace IotFunctions
{
    public class Function1
    {
        private static HttpClient client = new HttpClient();
        
        [FunctionName("Function1")]
        public void Run([IoTHubTrigger("messages/events", Connection = "iotCon")]EventData message, ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.ToArray())}");
        }
    }
}