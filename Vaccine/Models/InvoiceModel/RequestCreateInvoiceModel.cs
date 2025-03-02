using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using Vaccine.Repo.Entities;

namespace Vaccine.API.Models.InvoiceModel
{
    public class RequestCreateInvoiceModel
    {
        //public int InvoiceId { get; set; }

        public int? CustomerId { get; set; }

        [Required(ErrorMessage = "Total Amount is required")]
        public decimal TotalAmount { get; set; }

        [RegularExpression("Paid|Unpaid|Cancelled", ErrorMessage = "Status must be 'Paid' or 'Unpaid' or 'Cancelled'")]
        public string? Status { get; set; }

        [RegularExpression("Single|Combo", ErrorMessage = "Type must be 'Single' or 'Combo'")]
        public string? Type { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }


    }

    
}
