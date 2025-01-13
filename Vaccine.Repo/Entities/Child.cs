using System;
using System.Collections.Generic;


namespace Vaccine.Repo.Entities;

public partial class Child
{
    public int ChildId { get; set; }

    public int? UserId { get; set; }

    public string? FullName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? BloodType { get; set; }

    public string? AllergiesNotes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual UserAccount? User { get; set; }
}
