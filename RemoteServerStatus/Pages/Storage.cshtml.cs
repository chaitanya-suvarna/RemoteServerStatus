using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RemoteServerStatus.Services;
using System.Collections.Generic;

namespace RemoteServerStatus.Pages
{
    public class StorageModel : PageModel
    {
        private readonly ILogger<StorageModel> _logger;
        public List<StorageDetails> storageDetailsList;
        private readonly IManagementService _managementService;

        public StorageModel(ILogger<StorageModel> logger, IManagementService managementService)
        {
            _logger = logger;
            storageDetailsList = new List<StorageDetails>();
            _managementService = managementService;
        }

        public void OnGet()
        {
            storageDetailsList = _managementService.FetchStorageData();
        }
    }
}