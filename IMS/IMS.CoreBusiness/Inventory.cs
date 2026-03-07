namespace IMS.CoreBusiness
{
    public sealed class Inventory
    {
        public required string Id { get; set; }
        public required string Name { get; set; }

        public required double Price { get; set; }
        public required int Quantity { get; set; }
    }
}
