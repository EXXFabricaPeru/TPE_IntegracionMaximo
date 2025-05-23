using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities.Response
{
    public class GLComponentResponseClass
    {
        public string _rowstamp { get; set; }
        public string comptext { get; set; }
        public int glcomponentsid { get; set; }
        public int glorder { get; set; }
        public bool active { get; set; }
        public string compvalue { get; set; }
        public string href { get; set; }
        public string sourcesysid { get; set; }
        public string userid { get; set; }
        public string orgid { get; set; }
    }
}
