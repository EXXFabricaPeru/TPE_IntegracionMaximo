using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities.Response
{
    public class PurchaseOrderResponseClass
    {
        public bool historyflag { get; set; }
        public bool ignorecntrev { get; set; }
        public string customernum { get; set; }
        public double totaltax4 { get; set; }
        public double totaltax3 { get; set; }
        public string purchaseagent { get; set; }
        public double totaltax6 { get; set; }
        public double totaltax5 { get; set; }
        public string currencycode { get; set; }
        public double totaltax2 { get; set; }
        public double totalcost { get; set; }
        public double totaltax1 { get; set; }
        public string _rowstamp { get; set; }
        public bool buyahead { get; set; }
        public string poline_collectionref { get; set; }
        public string siteid { get; set; }
        public string href { get; set; }
        public List<PurchaseOrderResponseline> poline { get; set; }
        public string shipto { get; set; }
        public bool inclusive5 { get; set; }
        public bool inclusive4 { get; set; }
        public string status_description { get; set; }
        public bool inclusive6 { get; set; }
        public DateTime changedate { get; set; }
        public bool inclusive1 { get; set; }
        public int priority { get; set; }
        public bool inclusive3 { get; set; }
        public string orgid { get; set; }
        public bool inclusive2 { get; set; }
        public bool inspectionrequired { get; set; }
        public string status { get; set; }
        public bool @internal { get; set; }
        public string paymentterms { get; set; }
        public double exchangerate { get; set; }
        public string potype { get; set; }
        public string description { get; set; }
        public bool internalchange { get; set; }
        public DateTime orderdate { get; set; }
        public string potype_description { get; set; }
        public string vendor { get; set; }
        public bool payonreceipt { get; set; }
        public string changeby { get; set; }
        public bool po10 { get; set; }
        public int revisionnum { get; set; }
        public bool allowreceipt { get; set; }
        public int poid { get; set; }
        public DateTime exchangedate { get; set; }
        public DateTime statusdate { get; set; }
        public string receipts_description { get; set; }
        public string receipts { get; set; }
        public bool statusiface { get; set; }
        public string billto { get; set; }
        public string ponum { get; set; }
    }

    public class PurchaseOrderResponseline
    {
        public string localref { get; set; }
        public string _rowstamp { get; set; }
        public bool taxed { get; set; }
        public string href { get; set; }
        public bool enteredastask { get; set; }
        public string shipto { get; set; }
        public double loadedcost { get; set; }
        public double linecost { get; set; }
        public bool issue { get; set; }
        public bool chargestore { get; set; }
        public bool isdistributed { get; set; }
        public int polinenum { get; set; }
        public string orgid { get; set; }
        public bool taxexempt { get; set; }
        public string restype { get; set; }
        public string itemnum { get; set; }
        public bool inspectionrequired { get; set; }
        public string enterby { get; set; }
        public bool mktplcitem { get; set; }
        public double linecost1 { get; set; }
        public bool prorateservice { get; set; }
        public string tax1code { get; set; }
        public string itemsetid { get; set; }
        public double linecost2 { get; set; }
        public double orderqty { get; set; }
        public double receivedunitcost { get; set; }
        public string commodity { get; set; }
        public double unitcost { get; set; }
        public bool receiptscomplete { get; set; }
        public string linetype { get; set; }
        public string commoditygroup { get; set; }
        public double loadedcost1 { get; set; }
        public string description { get; set; }
        public string tositeid { get; set; }
        public string gldebitacct { get; set; }
        public double rejectedqty { get; set; }
        public string category_description { get; set; }
        public double tax1 { get; set; }
        public string requestedby { get; set; }
        public double tax2 { get; set; }
        public int polineid { get; set; }
        public bool consignment { get; set; }
        public double tax5 { get; set; }
        public double tax6 { get; set; }
        public double tax3 { get; set; }
        public double conversion { get; set; }
        public double tax4 { get; set; }
        public DateTime enterdate { get; set; }
        public double receivedtotalcost { get; set; }
        public string orderunit { get; set; }
        public string linetype_description { get; set; }
        public string restype_description { get; set; }
        public double proratecost { get; set; }
        public string category { get; set; }
        public bool receiptreqd { get; set; }
    }

}
