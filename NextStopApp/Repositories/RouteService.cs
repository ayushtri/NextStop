using Microsoft.EntityFrameworkCore;
using NextStopApp.Data;
using NextStopApp.DTOs;
using NextStopApp.Models;


namespace NextStopApp.Repositories
{
    public class RouteService : IRouteService
    {
        private readonly NextStopDbContext _context;

        public RouteService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<RouteDTO> AddRoute(RouteCreateDTO routeDto)
        {
            var route = new Models.Route
            {
                Origin = routeDto.Origin,
                Destination = routeDto.Destination,
                Distance = routeDto.Distance,
                EstimatedTime = routeDto.EstimatedTime
            };

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            return new RouteDTO
            {
                RouteId = route.RouteId,
                Origin = route.Origin,
                Destination = route.Destination,
                Distance = route.Distance,
                EstimatedTime = route.EstimatedTime
            };
        }

        public async Task<RouteDTO> UpdateRoute(int routeId, RouteUpdateDTO routeDto)
        {
            var route = await _context.Routes.FirstOrDefaultAsync(r => r.RouteId == routeId);
            if (route == null)
            {
                throw new Exception("Route not found.");
            }

            route.Origin = routeDto.Origin ?? route.Origin;
            route.Destination = routeDto.Destination ?? route.Destination;
            route.Distance = routeDto.Distance ?? route.Distance;
            route.EstimatedTime = routeDto.EstimatedTime ?? route.EstimatedTime;

            await _context.SaveChangesAsync();

            return new RouteDTO
            {
                RouteId = route.RouteId,
                Origin = route.Origin,
                Destination = route.Destination,
                Distance = route.Distance,
                EstimatedTime = route.EstimatedTime
            };
        }

        public async Task<bool> DeleteRoute(int routeId)
        {
            var route = await _context.Routes.FirstOrDefaultAsync(r => r.RouteId == routeId);
            if (route == null)
            {
                return false;
            }

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RouteDTO>> GetAllRoutes()
        {
            var routes = await _context.Routes.ToListAsync();
            return routes.Select(route => new RouteDTO
            {
                RouteId = route.RouteId,
                Origin = route.Origin,
                Destination = route.Destination,
                Distance = route.Distance,
                EstimatedTime = route.EstimatedTime
            });
        }

    }
}
