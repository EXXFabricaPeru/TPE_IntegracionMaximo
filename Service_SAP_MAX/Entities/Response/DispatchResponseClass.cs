using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities.Response
{
    public class DispatchResponseClass
    {
        public class Invuseline
        {
            public int invuselineid { get; set; }
            public string commodity { get; set; }
            public string localref { get; set; }
            public double unitcost { get; set; }
            public bool receiptscomplete { get; set; }
            public string linetype { get; set; }
            public string commoditygroup { get; set; }
            public double physcnt { get; set; }
            public string description { get; set; }
            public string tositeid { get; set; }
            public string gldebitacct { get; set; }
            public DateTime actualdate { get; set; }
            public List<Invuselinesplit> invuselinesplit { get; set; }
            public string _rowstamp { get; set; }
            public double receivedqty { get; set; }
            public bool split { get; set; }
            public double stagedqty { get; set; }
            public bool validated { get; set; }
            public string assetnum { get; set; }
            public double openqty { get; set; }
            public string href { get; set; }
            public string fromstoreloc { get; set; }
            public string financialperiod { get; set; }
            public string refwo { get; set; }
            public double conversion { get; set; }
            public bool enteredastask { get; set; }
            public double pickedqty { get; set; }
            public double issuedqty { get; set; }
            public double linecost { get; set; }
            public double quantity { get; set; }
            public bool returnagainstissue { get; set; }
            public string toorgid { get; set; }
            public string shipmentline_collectionref { get; set; }
            public string usetype_description { get; set; }
            public string linetype_description { get; set; }
            public string orgid { get; set; }
            public int invuselinenum { get; set; }
            public string itemnum { get; set; }
            public double returnedqty { get; set; }
            public bool inspectionrequired { get; set; }
            public string enterby { get; set; }
            public DateTime physcntdate { get; set; }
            public string invuselinesplit_collectionref { get; set; }
            public string itemsetid { get; set; }
            public string location { get; set; }
            public string usetype { get; set; }
        }

        public class Invuselinesplit
        {
            public int invuselineid { get; set; }
            public double quantity { get; set; }
            public string localref { get; set; }
            public double unitcost { get; set; }
            public double physcnt { get; set; }
            public string orgid { get; set; }
            public string itemnum { get; set; }
            public string _rowstamp { get; set; }
            public bool autocreated { get; set; }
            public string contentuid { get; set; }
            public DateTime physcntdate { get; set; }
            public int invuselinesplitid { get; set; }
            public string itemsetid { get; set; }
            public string href { get; set; }
            public string fromstoreloc { get; set; }
        }


        public string itc_tiptrans { get; set; }
        public double exchangerate { get; set; }
        public string description { get; set; }
        public string currencycode { get; set; }
        public string invusenum { get; set; }
        public string itc_tiptrans_description { get; set; }
        public string _rowstamp { get; set; }
        public double exchangerate2 { get; set; }
        public bool autosplit { get; set; }
        public string siteid { get; set; }
        public int invuseid { get; set; }
        public string href { get; set; }
        public string invuseline_collectionref { get; set; }
        public string fromstoreloc { get; set; }
        public List<Invuseline> invuseline { get; set; }
        public string status_description { get; set; }
        public string changeby { get; set; }
        public DateTime changedate { get; set; }
        public string usetype_description { get; set; }
        public string orgid { get; set; }
        public DateTime exchangedate { get; set; }
        public DateTime statusdate { get; set; }
        public string receipts_description { get; set; }
        public string receipts { get; set; }
        public bool autocreated { get; set; }
        public bool statusiface { get; set; }
        public bool itc_fuel { get; set; }
        public string usetype { get; set; }
        public string status { get; set; }
    }
}
