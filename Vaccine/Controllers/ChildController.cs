using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vaccine.Repo.Entities;
using Vaccine.Repo.UnitOfWork;

namespace Vaccine.API.Controllers
{
    //[EnableCors("MyPolicy")]
    [Route("[controller]")]
    [ApiController]
    public class ChildController : ControllerBase
    {
        //public static List<Child> childs = new List<Child>();
        private readonly UnitOfWork _unitOfWork;

        public ChildController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll()
        { 
            var childs = _unitOfWork.ChildRepository.Get();

            return Ok(childs);
        }
    }
}
