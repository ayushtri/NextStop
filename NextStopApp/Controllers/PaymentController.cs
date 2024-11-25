using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStopApp.DTOs;
using NextStopApp.Repositories;

namespace NextStopApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // 7.1 Initiate Payment API
        [HttpPost("InitiatePayment")]
        public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentDTO initiatePaymentDto)
        {
            try
            {
                var paymentStatus = await _paymentService.InitiatePayment(initiatePaymentDto);
                return Ok(paymentStatus);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 7.2 View Payment Status API
        [HttpGet("PaymentStatus/{bookingId}")]
        public async Task<IActionResult> ViewPaymentStatus(int bookingId)
        {
            try
            {
                var paymentStatus = await _paymentService.GetPaymentStatus(bookingId);
                return Ok(paymentStatus);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
