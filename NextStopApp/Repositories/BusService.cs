using Microsoft.EntityFrameworkCore;
using NextStopApp.Data;
using NextStopApp.DTOs;
using NextStopApp.Models;

namespace NextStopApp.Repositories
{
    public class BusService : IBusService
    {
        private readonly NextStopDbContext _context;

        public BusService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<BusOperator> RegisterBusOperator(BusOperatorRegisterDTO operatorDto)
        {
            var existingOperator = await _context.BusOperators.FirstOrDefaultAsync(o => o.Email == operatorDto.Email);
            if (existingOperator != null)
            {
                throw new Exception("A bus operator with this email already exists.");
            }

            var busOperator = new BusOperator
            {
                Name = operatorDto.Name,
                ContactNumber = operatorDto.ContactNumber,
                Email = operatorDto.Email,
                Address = operatorDto.Address
            };

            _context.BusOperators.Add(busOperator);
            await _context.SaveChangesAsync();

            return busOperator;
        }



        public async Task<IEnumerable<BusDTO>> GetBusesByOperatorId(int operatorId)
        {
            return await _context.Buses
                .Where(b => b.OperatorId == operatorId)
                .Select(b => new BusDTO
                {
                    BusId = b.BusId,
                    OperatorId = b.OperatorId,
                    BusName = b.BusName,
                    BusNumber = b.BusNumber,
                    BusType = b.BusType,
                    TotalSeats = b.TotalSeats,
                    Amenities = b.Amenities
                })
                .ToListAsync();
        }

        public async Task<BusDTO> AddBus(BusCreateDTO busDto)
        {
            var existingBus = await _context.Buses.FirstOrDefaultAsync(b => b.BusNumber == busDto.BusNumber);
            if (existingBus != null)
            {
                throw new Exception("A bus with this number already exists.");
            }

            var bus = new Bus
            {
                OperatorId = busDto.OperatorId,
                BusName = busDto.BusName,
                BusNumber = busDto.BusNumber,
                BusType = busDto.BusType,
                TotalSeats = busDto.TotalSeats,
                Amenities = busDto.Amenities
            };

            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();

            return new BusDTO
            {
                BusId = bus.BusId,
                OperatorId = bus.OperatorId,
                BusName = bus.BusName,
                BusNumber = bus.BusNumber,
                BusType = bus.BusType,
                TotalSeats = bus.TotalSeats,
                Amenities = bus.Amenities
            };
        }



        public async Task UpdateBus(int busId, BusUpdateDTO updateBusDto)
        {
            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.BusId == busId);
            if (bus == null)
                throw new Exception("Bus not found");

            bus.BusName = updateBusDto.BusName ?? bus.BusName;
            bus.BusType = updateBusDto.BusType ?? bus.BusType;
            bus.TotalSeats = updateBusDto.TotalSeats ?? bus.TotalSeats;
            bus.Amenities = updateBusDto.Amenities ?? bus.Amenities;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBus(int busId)
        {
            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.BusId == busId);
            if (bus == null)
                throw new Exception("Bus not found");

            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();
        }

        public async Task<BusOperator> GetBusOperatorByEmail(string email)
        {
            var busOperator = await _context.BusOperators.FirstOrDefaultAsync(o => o.Email == email);
            if (busOperator == null)
            {
                throw new Exception("Bus operator not found.");
            }
            return busOperator;
        }

        public async Task<BusDTO> GetBusByBusNumber(string busNumber)
        {
            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.BusNumber == busNumber);
            if (bus == null)
            {
                throw new Exception("Bus not found.");
            }

            return new BusDTO
            {
                BusId = bus.BusId,
                OperatorId = bus.OperatorId,
                BusName = bus.BusName,
                BusNumber = bus.BusNumber,
                BusType = bus.BusType,
                TotalSeats = bus.TotalSeats,
                Amenities = bus.Amenities
            };
        }

    }
}
