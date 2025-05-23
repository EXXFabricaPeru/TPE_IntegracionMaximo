using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    public class PurchaseOrderClass
    {
        [JsonIgnore]
        public string CodeSAP { get; set; }
        [JsonIgnore]
        public string idMaximo { get; set; }

        public string orgid { get; set; }
        public string siteid { get; set; }
        public string currencycode { get; set; }
        public string description { get; set; }
        public string ponum { get; set; }
        public string potype { get; set; }
        public string purchaseagent { get; set; }
        public string revisionnum { get; set; }
        public string priority { get; set; }
        public string sendersysid { get; set; }
        public string shipto { get; set; }
        public string billto { get; set; }
        public string status { get; set; }
        public string vendor { get; set; }
        public List<PurchaseOrderline> poline { get; set; }
    }

    public class PurchaseOrderline
    {
        public string polinenum { get; set; }
        public string linetype { get; set; }
        public string itemnum { get; set; }
        public string itemsetid { get; set; }
        public string storeloc { get; set; }
        public string conversion { get; set; }
        public string orderqty { get; set; }
        public string ref_prnum { get; set; }
        public string ref_prlinenum { get; set; }
        public string refwo { get; set; }
        public string unitcost { get; set; }
        public string orderunit { get; set; }
        public string description { get; set; }
        public string gldebitacct { get; set; }
        public string requestedby { get; set; }
        public string tax1 { get; set; }
        public string tax1code { get; set; }
        public string remark { get; set; }
    }

}
