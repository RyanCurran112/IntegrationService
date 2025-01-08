/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Model for Object Customer Contact
 *              
 */

using System;
using System.Collections.Generic;
using System.Text;

using Behrens_iQ_ClassLibrary.BaseClasses;

namespace Behrens_iQ_ClassLibrary.Models.Object
{
    public class Customer_Contact : GenericClassID
    {
        //ID                        - GenericClassID
        //CreatedOn                 - GenericClassID
        //UpdatedOn                 - GenericClassID

        public string UniqueID { get; set; }                //Unique ID is email address
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string D_ContactName { get; set; }

        public _Salutation Salutation;
        public enum _Salutation
        {
            Mr,
            Mrs,
            Dr,
            Ms,
            Miss
        }

        public Customer Customer = new Customer();
        public long CustomerID { get; set; }

        public Address Address = new Address();
        public long AddressID { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }

        public string ContactCompanyCode { get; set; }

        public string CustomerContactRoleCode { get; set; }
        public string EmailMarketingAllowed { get; set; }
        public string _Owner_ { get; set; }
        public long LookupCustomer { get; set; }
    }
}
