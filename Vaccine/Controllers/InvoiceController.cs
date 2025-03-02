using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vaccine.API.Models.InvoiceModel;
using Vaccine.Repo.Entities;
using Vaccine.Repo.UnitOfWork;

namespace Vaccine.API.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public InvoiceController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("get-invoices")]
        public IActionResult GetInvoices()
        {
            var invoices = _unitOfWork.InvoiceRepository.Get();
            return Ok(invoices);
        }

        [HttpGet("get-invoice/{id}")]
        public IActionResult GetInvoice(int id)
        {
            var invoice = _unitOfWork.InvoiceRepository.GetByID(id);
            if (invoice == null)
            {
                return NotFound(new { message = "Invoice not found." });
            }
            return Ok(invoice);
        }

        [HttpPost("create-invoice")]
        public IActionResult CreateInvoice(RequestCreateInvoiceModel requestCreateInvoiceModel)
        {
            var invoiceEntity = new Invoice
            {
                CustomerId = requestCreateInvoiceModel.CustomerId == 0 ? null : requestCreateInvoiceModel.CustomerId,
                TotalAmount = requestCreateInvoiceModel.TotalAmount,
                Status = requestCreateInvoiceModel.Status,
                Type = requestCreateInvoiceModel.Type,
                CreatedAt = requestCreateInvoiceModel.CreatedAt,
                UpdatedAt = requestCreateInvoiceModel.UpdatedAt

            };
            _unitOfWork.InvoiceRepository.Insert(invoiceEntity);
            _unitOfWork.Save();
            return Ok(invoiceEntity);
        }
    }
}
