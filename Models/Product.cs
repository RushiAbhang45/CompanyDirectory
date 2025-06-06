namespace CompanyDirectory.Models
{
    public class Product
    {
        public long Id { get; set; }                   // Primary key
        public string? Name { get; set; }
        public string? Description { get; set; }

        public long CompanyId { get; set; }            // Foreign key
        public Company? Company { get; set; }          // Navigation property
    }
}
