using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BehrensGroup_ClassLibrary.Functions
{
    public static class RestClient2
    {
        public static IRestResponse RestClientFunction(string apirequest, Method method)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");


            return client.Execute(request);
        }
    }

    public class RESTFilter
    {
        public string htmlOperator;
        public string htmlParameter;
        public string htmlAttribute;
    }
}
