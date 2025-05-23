using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    public class ServiceClass
    {
        [JsonIgnore]
        public string ItemCode { get; set; }
        [JsonIgnore]
        public string idMaximo { get; set; }

        public string commodity { get; set; }
        public string commoditygroup { get; set; }
        public string description { get; set; }
        public string itemnum { get; set; }
        public string itemsetid { get; set; }
        public string itemtype { get; set; }
        public string lottype { get; set; }
        public string orderunit { get; set; }
        public int inspectionrequired { get; set; }
        public int taxexempt { get; set; }
        public string sendersysid { get; set; }
        public string status { get; set; }
        public List<ItemorginfoService> itemorginfo { get; set; }


    
    }
    public class ItemorginfoService
    {
        public string orgid { get; set; }
        public string status { get; set; }
        public string category { get; set; }
        public string glaccount { get; set; }
    }


}
