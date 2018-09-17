using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FauxMo.NET.Controllers
{
    [ApiController]
    public class SetupController : ControllerBase
    {
        private readonly DeviceService _deviceService;
        private readonly ILogger _logger;

        public SetupController(DeviceService deviceService, ILogger<SetupController> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        [HttpGet("/setup.xml")]
        public IActionResult Setup()
        {
            return Ok(_deviceService.GetDeviceSetup("221517K0101769"));
        }
    }
}