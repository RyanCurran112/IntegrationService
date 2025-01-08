/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Model for Object Address
 *              
 */

using System;
using System.Collections.Generic;
using System.Text;

using Behrens_iQ_ClassLibrary.BaseClasses;

namespace Behrens_iQ_ClassLibrary.Models.Object
{
    public class Address : GenericClassID
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Town { get; set; }
        public string City { get; set; }
        public string County { get; set; }

        public Country Country = new Country();
        public long CountryID { get; set; }
  
        public string PostCode { get; set; }
    }
}
