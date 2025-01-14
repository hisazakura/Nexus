using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Data.Ngrok
{
    public class Tunnel
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("public_url")]
        public string? PublicUrl { get; set; }
    }
}
