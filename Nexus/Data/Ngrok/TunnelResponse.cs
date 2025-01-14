using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Data.Ngrok
{
    public class TunnelResponse
    {
        [JsonProperty("tunnels")]
        public List<Tunnel>? Tunnels { get; set; }
    }
}
