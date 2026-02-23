using System;
using System.Security.Cryptography;
using DeviceId.Components;
using DeviceId.Internal;

namespace DeviceId;

/// <summary>
/// Extension methods for <see cref="DeviceIdBuilder"/>.
/// </summary>
public static class DeviceIdBuilderExtensions
{
    /// <summary>
    /// Use the specified formatter.
    /// </summary>
    /// <param name="builder">The <see cref="DeviceIdBuilder"/> to use the formatter.</param>
    /// <param name="formatter">The <see cref="IDeviceIdFormatter"/> to use.</param>
    /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
    public static DeviceIdBuilder UseFormatter(this DeviceIdBuilder builder, IDeviceIdFormatter formatter)
    {
        builder.Formatter = formatter;
        return builder;
    }

    /// <summary>
    /// Adds the current user name to the device identifier.
    /// </summary>
    /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
    /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
    public static DeviceIdBuilder AddUserName(this DeviceIdBuilder builder)
    {
        // Default to false for backwards compatibility. May consider changing this to true in the next major version.

        return AddUserName(builder, false);
    }

    /// <summary>
    /// Adds the current user name to the device identifier.
    /// </summary>
    /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
    /// <param name="normalize">A value determining whether the user name should be normalized or not.</param>
    /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
    public static DeviceIdBuilder AddUserName(this DeviceIdBuilder builder, bool normalize)
    {
        var userName = normalize
            ? Environment.UserName?.ToLowerInvariant()
            : Environment.UserName;

        return builder.AddComponent("UserName", new DeviceIdComponent(userName));
    }

    /// <summary>
    /// Adds the machine name to the device identifier.
    /// </summary>
    /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
    /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
    public static DeviceIdBuilder AddMachineName(this DeviceIdBuilder builder)
    {
        return builder.AddComponent("MachineName", new DeviceIdComponent(Environment.MachineName));
    }

    /// <summary>
    /// Adds the operating system version to the device identifier.
    /// </summary>
    /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
    /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
    public static DeviceIdBuilder AddOsVersion(this DeviceIdBuilder builder)
    {
        return builder.AddComponent("OSVersion", new DeviceIdComponent(OS.Version));
    }

    /// <summary>
    /// Adds the MAC address to the device identifier, optionally excluding wireless adapters.
    /// </summary>
    /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
    /// <param name="excludeWireless">A value indicating whether wireless adapters should be excluded.</param>
    /// <param name="excludeDockerBridge">A value determining whether docker bridge should be excluded.</param>
    /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
    public static DeviceIdBuilder AddMacAddress(this DeviceIdBuilder builder, bool excludeWireless = false, bool excludeDockerBridge = false)
    {
        return builder.AddComponent("MACAddress", new MacAddressDeviceIdComponent(excludeWireless, excludeDockerBridge));
    }

    /// <summary>
    /// Adds a file-based token to the device identifier.
    /// </summary>
    /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
    /// <param name="path">The path of the token.</param>
    /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
    public static DeviceIdBuilder AddFileToken(this DeviceIdBuilder builder, string path)
    {
        // We used to use path.GetHashCode() here, but there's no guarantee that it's stable
        // across different runs of the application, so instead we can use a stable hash such
        // as SHA256 and take the first few characters.

        #if NET5_0_OR_GREATER
            var hash = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(path));
            var hashString = Convert.ToHexString(hash);
        #else
            using var hasher = SHA256.Create();
            var hash = hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(path));
            var hashString = BitConverter.ToString(hash).Replace("-", "");
        #endif

        var name = string.Concat("FileToken_", hashString);
        return builder.AddComponent(name, new FileTokenDeviceIdComponent(path));
    }
}
