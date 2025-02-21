using Microsoft.AspNetCore.Mvc;
using Vaccine.Repo.UnitOfWork;

namespace Vaccine.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public RoleController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    try
        //    {
        //        // Lấy tất cả các role
        //        var roles = _unitOfWork.RoleRepository.Get();

        //        // Trả về danh sách các role dưới dạng JSON
        //        return Ok(roles);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý lỗi và trả về phản hồi lỗi
        //        return StatusCode(500, new { Message = "An error occurred while retrieving roles.", Error = ex.Message });
        //    }
        //}
    }
}
