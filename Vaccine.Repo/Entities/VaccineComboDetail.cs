using System;
using System.Collections.Generic;

namespace Vaccine.Repo.Models;

public partial class VaccineComboDetail
{
    public int? ComboId { get; set; }

    public int? VaccineId { get; set; }

    public virtual VaccineCombo? Combo { get; set; }

    public virtual Vaccine? Vaccine { get; set; }
}
