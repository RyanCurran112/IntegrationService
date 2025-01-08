/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Division
 *              Methods for Retreiving Divisions via REST API
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
    class DAL_DivisionJSONObject
    {
        public string TotalCount;
        public List<Division> Data { get; set; }
    }
    class DAL_Division
    {
        public Division GetDivision(long ID)
        {
            string apirequest = "Divisions/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            Division D_Division = JsonSerializer.Deserialize<Division>(restResponse.Content);

            return D_Division;
        }

        public Division GetDivisionCode(string htmlparameter)
        {
            try
            {
                string apirequest = "Divisions?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_DivisionJSONObject Divisions = JsonSerializer.Deserialize<DAL_DivisionJSONObject>(restResponse.Content);

                List<Division> listDivision = new List<Division>();
                foreach (Division division in Divisions.Data)
                {
                    listDivision.Add(division);
                }
                return listDivision[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<Division> GetDivisions(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "Divisions?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_DivisionJSONObject Divisions = JsonSerializer.Deserialize<DAL_DivisionJSONObject>(restResponse.Content);

                List<Division> listDivision = new List<Division>();
                foreach (Division division in Divisions.Data)
                {
                    listDivision.Add(division);
                }
                return listDivision;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Division D_Division = new Division();
                List<Division> listDivision = new List<Division>(){ D_Division };
                return listDivision;
            }
        }
    }
}
