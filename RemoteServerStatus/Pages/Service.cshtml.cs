using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RemoteServerStatus.Services;

namespace RemoteServerStatus.Pages
{
    public class ServiceModel : PageModel
    {
        private readonly ILogger<ServiceModel> _logger;
        private List<WindowsService> windowsServiceList;
        private readonly IManagementService _managementService;

        public ServiceModel(ILogger<ServiceModel> logger, IManagementService managementService)
        {
            _managementService = managementService;
            _logger = logger;
            windowsServiceList = new List<WindowsService>();
        }

        public void OnGet()
        {
            windowsServiceList = _managementService.FetchServiceData();
        }

        public IActionResult OnPostStartService(string service, string server)
        {
            _managementService.StartServiceOnServer(service, server);
            return RedirectToPage("./Service");
        }

        public IActionResult OnPostStopService(string service, string server)
        {
            _managementService.StopServiceOnServer(service, server);
            return RedirectToPage("./Service");
        }
    }
}