using System;
using System.Collections.Generic;

namespace HealthcareApi.Models;

public partial class Doctor
{
    public int IdDoctor { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
