using Microsoft.AspNetCore.Mvc;
using HealthcareApi.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace HealthcareApi.Controllers
{
    [Route("api/adding_prescription")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly Apbd9Context _context;

        public PrescriptionController(Apbd9Context context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddPrescription([FromBody] PrescriptionRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid data.");
            }
            // Check if number of medicaments exceeds limit
            if (request.Medicaments.Count > 10)
            {
                return BadRequest("A prescription can contain no more than 10 medicaments.");
            }
            // Validate each medicament
            foreach (var med in request.Medicaments)
            {
                var existingMedicament = await _context.Medicaments.FindAsync(med.IdMedicament);
                if (existingMedicament == null)
                {
                    return BadRequest($"Medicament with ID {med.IdMedicament} does not exist.");
                }
            }
            
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.IdPatient == request.Patient.IdPatient);
            if (patient == null)
            {
                patient = new Patient
                {
                    IdPatient = request.Patient.IdPatient,
                    FirstName = request.Patient.FirstName,
                    LastName = request.Patient.LastName,
                    Birthdate = request.Patient.Birthdate
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync(); // Save the new patient to get the ID
            }

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.IdDoctor == request.Doctor.IdDoctor);
            if (doctor == null)
            {
                doctor = new Doctor
                {
                    IdDoctor = request.Doctor.IdDoctor,
                    FirstName = request.Doctor.FirstName,
                    LastName = request.Doctor.LastName,
                    Email = request.Doctor.Email
                };
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync(); // Save the new doctor to get the ID
            }
            
            // Validate DueDate >= Date
            if (request.Prescription.DueDate < request.Prescription.Date)
            {
                return BadRequest("DueDate must be greater than or equal to Date.");
            }
            
            var prescription = new Prescription
            {
                IdPrescription = request.Prescription.IdPrescription,
                Date = request.Prescription.Date,
                DueDate = request.Prescription.DueDate,
                IdPatient = patient.IdPatient,
                IdDoctor = doctor.IdDoctor
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync(); // Save the prescription to get the ID

            foreach (var med in request.Medicaments)
            {
                var prescriptionMedicament = new PrescriptionMedicament
                {
                    IdMedicament = med.IdMedicament,
                    IdPrescription = prescription.IdPrescription,
                    Dose = med.Dose,
                    Details = med.Details
                };
                _context.PrescriptionMedicaments.Add(prescriptionMedicament);
            }

            await _context.SaveChangesAsync(); // Save all prescription medicaments

            return Ok("Prescription added successfully.");
        }
    }

    public class PrescriptionRequest
    {
        public PatientRequest Patient { get; set; }
        public DoctorRequest Doctor { get; set; }
        public PrescriptionRequestDetails Prescription { get; set; }
        public List<MedicamentRequest> Medicaments { get; set; }
    }

    public class PatientRequest
    {
        public int IdPatient { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
    }

    public class DoctorRequest
    {
        public int IdDoctor { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class PrescriptionRequestDetails
    {
        public int IdPrescription { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class MedicamentRequest
    {
        public int IdMedicament { get; set; }
        public int Dose { get; set; }
        public string Details { get; set; }
    }
}
