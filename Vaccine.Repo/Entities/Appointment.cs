using System;
using System.Collections.Generic;


namespace Vaccine.Repo.Entities;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int? ChildId { get; set; }

    public int? ServiceId { get; set; }

    public int? PackageId { get; set; }

    public DateTime? AppointmentDate { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public bool? IsConfirmed { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public int? ConfirmedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Child? Child { get; set; }

    public virtual UserAccount? ConfirmedByNavigation { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ServicePackage? Package { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Vaccine? Service { get; set; }

    public virtual ICollection<VaccinationRecord> VaccinationRecords { get; set; } = new List<VaccinationRecord>();
}
