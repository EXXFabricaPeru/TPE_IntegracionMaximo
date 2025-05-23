using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities
{
    public class ExchangeClass
    {
        public string orgid { get; set; }
        public string currencycode { get; set; }
        public string currencycodeto { get; set; }
        public decimal exchangerate { get; set; }
        public decimal exchangerate2 { get; set; }
        public DateTime activedate { get; set; }
        public DateTime expiredate { get; set; }
        public string enterby { get; set; }
        public DateTime enterdate { get; set; }
        public string memo { get; set; }
        
    }
}
