using SimulatedDevice;


var ioTController = new IoTController();

while (true)
{
    Console.WriteLine("Choose an option:");
    Console.WriteLine("1. Create Device");
    Console.WriteLine("2. Setup New Configuration");
    Console.WriteLine("3. Get devices");
    Console.WriteLine("4. Send Message to Device");
    Console.WriteLine("4. Exit");

    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Write("Enter device name: ");
            await ioTController.CreateNewDevice(Console.ReadLine());
            break;
        case "2":
            Console.Write("Enter device name: ");
            await ioTController.SetupDefaultDesiredProperties(Console.ReadLine());
            break;
        case "3":
            await ioTController.GetDevices();
            return;
        case "4":
            Console.Write("Enter device name: ");
            await ioTController.SendMessageAsync(Console.ReadLine());
            return;
        default:
            Console.WriteLine("Invalid choice. Please select a valid option.");
            break;
    }

    Console.WriteLine();
}
