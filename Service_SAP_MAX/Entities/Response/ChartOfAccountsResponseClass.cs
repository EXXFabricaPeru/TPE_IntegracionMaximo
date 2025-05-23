using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities.Response
{
    public class ChartOfAccountsResponseClass
    {
        public string glcomp04 { get; set; }
        public bool active { get; set; }
        public int chartofaccountsid { get; set; }
        public string glcomp01 { get; set; }
        public string glcomp03 { get; set; }
        public string orgid { get; set; }
        public DateTime activedate { get; set; }
        public string glcomp02 { get; set; }
        public string _rowstamp { get; set; }
        public string glaccount { get; set; }
        public string accountname { get; set; }
        public string href { get; set; }
        public string sourcesysid { get; set; }
    }
}
