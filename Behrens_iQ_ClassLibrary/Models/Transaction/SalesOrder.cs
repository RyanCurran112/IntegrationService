/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Model for Transaction Sales Order
 *              
 */
 
using System;
using System.Collections.Generic;
using System.Text;

using RestSharp;

using Behrens_iQ_ClassLibrary.BaseClasses;
using Behrens_iQ_ClassLibrary.Functions;
using Behrens_iQ_ClassLibrary.Models.Object;

namespace Behrens_iQ_ClassLibrary.Models.Transaction
{
    public class SalesOrder : GenericClassTransaction
    {
        //ID                    - GenericClassID
        //CreatedOn             - GenericClassID
        //UpdatedOn             - GenericClassID
        //Number                - GenericClassTransactionNumber
        //CreatedDate           - GenericClassTransactionNumber

        public string AlternateReference { get; set; }

        public Branch Branch = new Branch();
        public long BranchID { get; set; }

        public Branch DespatchBranch = new Branch();
        public long DespatchBranchID { get; set; }

        public Division D_Division = new Division();
        public long D_DivisionID { get; set; }


        public SalesRep SalesRep = new SalesRep();
        public long SalesRepID { get; set; }

        public SalesOrderType OrderType = new SalesOrderType();
        public long OrderTypeID { get; set; }

        public SalesOrderSource Source = new SalesOrderSource();
        public long SourceID { get; set; }


        public string Date { get; set; }
        public string DateDue { get; set; }
        public string DateAnticipated { get; set; }


        public Customer Customer = new Customer();

        public CashCustomer CashCustomer = new CashCustomer();
        public long CashCustomerID;

        public Customer_DeliveryContact DeliveryContact = new Customer_DeliveryContact();
        public long DeliveryContactID { get; set; }

        public Customer_Contact Contact = new Customer_Contact();
        public long ContactID { get; set; }

        public Customer_Site Site = new Customer_Site();
        public long SiteID { get; set; }


        public DeliveryAgent DeliveryAgent = new DeliveryAgent();
        public long DeliveryAgentID { get; set; }

        public DeliveryAgent_Service DeliveryAgentService = new DeliveryAgent_Service();
        public long DeliveryAgentServiceID { get; set; }

        public DeliveryInstructions DeliveryInstructions = new DeliveryInstructions();
        public long DeliveryInstructionsID { get; set; }


        public Currency Currency = new Currency();
        public long CurrencyID { get; set; }
        public decimal TransactionExchangeRate { get; set; }
        public decimal AccountExchangeRate { get; set; }


        public string DeliveryPricingType { get; set; }
        public decimal DeliveryNetAmount { get; set; }
        public decimal DeliveryGrossAmount { get; set; }

        public TaxRate DeliveryTaxRate = new TaxRate();
        public decimal DeliveryPriceGross { get; set; }
        public string AdditionalDeliveryNotes { get; set; }
        public string ConsignmentNo { get; set; }

        public string Particulars { get; set; }
        public string D_TagNumber { get; set; }
        public string D_ReqPt { get; set; }
        public string D_ReqNo { get; set; }


        public string OrderPricingType { get; set; }
        public decimal NetCost { get; set; }
        public decimal NetAmount { get; set; }
        public SalesPromotion Promotion = new SalesPromotion();
        public long PromotionID { get; set; }
        public decimal NetAmountLessDiscount { get; set; }
        public decimal Margin { get; set; }

        public List<SalesOrder_Line> SalesOrderLines = new List<SalesOrder_Line>();
        public List<Receipt> SalesOrderReceipts = new List<Receipt>();
    }
}
