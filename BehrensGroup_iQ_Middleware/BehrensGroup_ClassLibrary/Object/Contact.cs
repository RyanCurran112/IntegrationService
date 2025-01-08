using System;
using System.Collections.Generic;
using System.Text;

using BehrensGroup_ClassLibrary.BaseClasses;

namespace BehrensGroup_ClassLibrary.Object
{
    public class Contact : GenericClassID
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //CreatedBy     - GenericClassID
        //UpdatedOn     - GenericClassID
        //UpdatedBy     - GenericClassID

        public string UniqueID1 { get; set; }
        public string UniqueID2 { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }

        public string ContactCustomerCode { get; set; }
        public string ContactCompanyCode { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Town { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string CountryCode { get; set; }
        public string Postcode { get; set; }

        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }

        public string CustomerContactRoleCode { get; set; }
        public string EmailMarketingAllowed { get; set; }


    }
}
