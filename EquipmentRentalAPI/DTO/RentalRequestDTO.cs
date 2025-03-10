namespace EquipmentRentalAPI.DTO
{
    public class RentalRequestDTO
    {
        public int EquipmentModelId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
