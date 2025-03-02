using Azure.Core;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkStore.API.Models.CustomerModel;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Vaccine.API.Models.CustomerModel;
using Vaccine.Repo.Entities;
using Vaccine.Repo.UnitOfWork;

namespace Vaccine.API.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public AppointmentController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet("get-appointments")]
        public IActionResult GetAppointments()
        {
            var appointments = _unitOfWork.AppointmentRepository.Get();
            return Ok(appointments);
        }

        [HttpGet("get-appointment/{id}")]
        public IActionResult GetAppointment(int id)
        {
            var appointment = _unitOfWork.AppointmentRepository.GetByID(id);
            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }
            return Ok(appointment);
        }

        [HttpPost("create-appointment")]
        public IActionResult CreateAppointment(RequestCreateInvoice requestCreateAppointmentModel)
        {
            //var existingappoint = _unitofwork.appointmentrepository.get(c => c.appointmentid == requestcreatecappointmentmodel.appointmentid).firstordefault();
            //if (existingappoint != null)
            //{
            //    return badrequest(new { message = "appointment đã tồn tại." });
            //}
            

            // para input to create 
            var appointEntity = new Appointment
            {
                //AppointmentId = requestCreateCAppointmentModel.AppointmentId,
                AppointmentDate = requestCreateAppointmentModel.AppointmentDate,
                Status = requestCreateAppointmentModel.Status,
                Notes = requestCreateAppointmentModel.Notes,
                CreatedAt = requestCreateAppointmentModel.CreatedAt,
                ChildId = requestCreateAppointmentModel.ChildId == 0 ? null : requestCreateAppointmentModel.ChildId,
                StaffId = requestCreateAppointmentModel.StaffId == 0 ? null : requestCreateAppointmentModel.StaffId,
                DoctorId = requestCreateAppointmentModel.DoctorId == 0 ? null : requestCreateAppointmentModel.DoctorId,
                // if null set null, not null require single or combo
                VaccineType = string.IsNullOrEmpty(requestCreateAppointmentModel.VaccineType) ? null : requestCreateAppointmentModel.VaccineType,
                ComboId = requestCreateAppointmentModel.ComboId == 0 ? null : requestCreateAppointmentModel.ComboId,
                CustomerId = requestCreateAppointmentModel.CustomerId == 0 ? null : requestCreateAppointmentModel.CustomerId,
                VaccineId = requestCreateAppointmentModel.VaccineId == 0 ? null : requestCreateAppointmentModel.VaccineId,
            };

            _unitOfWork.AppointmentRepository.Insert(appointEntity);
            _unitOfWork.Save();

            var responseAppoint = new RequestResponeAppointmentModel
            {
                AppointmentId = appointEntity.AppointmentId,
                AppointmentDate = requestCreateAppointmentModel.AppointmentDate,
                Status = requestCreateAppointmentModel.Status,
                Notes = requestCreateAppointmentModel.Notes,
                CreatedAt = requestCreateAppointmentModel.CreatedAt,
                ChildId = requestCreateAppointmentModel.ChildId,
                StaffId = requestCreateAppointmentModel.StaffId,
                DoctorId = requestCreateAppointmentModel.DoctorId,
                VaccineType = requestCreateAppointmentModel.VaccineType,
                ComboId = requestCreateAppointmentModel.ComboId,
                CustomerId = requestCreateAppointmentModel.CustomerId,
                VaccineId = requestCreateAppointmentModel.VaccineId,
            };

            return Ok(responseAppoint);
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
