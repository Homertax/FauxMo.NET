using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FauxMo.NET
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args, new List<int>() { 49153, 11001 }).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, List<int> ports) =>
            WebHost.CreateDefaultBuilder(args)
            .UseUrls(string.Join(";", ports.Select(p => $"http://*:{p}")))
                .UseStartup<Startup>();
    }
}
