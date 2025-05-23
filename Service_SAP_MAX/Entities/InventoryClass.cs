using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    public class InventoryClass
    {
        [JsonIgnore]
        public string CodeSAP { get; set; }
        [JsonIgnore]
        public string idMaximo { get; set; }
        [JsonIgnore]
        public string TypeSAP { get; set; }

        public string orgid { get; set; }
        public string siteid { get; set; }
        public string itemnum { get; set; }
        public string itemsetid { get; set; }
        public string itemtype { get; set; }
        public string location { get; set; }
        public string issueunit { get; set; }
        public string orderunit { get; set; }
        public string binnum { get; set; }
        public string category { get; set; }
        public string costtype { get; set; }
        public string glaccount { get; set; }
        public string sendersysid { get; set; }
        public string status { get; set; }
        public List<Invcost> invcost { get; set; }
        public List<Invbalance> invbalances { get; set; }
    }

    public class Invbalance
    {
        public string curbal { get; set; }
        public string binnum { get; set; }
        public string lotnum { get; set; }
        public string conditioncode { get; set; }
    }

    public class Invcost
    {
        public string avgcost { get; set; }
        public string conditioncode { get; set; }
        public string glaccount { get; set; }
        public string controlacc { get; set; }
        public string invcostadjacc { get; set; }
        public string shrinkageacc { get; set; }
    }

   


}
