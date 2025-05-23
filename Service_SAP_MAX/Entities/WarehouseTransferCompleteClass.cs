using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    public class WarehouseTransferCompleteClass
    {

        [JsonIgnore]
        public string CodeSAP { get; set; }
        [JsonIgnore]
        public string idMaximo { get; set; }

        public string orgid { get; set; }
        public string siteid { get; set; }
        public string invusenum { get; set; }
        public string status { get; set; }
        public string usetype { get; set; }
    }
}
