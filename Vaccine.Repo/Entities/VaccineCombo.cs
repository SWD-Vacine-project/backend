using System;
using System.Collections.Generic;

namespace Vaccine.Repo.Models;

public partial class VaccineCombo
{
    public int ComboId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }
}
