// See https://aka.ms/new-console-template for more information
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

string deviceId = "TempSensorDevice";
string deviceKey = "r/hUNiFEaTBI+zVb3J/wEvm9ak9Ji4wqdXwXUjkQI4s=";
string hostname = $"IotTestDevices.azure-devices.net";
string endpoint = $"{hostname}/devices/{deviceId}";
var token = GenerateSasToken(endpoint, deviceKey);
var conn = $"HostName=IotTestDevices.azure-devices.net;DeviceId=TempSensorDevice;SharedAccessKey=r/hUNiFEaTBI+zVb3J/wEvm9ak9Ji4wqdXwXUjkQI4s=";
var deviceClient = DeviceClient.CreateFromConnectionString(conn, TransportType.Amqp_WebSocket_Only);
SendEvent(deviceClient).ConfigureAwait(false).GetAwaiter().GetResult();


static string GenerateSasToken(string resourceUri, string key, string policyName = null, int expiryInSeconds = 3600)
{
    TimeSpan fromEpochStart = DateTime.UtcNow - new DateTime(1970, 1, 1);
    string expiry = Convert.ToString((int)fromEpochStart.TotalSeconds + expiryInSeconds);

    string stringToSign = WebUtility.UrlEncode(resourceUri) + "\n" + expiry;

    HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(key));
    string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

    string token = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}", WebUtility.UrlEncode(resourceUri), WebUtility.UrlEncode(signature), expiry);

    if (!String.IsNullOrEmpty(policyName))
    {
        token += "&skn=" + policyName;
    }

    return token;
}

static async Task SendEvent(DeviceClient deviceClient)
{

    var dataBuffer = JsonConvert.SerializeObject(
        new
        {
            Wind = 1115,
            Humidity = 750,
            Precipitation = 15
        });
    var eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));
    // Add  key value to each message for routing.
    // key use for controller name, value for function to invoke 
    eventMessage.Properties.Add("Temp", "AddAsync");

    await deviceClient.SendEventAsync(eventMessage);


}