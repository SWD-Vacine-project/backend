using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Vaccine.API.Models.StaffModel;
using Vaccine.Repo.Entities;
using Vaccine.Repo.UnitOfWork;

namespace Vaccine.API.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public StaffController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("create-staff")]
        public async Task<IActionResult> CreateStaff([FromBody] RequestCreateStaffModel newStaff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra số staff đã tồn tại chưa (phone, email, userName)
            var existingStaff = _unitOfWork.StaffRepository
                .Get(x => x.Phone == newStaff.Phone || x.Email == newStaff.Email || x.UserName == newStaff.UserName )
                .FirstOrDefault();
           

            if (existingStaff != null)
            {
                return BadRequest("Staff is already exists.");
            }

            var staff = new Staff
            {
                Name = newStaff.Name,
                Dob = newStaff.Dob,
                Gender = newStaff.Gender,
                Phone = newStaff.Phone,
                Email = newStaff.Email,
                Role = newStaff.Role,
                UserName = newStaff.UserName,
                Password = newStaff.Password,
                Status = "Active"

            };

            _unitOfWork.StaffRepository.Insert(staff);
            await _unitOfWork.SaveAsync();

            return Ok(new { Message = "Staff created successfully", staff });
        }

        [HttpPut("update-staff/{id}")]
        public IActionResult UpdateStaff(int id, RequestUpdateStaffModel updateStaff)
        {
            if (updateStaff == null)
            {
                return BadRequest(new { message = "Staff data is required" });
            }

            var existingStaff = _unitOfWork.StaffRepository
               .Get(x => x.Phone == updateStaff.Phone || x.Email == updateStaff.Email)
               .FirstOrDefault();

            if (existingStaff != null)
            {
                return BadRequest("Staff is already exists.");
            }

            // Tìm staff theo ID
            var staff = _unitOfWork.StaffRepository.GetByID(id);
            if (staff == null)
            {
                return NotFound(new { message = "Staff not found" });
            }

            // Cập nhật thông tin staff
            staff.Name = updateStaff.Name;
            staff.Gender = updateStaff.Gender;
            staff.Phone = updateStaff.Phone;
            staff.Email = updateStaff.Email;
            staff.Role = updateStaff.Role;
            staff.Password = updateStaff.Password;

            // Lưu thay đổi vào database
            _unitOfWork.StaffRepository.Update(staff);
            _unitOfWork.Save();

            return Ok(staff);
        }

        [HttpPut("update-status-staff/{id}/{status}")]
        public IActionResult UpdateStatusStaff(int id, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { message = "Status is required" });
            }

            // Tìm staff theo ID
            var staff = _unitOfWork.StaffRepository.GetByID(id);
            if (staff == null)
            {
                return NotFound(new { message = "Staff not found" });
            }

            // Cập nhật thông tin staff
            staff.Status = status;

            // Lưu thay đổi vào database
            _unitOfWork.StaffRepository.Update(staff);
            _unitOfWork.Save();

            return Ok(staff);
        }

        [HttpDelete("delete-status-staff/{id}")]
        public IActionResult DeleteStaff(int id)
        {
            // Tìm staff theo ID
            var staff = _unitOfWork.StaffRepository.GetByID(id);
            if (staff == null)
            {
                return NotFound(new { message = "Staff not found" });
            }

            if (staff.Status == "Inactive")
            {
                return BadRequest(new { message = "Staff has been deleted" });
            }


            // Cập nhật thông tin staff
            staff.Status = "Inactive";

            // Lưu thay đổi vào database
            _unitOfWork.StaffRepository.Update(staff);
            _unitOfWork.Save();

            return Ok(staff);
        }

        [HttpGet("get-staff-by-id/{id}")]
        public IActionResult GetStaffById(int id) {
            // 
            var staff = _unitOfWork.StaffRepository.GetQueryable().Where(x => x.StaffId == id).Select(x => new
            {
                x.StaffId,
                x.Name,
                x.Role,
                x.Gender,
                x.Phone,
                x.Dob,
                x.Email,
                x.Status,
                x.UserName
            }).FirstOrDefault();

            if (staff == null)
            {
                return NotFound(new { message = "Staff not found" });
            }

            return Ok(staff);

        }

        [HttpGet("get-staffs")]
        public IActionResult GetStaffs()
        {
            var staffs = _unitOfWork.StaffRepository.GetQueryable().Select(x => new
            {
                x.StaffId,
                x.Name,
                x.Role,
                x.Gender,
                x.Phone,
                x.Dob,
                x.Email,
                x.Status,
                x.UserName
            }).ToList();

            if (staffs.Count == 0)
            {
                return Ok();
            }

            return Ok(staffs);

        }
    }
}
