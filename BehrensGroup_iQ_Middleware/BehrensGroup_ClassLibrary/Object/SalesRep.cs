/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Sales Rep
 *              Methods for Retreiving Sales Reps via REST API
 */
 
using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using RestSharp;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Functions;

namespace BehrensGroup_ClassLibrary.Object
{
    public class SalesRepJSONObject
    {
        public string TotalCount;
        public List<SalesRep> Data { get; set; }
    }

    public class SalesRep : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public const long SalesRep_LBY_ID = 8602819625969;              //LaBeeby Website
        public const long SalesRep_DAD_ID = 8602833145265;              //DipAndDoze Website

        public SalesRep GetSalesRep(long ID)
        {
            string apirequest = "SalesReps/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            SalesRep SalesRep = JsonConvert.DeserializeObject<SalesRep>(restResponse.Content);

            return SalesRep;
        }

        public SalesRep GetSalesRepCode(string htmlparameter)
        {
            try
            {
                string apirequest = "SalesReps?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                SalesRepJSONObject SalesReps = JsonConvert.DeserializeObject<SalesRepJSONObject>(restResponse.Content);

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
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                SalesRepJSONObject SalesReps = JsonConvert.DeserializeObject<SalesRepJSONObject>(restResponse.Content);

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
                List<SalesRep> listSalesRep = new List<SalesRep>() { SalesRep };
                return listSalesRep;
            }
        }
    }
}
