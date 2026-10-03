namespace Envirotrax.App.Server.Data.Models.Backflow;

// BackflowTest.DeviceType is stored as the BackflowDeviceType name, so rules keyed on the device
// type compare against these names rather than the enum values.
public static class BackflowDeviceTypes
{
    // Detector assemblies carry a second (bypass) assembly with its own manufacturer, model, size
    // and serial number.
    public static bool HasBypassAssembly(string? deviceType)
    {
        return deviceType == nameof(BackflowDeviceType.DCD)
            || deviceType == nameof(BackflowDeviceType.DCD2)
            || deviceType == nameof(BackflowDeviceType.RPPD)
            || deviceType == nameof(BackflowDeviceType.RPPD2);
    }
}
