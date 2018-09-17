using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FauxMo.NET
{
    public class SsdpService : IHostedService, IDisposable
    {
        private readonly UdpClient _udpServer;
        private readonly IPAddress _ipAddress;
        private readonly ILogger _logger;

        public SsdpService(IServiceProvider services, ILogger<SsdpService> logger)
        {
            _logger = logger;
            Services = services;
            _ipAddress = IPAddress.Parse("192.168.0.21");// GetLocalIP();
            IPEndPoint localEp = new IPEndPoint(_ipAddress, 1900);
            _udpServer = new UdpClient(1900, AddressFamily.InterNetwork);

            IPAddress multicastaddress = IPAddress.Parse("239.255.255.250");
            _udpServer.JoinMulticastGroup(multicastaddress, _ipAddress);
        }

        public IServiceProvider Services { get; }

        private IPAddress GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("Cannot find network adapter with IPv4 address");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _udpServer.ReceiveAsync();
                string message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length);
                if (message.IndexOf("ssdp:discover") > 0)
                {
                    GetDiscoveryResponses().ForEach(async s =>
                    {
                        _logger.LogInformation(result.RemoteEndPoint.ToString());
                        var datagram = Encoding.ASCII.GetBytes(s);
                        _logger.LogInformation(s);
                        await _udpServer.SendAsync(datagram, datagram.Length, result.RemoteEndPoint);
                    });
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _udpServer?.Close();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _udpServer?.Dispose();
        }

        private List<string> GetDiscoveryResponses()
        {
            var responses = new List<string>();

            using (var scope = Services.CreateScope())
            {
                var deviceService =
                    scope.ServiceProvider
                        .GetRequiredService<DeviceService>();

                deviceService.Devices.ForEach(d =>
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("HTTP/1.1 200 OK");
                    sb.AppendLine("CACHE-CONTROL: max-age=86400");
                    sb.AppendLine("DATE: Fri, 15 Apr 2016 04:56:29 GMT");
                    sb.AppendLine("EXT:");
                    sb.AppendLine($"LOCATION: http://{_ipAddress}:{d.Port}/setup.xml");
                    sb.AppendLine(@"OPT: ""http://schemas.upnp.org/upnp/1/0/""; ns=01");
                    sb.AppendLine("01-NLS: b9200ebb-736d-4b93-bf03-835149d13983");
                    sb.AppendLine("SERVER: Unspecified, UPnP/1.0, Unspecified");
                    sb.AppendLine("ST: urn:Belkin:device:**");
                    sb.AppendLine("USN: uuid:Socket-1_0-{d.Id}::urn:Belkin:device:**");
                    sb.AppendLine("");
                    sb.AppendLine("");
                    responses.Add(sb.ToString());
                });

                return responses;
            }
        }
    }
}
