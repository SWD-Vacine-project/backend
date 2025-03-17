using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vaccine.API.Models.VaccineBatchModel;
using Vaccine.Repo.Entities;
using Vaccine.Repo.UnitOfWork;

namespace Vaccine.API.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("[controller]")]
    [ApiController]
    public class VaccineBatchController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        public VaccineBatchController(UnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
        }
        [HttpGet("get-vaccine-batch")]
        public IActionResult GetAllVaccineBatch()
        {
            var batch = _unitOfWork.VaccineBatchRepository.Get();
            if(batch == null)
            {
                return NotFound();
            }
            return Ok(batch);
        }
        [HttpGet("get-vaccine-by-batch/{batchId}")]
        public IActionResult GetVaccineByBatch(string batchId)
        {
            var batchDetails = _unitOfWork.VaccineBatchDetailRepository.Get(
                v => v.BatchNumber == batchId,
                includeProperties: "Vaccine"
            );

            if (batchDetails == null || !batchDetails.Any())
            {
                return NotFound(new { message = "No vaccines found for the given batch." });
            }

            var result = batchDetails.Select(v => new
            {
                v.BatchNumber,
                VaccineName = v.Vaccine.Name,
                v.Vaccine.Description,
                v.Vaccine.Price,
              
            });

            return Ok(result);
        }
        [HttpGet("get-batch-by-vaccineID/{vaccineID}/{appointmentDate}")]
        public IActionResult GetVaccineBatchByVaccineID(int vaccineID, DateTime appointmentDate)
        {
            // loc theo dieu kien : expiredDate phai lon hon ngay di chich +3, 
            var batchDetails = _unitOfWork.VaccineBatchDetailRepository // expiryDate phai lon hon ngay hom nay
            .Get(v => v.VaccineId == vaccineID && v.BatchNumberNavigation.ExpiryDate > DateOnly.FromDateTime(appointmentDate).AddDays(3) && v.Quantity > 0, includeProperties: "Vaccine,BatchNumberNavigation")
            .OrderBy(v => v.BatchNumberNavigation.ExpiryDate)
            .ThenBy(v => v.Quantity)
            .ToList();


            if (batchDetails == null || !batchDetails.Any())
            {
                return NotFound(new { message = "No vaccines found for the given batch." });
            }

            var result = batchDetails.
              Select(v => new
              {
                  v.BatchNumber,
                  VaccineName = v.Vaccine.Name,
                  v.VaccineId,
                  v.Vaccine.Description,
                  v.Vaccine.Price,
                  v.Quantity,
                  v.PreOrderQuantity,
                  v.BatchNumberNavigation.ExpiryDate
              });


            return Ok(result);
        }



        [HttpPost("create-vaccine-batch")]
        public async Task<IActionResult> CreateVaccineBatch([FromBody] RequestCreateVaccineBatch newBatch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về danh sách lỗi từ Data Annotation
            }

            var existingBatch = _unitOfWork.VaccineBatchRepository.Get(filter : x=> x.BatchNumber== newBatch.BatchNumber).FirstOrDefault();
            if (existingBatch != null)
            {
                return BadRequest("Batch number already exists.");
            }
            // Lấy danh sách VaccineId hợp lệ từ database
            var validVaccineIds = _unitOfWork.VaccineRepository
                .Get()
                .Select(v => v.VaccineId) 
                .ToHashSet();

            // Kiểm tra tất cả VaccineId có hợp lệ không trước khi tiếp tục
            foreach (var detailDto in newBatch.VaccineBatchDetails)
            {
                if (!validVaccineIds.Contains(detailDto.VaccineId))
                {
                    return BadRequest($"Vaccine ID {detailDto.VaccineId} does not exist.");
                }
            }
            var batch = new VaccineBatch
            {
                BatchNumber = newBatch.BatchNumber,
                Manufacturer = newBatch.Manufacturer,
                ManufactureDate = newBatch.ManufactureDate,
                ExpiryDate = newBatch.ExpiryDate,
                Country = newBatch.Country,
                //Duration = newBatch.Duration,
                Status = newBatch.Status
            };

            _unitOfWork.VaccineBatchRepository.Insert(batch);
            await _unitOfWork.SaveAsync();

            foreach (var detailDto in newBatch.VaccineBatchDetails)
            {
                var detail = new VaccineBatchDetail
                {
                    BatchNumber = batch.BatchNumber,
                    VaccineId = detailDto.VaccineId,
                    Quantity = detailDto.Quantity
                };
                _unitOfWork.VaccineBatchDetailRepository.Insert(detail);
            }

            await _unitOfWork.SaveAsync();
            return Ok(new { Message = "Vaccine batch created successfully", batch });
        }
        [HttpPut("update/{batchNumber}")]
        public async Task<IActionResult> UpdateVaccineBatch(string batchNumber, [FromBody] RequestUpdateVaccineBatch batchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem lô có tồn tại không
            var existingBatch = _unitOfWork.VaccineBatchRepository
                .Get(filter: x => x.BatchNumber == batchNumber)
                .FirstOrDefault();
            Console.WriteLine(existingBatch);
            if (existingBatch == null)
            {
                return NotFound("Batch not found.");
            }
            // ko thay đổi batchNumber
            // Cập nhật thông tin lô vaccine
            existingBatch.Manufacturer = batchDto.Manufacturer;
            existingBatch.ManufactureDate = batchDto.ManufactureDate;
            existingBatch.ExpiryDate = batchDto.ExpiryDate;
            existingBatch.Country = batchDto.Country;
           // existingBatch.Duration = batchDto.Duration;
            existingBatch.Status = batchDto.Status;
            // Lấy danh sách VaccineId hợp lệ từ database
            var validVaccineIds = _unitOfWork.VaccineRepository
                .Get()
                .Select(v => v.VaccineId)
                .ToHashSet();

            // Kiểm tra tất cả VaccineId có hợp lệ không trước khi tiếp tục
            foreach (var detailDto in batchDto.VaccineBatchDetails)
            {
                if (!validVaccineIds.Contains(detailDto.VaccineId))
                {
                    return BadRequest($"Vaccine ID {detailDto.VaccineId} does not exist.");
                }
            }
            _unitOfWork.VaccineBatchRepository.Update(existingBatch);
            await _unitOfWork.SaveAsync();

            // Xóa chi tiết vaccine cũ và thêm mới
            var existingDetails = _unitOfWork.VaccineBatchDetailRepository
                .Get(filter: x => x.BatchNumber == batchNumber)
                .ToList();

            foreach (var detail in existingDetails)
            {
                _unitOfWork.VaccineBatchDetailRepository.Delete(detail);
            }

            await _unitOfWork.SaveAsync(); // Lưu thay đổi

            // Thêm danh sách vaccine mới
            foreach (var detailDto in batchDto.VaccineBatchDetails)
            {
                var newDetail = new VaccineBatchDetail
                {
                    BatchNumber = batchNumber,
                    VaccineId = detailDto.VaccineId,
                    Quantity = detailDto.Quantity
                };
                _unitOfWork.VaccineBatchDetailRepository.Insert(newDetail);
            }

            await _unitOfWork.SaveAsync();

            return Ok(new { Message = "Vaccine batch updated successfully", batch = existingBatch });
        }

    }
}
