using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vaccine.API.Models.FeedbackModel;
using Vaccine.Repo.Entities;
using Vaccine.Repo.UnitOfWork;

namespace Vaccine.API.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("[controller]")]
    [ApiController]
    public class FeedBackController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public FeedBackController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("create-holiday")]
        public async Task<IActionResult> CreateHoliday([FromBody] RequestCreateFeedBackModel newFeedBack)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem CustomerId có đúng không
            var customer = _unitOfWork.CustomerRepository
                .GetQueryable().Where(x => x.CustomerId == newFeedBack.CustomerId)
                .FirstOrDefault();
            // Kiểm tra xem DoctorId có đúng không
            var doctor = _unitOfWork.DoctorRepository
               .GetQueryable().Where(x => x.DoctorId == newFeedBack.DoctorId)
               .FirstOrDefault();
            // Kiểm tra xem StaffId có đúng không
            var staff = _unitOfWork.StaffRepository
               .GetQueryable().Where(x => x.StaffId == newFeedBack.StaffId)
               .FirstOrDefault();
            // Kiểm tra xem VaccineId có đúng không
            var vaccine = _unitOfWork.VaccineRepository
               .GetQueryable().Where(x => x.VaccineId == newFeedBack.VaccineId)
               .FirstOrDefault();
            // Kiểm tra xem AppointmentId có đúng không
            var appointment = _unitOfWork.AppointmentRepository
               .GetQueryable().Where(x => x.AppointmentId == newFeedBack.AppointmentId)
               .FirstOrDefault();

            if (customer == null)
            {
                return BadRequest("Customer not found");
            }

            if (doctor == null)
            {
                return BadRequest("Doctor not found");
            }

            if (staff == null)
            {
                return BadRequest("Staff not found");
            }

            if (vaccine == null)
            {
                return BadRequest("Vaccine not found");
            }

            if (appointment == null)
            {
                return BadRequest("Appointment not found");
            }

            var feedBack = new Feedback
            {
                CustomerId = newFeedBack.CustomerId,
                DoctorId = newFeedBack.DoctorId,
                StaffId = newFeedBack.StaffId,
                VaccineId = newFeedBack.VaccineId,
                AppointmentId = newFeedBack.AppointmentId,
                Rating = newFeedBack.Rating,
                Comment = newFeedBack.Comment,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            _unitOfWork.FeedbackRepository.Insert(feedBack);
            await _unitOfWork.SaveAsync();

            return Ok(new { Message = "FeedBack created successfully", feedBack });
        }

        [HttpGet("get-feedback-by-id/{id}")]
        public IActionResult GetFeedbackById(int id)
        {
            // 
            var feedback = _unitOfWork.FeedbackRepository.GetQueryable().Include(x => x.Customer)
                                                                        .Include(x => x.Appointment)
                                                                        .Include(x => x.Doctor)
                                                                        .Include(x => x.Staff)
                                                                        .Where(x => x.ReviewId == id)
                                                                        .Select(x => new
                                                                        {
                                                                            x.ReviewId,
                                                                            x.CustomerId,
                                                                            x.StaffId,
                                                                            x.VaccineId,
                                                                            x.AppointmentId,
                                                                            x.Rating,
                                                                            x.Comment,
                                                                            x.CreatedAt,
                                                                            x.UpdatedAt,
                                                                        }).FirstOrDefault();

            if (feedback == null)
            {
                return NotFound(new { message = "Feedback not found" });
            }

            return Ok(feedback);

        }

        [HttpGet("get-feedback")]
        public IActionResult GetFeedbacks()
        {
            var feedback = _unitOfWork.FeedbackRepository.GetQueryable().Include(x => x.Customer)
                                                                       .Include(x => x.Appointment)
                                                                       .Include(x => x.Doctor)
                                                                       .Include(x => x.Staff)
                                                                       .Select(x => new
                                                                       {
                                                                           x.ReviewId,
                                                                           x.CustomerId,
                                                                           x.StaffId,
                                                                           x.VaccineId,
                                                                           x.AppointmentId,
                                                                           x.Rating,
                                                                           x.Comment,
                                                                           x.CreatedAt,
                                                                           x.UpdatedAt,
                                                                       }).ToList();

            if (feedback.Count == 0)
            {
                return Ok();
            }
            return Ok(feedback);

        }
    }
}
