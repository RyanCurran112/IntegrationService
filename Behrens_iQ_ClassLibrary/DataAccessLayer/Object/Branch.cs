/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Data Access Layer for Object Branch
 *              Methods for Retreiving, Updating & Adding Branches via REST API
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
    class DAL_BranchJSONObject
    {
        public string TotalCount;
        public List<Branch> Data { get; set; }
    }

    class DAL_Branch
    {
        public Branch GetBranch(long ID)
        {
            string apirequest = "Branches/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            Branch Branch = JsonSerializer.Deserialize<Branch>(restResponse.Content);

            return Branch;
        }

        public Branch GetBranchCode(string htmlparameter)
        {
            try
            {
                string apirequest = "Branches?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_BranchJSONObject Branches = JsonSerializer.Deserialize<DAL_BranchJSONObject>(restResponse.Content);

                List<Branch> listBranch = new List<Branch>();
                foreach (Branch branch in Branches.Data)
                {
                    listBranch.Add(branch);
                }
                return listBranch[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<Branch> GetBranches(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "Branches?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_BranchJSONObject Branches = JsonSerializer.Deserialize<DAL_BranchJSONObject>(restResponse.Content);

                List<Branch> listBranch = new List<Branch>();
                foreach (Branch branch in Branches.Data)
                {
                    listBranch.Add(branch);
                }
                return listBranch;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Branch Branch = new Branch();
                List<Branch> listBranch = new List<Branch>()    {   Branch  };
                return listBranch;
            }
        }
    }
}
