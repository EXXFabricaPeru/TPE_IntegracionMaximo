using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities.Response
{
    public class CostResponseClass
    {
        public bool @internal { get; set; }
        public double avblbalance { get; set; }
        public int ccf { get; set; }
        public double reservedqty { get; set; }
        public bool reorder { get; set; }
        public double issue2yrago { get; set; }
        public string issueunit { get; set; }
        public string _rowstamp { get; set; }
        public double issueytd { get; set; }
        public DateTime lastissuedate { get; set; }
        public string invbalances_collectionref { get; set; }
        public bool consignment { get; set; }
        public double minlevel { get; set; }
        public double curbal { get; set; }
        public int inventoryid { get; set; }
        public string siteid { get; set; }
        public string href { get; set; }
        public double issue1yrago { get; set; }
        public string invcost_collectionref { get; set; }
        public string status_description { get; set; }
        public double maxlevel { get; set; }
        public string costtype_description { get; set; }
        public List<InvBalanceResponse> invbalances { get; set; }
        public string binnum { get; set; }
        public string orderunit { get; set; }
        public string orgid { get; set; }
        public List<InvCostREsponse> invcost { get; set; }
        public DateTime statusdate { get; set; }
        public string itemnum { get; set; }
        public string itemtype { get; set; }
        public string shrinkageacc { get; set; }
        public bool statusiface { get; set; }
        public int deliverytime { get; set; }
        public string costtype { get; set; }
        public string itemsetid { get; set; }
        public string controlacc { get; set; }
        public string invcostadjacc { get; set; }
        public string location { get; set; }
        public double orderqty { get; set; }
        public double issue3yrago { get; set; }
        public bool hardresissue { get; set; }
        public string status { get; set; }
    }

    public class InvBalanceResponse
    {
        public bool reconciled { get; set; }
        public string localref { get; set; }
        public double physcnt { get; set; }
        public string binnum { get; set; }
        public string orgid { get; set; }
        public string _rowstamp { get; set; }
        public bool stagingbin { get; set; }
        public double stagedcurbal { get; set; }
        public DateTime physcntdate { get; set; }
        public double curbal { get; set; }
        public int invbalancesid { get; set; }
        public DateTime nextphycntdate { get; set; }
        public string href { get; set; }
    }

    public class InvCostREsponse
    {
        public string localref { get; set; }
        public double lastcost { get; set; }
        public double avgcost { get; set; }
        public int invcostid { get; set; }
        public string orgid { get; set; }
        public int condrate { get; set; }
        public string _rowstamp { get; set; }
        public string glaccount { get; set; }
        public string shrinkageacc { get; set; }
        public string controlacc { get; set; }
        public string invcostadjacc { get; set; }
        public string href { get; set; }
        public double stdcost { get; set; }
    }
}
