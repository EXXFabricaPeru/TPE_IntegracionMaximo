using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    public class ChartAccountsClass
    {
        [JsonIgnore]
        public string idMaximo { get; set; } 

        [JsonIgnore]
        public string codeSAP { get; set; }
        public bool active { get; set; }
        public string glaccount { get; set; }
        public DateTime activedate { get; set; }
        public string accountname { get; set; }
        public string glcomp01 { get; set; }
        public string glcomp02 { get; set; }
        public string glcomp03 { get; set; }
        public string glcomp04 { get; set; }
        public string orgid { get; set; }
        public string sendersysid { get; set; }
        public string sourcesysid { get; set; }
    }
}
