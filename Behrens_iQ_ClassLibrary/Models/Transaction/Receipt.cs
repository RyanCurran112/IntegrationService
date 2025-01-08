using System;
using System.Collections.Generic;
using System.Text;

using Behrens_iQ_ClassLibrary.BaseClasses;

namespace Behrens_iQ_ClassLibrary.Models.Transaction
{
    public class Receipt : GenericClassTransaction
    {
        //ID                    - GenericClassID
        //Number                - GenericClassTransactionNumber
        //CreatedDate           - GenericClassTransactionNumber

        public string ReceiptType { get; set; }
        public decimal Amount { get; set; }
    }
}
