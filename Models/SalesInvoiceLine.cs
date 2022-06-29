namespace navision.api.Models
{
    public class SalesInvoiceLine
    {
     public string ArticleCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string License { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double NetAmount { get; set; }
        public double VATAmount { get; set; }
        public double TotalAmount { get; set; }   
    }
}