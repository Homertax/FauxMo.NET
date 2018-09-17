using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FauxMo.NET
{
    public class FauxDevice
    {
        public FauxDevice()
        {

        }

        public FauxDevice(string id, string name, int port, Action<bool> handler)
        {
            Id = id;
            Name = name;
            Port = port;
            Handler = handler;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }
        public Action<bool> Handler { get; set; }
    }
}
