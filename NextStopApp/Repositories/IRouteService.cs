using NextStopApp.DTOs;

namespace NextStopApp.Repositories
{
    public interface IRouteService
    {
        Task<RouteDTO> AddRoute(RouteCreateDTO routeDto);
        Task<RouteDTO> UpdateRoute(int routeId, RouteUpdateDTO routeDto);
        Task<bool> DeleteRoute(int routeId);
        Task<IEnumerable<RouteDTO>> GetAllRoutes();
    }

}
