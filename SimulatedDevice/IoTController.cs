using System.Text;
using DeviceCommunicationLibrary;
using DeviceCommunicationLibrary.MessagePayloads;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Message = Microsoft.Azure.Devices.Client.Message;

namespace SimulatedDevice;

public class IoTController
{
    private const string ConnectionString = "connectionString";

    public async Task SendMessageAsync(string targetDeviceId)
    {
        var deviceInfo = CreateMockedMessage(out var payload);
        await SendMessage(targetDeviceId, deviceInfo, payload);
    }

    private static DeviceInfo CreateMockedMessage(out WindowSensorPayload payload)
    {
        var deviceInfo = new DeviceInfo
        {
            DeviceId = Guid.NewGuid(),
            Location = "BA main office",
            Manufacturer = "Raspberry",
            Model = "Raspberry Pi 4"
        };
        payload = new WindowSensorPayload()
        {
            IsWindowOpen = true,
            BuildingName = "Building A",
            FloorNumber = 4,
            WindowName = "Window 101",
            OfficeName = "Office 201"
        };
        return deviceInfo;
    }

    public async Task SetupDefaultDesiredProperties(string targetDeviceId)
    {
        var sensors = new[]
        {
            new
            {
                sensorId = "sensor1",
                type = "temperature",
                threshold = 25
            },
            new
            {
                sensorId = "sensor2",
                type = "humidity",
                threshold = 70
            }
        };

        var twinProperties = new TwinCollection
        {
            ["sensors"] = sensors
        };

        var registryManager = RegistryManager.CreateFromConnectionString(ConnectionString);

        var twin = await registryManager.GetTwinAsync(targetDeviceId);

        Console.WriteLine("Desired properties:" + twin.Properties.Desired.ToJson());

        twin.Properties.Desired = twinProperties;

        await registryManager.UpdateTwinAsync(targetDeviceId, twin, "*");

        Console.WriteLine("New desired properties:" + twin.Properties.Desired.ToJson());
    }

    private async Task SendMessage<TDevice, TPayload>(string targetDeviceId, TDevice device, TPayload payload)
        where TDevice : IDevice
    {
        var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
        message.Properties.Add("DeviceId", device.DeviceId.ToString()); // Adding DeviceId as a property
        message.Properties.Add("Location", device.Location); // Adding Location as a property
        message.Properties.Add("Manufacturer", device.Manufacturer); // Adding Manufacturer as a property
        message.Properties.Add("Model", device.Model); // Adding Model as a property

        var deviceClient = DeviceClient.CreateFromConnectionString(ConnectionString, targetDeviceId,
            Microsoft.Azure.Devices.Client.TransportType.Mqtt);
        await deviceClient.SendEventAsync(message);

        Console.WriteLine("Message sent.");
    }

    public async Task CreateNewDevice(string targetDeviceId)
    {
        try
        {
            var registryManager = RegistryManager.CreateFromConnectionString(ConnectionString);
            await registryManager.AddDeviceAsync(new Device(targetDeviceId));

            Console.WriteLine($"Device '{targetDeviceId}' created successfully.");
        }
        catch (DeviceAlreadyExistsException)
        {
            Console.WriteLine($"Device '{targetDeviceId}' already exists.");
        }
    }

    public async Task GetDevices()
    {
        var registryManager = RegistryManager.CreateFromConnectionString(ConnectionString);
        var devices = await registryManager.CreateQuery("select * from devices", 100).GetNextAsTwinAsync();
        foreach (var device in devices)
        {
            Console.WriteLine($"Device: {device.DeviceId}");
        }
    }
}