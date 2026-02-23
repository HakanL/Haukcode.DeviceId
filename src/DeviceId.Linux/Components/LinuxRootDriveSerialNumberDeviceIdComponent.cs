using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using DeviceId.CommandExecutors;
using DeviceId.Linux.Serialization;

namespace DeviceId.Linux.Components;

/// <summary>
/// An implementation of <see cref="IDeviceIdComponent"/> that uses the root drive's serial number.
/// </summary>
public class LinuxRootDriveSerialNumberDeviceIdComponent : IDeviceIdComponent
{
    /// <summary>
    /// Command executor.
    /// </summary>
    private readonly ICommandExecutor _commandExecutor;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinuxRootDriveSerialNumberDeviceIdComponent"/> class.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This constructor is obsolete and will be removed in a future version. Use the constructor that accepts an ICommandExecutor instead.")]
    public LinuxRootDriveSerialNumberDeviceIdComponent()
        : this(CommandExecutor.Bash) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinuxRootDriveSerialNumberDeviceIdComponent"/> class.
    /// </summary>
    /// <param name="commandExecutor">The command executor to use.</param>
    public LinuxRootDriveSerialNumberDeviceIdComponent(ICommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    /// <summary>
    /// Gets the component value.
    /// </summary>
    /// <returns>The component value.</returns>
    public string GetValue()
    {
        try
        {
            var outputJson = _commandExecutor.Execute("lsblk -f -J -o Name,MountPoint");
            if (string.IsNullOrEmpty(outputJson))
            {
                return null;
            }

            LsblkOutput output = null;
            try
            {
                output = JsonSerializer.Deserialize(outputJson, SourceGenerationContext.Default.LsblkOutput);
            }
            catch
            {
                return null;
            }

            if (output == null || output.BlockDevices == null || output.BlockDevices.Count == 0)
            {
                return null;
            }

            var device = FindRootParent(output);
            if (device == null)
            {
                return null;
            }

            var udevInfo = _commandExecutor.Execute($"udevadm info --query=all --name=/dev/{device.Name} | grep ID_SERIAL=");
            if (string.IsNullOrEmpty(udevInfo))
            {
                return null;
            }

            var components = udevInfo.Split('=');
            if (components.Length < 2)
            {
                return null;
            }

            return components[1];
        }
        catch
        {
            return null;
        }
    }

    private static LsblkDevice FindRootParent(LsblkOutput devices)
    {
        return devices.BlockDevices.FirstOrDefault(x => DeviceContainsRoot(x));
    }

    private static bool DeviceContainsRoot(LsblkDevice device)
    {
        if (device.MountPoint == "/")
        {
            return true;
        }

        if (device.Children == null || device.Children.Count == 0)
        {
            return false;
        }

        return device.Children.Any(x => DeviceContainsRoot(x));
    }
}
