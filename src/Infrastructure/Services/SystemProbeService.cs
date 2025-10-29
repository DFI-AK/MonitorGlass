using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using MonitorGlass.Application.Common.Interfaces;
using MonitorGlass.Domain.Entities;
using System.Diagnostics;

namespace MonitorGlass.Infrastructure.Services;

[SupportedOSPlatform("windows")]
internal sealed class SystemProbeService(ILogger<SystemProbeService> logger) : ISystemProbeService
{
    private readonly ILogger<SystemProbeService> _logger = logger;
    public async Task<bool> CheckServerAvailabilityAsync(string hostName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking availability for host: {HostName}", hostName);

        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(hostName, 1000);

            if (reply.Status == IPStatus.Success)
            {
                _logger.LogInformation("Host {HostName} is reachable", hostName);
            }
            else
            {
                _logger.LogWarning("Host {HostName} is not reachable. Status: {Status}", hostName, reply.Status);
            }

            return reply.Status == IPStatus.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking availability for host: {HostName}", hostName);
            return false;
        }
    }

    public async Task<SystemMetric> CollectSystemMetricsAsync(string hostName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Collecting system metrics for {MachineName}", hostName);

        return await Task.Run(() =>
        {
            var cpuDetail = GetCpuDetail();
            var memoryDetail = GetMemoryDetail();
            var diskDetails = GetDiskDetails();
            var networkDetails = GetNetworkDetails();

            var metric = new SystemMetric
            {
                Id = Guid.CreateVersion7(DateTimeOffset.UtcNow),
                SystemInfoId = Guid.Empty, // Will be set by service/handler before saving
                SystemInfo = null!,
                CpuDetail = cpuDetail,
                MemoryDetail = memoryDetail,
                DiskDetails = diskDetails,
                NetworkDetails = networkDetails,
            };

            _logger.LogInformation("Metrics collected for {MachineName}: {CpuCores} cores, {UsedMem}MB used, {DiskCount} disks, {NicCount} NICs",
                hostName, cpuDetail.CpuCores, memoryDetail.UsedMemoryMB, diskDetails.Count, networkDetails.Count);

            return metric;
        }, cancellationToken);
    }

    public async Task<string> GetLocalOperatingSystemAsync(string hostName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving operating system for host: {HostName}", hostName);

        try
        {
            return await Task.Run(() =>
            {
                var query = new System.Management.ManagementObjectSearcher(
                    $"SELECT Caption FROM Win32_OperatingSystem WHERE CSName = '{hostName}'");

                var results = query.Get();
                foreach (System.Management.ManagementObject os in results)
                {
                    var osName = os["Caption"]?.ToString() ?? "Unknown";
                    _logger.LogInformation("Operating system for host {HostName} is {OSName}", hostName, osName);
                    return osName;
                }

                _logger.LogWarning("No operating system information found for host: {HostName}", hostName);
                return "Unknown";
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving operating system for host: {HostName}", hostName);
            return "Unknown";
        }
    }

    public async Task<string> GetOperatingSystemRemoteAsync(string hostName, string username, string password, CancellationToken cancellationToken = default)
    {

        _logger.LogInformation("Retrieving operating system for remote host: {HostName}", hostName);

        try
        {
            return await Task.Run(() =>
            {
                var options = new ConnectionOptions
                {
                    Username = username,
                    Password = password,
                    Impersonation = ImpersonationLevel.Impersonate,
                    Authentication = AuthenticationLevel.PacketPrivacy,
                    EnablePrivileges = true
                };

                var scope = new ManagementScope($"\\\\{hostName}\\root\\cimv2", options);
                scope.Connect();

                var query = new ObjectQuery("SELECT Caption FROM Win32_OperatingSystem");
                var searcher = new ManagementObjectSearcher(scope, query);
                var results = searcher.Get();

                foreach (ManagementObject os in results)
                {
                    var osName = os["Caption"]?.ToString() ?? "Unknown";
                    _logger.LogInformation("Operating system for host {HostName} is {OSName}", hostName, osName);
                    return osName;
                }

                _logger.LogWarning("No operating system information found for host: {HostName}", hostName);
                return "Unknown";
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving operating system for host: {HostName}", hostName);
            return "Unknown";
        }

    }

    private static CpuDetail GetCpuDetail()
    {
        using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        _ = cpuCounter.NextValue(); // warm-up
        Thread.Sleep(1000);

        var usage = Math.Round(cpuCounter.NextValue());

        return new CpuDetail
        {
            CpuCores = Environment.ProcessorCount,
            CpuCoreUsage = usage,
            CpuProcessCount = Process.GetProcesses().Length,
            CpuThreadCount = Process.GetProcesses().Sum(p => p.Threads.Count)
        };
    }

    private static MemoryDetail GetMemoryDetail()
    {
        var gcMemory = GC.GetGCMemoryInfo();
        var total = gcMemory.TotalAvailableMemoryBytes / (1024.0 * 1024.0);

        // Windows-specific PerformanceCounter still used for available MB
        using var availableCounter = new PerformanceCounter("Memory", "Available MBytes");
        var available = availableCounter.NextValue();

        var used = total - available;

        return new MemoryDetail
        {
            TotalMemoryMB = Math.Round(total, 2),
            UsedMemoryMB = Math.Round(used, 2),
            AvailableMemoryMB = Math.Round(available, 2)
        };
    }


    private static ICollection<DiskDetail> GetDiskDetails()
    {
        var disks = new List<DiskDetail>();

        try
        {
            var category = new PerformanceCounterCategory("PhysicalDisk");
            var instanceNames = category.GetInstanceNames()
                .Where(n => !string.Equals(n, "_Total", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (var instanceName in instanceNames)
            {
                // Match instance (e.g. "0 C:") to actual drive info if possible
                var driveLetter = instanceName.Split(' ').LastOrDefault()?.TrimEnd(':');

                DriveInfo? driveInfo = null;
                if (!string.IsNullOrWhiteSpace(driveLetter))
                {
                    driveInfo = DriveInfo.GetDrives()
                        .FirstOrDefault(d => d.IsReady && d.Name.StartsWith(driveLetter, StringComparison.OrdinalIgnoreCase));
                }

                using var readCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", instanceName, true);
                using var writeCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", instanceName, true);
                using var iopsCounter = new PerformanceCounter("PhysicalDisk", "Disk Transfers/sec", instanceName, true);

                // Warm-up (first call always returns 0)
                _ = readCounter.NextValue();
                _ = writeCounter.NextValue();
                _ = iopsCounter.NextValue();

                Thread.Sleep(1000); // Small delay to get accurate metrics

                var readBytesPerSec = readCounter.NextValue();
                var writeBytesPerSec = writeCounter.NextValue();
                var iops = iopsCounter.NextValue();

                disks.Add(new DiskDetail
                {
                    DriveLetter = driveInfo?.Name ?? instanceName,
                    DiskTotalSpaceGB = driveInfo != null
                        ? Math.Round(driveInfo.TotalSize / (1024.0 * 1024.0 * 1024.0), 2)
                        : null,
                    DiskFreeSpaceGB = driveInfo != null
                        ? Math.Round(driveInfo.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0), 2)
                        : null,
                    DiskReadSpeedMBps = Math.Round(readBytesPerSec / (1024.0 * 1024.0), 2),
                    DiskWriteSpeedMBps = Math.Round(writeBytesPerSec / (1024.0 * 1024.0), 2),
                    DiskIOPS = Convert.ToInt32(iops)
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DiskMonitor] Error collecting disk metrics: {ex.Message}");
        }

        return disks;
    }

    private static ICollection<NetworkDetail> GetNetworkDetails()
    {
        var list = new List<NetworkDetail>();

        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            var properties = nic.GetIPProperties();
            var ipv4 = properties.UnicastAddresses
                .FirstOrDefault(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.Address.ToString();

            var ipv6 = properties.UnicastAddresses
                .FirstOrDefault(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)?.Address.ToString();

            var stats = nic.GetIPv4Statistics();

            list.Add(new NetworkDetail
            {
                InterfaceName = nic.Name,
                Description = nic.Description ?? string.Empty,
                MACAddress = nic.GetPhysicalAddress().ToString() ?? string.Empty,
                IPv4Address = ipv4 ?? string.Empty,
                IPv6Address = ipv6,
                IsUp = nic.OperationalStatus == OperationalStatus.Up,
                SpeedMbps = nic.Speed > 0 ? nic.Speed / 1_000_000 : 0,
                BytesSent = stats.BytesSent,
                BytesReceived = stats.BytesReceived
            });
        }

        return list;
    }
}
