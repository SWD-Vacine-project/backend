using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.API.Models.CustomerModel;
using Swashbuckle.AspNetCore.Annotations;
using Vaccine.API.Models;
using Vaccine.API.Models.CustomerModel;
using Vaccine.Repo.Entities;
using Vaccine.Repo.UnitOfWork;

namespace Vaccine.API.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class HealthRecordController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public HealthRecordController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("get-health-records")]
        public IActionResult GetHealthRecords()
        {
            var healths = _unitOfWork.HealthRecordRepository.Get();
            return Ok(healths);
        }

        [HttpGet("get-health/{id}")]
        public IActionResult GetHealthRecordsByID(int id)
        {
            var health = _unitOfWork.HealthRecordRepository.GetByID(id);
            if (health == null)
            {
                return NotFound(new { message = "Health records not found." });
            }
            return Ok(health);
        }

        [HttpPost("create-health-record")]
        [SwaggerOperation(Description = "Creates a health record. Requires appointment_id.")]
        public IActionResult CreateHealthRecord([FromBody] RequestCreateHealthRecordsModel request)
        {
            if (request == null || request.AppointmentId == null)
            {
                return BadRequest(new { message = "Appointment ID is required." });
            }
            // para input to create 
            var healthEntity = new HealthRecord
            {
                StaffId = request.StaffId == 0 ? null : request.StaffId,
                AppointmentId = request.AppointmentId ,
                DoctorId = request.DoctorId == 0 ? null : request.StaffId,
                BloodPressure = request.BloodPressure,
                HeartRate = request.HeartRate,
                Height = request.Height,
                Weight = request.Weight,
                Temperature = request.Temperature,
                AteBeforeVaccine = request.AteBeforeVaccine,
                ConditionOk = request.ConditionOk,
                ReactionNotes = request.ReactionNotes,
                CreatedAt = request.CreatedAt ?? DateTime.UtcNow,
            };

            _unitOfWork.HealthRecordRepository.Insert(healthEntity);
            _unitOfWork.Save();
            return Ok(healthEntity);
        }

        

        [HttpDelete("delete-appointment/{id}")]

        public IActionResult DeleteAppointment(int id)
        {
            var appointment = _unitOfWork.AppointmentRepository.GetByID(id);
            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }
            _unitOfWork.AppointmentRepository.Delete(appointment);
            _unitOfWork.Save();
            return Ok(new { message = "Appointment deleted successfully." });
        }


        [HttpPut("update-appointment-date/{id}")]
        public async Task<IActionResult> UpdateAppointmentDate(int id, [FromBody] DateTime newDate)
        {
            var appointment = _unitOfWork.AppointmentRepository.GetByID(id);
            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }

            // Ensure the new date is not before the existing date
            if (newDate < appointment.AppointmentDate)
            {
                return BadRequest(new { message = "Cannot update to a past date." });
            }

            // Update only the AppointmentDate field
            appointment.AppointmentDate = newDate;
            _unitOfWork.Save();

            return Ok(new { message = "Appointment date updated successfully." });
        }

    }
}
