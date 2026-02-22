using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;

Console.WriteLine("Getting Device Id");
var deviceIdManager = new DeviceIdManager()
    .AddBuilder(1, builder => builder
        .OnWindows(windows => windows
            .AddMachineGuid()
            .AddProcessorId()
            .AddMotherboardSerialNumber()
            .AddSystemDriveSerialNumber())
        .OnLinux(linux => linux
            .AddMachineId()
            .AddCpuInfo()
            .AddMotherboardSerialNumber()
            .AddSystemDriveSerialNumber())
        //.OnMac(mac => mac
        //    .AddSystemDriveSerialNumber()
        //    .AddPlatformSerialNumber())
        .UseFormatter(new XmlDeviceIdFormatter(new PlainTextDeviceIdComponentEncoder())));


Console.WriteLine($"Device Id = {deviceIdManager.GetDeviceId()}");
