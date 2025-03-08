using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vaccine.API.Models.CustomerModel;
using Vaccine.Repo.Entities;
using Vaccine.Repo.UnitOfWork;

namespace Vaccine.API.Controllers
{
    [EnableCors("MyPolicy")]
    [ApiController]
    [Route("[controller]")]
    public class CustomerController:ControllerBase
    {
      
        private readonly UnitOfWork _unitOfWork;
        public CustomerController( UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("get-customer")]
        public IActionResult GetCustomer()
        {
            var customerList = _unitOfWork.CustomerRepository.Get();
            if(customerList == null) 
            { 
              return NotFound(new { message = "Cannot find customer"});
            }
            return Ok(customerList);
        }
        [HttpPost("create-customer")]
        public IActionResult CreateCustomer(RequestCreateCustomerModel customer)
        {
            if(customer == null)
            {
                return BadRequest(new { message = "Customer data is required" });
            }
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingEmail = _unitOfWork.CustomerRepository.Get(filter: x=> x.Email== customer.Email);
            if (existingEmail!= null)
            {
                return BadRequest(new { message = "Email is existed" });
            }
            var customerEntity = new Customer
            {
                Name = customer.Name,
                Dob = customer.Dob,
                Gender = customer.Gender,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                BloodType = customer.BloodType,
                UserName = customer.UserName,
                Password = customer.Password
            };
            _unitOfWork.CustomerRepository.Insert(customerEntity);
            _unitOfWork.Save();
            return Ok(new { message ="Customer is created successfully"});

        }
        [HttpPost("update-customer/{id}")]
        public IActionResult UpdateCustomer(int id, RequestUpdateCustomerModel updatedCustomer)
        {
            if (updatedCustomer == null)
            {
                return BadRequest(new { message = "Customer data is required" });
            }

            // Tìm khách hàng theo ID
            var existingCustomer = _unitOfWork.CustomerRepository.GetByID(id);
            if (existingCustomer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }

            // Cập nhật thông tin khách hàng
            existingCustomer.Name = updatedCustomer.Name;
            //existingCustomer.Dob = updatedCustomer.Dob;
            existingCustomer.Gender = updatedCustomer.Gender; 
            existingCustomer.Phone = updatedCustomer.Phone;
            existingCustomer.Email = updatedCustomer.Email;
            existingCustomer.Address = updatedCustomer.Address;
            existingCustomer.BloodType = updatedCustomer.BloodType;

            // Nếu có cập nhật mật khẩu, phải mã hóa lại
            // Lưu thay đổi vào database
            _unitOfWork.CustomerRepository.Update(existingCustomer);
            _unitOfWork.Save();

            return Ok(new { message = "Customer updated successfully", customer = existingCustomer });

        }
    }
}
