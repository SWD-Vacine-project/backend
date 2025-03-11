using System;
using System.Collections.Generic;

namespace Vaccine.Repo.Entities;

public partial class Vaccine
{
    public int VaccineId { get; set; }

    public string Name { get; set; } = null!;

    public int? MaxLateDate { get; set; }

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public int? InternalDurationDoses { get; set; }


}
