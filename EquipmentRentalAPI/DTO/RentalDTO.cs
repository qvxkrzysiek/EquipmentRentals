using EquipmentRentalAPI.Models;

namespace EquipmentRentalAPI.DTO
{
    public class RentalDTO
    {
        public int RentalId { get; set; }
        public int EquipmentId { get; set; }
        public string EquipmentSerialNumber { get; set; } = null!;
        public int UserId { get; set; }
        public string UserEmail { get; set; } = null!;
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool IsReturned { get; set; }
        public string EquipmentModelName { get; set; } = null!;
    }
}
