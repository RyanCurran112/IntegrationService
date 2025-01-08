/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Model for Object Customer Delivery Contact
 *              
 */

using System;
using System.Collections.Generic;
using System.Text;

using Behrens_iQ_ClassLibrary.BaseClasses;

namespace Behrens_iQ_ClassLibrary.Models.Object
{
    public class Customer_DeliveryContact : GenericClassID
    {
        //ID                        - GenericClassID
        //CreatedOn                 - GenericClassID
        //UpdatedOn                 - GenericClassID

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }

        public string UniqueID1 { get; set; }               //UniqueID1 is delivery contacts email address
        public string UniqueID2 { get; set; }               //UniqueID2 is delivery contacts postcode with spacing/characters removed.
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

        public string CompanyName { get; set; }

        public Customer LookupCustomer = new Customer();
        public long LookupCustomerID { get; set; }

        public Customer Customer = new Customer();
        public string _Owner_;
        public long CustomerID { get; set; }

        public CashCustomer CashCustomer = new CashCustomer();
        public long CashCustomerID { get; set; }

        public Address Address = new Address();
        public long AddressID { get; set; }

        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
    }
}
