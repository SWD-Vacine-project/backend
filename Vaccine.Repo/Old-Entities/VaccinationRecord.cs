using System;
using System.Collections.Generic;

namespace Vaccine.Repo.Entities;

public partial class VaccinationRecord
{
    public int RecordId { get; set; }

    public int? AppointmentId { get; set; }

    public DateTime? VaccineDate { get; set; }

    public int? DoseNumber { get; set; }

    public string? BatchNumber { get; set; }

    public string? AdministeringStaff { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual ICollection<VaccineReaction> VaccineReactions { get; set; } = new List<VaccineReaction>();
}
