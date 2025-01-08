/*
 * Author:      Ryan Curran
 * Date:        Jan 2021
 * Description: Class for Object Product Batch
 *              Methods for Retreiving Product Batches via REST API
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
    public class ProductBatchJSONObject
    {
        public string TotalCount;
        public List<ProductBatch> Data { get; set; }
    }

    public class ProductBatch : GenericClassTransaction
    {

    }
}
