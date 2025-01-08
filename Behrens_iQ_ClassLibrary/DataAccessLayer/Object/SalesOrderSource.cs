/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Sales Order Source
 *              Methods for Retreiving Sales Order Sources via REST API
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using RestSharp;

using Behrens_iQ_ClassLibrary.BaseClasses;
using Behrens_iQ_ClassLibrary.Functions;
using Behrens_iQ_ClassLibrary.Models.Object;

namespace Behrens_iQ_ClassLibrary.DataAccessLayer.Object
{
    class DAL_SalesOrderSourceJSONObject
    {
        public string TotalCount;
        public List<SalesOrderSource> Data { get; set; }
    }

    class DAL_SalesOrderSource : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public SalesOrderSource GetSalesOrderSource(long ID)
        {
            string apirequest = "SalesOrderSources/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            SalesOrderSource SalesOrderSource = JsonSerializer.Deserialize<SalesOrderSource>(restResponse.Content);

            return SalesOrderSource;
        }

        public SalesOrderSource GetSalesOrderSourceCode(string htmlparameter)
        {
            try
            {
                string apirequest = "SalesOrderSources?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_SalesOrderSourceJSONObject SalesOrderSources = JsonSerializer.Deserialize<DAL_SalesOrderSourceJSONObject>(restResponse.Content);

                List<SalesOrderSource> listSalesOrderSource = new List<SalesOrderSource>();
                foreach (SalesOrderSource salesOrderSource in SalesOrderSources.Data)
                {
                    listSalesOrderSource.Add(salesOrderSource);
                }
                return listSalesOrderSource[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<SalesOrderSource> GetSalesOrderSources(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "SalesOrderSources?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_SalesOrderSourceJSONObject SalesOrderSources = JsonSerializer.Deserialize<DAL_SalesOrderSourceJSONObject>(restResponse.Content);

                List<SalesOrderSource> listSalesOrderSource = new List<SalesOrderSource>();
                foreach (SalesOrderSource salesOrderSource in SalesOrderSources.Data)
                {
                    listSalesOrderSource.Add(salesOrderSource);
                }
                return listSalesOrderSource;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SalesOrderSource salesOrderSource = new SalesOrderSource();
                List<SalesOrderSource> listSalesOrderSource = new List<SalesOrderSource>() { salesOrderSource };
                return listSalesOrderSource;
            }
        }
    }
}
