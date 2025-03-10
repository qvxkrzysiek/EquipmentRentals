using System;
using System.Collections.Generic;

namespace EquipmentRentalAPI.Models;

public partial class Rental
{
    public int RentalId { get; set; }

    public DateTime RentalDate { get; set; }

    public DateTime ReturnDate { get; set; }

    public bool IsReturned { get; set; }

    public int UserId { get; set; }

    public int EquipmentId { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
