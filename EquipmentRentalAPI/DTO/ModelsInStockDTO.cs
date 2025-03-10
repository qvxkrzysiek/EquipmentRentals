namespace EquipmentRentalAPI.DTO
{
    public class ModelsInStockDTO
    {
        public int ModelId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }
    }
}
