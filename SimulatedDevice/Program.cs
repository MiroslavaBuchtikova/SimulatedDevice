using SimulatedDevice;
using DeviceCommunicationLibrary;
using DeviceCommunicationLibrary.MessagePayloads;

var publisher = new Publisher();

var deviceInfo = new DeviceInfo
{
    DeviceId = Guid.NewGuid(),
    Location = "BA main office",
    Manufacturer = "Raspberry",
    Model = "Raspberry Pi 4"
};

var magneticPayload = new WindowSensorPayload()
{
    IsWindowOpen = true,
    BuildingName = "Building A",   
    FloorNumber = 4,
    WindowName = "Window 101",
    OfficeName = "Office 201"
};

await publisher.SendMessageAsync(deviceInfo, magneticPayload);