using NextStopApp.DTOs;

namespace NextStopApp.Repositories
{
    public interface IPaymentService
    {
        Task<PaymentStatusDTO> InitiatePayment(InitiatePaymentDTO initiatePaymentDto);
        Task<PaymentStatusDTO> GetPaymentStatus(int bookingId);
    }
}
