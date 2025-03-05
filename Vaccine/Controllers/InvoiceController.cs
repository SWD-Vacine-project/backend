using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Vaccine.API.Models.CustomerModel;
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
        [SwaggerOperation(
        Description = "customerID \n"
            + " totalAmount: tổng tiền\n"
            + " status: unpaid\n"
            + " type: single or combo\n")]
        public IActionResult CreateInvoice(RequestCreateInvoiceModel requestCreateInvoiceModel)
        {
            if(requestCreateInvoiceModel.CustomerId == 0)
            {
                return BadRequest(new { message = "Customer ID is required." });
            }
            var invoiceEntity = new Invoice
            {
                CustomerId = requestCreateInvoiceModel.CustomerId ,
                TotalAmount = requestCreateInvoiceModel.TotalAmount,
                Status = "Unpaid",
                Type = requestCreateInvoiceModel.Type,
                CreatedAt = requestCreateInvoiceModel.CreatedAt,
                UpdatedAt = requestCreateInvoiceModel.UpdatedAt

            };
            _unitOfWork.InvoiceRepository.Insert(invoiceEntity);
            _unitOfWork.Save();
            return Ok(invoiceEntity);
        }

        [HttpPut("update-invoice-status/{id}")]
        [SwaggerOperation(
        Summary = "Update status of invoice to paid",
        Description = "get id of invoice" +
            "Get purchased vaccines from InvoiceDetail "
    )]
        public IActionResult UpdateInvoiceStatusToPaid(int invoice_id)
        {
            var invoice = _unitOfWork.InvoiceRepository.GetByID(invoice_id);
            if (invoice == null)
            {
                return NotFound(new { message = "Invoice not found." });
            }

            if (invoice.Status == "Paid")
            {
                return BadRequest(new { message = "Invoice is already paid." });
            }

            // Update invoice status
            invoice.Status = "Paid";
            _unitOfWork.InvoiceRepository.Update(invoice);

            // Get purchased vaccines from InvoiceDetail (assuming InvoiceDetail stores vaccine purchases)
            var invoiceDetails = _unitOfWork.InvoiceDetailRepository.Get(d => d.InvoiceId == invoice_id);

            foreach (var detail in invoiceDetails)
            {
                var vaccineBatch = _unitOfWork.VaccineBatchDetailRepository
                    .Get(vb => vb.VaccineId == detail.VaccineId)
                    .OrderBy(vb => vb.BatchNumber)
                    .FirstOrDefault();

                if (vaccineBatch == null || vaccineBatch.Quantity < detail.Quantity)
                {
                    return BadRequest(new { message = $"Insufficient stock for vaccine ID {detail.VaccineId}" });
                }

                // Deduct vaccine quantity
                vaccineBatch.Quantity -= detail.Quantity;
                _unitOfWork.VaccineBatchDetailRepository.Update(vaccineBatch);
            }

            _unitOfWork.Save();
            return Ok(new { message = "Invoice updated to Paid and stock deducted." });
        }
    }
}
