using System.Text;
using DeviceCommunicationLibrary;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace SimulatedDevice;

public class Publisher
{
    public async Task SendMessageAsync<TDevice, TPayload>(TDevice device, TPayload payload)
        where TDevice : IDevice
    {
        string deviceConnectionString = "connectionstring";
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);

        var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
        message.Properties.Add("DeviceId", device.DeviceId.ToString()); // Adding DeviceId as a property
        message.Properties.Add("Location", device.Location); // Adding Location as a property
        message.Properties.Add("Manufacturer", device.Manufacturer); // Adding Manufacturer as a property
        message.Properties.Add("Model", device.Model); // Adding Model as a property

        await deviceClient.SendEventAsync(message);

        Console.WriteLine("Message sent.");
    }
}