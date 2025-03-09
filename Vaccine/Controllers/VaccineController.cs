using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;
using Vaccine.Repo.Entities;
using Vaccine.Repo.UnitOfWork;
using Vaccine.API.Models;
using Vaccine.API.Models.VaccineModel;
using Swashbuckle.AspNetCore.Filters;
using Vaccine.API.Models.ChildModel;

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
        [HttpPost("create-vaccine")]
        [SwaggerRequestExample(typeof(RequestCreateVaccineModel), typeof(ExampleRequestCreateVaccineModel))]
        public IActionResult CreateVaccine(RequestCreateVaccineModel newVaccine)
        {
            if (newVaccine == null)
            {
                return BadRequest(new {message= "Vaccine data is required"});
            }
            var vaccineEntity = new Vaccine.Repo.Entities.Vaccine
            {
                Name = newVaccine.Name,
                MaxLateDate = newVaccine.MaxLateDate,
                Price = newVaccine.Price,
                Description = newVaccine.Description,
                InternalDurationDoses = newVaccine.InternalDurationDoses
            };
            _unitOfWork.VaccineRepository.Insert(vaccineEntity);
            _unitOfWork.Save();
            return Ok(new {message = "Create new vaccine successfully"});
        }
        [HttpPut("update-vaccine/{id}")]
        [SwaggerRequestExample(typeof(RequestUpdateVaccineModel), typeof(ExampleRequestUpdateVaccineModel))]
        public IActionResult UpdateVaccine(int id, RequestUpdateVaccineModel updateVaccine)
        {
            if (updateVaccine == null)
            {
                return BadRequest(new { message = "Vaccine data is requied" });
            }
            var found = _unitOfWork.VaccineRepository.GetByID(id);
            if (found == null)
            {
                return BadRequest(new { message = "Cannot find vaccine to update" });
            }
            found.Name = updateVaccine.Name;
            found.MaxLateDate = updateVaccine.MaxLateDate;
            found.Price = updateVaccine.Price;
            found.Description = updateVaccine.Description;
            found.InternalDurationDoses = updateVaccine.InternalDurationDoses;
            _unitOfWork.VaccineRepository.Update(found);
            _unitOfWork.Save();
            return Ok(new { mesage = "Upate vaccine successfully" }); 

        }


    }
}
