using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities.Response
{
    public class ItemResponseClass
    {
        public string commodity { get; set; }
        public bool rotating { get; set; }
        public string status_description { get; set; }
        public string commoditygroup { get; set; }
        public string description { get; set; }
        public string itemcondition_collectionref { get; set; }
        public string orderunit { get; set; }
        public string lottype_description { get; set; }
        public string itemspec_collectionref { get; set; }
        public string conversion_collectionref { get; set; }
        public string issueunit { get; set; }
        public string lottype { get; set; }
        public int itemid { get; set; }
        public string itemnum { get; set; }
        public string _rowstamp { get; set; }
        public string itemorginfo_collectionref { get; set; }
        public string itemtype { get; set; }
        public string itemsetid { get; set; }
        public string href { get; set; }
        public string status { get; set; }
    }
}
