using System;
using System.Collections.Generic;

namespace Vaccine.Repo.Entities;

public partial class VaccineReaction
{
    public int ReactionId { get; set; }

    public int? RecordId { get; set; }

    public string? ReactionDescription { get; set; }

    public string? Severity { get; set; }

    public DateTime? OnsetTime { get; set; }

    public DateTime? ResolvedTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual VaccinationRecord? Record { get; set; }
}
