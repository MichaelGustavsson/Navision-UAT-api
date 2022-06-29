using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace navision.api.Models
{
    [XmlRoot("SalesInvoices")]
    public class SalesInvoices
    {
        [XmlAttribute]
        public string UpdatedDate { get; set; } = string.Empty;
        [XmlElement("SalesInvoice")]
        public List<SalesInvoice>? SalesInvoiceList { get; set; }
    }

    public class SalesInvoice
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public string PaymentOCRNumber { get; set; } = string.Empty;
        public double NetAmount { get; set; }
        public double VATAmount { get; set; }
        public double RoundingAmount { get; set; }
        public double TotalAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string InvoicePDFCopy { get; set; } = string.Empty;
        public string CustomerNumber { get; set; } = string.Empty;
        public string CustomerOrganisationName { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string SellerOrganisationName { get; set; } = string.Empty;
        public string SellerAddress { get; set; } = string.Empty;
        public string SellerReference { get; set; } = string.Empty;
        public string PaymentTerms { get; set; } = string.Empty;
        public string InterestRate { get; set; } = string.Empty;
        public string SellerVATNumber { get; set; } = string.Empty;
        public List<SalesInvoiceLine>? SalesInvoiceLines { get; set; }
    }
}