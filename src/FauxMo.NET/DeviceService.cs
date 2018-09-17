using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FauxMo.NET
{
    public class DeviceService
    {
        private readonly DeviceSettings _deviceSettings;
        private readonly ILogger _logger;

        public DeviceService(IOptionsSnapshot<DeviceSettings> deviceSettings, ILogger<DeviceService> logger)
        {
            _deviceSettings = deviceSettings.Value;
            _logger = logger;
        }

        public List<FauxDevice> Devices => _deviceSettings.Devices.ToList();

        public string GetDeviceSetup(string deviceId)
        {
            string response = @"<?xml version=""1.0""?><root>";

            if (deviceId == null)
            {
                Devices.ForEach(d =>
                {
                    response += $@"<device>
                            <deviceType>urn:Fauxmo:device:controllee:1</deviceType>
                            <friendlyName>${d.Name}</friendlyName>
                            <manufacturer>Belkin International Inc.</manufacturer>
                            <modelName>Emulated Socket</modelName>
                            <modelNumber>3.1415</modelNumber>
                            <UDN>uuid:Socket-1_0-${d.Id}</UDN>
                            </device>";
                });
            }
            else
            {
                var device = Devices.Where(x => x.Id == deviceId).FirstOrDefault();
                response += $@"<device>
                            <deviceType>urn:Fauxmo:device:controllee:1</deviceType>
                            <friendlyName>${device.Name}</friendlyName>
                            <manufacturer>Belkin International Inc.</manufacturer>
                            <modelName>Emulated Socket</modelName>
                            <modelNumber>3.1415</modelNumber>
                            <UDN>uuid:Socket-1_0-${device.Id}</UDN>
                            </device>";
            }
            response += "</root>";
            return response;
        }
    }
}
