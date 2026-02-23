using System;
using System.ComponentModel;
using DeviceId.CommandExecutors;
using DeviceId.Components;

namespace DeviceId;

/// <summary>
/// Extension methods for <see cref="MacDeviceIdBuilder"/>.
/// </summary>
public static class MacDeviceIdBuilderExtensions
{
    /// <summary>
    /// Adds the system drive serial number to the device identifier.
    /// </summary>
    /// <param name="builder">The <see cref="MacDeviceIdBuilder"/> to add the component to.</param>
    /// <returns>The <see cref="MacDeviceIdBuilder"/> instance.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This never really worked properly. Use AddSystemDriveVolumeUUID instead.")]
    public static MacDeviceIdBuilder AddSystemDriveSerialNumber(this MacDeviceIdBuilder builder)
    {
        return AddSystemDriveSerialNumber(builder, CommandExecutor.Bash);
    }

    /// <summary>
    /// Adds the system drive serial number to the device identifier.
    /// </summary>
    /// <param name="builder">The <see cref="MacDeviceIdBuilder"/> to add the component to.</param>
    /// <param name="commandExecutor">The command executor to use.</param>
    /// <returns>The <see cref="MacDeviceIdBuilder"/> instance.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This never really worked properly. Use AddSystemDriveVolumeUUID instead.")]
    public static MacDeviceIdBuilder AddSystemDriveSerialNumber(this MacDeviceIdBuilder builder, ICommandExecutor commandExecutor)
    {
        return builder.AddComponent("SystemDriveSerialNumber", new CommandComponent("system_profiler SPSerialATADataType | sed -En 's/.*Serial Number: ([\\d\\w]*)//p'", commandExecutor));
    }

    /// <summary>
    /// Adds the platform serial number to the device identifier.
    /// </summary>
    /// <param name="builder">The <see cref="MacDeviceIdBuilder"/> to add the component to.</param>
    /// <returns>The <see cref="MacDeviceIdBuilder"/> instance.</returns>
    public static MacDeviceIdBuilder AddPlatformSerialNumber(this MacDeviceIdBuilder builder)
    {
        return AddPlatformSerialNumber(builder, CommandExecutor.Bash);
    }

    /// <summary>
    /// Adds the platform serial number to the device identifier.
    /// </summary>
    /// <param name="builder">The <see cref="MacDeviceIdBuilder"/> to add the component to.</param>
    /// <param name="commandExecutor">The command executor to use.</param>
    /// <returns>The <see cref="MacDeviceIdBuilder"/> instance.</returns>
    public static MacDeviceIdBuilder AddPlatformSerialNumber(this MacDeviceIdBuilder builder, ICommandExecutor commandExecutor)
    {
        return builder.AddComponent("IOPlatformSerialNumber", new CommandComponent("ioreg -l | grep IOPlatformSerialNumber | sed 's/.*= //' | sed 's/\"//g'", commandExecutor));
    }

    /// <summary>
    /// Adds the system drive Volume UUID to the device identifier.
    /// </summary>
    /// <param name="builder">The <see cref="MacDeviceIdBuilder"/> to add the component to.</param>
    /// <returns>The <see cref="MacDeviceIdBuilder"/> instance.</returns>
    public static MacDeviceIdBuilder AddSystemDriveVolumeUUID(this MacDeviceIdBuilder builder)
    {
        return AddSystemDriveVolumeUUID(builder, CommandExecutor.Bash);
    }

    /// <summary>
    /// Adds the system drive Volume UUID to the device identifier.
    /// </summary>
    /// <param name="builder">The <see cref="MacDeviceIdBuilder"/> to add the component to.</param>
    /// <param name="commandExecutor">The command executor to use.</param>
    /// <returns>The <see cref="MacDeviceIdBuilder"/> instance.</returns>
    public static MacDeviceIdBuilder AddSystemDriveVolumeUUID(this MacDeviceIdBuilder builder, ICommandExecutor commandExecutor)
    {
        return builder.AddComponent("SystemDriveVolumeUUID", new CommandComponent("diskutil info / | sed -n 's/.*Volume UUID: *//p'", commandExecutor));
    }
}
