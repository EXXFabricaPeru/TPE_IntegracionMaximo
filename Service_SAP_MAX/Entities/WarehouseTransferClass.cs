using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    
    public class WarehouseTransferClass
    {
        [JsonIgnore]
        public string CodeSAP { get; set; }
        [JsonIgnore]
        public string idMaximo { get; set; }

        public string description { get; set; }
        public string fromstoreloc { get; set; }
        public string orgid { get; set; }
        public string sendersysid { get; set; }
        public string siteid { get; set; }
        public string status { get; set; }
        public string usetype { get; set; }
        public List<Invuseline> invuseline { get; set; }

        public class Invuseline
        {
            public DateTime actualdate { get; set; }
            public string conversion { get; set; }
            public string description { get; set; }
            public string enterby { get; set; }
            public string frombin { get; set; }
            public string tobin { get; set; }
            public string fromstoreloc { get; set; }
            public string tostoreloc { get; set; }
            public string gldebitacct { get; set; }
            public string invuselinenum { get; set; }
            public string itemnum { get; set; }
            public string itemsetid { get; set; }
            public string linetype { get; set; }
            public string orgid { get; set; }
            public string siteid { get; set; }
            public string quantity { get; set; }
            public string refwo { get; set; }
            public string remark { get; set; }
            public string sendersysid { get; set; }
            public string usetype { get; set; }
        }

    }
}
