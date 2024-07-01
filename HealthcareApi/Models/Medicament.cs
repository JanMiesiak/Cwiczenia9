using System;
using System.Collections.Generic;

namespace HealthcareApi.Models;

public partial class Medicament
{
    public int IdMedicament { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; } = new List<PrescriptionMedicament>();
}
