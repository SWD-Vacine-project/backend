using System;
using System.Collections.Generic;

namespace Vaccine.Repo.Entities;
public partial class VaccineBatchDetail
{
    public string? BatchNumber { get; set; }

    public int? VaccineId { get; set; }

    public int? Quantity { get; set; }

    public virtual VaccineBatch? BatchNumberNavigation { get; set; }

    public virtual Vaccine? Vaccine { get; set; }
}
