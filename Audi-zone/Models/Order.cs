using System.ComponentModel.DataAnnotations.Schema;

namespace Audi_zone.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public Client Client { get; set; }
        public string OrderedItems { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        public DateTime OrderedOn { get; set; }
    }
}
