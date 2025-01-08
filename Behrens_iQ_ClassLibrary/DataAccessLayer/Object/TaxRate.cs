/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Tax Rate
 *              Methods for Retreiving Tax Rates via REST API
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
    class DAL_TaxRateJSONObject
    {
        public string TotalCount;
        public List<TaxRate> Data { get; set; }
    }

    class DAL_TaxRate
    {
        public TaxRate GetTaxRate(long ID)
        {
            string apirequest = "TaxRates/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            TaxRate TaxRate = JsonSerializer.Deserialize<TaxRate>(restResponse.Content);

            return TaxRate;
        }

        public TaxRate GetTaxRateCode(string htmlparameter)
        {
            try
            {
                string apirequest = "TaxRates?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_TaxRateJSONObject TaxRates = JsonSerializer.Deserialize<DAL_TaxRateJSONObject>(restResponse.Content);

                List<TaxRate> listTaxRate = new List<TaxRate>();
                foreach (TaxRate taxRate in TaxRates.Data)
                {
                    listTaxRate.Add(taxRate);
                }
                return listTaxRate[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<TaxRate> GetTaxRates(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "TaxRates?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_TaxRateJSONObject TaxRates = JsonSerializer.Deserialize<DAL_TaxRateJSONObject>(restResponse.Content);

                List<TaxRate> listTaxRate = new List<TaxRate>();
                foreach (TaxRate taxRate in TaxRates.Data)
                {
                    listTaxRate.Add(taxRate);
                }
                return listTaxRate;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                TaxRate TaxRate = new TaxRate();
                List<TaxRate> listTaxRate = new List<TaxRate>() { TaxRate };
                return listTaxRate;
            }
        }
}
}
