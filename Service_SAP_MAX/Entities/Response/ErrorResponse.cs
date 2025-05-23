using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Entities.Response
{
    public class ErrorResponse
    {

        public Error Error { get; set; }


    }
    public class Error
    {
        public ExtendedError extendedError { get; set; }
        public string reasonCode { get; set; }
        public string message { get; set; }
        public string statusCode { get; set; }
    }

    public class ExtendedError
    {
        public MoreInfo moreInfo { get; set; }
    }

    public class MoreInfo
    {
        public string href { get; set; }
    }
}
