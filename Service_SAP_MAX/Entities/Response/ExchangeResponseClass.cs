using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities.Response
{
    public class ExchangeResponseClass
    {
        public string _rowstamp { get; set; }
        public double exchangerate2 { get; set; }
        public string enterby { get; set; }
        public DateTime enterdate { get; set; }
        public string currencycodeto { get; set; }
        public double exchangerate { get; set; }
        public string memo { get; set; }
        public string href { get; set; }
        public DateTime expiredate { get; set; }
        public string currencycode { get; set; }
        public string orgid { get; set; }
        public DateTime activedate { get; set; }
    }
}
