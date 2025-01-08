/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Data Access Layer for Object Currency
 *              Methods for Retreiving, Updating & Adding Currencies via REST API
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
    class DAL_CurrencyJSONObject
    {
        public string TotalCount;
        public List<Currency> Data { get; set; }
    }

    class DAL_Currency : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public decimal ExchangeRate { get; set; }

        public Currency GetCurrency(long ID)
        {
            string apirequest = "Currencies/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            Currency Currency = JsonSerializer.Deserialize<Currency>(restResponse.Content);

            return Currency;
        }

        public Currency GetCurrencyCode(string htmlparameter)
        {
            try
            {
                string apirequest = "Currencies?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_CurrencyJSONObject Currencies = JsonSerializer.Deserialize<DAL_CurrencyJSONObject>(restResponse.Content);

                List<Currency> listCurrency = new List<Currency>();
                foreach (Currency currency in Currencies.Data)
                {
                    listCurrency.Add(currency);
                }
                return listCurrency[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<Currency> GetCurrencies(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "Currencies?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_CurrencyJSONObject Currencies = JsonSerializer.Deserialize<DAL_CurrencyJSONObject>(restResponse.Content);

                List<Currency> listCurrency = new List<Currency>();
                foreach (Currency currency in Currencies.Data)
                {
                    listCurrency.Add(currency);
                }
                return listCurrency;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Currency Currency = new Currency();
                List<Currency> listCurrency = new List<Currency>() { Currency };
                return listCurrency;
            }
        }
    }
}
