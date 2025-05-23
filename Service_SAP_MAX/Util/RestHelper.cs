using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Util
{
    public class RestHelper
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(RestHelper));

        public static RestResponse SendRest(string url, Method method, string maxauth, string Authorization, string body, string properties, string methodOver = "", string patchType = "")
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                var client = new RestClient();
                var request = new RestRequest(url, method);

                if (!string.IsNullOrEmpty(Authorization))
                    request.AddHeader("Authorization", Authorization);

                request.AddHeader("Content-Type", "application/json");

                if (!string.IsNullOrEmpty(maxauth))
                    request.AddHeader("maxauth", maxauth);

                if (!string.IsNullOrEmpty(properties))
                    request.AddHeader("properties", properties);

                if (!string.IsNullOrEmpty(methodOver))
                    request.AddHeader("x-method-override", methodOver);

                if (!string.IsNullOrEmpty(patchType))
                    request.AddHeader("patchType", patchType);

                request.AddParameter("application/json", body, ParameterType.RequestBody);

                RestResponse response = client.Execute(request);

                return response;

            }
            catch (Exception ex)
            {
                logger.Info("SendRest: " + ex.Message);
                throw;
            }
        }

        public static RestResponse SendRest(string url, Method method, string auth, string body)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                var client = new RestClient();
                var request = new RestRequest(url, method);

                request.AddHeader("Authorization", "Basic " + auth);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("maxauth", auth);

                request.AddParameter("application/json", body, ParameterType.RequestBody);

                RestResponse response = client.Execute(request);

                return response;

            }
            catch (Exception ex)
            {
                logger.Info("SendRest: " + ex.Message);
                throw;
            }
        }
    }
}
