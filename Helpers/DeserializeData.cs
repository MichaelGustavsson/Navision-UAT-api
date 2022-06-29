using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using navision.api.Models;

namespace navision.api.Helpers
{
    public static class DeserializeData
    {
        public static SalesInvoices Deserialize(string data)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SalesInvoices));

                var stream = GenerateStream(data);
                
                var result = (SalesInvoices)serializer.Deserialize(stream)!;

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }            
        }

        public static InternalCarsInvoice MapToInternalCars(SalesInvoice invoice)
        {            
            var ic = new InternalCarsInvoice
            {
                CustomerAddress = invoice.CustomerAddress,
                CustomerNumber = invoice.CustomerNumber,
                CustomerOrganisationName = invoice.CustomerOrganisationName,
                DueDate = invoice.DueDate,
                InterestRate = invoice.InterestRate,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoicePdfCopy = invoice.InvoicePDFCopy,
                NetAmount = invoice.NetAmount,
                PaymentOCRNumber = invoice.PaymentOCRNumber,
                PaymentTerms = invoice.PaymentTerms,
                RoundingAmount = invoice.RoundingAmount,
                SellerAddress = invoice.SellerAddress,
                SellerOrganisationName = invoice.SellerOrganisationName,
                SellerReference = invoice.SellerReference,
                SellerVATNumber = invoice.SellerVATNumber,
                TotalAmount = invoice.TotalAmount,
                VatAmount = invoice.VATAmount
            };

            foreach(var invoiceLine in invoice.SalesInvoiceLines!)
            {
                ic.InvoiceItems.Add(new InvoiceItem
                {
                    ArticleCode = invoiceLine.ArticleCode,
                    Description = invoiceLine.Description,
                    License = invoiceLine.License,
                    NetAmount = (int)invoiceLine.NetAmount,
                    Quantity = invoiceLine.Quantity,
                    TotalAmount = (int)invoiceLine.TotalAmount,
                    VatAmount = (int)invoiceLine.VATAmount
                });
            }
            return ic;
        }   
        private static Stream GenerateStream(string data)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(data));
        }
    }
}