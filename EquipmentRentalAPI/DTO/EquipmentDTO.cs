namespace EquipmentRentalAPI.DTO
{
    public class EquipmentDTO
    {
        public int EquipmentId { get; set; }
        public string SerialNumber { get; set; } = null!;
        public int ModelId { get; set; }
    }
}
