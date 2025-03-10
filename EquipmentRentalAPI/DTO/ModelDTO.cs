namespace EquipmentRentalAPI.DTO
{
    public class ModelDTO
    {
        public int ModelId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
