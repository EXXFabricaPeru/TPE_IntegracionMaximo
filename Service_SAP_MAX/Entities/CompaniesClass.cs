using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    public class CompaniesClass
    {
        [JsonIgnore] 
        public string CardCode { get; set; }
        [JsonIgnore] 
        public string idMaximo { get; set; }


        public string company { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string orgid { get; set; }
        public string currencycode { get; set; }
        public string tax1code { get; set; }
        public string paymentterms { get; set; }
        public string disabled { get; set; }
        public string inclusive1 { get; set; }
        public string sendersysid { get; set; }
        public string address1 { get; set; }
    }
}
