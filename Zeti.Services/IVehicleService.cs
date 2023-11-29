using Zeti.Models;

namespace Zeti.Services;

public interface IVehicleService
{
    public Task<IEnumerable<Vehicle>> GetVehicles(Func<Vehicle, bool> vehicleQuery, string vehiclesEndPoint = "/vehicles");
    public Task<IEnumerable<Vehicle>> GetHistory(Func<Vehicle, bool> vehicleQuery,DateTime time, string vehicleHistoryEndPoint = "/vehicles/history/");

}

