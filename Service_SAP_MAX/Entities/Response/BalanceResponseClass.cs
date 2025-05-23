using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities.Response
{
    public class BalanceResponseClass
    {
        public bool reconciled { get; set; }
        public double physcnt { get; set; }
        public string binnum { get; set; }
        public string orgid { get; set; }
        public string itemnum { get; set; }
        public string _rowstamp { get; set; }
        public string itemtype { get; set; }
        public bool stagingbin { get; set; }
        public double stagedcurbal { get; set; }
        public DateTime physcntdate { get; set; }
        public double curbal { get; set; }
        public int invbalancesid { get; set; }
        public DateTime nextphycntdate { get; set; }
        public string itemsetid { get; set; }
        public string siteid { get; set; }
        public string location { get; set; }
        public string href { get; set; }
    }
}
