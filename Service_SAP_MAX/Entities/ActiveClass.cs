using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    public class ActiveClass
    {
        [JsonIgnore]
        public string CodeSAP { get; set; }
        [JsonIgnore]
        public string idMaximo { get; set; }
        public string assetnum { get; set; }
        public string assettype { get; set; }
        public string calnum { get; set; }
        public string defaultrepfac { get; set; }
        public string description { get; set; }
        public string defaultrepfacsiteid { get; set; }
        public string glaccount { get; set; }
        public string itc_financialassetnum { get; set; }
        public string itc_modelo { get; set; }
        public string itc_oldassetnum { get; set; }
        public string itc_propiedad { get; set; }
        public string location { get; set; }
        public string shiftnum { get; set; }
        public string siteid { get; set; }
        public string orgid { get; set; }
        public string usage { get; set; }
        public string changeby { get; set; }
        public DateTime changedate { get; set; }
        public string sendersysid { get; set; }
        public string status { get; set; }
    }
}
