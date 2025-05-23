using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    public class CostClass
    {
        [JsonIgnore]
        public string CodeSAP { get; set; }
        [JsonIgnore]
        public string idMaximo { get; set; }
        public string orgid { get; set; }
        public string siteid { get; set; }
        public string itemnum { get; set; }
        public string itemsetid { get; set; }
        public string location { get; set; }
        public string issueunit { get; set; }
        public string costtype { get; set; }
        public string category { get; set; }
        public string status { get; set; }
        public string sendersysid { get; set; }
        public List<InvCost> invcost { get; set; }
    }
    public class InvCost
    {
        public string avgcost { get; set; }
        public string conditioncode { get; set; }
        public string glaccount { get; set; }
    }
}
