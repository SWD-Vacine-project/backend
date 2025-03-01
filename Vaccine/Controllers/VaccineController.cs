using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;
using Vaccine.Repo.Entities;
using Vaccine.Repo.UnitOfWork;

namespace Vaccine.API.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class VaccineController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public VaccineController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var vaccines = _unitOfWork.VaccineRepository.Get();

            return Ok(vaccines);
        }

        [HttpGet("{name}")]
        public IActionResult GetVaccineByName(string name)
        {
            var vaccines = _unitOfWork.VaccineRepository.Get(v => v.Name == name).FirstOrDefault();
            if (vaccines == null)
            {
                return NotFound(new { message = "Vaccine not found." });
            }

            return Ok(vaccines);
        }

        [HttpGet("sort-by-name")]
        public IActionResult SortVaccineByName()
        {
            var vaccines = _unitOfWork.VaccineRepository.Get()
                     .OrderBy(v => v.Name)
                     .ToList();

            if (!vaccines.Any())
            {
                return NotFound(new { message = "No vaccines found." });
            }

            return Ok(vaccines);
        }

        [HttpGet("sort-by-price")]
        public IActionResult SortVaccineByPrice()
        {
            var vaccines = _unitOfWork.VaccineRepository.Get()
                    .OrderBy(v => v.Price)
                    .ToList();

            if (!vaccines.Any())
            {
                return NotFound(new { message = "No vaccines found." });
            }

            return Ok(vaccines);
        }
    }
}
