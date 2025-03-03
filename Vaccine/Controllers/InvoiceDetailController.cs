using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vaccine.API.Models.InvoiceDetailModel;
using Vaccine.Repo.Entities;
using Vaccine.Repo.UnitOfWork;

namespace Vaccine.API.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceDetailController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public InvoiceDetailController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("create-invoice-detail")]
        public IActionResult CreateInvoiceDetail(RequestCreateInvoiceDetailModel requestCreateInvoiceDetailModel)
        {
            var invoiceDetailEntity = new InvoiceDetail
            {
                InvoiceId = requestCreateInvoiceDetailModel.InvoiceId,
                VaccineId = requestCreateInvoiceDetailModel.VaccineId,
                AppointmentId = requestCreateInvoiceDetailModel.AppointmentId,
                ComboId = requestCreateInvoiceDetailModel.ComboId,
                Quantity = requestCreateInvoiceDetailModel.Quantity,
                Price = requestCreateInvoiceDetailModel.Price
            };
            _unitOfWork.InvoiceDetailRepository.Insert(invoiceDetailEntity);
            _unitOfWork.Save();
            return Ok(invoiceDetailEntity);
        }
    }
}
