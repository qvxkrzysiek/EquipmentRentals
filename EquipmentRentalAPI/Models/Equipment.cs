using System;
using System.Collections.Generic;

namespace EquipmentRentalAPI.Models;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public string SerialNumber { get; set; } = null!;

    public int ModelId { get; set; }

    public virtual Models Model { get; set; } = null!;

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
