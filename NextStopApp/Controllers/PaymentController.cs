﻿using log4net;
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
        private static readonly ILog _log = LogManager.GetLogger(typeof(PaymentController));

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("InitiatePayment")]
        public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentDTO initiatePaymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            try
            {
                var paymentStatus = await _paymentService.InitiatePayment(initiatePaymentDto);
                return Ok(paymentStatus);
            }
            catch (Exception ex)
            {
                _log.Error("Error occurred while initiating payment.", ex);
                return BadRequest(ex.Message);
            }
        }


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
                _log.Error($"Error occurred while fetching payment status for booking ID {bookingId}.", ex);
                return NotFound(ex.Message);
            }
        }
    }
}
