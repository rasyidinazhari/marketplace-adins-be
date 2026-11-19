namespace marketplace_AdIns_BE.DTOs
{
    public class ProductResponse
    {
        public long Id { get; set; }
        public long StoreId { get; set; }
        public string StoreName { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public decimal ProductPrice { get; set; }

        public int ProductStock { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}
