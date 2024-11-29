using Microsoft.EntityFrameworkCore;
using NextStopApp.Data;
using NextStopApp.DTOs;
using NextStopApp.Models;

namespace NextStopApp.Repositories
{
    public class PaymentService : IPaymentService
    {
        private readonly NextStopDbContext _context;

        public PaymentService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentStatusDTO> InitiatePayment(InitiatePaymentDTO initiatePaymentDto)
        {
            // Validate the booking exists
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == initiatePaymentDto.BookingId);
            if (booking == null)
                throw new Exception("Booking not found.");

            // Process the payment (mock payment for now)
            var paymentStatus = initiatePaymentDto.PaymentStatus.ToLower() == "successful" ? "successful" : "failed";

            var payment = new Payment
            {
                BookingId = initiatePaymentDto.BookingId,
                Amount = initiatePaymentDto.Amount,
                PaymentStatus = paymentStatus
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return new PaymentStatusDTO
            {
                PaymentId = payment.PaymentId,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentStatus = payment.PaymentStatus,
                PaymentDate = payment.PaymentDate
            };
        }

        public async Task<PaymentStatusDTO> GetPaymentStatus(int bookingId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);

            if (payment == null)
                throw new Exception("Payment not found for the specified booking.");

            return new PaymentStatusDTO
            {
                PaymentId = payment.PaymentId,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentStatus = payment.PaymentStatus,
                PaymentDate = payment.PaymentDate
            };
        }
    }
}
