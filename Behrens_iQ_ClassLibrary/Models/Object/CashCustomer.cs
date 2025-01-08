/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Model for Object Cash Customer
 *              
 */

using System;
using System.Collections.Generic;
using System.Text;

using Behrens_iQ_ClassLibrary.BaseClasses;

namespace Behrens_iQ_ClassLibrary.Models.Object
{
    public class CashCustomer : GenericClassObject
    {
        //ID                        - GenericClassID
        //CreatedOn                 - GenericClassID
        //UpdatedOn                 - GenericClassID

        //Code                      - GenericClassObject
        //Description               - GenericClassObject

        public string FirstName { get; set; }                //UniqueID is cash customers email address
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }

        public Branch Branch = new Branch();
        public long BranchID;

        public Division D_Division { get; set; }
        public long D_DivisionID { get; set; }

        public Address Address = new Address();
        public long AddressID { get; set; }

        public _Salutation Salutation;
        public List<long> CashCustomer_Contacts = new List<long>();

        public enum _Salutation
        {
            Mr,
            Mrs,
            Dr,
            Ms,
            Miss
        }
    }
}
