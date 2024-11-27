using NextStopApp.DTOs;
using NextStopApp.Models;

namespace NextStopApp.Repositories
{
    public interface IBusService
    {
        Task<BusOperator> RegisterBusOperator(BusOperatorRegisterDTO operatorDto);
        Task<IEnumerable<BusDTO>> GetBusesByOperatorId(int operatorId);
        Task<BusDTO> AddBus(BusCreateDTO busDto);
        Task UpdateBus(int busId, BusUpdateDTO updateBusDto);
        Task DeleteBus(int busId);
        Task<BusOperator> GetBusOperatorByEmail(string email);
        Task<BusDTO> GetBusByBusNumber(string busNumber);
        Task<IEnumerable<BusOperatorDTO>> ViewAllOperators();
    }
}
