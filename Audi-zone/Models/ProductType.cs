namespace Audi_zone.Models
{
    public class ProductType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateRegOn { get; set; }
        public ICollection<Product> Products { get; set; } 
    }
}
