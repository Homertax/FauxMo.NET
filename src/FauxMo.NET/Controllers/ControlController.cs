using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FauxMo.NET.Controllers
{
    [ApiController]
    public class ControlController : ControllerBase
    {
        private readonly DeviceService _deviceService;
        private readonly ILogger _logger;

        public ControlController(DeviceService deviceService, ILogger<ControlController> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        [HttpPost("/upnp/control/basicevent1")]
        public async Task<IActionResult> PostEvent()
        {
            string hostHeader = Request.Headers["Host"].FirstOrDefault();
            int port = int.Parse(hostHeader.Split(':')[1]);
            string xml = null;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                xml = await reader.ReadToEndAsync();
            }

            var device = _deviceService.Devices.Where(x => x.Port == port).FirstOrDefault();
            bool state = false;

            if (device == null)
            {
                return NotFound();
            }

            if (xml == null)
            {
                return BadRequest();
            }

            if (xml.IndexOf("<BinaryState>1</BinaryState>") > 0)
            {
                state = true;
                _logger.LogInformation($"{device.Name} - {state}");
            }
            else if (xml.IndexOf("<BinaryState>0</BinaryState>") > 0)
            {
                state = false;
                _logger.LogInformation($"{device.Name} - {state}");
            }


            device.Handler?.Invoke(state);

            return Ok(@"<s:Envelope xmlns:s="""" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><s:Body>
<u:SetBinaryStateResponse xmlns:u=""urn:Belkin:service:basicevent:1"">
<CountdownEndTime>0</CountdownEndTime>
</u:SetBinaryStateResponse>
</s:Body> </s:Envelope>");
        }
    }
}