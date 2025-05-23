using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    public class BalanceClass
    {
        [JsonIgnore]
        public string CodeSAP { get; set; }
        [JsonIgnore]
        public string idMaximo { get; set; }
        public string binnum { get; set; }
        public string curbal { get; set; }
        public string itemnum { get; set; }
        public string itemsetid { get; set; }
        public string location { get; set; }
        public string orgid { get; set; }
        public string sendersysid { get; set; }
        public string siteid { get; set; }
    }
}
