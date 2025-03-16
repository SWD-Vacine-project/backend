using Azure.Core;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkStore.API.Models.CustomerModel;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net.WebSockets;
using System.Reflection.PortableExecutable;
using System.Runtime;
using Vaccine.API.Models.AppointmentModel;
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

        [HttpGet("get-appointment-by-childid/{child_id}")]
        public IActionResult GetAppointmentsByChildId(int child_id)
        {
            var appointments = _unitOfWork.AppointmentRepository
                .Get(filter: a => a.ChildId == child_id);

            if (!appointments.Any())
            {
                return NotFound(new { message = "No appointments found for this child." });
            }
            return Ok(appointments);
        }
        [HttpGet("get-appointment-checkin")]
        public IActionResult GetAppointmentsForCheckin()
        {

            var allAppointments = _unitOfWork.AppointmentRepository.Get().ToList();

            var appointments = allAppointments
                .Where(x => (x.Status.Trim().ToLower() == "late" || (x.Status.Trim().ToLower() == "approved"
                            && x.AppointmentDate.ToLocalTime().Date == DateTime.Today.Date)))
                .ToList();

            if (appointments == null)
            {
                return NotFound(new { message = "No appointments found for today" });
            }
            return Ok(appointments);
        }
        [HttpPatch("set-appointment-inprogress/{appointmentId}")]
        public IActionResult SetAppointmentInprogress(int appointmentId)
        {
            var appointment = _unitOfWork.AppointmentRepository.GetByID(appointmentId);
            if (appointment == null)
            {
                return NotFound(new { message = "Cannot find appointment" });
            }
            appointment.Status = "InProgress";
            _unitOfWork.AppointmentRepository.Update(appointment);
            _unitOfWork.Save();
            return Ok(new { message = $"Sucessfully update status of appointment: {appointment.AppointmentId}" });
        }
        [HttpGet("get-appointment-pending")]
        public IActionResult GetAppointmentPenidng()
        {
            var appointments = _unitOfWork.AppointmentRepository.Get(filter: x => x.Status == "Pending");
            if (appointments.Count() == 0)
            {
                return Ok(new { message = "No pending appointments found", appointments = new List<Appointment>() });
            }
            return Ok(appointments);
        }
        //[HttpPost("create-appointment")]
        //[SwaggerOperation(
        //    Description = "Create appointment with status = Pending "
        //)]
        //public IActionResult CreateAppointment(RequestCreateAppointmentModel requestCreateAppointmentModel)
        //{
        //    //var existingappoint = _unitofwork.appointmentrepository.get(c => c.appointmentid == requestcreatecappointmentmodel.appointmentid).firstordefault();
        //    //if (existingappoint != null)
        //    //{
        //    //    return badrequest(new { message = "appointment đã tồn tại." });
        //    //}


        //    // para input to create 
        //    var appointEntity = new Appointment
        //    {
        //        //AppointmentId = requestCreateCAppointmentModel.AppointmentId,
        //        AppointmentDate = requestCreateAppointmentModel.AppointmentDate,
        //        Status = "Pending",
        //        Notes = requestCreateAppointmentModel.Notes,
        //        CreatedAt = requestCreateAppointmentModel.CreatedAt,
        //        ChildId = requestCreateAppointmentModel.ChildId,
        //        StaffId = null,
        //        DoctorId = null,
        //        // if null set null, not null require single or combo
        //        VaccineType = string.IsNullOrEmpty(requestCreateAppointmentModel.VaccineType) ? null : requestCreateAppointmentModel.VaccineType,
        //        ComboId = requestCreateAppointmentModel.ComboId,
        //        CustomerId = requestCreateAppointmentModel.CustomerId,
        //        VaccineId = requestCreateAppointmentModel.VaccineId,
        //    };

        //    _unitOfWork.AppointmentRepository.Insert(appointEntity);
        //    _unitOfWork.Save();

        //    var responseAppoint = new RequestResponeAppointmentModel
        //    {
        //        AppointmentId = appointEntity.AppointmentId,
        //        AppointmentDate = requestCreateAppointmentModel.AppointmentDate,
        //        Status = "Pending",
        //        Notes = requestCreateAppointmentModel.Notes,
        //        CreatedAt = requestCreateAppointmentModel.CreatedAt,
        //        ChildId = requestCreateAppointmentModel.ChildId,
        //        StaffId = null, 
        //        DoctorId = null,
        //        VaccineType = requestCreateAppointmentModel.VaccineType,
        //        ComboId = requestCreateAppointmentModel.ComboId,
        //        CustomerId = requestCreateAppointmentModel.CustomerId,
        //        VaccineId = requestCreateAppointmentModel.VaccineId,
        //    };

        //    return Ok(responseAppoint);
        //}

        [HttpPut("Approved-status-appointment/{id}")]
        [SwaggerOperation(
            Description = "Confirm an appointment by setting the status to Approved."
        )]
        public IActionResult ApprovedAppointment(int id)
        {
            var appointment = _unitOfWork.AppointmentRepository.GetByID(id);
            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }
            // Ensure the appointment is not already confirmed
            if (appointment.Status == "Approved")
            {
                return BadRequest(new { message = "Appointment is already confirmed." });
            }
            var batchPreOrderQuanity = _unitOfWork.VaccineBatchDetailRepository.Get(filter: x => x.BatchNumber == appointment.BatchNumber && x.VaccineId == appointment.VaccineId).FirstOrDefault();

            if (string.IsNullOrEmpty(appointment.BatchNumber))
            {
                return BadRequest(new { message = "Appointment batch number is missing" });
            }
            if (batchPreOrderQuanity.Quantity <= 0)
            {
                return BadRequest(new { message = "Not enough quanity in batch for this vaccine." });
            }
            // Update the appointment status to Confirmed
            appointment.Status = "Approved";
            batchPreOrderQuanity.PreOrderQuantity += 1;
            _unitOfWork.AppointmentRepository.Update(appointment);
            _unitOfWork.VaccineBatchDetailRepository.Update(batchPreOrderQuanity);
            _unitOfWork.Save();
            return Ok(new { message = "Appointment approved successfully." });
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

        //[HttpPut("update-appointment-status/{id}")]
        //[SwaggerOperation(
        //    Description = "Update appointment status"
        //)]
        //public IActionResult UpdateAppointmentStatus(int id)
        //{
        //    var appointment = _unitOfWork.AppointmentRepository.GetByID(id);
        //    if (appointment == null)
        //    {
        //        return NotFound(new { message = "Appointment not found." });
        //    }
        //    // Ensure the appointment is not already confirmed
        //    if (appointment.Status == "Approved")
        //    {
        //        return BadRequest(new { message = "Appointment is already confirmed." });
        //    }
        //    // Update the appointment status to Confirmed
        //    appointment.Status = "Approved";
        //    _unitOfWork.AppointmentRepository.Update(appointment);
        //    _unitOfWork.Save();
        //    return Ok(new { message = "Appointment approved successfully." });
        //}


        //[HttpPost("create-appointment-combo")]
        //[SwaggerOperation(
        //    Description = "Create appointment when purchase combo"
        //)]
        //public IActionResult CreateAppointmentsCombo(RequestCreateComboAppointment request)
        //{
        //    if (request == null)
        //    {
        //        return BadRequest(new { message = "Combo data is required" });
        //    }
        //    var comboDetails = _unitOfWork.VaccineComboDetailRepository.Get(filter: x => x.ComboId == request.ComboId).ToList();
        //    if (comboDetails == null || !comboDetails.Any())
        //    {
        //        return BadRequest(new { message = "ComboId is not found" });
        //    }
        //    var appointmentList = new List<Appointment>();
        //    DateTime currentAppointmentDate = request.AppointmentDate;
        //    foreach (var detail in comboDetails)
        //    {
        //        // tìm kiến vaccine theo vaccineID
        //        var vaccine = _unitOfWork.VaccineRepository.Get(filter: x => x.VaccineId == detail.VaccineId).FirstOrDefault();
        //        if (vaccine == null)
        //        {
        //            return BadRequest(new { message = $"Vaccine Error: {detail.VaccineId} does not contain internalDurationDay" });
        //        }
        //        // từ vaccineID => truy xuất ra ngày cần tiêm tiếp theo
        //        int durationDays = vaccine.InternalDurationDoses;
        //        // Kiểm tra tổng số vaccine theo vaccineID trong kho
        //        var totalStock = _unitOfWork.VaccineBatchDetailRepository.Get(
        //            v => v.VaccineId == detail.VaccineId && v.BatchNumberNavigation.ExpiryDate > DateOnly.FromDateTime(currentAppointmentDate.AddDays(vaccine.MaxLateDate)), includeProperties: "BatchNumberNavigation"
        //        ).Sum(v => v.Quantity);

        //        if (totalStock < 10)
        //        {
        //            return BadRequest(new { message = $"Insufficient stock for vaccine {detail.VaccineId}. Appointment combo cannot be scheduled." });
        //        }
        //                       // tạo mới appointment cho lần tiêm tiếp theo
        //        var appointment = new Appointment
        //        {
        //            AppointmentDate = currentAppointmentDate,
        //            Status = totalStock >= 10 ? "Approved" : "Pending",
        //            Notes = request.Notes,
        //            CreatedAt = DateTime.Now,
        //            ChildId = request.ChildId,
        //            StaffId = null,  // Nhân viên xử lý nếu Pending
        //            DoctorId = null,
        //            VaccineType = "Combo",
        //            ComboId = request.ComboId,
        //            CustomerId = request.CustomerId,
        //            VaccineId = detail.VaccineId
        //        };
        //        if (totalStock >= 10)
        //        {
        //            // chọn batch của vaccine với điều kiện( gần hết ngày hết hạn nhất & số lượng còn ít trước)
        //            var availableBatch = _unitOfWork.VaccineBatchDetailRepository.Get(x => x.VaccineId == vaccine.VaccineId && x.Quantity > 0 && x.BatchNumberNavigation.ExpiryDate > DateOnly.FromDateTime(currentAppointmentDate.Date.AddDays(vaccine.MaxLateDate)), includeProperties: "BatchNumberNavigation")
        //              .OrderBy(v => v.BatchNumberNavigation.ExpiryDate)
        //              .ThenBy(v => v.Quantity)
        //              .FirstOrDefault();
        //            if(availableBatch != null)
        //            {
        //                appointment.BatchNumber= availableBatch.BatchNumber;
        //                //availableBatch.PreOrderQuantity++;
        //            }

        //        }
        //        appointmentList.Add(appointment);

        //        currentAppointmentDate = currentAppointmentDate.AddDays(durationDays);
        //    }
        //    _unitOfWork.AppointmentRepository.InsertRange(appointmentList);

        //    _unitOfWork.Save();
        //    return Ok(new
        //    {
        //        message = appointmentList.All(a => a.Status == "Approved") ?
        //   "Tất cả lịch hẹn trong combo đã được chấp nhận." :
        //   "Một số lịch hẹn đang chờ xác nhận từ nhân viên.",
        //        Appointments = appointmentList
        //    });

        //}
        [HttpPost("create-appointment-combo")]
        [SwaggerOperation(Description = "Tạo lịch hẹn khi khách hàng mua combo vaccine")]
        public IActionResult CreateAppointmentsCombo(RequestCreateComboAppointment request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Thiếu dữ liệu combo" });
            }

            var comboDetails = _unitOfWork.VaccineComboDetailRepository.Get(filter: x => x.ComboId == request.ComboId).ToList();
            if (comboDetails == null || !comboDetails.Any())
            {
                return BadRequest(new { message = "Không tìm thấy ComboId" });
            }

            DateTime currentAppointmentDate = request.AppointmentDate;

            // **Bước 1: Kiểm tra tồn kho của tất cả vaccine trước khi tạo lịch hẹn**
            foreach (var detail in comboDetails)
            {
                var vaccine = _unitOfWork.VaccineRepository.Get(filter: x => x.VaccineId == detail.VaccineId).FirstOrDefault();
                if (vaccine == null)
                {
                    return BadRequest(new { message = $"Lỗi vaccine: Không tìm thấy VaccineId {detail.VaccineId}." });
                }

                var totalStock = _unitOfWork.VaccineBatchDetailRepository.Get(
                    v => v.VaccineId == detail.VaccineId && v.BatchNumberNavigation.ExpiryDate > DateOnly.FromDateTime(currentAppointmentDate.AddDays(vaccine.MaxLateDate)),
                    includeProperties: "BatchNumberNavigation"
                ).Sum(v => v.Quantity);

                if (totalStock < 10)
                {
                    return BadRequest(new { message = $"Không đủ hàng cho vaccine {detail.VaccineId}. Combo bị từ chối." });
                }
            }

            // **Bước 2: Tạo danh sách lịch hẹn vì tất cả vaccine đều đủ hàng**
            var appointmentList = new List<Appointment>();

            foreach (var detail in comboDetails)
            {
                var vaccine = _unitOfWork.VaccineRepository.Get(filter: x => x.VaccineId == detail.VaccineId).FirstOrDefault();
                int durationDays = vaccine.InternalDurationDoses;

                // Chọn lô vaccine có ngày hết hạn gần nhất và số lượng ít nhất trước
                var availableBatch = _unitOfWork.VaccineBatchDetailRepository.Get(
                    x => x.VaccineId == vaccine.VaccineId && x.Quantity > 0 && x.BatchNumberNavigation.ExpiryDate > DateOnly.FromDateTime(currentAppointmentDate.Date.AddDays(vaccine.MaxLateDate)),
                    includeProperties: "BatchNumberNavigation"
                )
                .OrderBy(v => v.BatchNumberNavigation.ExpiryDate)
                .ThenBy(v => v.Quantity)
                .FirstOrDefault();

                var appointment = new Appointment
                {
                    AppointmentDate = currentAppointmentDate,
                    Status = "Approved",  // ✅ Vì tất cả vaccine đã đủ hàng, trạng thái luôn là "Approved"
                    Notes = request.Notes,
                    CreatedAt = DateTime.Now,
                    ChildId = request.ChildId,
                    StaffId = null,
                    DoctorId = null,
                    VaccineType = "Combo",
                    ComboId = request.ComboId,
                    CustomerId = request.CustomerId,
                    VaccineId = detail.VaccineId
                };

                if (availableBatch != null)
                {
                    appointment.BatchNumber = availableBatch.BatchNumber;
                    availableBatch.PreOrderQuantity++;
                }

                appointmentList.Add(appointment);
                currentAppointmentDate = currentAppointmentDate.AddDays(durationDays);
            }

            // **Bước 3: Lưu tất cả lịch hẹn vào database**
            _unitOfWork.AppointmentRepository.InsertRange(appointmentList);
            _unitOfWork.Save();

            return Ok(new
            {
                message = "Tất cả lịch hẹn trong combo đã được chấp nhận.",
                Appointments = appointmentList
            });
        }

        [HttpPost("create-appointment")]
        [SwaggerOperation(
            Description = "Create appointment single"
        )]
        public IActionResult CreateAppointment(RequestCreateAppointmentModel request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Appointment data is required !" });
            }
            // kiểm tra số lượng trong kho => nếu còn đủ thì set status approve
            // xét 2 điều kiện, một là quantity >10, hai là  expireDate phải lớn hơn ngày đặt lịch + số ngày giới hạn của policy dời lịch: 3 ngày
            var vaccine = _unitOfWork.VaccineRepository.Get(x => x.VaccineId == request.VaccineId).FirstOrDefault();
            if (vaccine == null)
            {
                return BadRequest(new { message = $"Vaccine with id {request.VaccineId} is not found !" });
            }
            var totalStock = _unitOfWork.VaccineBatchDetailRepository.Get(
                   v => v.VaccineId == request.VaccineId && v.BatchNumberNavigation.ExpiryDate > DateOnly.FromDateTime(request.AppointmentDate.Date.AddDays(vaccine.MaxLateDate)), includeProperties: "BatchNumberNavigation").Sum(v => v.Quantity);
            //Nếu không đủ vaccine, giữ trạng thái "Pending" và gửi cho nhân viên xử lý
            if (totalStock < 10)
            {
                var appointEntity = new Appointment
                {
                    AppointmentDate = request.AppointmentDate,
                    Status = "Pending",
                    Notes = request.Notes,
                    CreatedAt = request.CreatedAt,
                    ChildId = request.ChildId,
                    StaffId = null,
                    DoctorId = null,
                    // if null set null, not null require single or combo
                    VaccineType = "Single",
                    ComboId = null,
                    CustomerId = request.CustomerId,
                    VaccineId = request.VaccineId,
                    BatchNumber = null,
                };
                _unitOfWork.AppointmentRepository.Insert(appointEntity);
                _unitOfWork.Save();
                return Ok(new
                {
                    message = "Appointment is in pending status, please wait for the staff to approve.",
                    appointment = appointEntity
                });
            }
            var batchNumberAvailable = _unitOfWork.VaccineBatchDetailRepository.Get(filter: x => x.VaccineId == request.VaccineId && x.BatchNumberNavigation.ExpiryDate > DateOnly.FromDateTime(request.AppointmentDate.Date.AddDays(3)), includeProperties: "BatchNumberNavigation").
                 OrderBy(x => x.BatchNumberNavigation.ExpiryDate).
                 ThenBy(x => x.Quantity).
                 FirstOrDefault();
            // para input to create
            if (batchNumberAvailable == null)
            {
                return BadRequest(new { message = $"There is no vaccine batch of vaccine {request.VaccineId}" });
            }
            var appointEntityAuto = new Appointment
            {
                AppointmentDate = request.AppointmentDate,
                Status = "Approved",
                Notes = request.Notes,
                CreatedAt = request.CreatedAt,
                ChildId = request.ChildId,
                StaffId = null,
                DoctorId = null,
                // if null set null, not null require single or combo
                VaccineType = "Single",
                ComboId = null,
                CustomerId = request.CustomerId,
                VaccineId = request.VaccineId,
                BatchNumber = batchNumberAvailable.BatchNumber,
            };
            batchNumberAvailable.PreOrderQuantity++;
            _unitOfWork.AppointmentRepository.Insert(appointEntityAuto);
            _unitOfWork.Save();

            var responseAppoint = new RequestResponeAppointmentModel
            {
                AppointmentId = appointEntityAuto.AppointmentId,
                AppointmentDate = request.AppointmentDate,
                Status = "Approved",
                Notes = request.Notes,
                CreatedAt = request.CreatedAt,
                ChildId = request.ChildId,
                StaffId = null,
                DoctorId = null,
                VaccineType = "Single",
                ComboId = null,
                CustomerId = request.CustomerId,
                VaccineId = request.VaccineId,
            };

            return Ok(new
            {
                message = "Appointment is approved.",
                appointment = responseAppoint
            });

        }
        [HttpPut("update-status-appointment-success/{appointmentId}/{batchNumber}/{vaccineId}")]
        public IActionResult updateStatusSucess(int appointmentId, string batchNumber, int vaccineId)
        {
            if (appointmentId == null)
            {
                return BadRequest(new { message = " appointmentId is required !" });
            }
            var appointment= _unitOfWork.AppointmentRepository.Get(filter: x=> x.AppointmentId== appointmentId).FirstOrDefault();  
            if (appointment == null)
            {
                return BadRequest(new { message = "appointment is not found ." });
            }
            var batchVaccine = _unitOfWork.VaccineBatchDetailRepository
                               .Get(filter: x => x.BatchNumber == batchNumber && x.VaccineId == vaccineId)
                               .FirstOrDefault();
            if (batchVaccine == null)
            {
                return NotFound(new { message = $"Vaccine {vaccineId} not found in batch {batchNumber}." });
            }
            // Kiểm tra số lượng trước khi giảm
            if (batchVaccine.PreOrderQuantity <= 0 || batchVaccine.Quantity <= 0)
            {
                return BadRequest(new { message = $"Insufficient stock for vaccine {vaccineId} in batch {batchNumber}." });
            }
            appointment.Status = "Success";
            batchVaccine.PreOrderQuantity--;
            batchVaccine.Quantity--;
            _unitOfWork.AppointmentRepository.Update(appointment);
            _unitOfWork.VaccineBatchDetailRepository.Update(batchVaccine);
            _unitOfWork.Save();
            return Ok(new { message = "Updated success status for appointment" });
        }
        [HttpPut("update-status-appointment-rejected/{appointmentId}")]
        public IActionResult updateStatusRejected(int appointmentId)
        {
            if (appointmentId == null)
            {
                return BadRequest(new { message = " appointmentId is required !" });
            }
            var appointment = _unitOfWork.AppointmentRepository.Get(filter: x => x.AppointmentId == appointmentId).FirstOrDefault();
            if (appointment == null)
            {
                return BadRequest(new { message = "appointment is not found ." });
            }
            appointment.Status = "Rejected";
            _unitOfWork.AppointmentRepository.Update(appointment);
            _unitOfWork.Save();
            return Ok(new { message = "Updated rejected status for appointment" });
        }
    }
}
