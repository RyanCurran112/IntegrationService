using BehrensGroup_ClassLibrary.BaseClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BehrensGroup_ClassLibrary.Transactions
{
    public class Receipt : GenericClassTransaction
    {
        public long ReceiptType { get; set; }
        public decimal Amount { get; set; }

        public const long ReceiptType_Paypal_ID = 19434750676396;
        public const long ReceiptType_Realex_ID = 19434740627053;
        public const long ReceiptType_Klarna_ID = 19434750734798;

    }
}
