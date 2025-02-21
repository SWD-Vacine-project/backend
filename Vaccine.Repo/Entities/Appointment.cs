using System;
using System.Collections.Generic;

namespace Vaccine.Repo.Entities;
public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int? CustomerId { get; set; }

    public int? ChildId { get; set; }

    public int? StaffId { get; set; }

    public int? DoctorId { get; set; }

    public string? VaccineType { get; set; }

    public int? VaccineId { get; set; }

    public int? ComboId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Child? Child { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<HealthRecord> HealthRecords { get; set; } = new List<HealthRecord>();

    public virtual Staff? Staff { get; set; }
}
