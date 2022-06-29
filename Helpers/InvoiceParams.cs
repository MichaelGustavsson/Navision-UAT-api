namespace navision.api.Helpers
{
    public class InvoiceParams
    {
        private const int MAXPAGESIZE = 100;
        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MAXPAGESIZE) ? MAXPAGESIZE : value; }
        }
        
    }
}