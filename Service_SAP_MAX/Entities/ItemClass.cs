using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    public class ItemClass
    {
        [JsonIgnore]
        public string ItemCode { get; set; }
        [JsonIgnore]
        public string idMaximo { get; set; }
        public string commodity { get; set; }
        public string commoditygroup { get; set; }
        public string itc_tipo { get; set; }
        public string itc_rubro { get; set; }
        public string description { get; set; }
        public string issueunit { get; set; }
        public string itemnum { get; set; }
        public string itemsetid { get; set; }
        public string itemtype { get; set; }
        public string lottype { get; set; }
        public string orderunit { get; set; }
        public int rotating { get; set; }
        public string sendersysid { get; set; }
        public string status { get; set; }
        public List<Itemorginfo> itemorginfo { get; set; }
    }

    public class Itemorginfo
    {
        public string orgid { get; set; }
        public string status { get; set; }
        public string category { get; set; }
    }
}
