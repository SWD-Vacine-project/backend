using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;
using Vaccine.Repo.UnitOfWork;
using System.Security.Cryptography;
using System.Text;

namespace Vaccine.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
        private readonly IVnpay _vnpay;
        private readonly IConfiguration _configuration;
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger<VnPayController> _logger;


        public VnPayController(IVnpay vnpay, IConfiguration configuration, UnitOfWork unitOfWork, ILogger<VnPayController> logger)
        {
            _vnpay = vnpay;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _vnpay.Initialize(_configuration["Vnpay:TmnCode"], _configuration["Vnpay:HashSecret"], _configuration["Vnpay:BaseUrl"], _configuration["Vnpay:CallbackUrl"]);
            _logger = logger;
        }


        /// <summary>
        /// Tạo url thanh toán
        /// </summary>
        /// <param name="money">Số tiền phải thanh toán</param>
        /// <param name="description">Mô tả giao dịch</param>
        /// <returns></returns>
        [HttpGet("CreatePaymentUrl")]
        //public ActionResult<string> CreatePaymentUrl(double moneyToPay, string description)
        //{
        //    try
        //    {
        //        var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

        //        var request = new PaymentRequest
        //        {
        //            PaymentId = DateTime.Now.Ticks,
        //            Money = moneyToPay,
        //            Description = description,
        //            IpAddress = ipAddress,
        //            BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
        //            CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
        //            Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
        //            Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
        //        };

        //        var paymentUrl = _vnpay.GetPaymentUrl(request);

        //        return Created(paymentUrl, paymentUrl);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        public ActionResult<string> CreatePaymentUrl(double moneyToPay, string description)
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

                var request = new Dictionary<string, string>
        {
            { "vnp_Amount", ((long)(moneyToPay * 100)).ToString() }, // Nhân 100 theo yêu cầu VNPay
            { "vnp_Command", "pay" },
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", "VND" },
            { "vnp_IpAddr", ipAddress },
            { "vnp_Locale", "vn" },
            { "vnp_OrderInfo", description },
            { "vnp_OrderType", "other" },
            { "vnp_ReturnUrl", _configuration["Vnpay:CallbackUrl"] },
            { "vnp_TmnCode", _configuration["Vnpay:TmnCode"] },
            { "vnp_TxnRef", DateTime.Now.Ticks.ToString() },
            { "vnp_Version", "2.1.0" }
        };

                // 🌟 Bước 1: Sắp xếp tham số theo thứ tự bảng chữ cái
                var sortedParams = request.OrderBy(p => p.Key).ToDictionary(k => k.Key, v => v.Value);

                // 🌟 Bước 2: Tạo chuỗi dữ liệu cần ký
                string rawData = string.Join("&", sortedParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

                // 🌟 Bước 3: Tạo chữ ký HMAC-SHA512
                string secureHash = CreateHmacSha512(_configuration["Vnpay:HashSecret"], rawData);

                // 🌟 Bước 4: Thêm chữ ký vào request
                request.Add("vnp_SecureHash", secureHash);

                // 🌟 Bước 5: Tạo URL thanh toán
                string paymentUrl = $"{_configuration["Vnpay:BaseUrl"]}?{string.Join("&", request.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}";

                return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ✅ Hàm tạo HMAC-SHA512
        private static string CreateHmacSha512(string key, string data)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashValue).Replace("-", "").ToUpper();
            }
        }

        /// <summary>
        /// Thực hiện hành động sau khi thanh toán. URL này cần được khai báo với VNPAY để API này hoạt đồng (ví dụ: http://localhost:1234/api/Vnpay/IpnAction)
        /// </summary>
        /// <returns></returns>
        [HttpGet("IpnAction")]
        public IActionResult IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess)
                    {
                        // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
                        return Ok();
                    }

                    // Thực hiện hành động nếu thanh toán thất bại tại đây. Ví dụ: Hủy đơn hàng.
                    return BadRequest("Thanh toán thất bại");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }

        /// <summary>
        /// Trả kết quả thanh toán về cho người dùng
        /// </summary>
        /// <returns></returns>
        [HttpGet("Callback")]
        public ActionResult<PaymentResult> Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);

                    if (paymentResult.IsSuccess)
                    {
                        return Ok(paymentResult);
                    }

                    return BadRequest(paymentResult);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
    }
}


