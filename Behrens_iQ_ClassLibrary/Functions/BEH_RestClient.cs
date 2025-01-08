using System;
using System.Collections.Generic;
using System.Text;

using RestSharp;

namespace Behrens_iQ_ClassLibrary.Functions
{
    public class BEH_RestClient
    {
        public static IRestResponse RestClientFunction(string apirequest, Method method)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");


            return client.Execute(request);
        }
    }

    public class BEH_RESTFilter
    {
        public string htmlOperator;
        public string htmlParameter;
        public string htmlAttribute;
    }
}
