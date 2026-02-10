namespace Audi_zone.Models
{
    public class Model
    {
        public int Id { get; set; }
        public string ModelName { get; set; }
        public DateTime DateRegOn { get; set; }
    public ICollection<Product> Products { get; set; }
    }
}
