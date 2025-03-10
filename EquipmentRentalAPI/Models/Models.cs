using System;
using System.Collections.Generic;

namespace EquipmentRentalAPI.Models;

public partial class Models
{
    public int ModelId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
