using System;

namespace navision.api.Models
{
    public class Invoice
    {
        public int Id{ get; set; }
        public string InvoiceNumber{ get; set; } = string.Empty;
        public DateTime? UploadDate { get; set; }
        public string Data { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

    }
}