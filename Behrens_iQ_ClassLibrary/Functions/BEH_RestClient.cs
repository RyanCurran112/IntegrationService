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
            var client = new RestClient("API URL");
            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer ExampleBearerToken");


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
