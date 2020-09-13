using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace RemoteServerStatus.Services
{
    public class ManagementService : IManagementService
    {
        private readonly ILogger<ManagementService> _logger;
        private readonly IConfiguration _config;
        private List<WindowsService> windowsServiceList;
        private List<StorageDetails> storageDetailsList;
        private ConnectionOptions connection;
        private ManagementScope scope;
        private List<string> serverNames;
        private List<string> serviceDisplayNames;

        public ManagementService(ILogger<ManagementService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            windowsServiceList = new List<WindowsService>();
            storageDetailsList = new List<StorageDetails>();
            InitializeConnection();
            serverNames = config.GetSection("ServerNames").Get<string[]>().ToList();
            serviceDisplayNames = config.GetSection("ServiceDisplayNames").Get<string[]>()?.ToList();
        }

        public List<WindowsService> FetchServiceData()
        {
            foreach (string servername in serverNames)
            {
                FetchServiceDataForServer(servername);
            }

            return windowsServiceList;
        }

        public void StartServiceOnServer(string service, string server)
        {
            scope = new ManagementScope($"\\\\{server}\\root\\CIMV2", connection);
            scope.Connect();

            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Service where DisplayName= '" + service + "'");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                ManagementObjectCollection collection = searcher.Get();
                foreach (ManagementObject myservice in collection)
                {
                    myservice.InvokeMethod("StartService", null);
                }
            }
        }

        public void StopServiceOnServer(string service, string server)
        {
            scope = new ManagementScope($"\\\\{server}\\root\\CIMV2", connection);
            scope.Connect();

            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Service where DisplayName= '" + service + "'");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                ManagementObjectCollection collection = searcher.Get();
                foreach (ManagementObject myservice in collection)
                {
                    myservice.InvokeMethod("StopService", null);
                }
            }
        }

        private void FetchServiceDataForServer(string serverName)
        {
            scope = new ManagementScope($"\\\\{serverName}\\root\\CIMV2", connection);
            scope.Connect();

            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Service");

            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher(scope, query);

            foreach (ManagementObject managementObject in searcher.Get())
            {
                if (serviceDisplayNames == null)
                {
                    windowsServiceList.Add(
                    new WindowsService()
                    {
                        ServerName = scope.Path.Server,
                        ServiceName = managementObject["DisplayName"].ToString(),
                        ServiceStatus = managementObject["State"].ToString()
                    });
                }
                else if (serviceDisplayNames.Contains(managementObject["DisplayName"].ToString()))
                {
                    windowsServiceList.Add(
                    new WindowsService()
                    {
                        ServerName = scope.Path.Server,
                        ServiceName = managementObject["DisplayName"].ToString(),
                        ServiceStatus = managementObject["State"].ToString()
                    });
                }
            }
        }

        public List<StorageDetails> FetchStorageData()
        {
            foreach (string servername in serverNames)
            {
                scope = new ManagementScope($"\\\\{servername}\\root\\CIMV2", connection);
                scope.Connect();

                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_LogicalDisk");

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                foreach (ManagementObject managementObject in searcher.Get())
                {
                    var size = managementObject["Size"] != null ? managementObject["Size"].ToString() : "0";
                    var freeSpace = managementObject["FreeSpace"] != null ? managementObject["FreeSpace"].ToString() : "0";

                    var diskSize = Math.Round(double.Parse(size) / (1024 * 1024 * 1024), 2);
                    var diskFreespace = Math.Round(double.Parse(freeSpace) / (1024 * 1024 * 1024), 2);

                    storageDetailsList.Add(
                        new StorageDetails()
                        {
                            ServerName = scope.Path.Server,
                            DriveName = managementObject["Name"]?.ToString(),
                            DriveLabel = managementObject["VolumeName"]?.ToString(),
                            DiskSize = diskSize,
                            DiskFreeSpace = diskFreespace,
                            DiskUsedSpace = Math.Round(diskSize - diskFreespace, 2),
                            FreeSpacePercentage = Math.Round(diskFreespace / diskSize * 100, 2)
                        });
                }
            }

            return storageDetailsList;
        }

        private void InitializeConnection()
        {
            connection = new ConnectionOptions();
            connection.Username = _config.GetValue<string>("Credential:Username");
            connection.Password = _config.GetValue<string>("Credential:Password");
            connection.Authority = _config.GetValue<string>("Credential:Authority");
            connection.EnablePrivileges = true;
            connection.Authentication = AuthenticationLevel.Default;
            connection.Impersonation = ImpersonationLevel.Impersonate;
        }
    }

    public class WindowsService
    {
        public string ServerName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceStatus { get; set; }
    }

    public class StorageDetails
    {
        public string ServerName { get; set; }
        public string DriveName { get; set; }
        public string DriveLabel { get; set; }
        public double DiskSize { get; set; }
        public double DiskUsedSpace { get; set; }
        public double DiskFreeSpace { get; set; }
        public double FreeSpacePercentage { get; set; }
    }
}