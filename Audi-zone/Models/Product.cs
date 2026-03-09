using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Audi_zone.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ModelId { get; set; }
        public Model Models { get; set; }
        public string? Description { get; set; }
        public string ImageURL { get; set; }
        [Column(TypeName ="decimal(10,2)")]
        public decimal Price { get; set; }
        public DateTime DateRegOn { get; set; }
        public int ProductTypeId { get; set; }
        public ProductType ProductTypes { get; set; }
        public ICollection<Cart> Carts { get; set; }
    }
}
