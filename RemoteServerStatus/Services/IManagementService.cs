using System.Collections.Generic;

namespace RemoteServerStatus.Services
{
    public interface IManagementService
    {
        List<WindowsService> FetchServiceData();
        List<StorageDetails> FetchStorageData();
        void StartServiceOnServer(string service, string server);
        void StopServiceOnServer(string service, string server);
    }
}