using System;
using System.Collections.Generic;


namespace Vaccine.Repo.Entities;
public partial class Notification
{
    public int NotificationId { get; set; }

    public int? UserId { get; set; }

    public int? ChildId { get; set; }

    public string? Type { get; set; }

    public string? Message { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Child? Child { get; set; }

    public virtual UserAccount? User { get; set; }
}
