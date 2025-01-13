using System;
using System.Collections.Generic;


namespace Vaccine.Repo.Entities;

public partial class PackageDetail
{
    public int DetailId { get; set; }

    public int? PackageId { get; set; }

    public int? ServiceId { get; set; }

    public virtual ServicePackage? Package { get; set; }

    public virtual Vaccine? Service { get; set; }
}
