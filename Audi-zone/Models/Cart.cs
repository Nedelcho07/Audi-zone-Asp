namespace Audi_zone.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public Client Clients { get; set; }
        public int ProductId { get; set; }
        public Product Products { get; set; }
        public int Quantity { get; set; }
        public DateTime DataRegOn { get; set; }
    }
}
