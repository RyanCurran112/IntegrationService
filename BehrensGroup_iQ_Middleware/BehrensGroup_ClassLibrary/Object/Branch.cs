/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Branch
 *              Methods for Retreiving Branches via REST API
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
    public class BranchJSONObject
    {
        public string TotalCount;
        public List<Branch> Data { get; set; }
    }

    public class Branch : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public const long Branch_HQ_ID = 4337916983871;
        public const long Branch_TP_ID = 4337917133313;

        public static Branch GetBranch(long ID)
        {
            string apirequest = "Branches/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            Branch Branch = JsonConvert.DeserializeObject<Branch>(restResponse.Content);

            return Branch;
        }

        public static Branch GetBranchCode(string htmlparameter)
        {
            try
            {
                string apirequest = "Branches?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                BranchJSONObject Branches = JsonConvert.DeserializeObject<BranchJSONObject>(restResponse.Content);

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

        public static List<Branch> GetBranches(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "Branches?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                BranchJSONObject Branches = JsonConvert.DeserializeObject<BranchJSONObject>(restResponse.Content);

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
                Branch Branch = new Branch { ID = 0 };
                List<Branch> listBranch = new List<Branch> { Branch };
                return listBranch;
            }
        }
    }
}
