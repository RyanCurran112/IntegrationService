/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Sales Rep
 *              Methods for Retreiving Sales Reps via REST API
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
    class DAL_SalesRepJSONObject
    {
        public string TotalCount;
        public List<SalesRep> Data { get; set; }
    }

    class DAL_SalesRep
    {
        public SalesRep GetSalesRep(long ID)
        {
            string apirequest = "SalesReps/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            SalesRep SalesRep = JsonSerializer.Deserialize<SalesRep>(restResponse.Content);

            return SalesRep;
        }

        public SalesRep GetSalesRepCode(string htmlparameter)
        {
            try
            {
                string apirequest = "SalesReps?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_SalesRepJSONObject SalesReps = JsonSerializer.Deserialize<DAL_SalesRepJSONObject>(restResponse.Content);

                List<SalesRep> listSalesRep = new List<SalesRep>();
                foreach (SalesRep salesRep in SalesReps.Data)
                {
                    listSalesRep.Add(salesRep);
                }
                return listSalesRep[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<SalesRep> GetSalesReps(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "SalesReps?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_SalesRepJSONObject SalesReps = JsonSerializer.Deserialize<DAL_SalesRepJSONObject>(restResponse.Content);

                List<SalesRep> listSalesRep = new List<SalesRep>();
                foreach (SalesRep salesRep in SalesReps.Data)
                {
                    listSalesRep.Add(salesRep);
                }
                return listSalesRep;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SalesRep SalesRep = new SalesRep();
                SalesRep.ID = 0;
                List<SalesRep> listSalesRep = new List<SalesRep>();

                listSalesRep.Add(SalesRep);
                return listSalesRep;
            }
        }
    }
}
