using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities.Response
{
    public class InventoryResponseClass
    {
        public string status_description { get; set; }
        public string costtype_description { get; set; }
        public Invbalances invbalances { get; set; }
        public string binnum { get; set; }
        public string orderunit { get; set; }
        public string issueunit { get; set; }
        public string itemnum { get; set; }
        public string _rowstamp { get; set; }
        public string itemtype { get; set; }
        public string invbalances_collectionref { get; set; }
        public int inventoryid { get; set; }
        public string costtype { get; set; }
        public string itemsetid { get; set; }
        public string siteid { get; set; }
        public string location { get; set; }
        public string href { get; set; }
        public string invcost_collectionref { get; set; }
        public string status { get; set; }
    }

    public class Invbalances
    {
        public int invbalancesid { get; set; }
    }
}
