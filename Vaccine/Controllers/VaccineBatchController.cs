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
        public IActionResult GetAllVaccineBacth()
        {
            var batch = _unitOfWork.VaccineBatchRepository.Get();
            if(batch == null)
            {
                return NotFound();
            }
            return Ok(batch);
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
