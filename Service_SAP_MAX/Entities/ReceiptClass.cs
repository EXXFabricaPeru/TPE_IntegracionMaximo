using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    public class ReceiptClass
    {
        [JsonIgnore]
        public string CodeSAP { get; set; }
        [JsonIgnore]
        public string idMaximo { get; set; }
        public DateTime actualdate { get; set; }
        public string conversion { get; set; }
        public string enterby { get; set; }
        public DateTime enterdate { get; set; }
        public string externalrefid { get; set; }
        public string issuetype { get; set; }
        public string itemnum { get; set; }
        public string itemsetid { get; set; }
        public string linetype { get; set; }
        public string orderqty { get; set; }// solo recepción material y servicio y devolver servicio
        public string receiptquantity { get; set; }
        public string orgid { get; set; }
        public string ponum { get; set; }
        public string polinenum { get; set; }
        public string porevisionnum { get; set; }
        public string positeid { get; set; }
        public string refwo { get; set; }
        public string sendersysid { get; set; }
        public string sourcesysid { get; set; }
        public string siteid { get; set; }
        public string status { get; set; }



        public string qtytoreceive { get; set; } // solo devolver material y servicio y recepción servicio



        public string tolot { get; set; }//devolver material
    }
}
