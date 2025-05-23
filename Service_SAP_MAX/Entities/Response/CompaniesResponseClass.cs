using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities.Response
{
    public class CompaniesResponseClass
    {
        public string paymentterms { get; set; }
        public string address1 { get; set; }
        public string currencycode { get; set; }
        public bool inclusive1 { get; set; }
        public string type { get; set; }
        public string compcontact_collectionref { get; set; }
        public int companiesid { get; set; }
        public string orgid { get; set; }
        public string _rowstamp { get; set; }
        public string name { get; set; }
        public string tax1code { get; set; }
        public string company { get; set; }
        public bool disabled { get; set; }
        public string href { get; set; }
        public string type_description { get; set; }
    }
}
