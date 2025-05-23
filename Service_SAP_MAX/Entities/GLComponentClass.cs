using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    class GLComponentClass
    {
        [JsonIgnore]
        public string codeSAP { get; set; } 
        [JsonIgnore]
        public string idMaximo { get; set; }
        public bool active { get; set; }
        public string compvalue { get; set; }
        public string comptext { get; set; }
        public int glorder { get; set; }
        public string orgid { get; set; }
        public string userid { get; set; }
        public string sourcesysid { get; set; }
    }
}
