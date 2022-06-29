using System;
using System.Collections.Generic;

namespace navision.api.Models
{
    public class InternalCarsInvoice
    {
        public InternalCarsInvoice()
        {
            InvoiceItems = new List<InvoiceItem>();
        }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string PaymentOCRNumber { get; set; } = string.Empty;
        public double NetAmount { get; set; }
        public double VatAmount { get; set; }
        public double RoundingAmount { get; set; }
        public double TotalAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string InvoicePdfCopy { get; set; } = string.Empty;
        public string CustomerNumber { get; set; } = string.Empty;
        public string CustomerOrganisationName { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string SellerOrganisationName { get; set; } = string.Empty;
        public string SellerAddress { get; set; } = string.Empty;
        public string SellerReference { get; set; } = string.Empty;
        public string PaymentTerms { get; set; } = string.Empty;
        public string InterestRate { get; set; } = string.Empty;
        public string SellerVATNumber { get; set; } = string.Empty;
        public List<InvoiceItem> InvoiceItems { get; set; }
    }

    public class InvoiceItem
    {
        public string ArticleCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string License { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int NetAmount { get; set; }
        public int VatAmount { get; set; }
        public int TotalAmount { get; set; }
    }
}