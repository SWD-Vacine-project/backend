﻿using System;
using System.Collections.Generic;

namespace Vaccine.Repo.Models;

public partial class InvoiceDetail
{
    public int DetailId { get; set; }

    public int? InvoiceId { get; set; }

    public int? VaccineId { get; set; }

    public int? AppointmentId { get; set; }

    public int? ComboId { get; set; }

    public int? Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Invoice? Invoice { get; set; }
}
