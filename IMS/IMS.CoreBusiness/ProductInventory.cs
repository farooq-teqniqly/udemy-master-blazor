using System.Text.Json.Serialization;

namespace IMS.CoreBusiness
{
    public sealed class ProductInventory
    {
        [JsonIgnore]
        public Inventory? Inventory { get; set; }
        public string InventoryId { get; set; } = null!;
        public int InventoryQuantity { get; set; }

        [JsonIgnore]
        public Product? Product { get; set; }
        public string ProductId { get; set; } = null!;
    }
}
