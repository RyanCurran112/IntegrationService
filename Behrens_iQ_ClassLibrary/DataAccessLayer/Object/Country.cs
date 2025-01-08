/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Data Access Layer for Object Country
 *              Methods for Retreiving, Updating & Adding Countries via REST API
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
    class DAL_CountryJSONObject
    {
        public string TotalCount;
        public List<Country> Data { get; set; }
    }

    class DAL_Country
    {
        public Country GetCountry(long ID)
        {
            string apirequest = "Countries/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            Country Country = JsonSerializer.Deserialize<Country>(restResponse.Content);

            return Country;
        }

        public Country GetCountryCode(string htmlparameter)
        {
            try
            {
                string apirequest = "Countries?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_CountryJSONObject Countries = JsonSerializer.Deserialize<DAL_CountryJSONObject>(restResponse.Content);

                List<Country> listCountry = new List<Country>();
                foreach (Country country in Countries.Data)
                {
                    listCountry.Add(country);
                }
                return listCountry[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<Country> GetCountries(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "Countries?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_CountryJSONObject Countries = JsonSerializer.Deserialize<DAL_CountryJSONObject>(restResponse.Content);

                List<Country> listCountry = new List<Country>();
                foreach (Country country in Countries.Data)
                {
                    listCountry.Add(country);
                }
                return listCountry;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Country Country = new Country();

                List<Country> listCountry = new List<Country>()
                {
                    Country
                };

                return listCountry;
            }
        }
    }
}
