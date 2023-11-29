using Microsoft.Extensions.Configuration;
using Zeti.Models;
using Newtonsoft.Json;

namespace Zeti.Services;

public class VehicleService : IVehicleService
{
    private readonly HttpClient _client;
    private readonly string _apiPrefix = "api";

    public VehicleService(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        if(_client.BaseAddress == null) _client.BaseAddress= new Uri(configuration.GetSection("apiBaseAddress")?.Value);
    }
    public async Task<IEnumerable<Vehicle>> GetHistory(Func<Vehicle, bool> vehicleQuery, DateTime journeyTimes,string vehicleHistoryEndPoint = "vehicles/history/")
    {
        var responseContent = await getVehiclesresponsebyUrl(string.Format($"{_apiPrefix}{vehicleHistoryEndPoint}{{0}}", journeyTimes.ToString("yyyy-MM-ddTHH:mm:ssZ")));
        return DeserializeVehicles(responseContent).Where(vehicleQuery);

    }

    public async Task<IEnumerable<Vehicle>> GetVehicles(Func<Vehicle, bool> vehicleQuery,string vehiclesEndPoint="vehicles")
    {
       
        var responseContent = await getVehiclesresponsebyUrl($"{_apiPrefix}{vehiclesEndPoint}");
        return DeserializeVehicles(responseContent).Where(vehicleQuery);
    }

    private async Task<string> getVehiclesresponsebyUrl(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            var response = await _client.GetAsync(url);

            return response != null && response.StatusCode == System.Net.HttpStatusCode.OK ?
                await response.Content.ReadAsStringAsync() :
                throw new Exception("problem reading response");
        }

        throw new Exception("url is invalid or invalid request");
    }

    private IEnumerable<Vehicle> DeserializeVehicles(string content)
    {
        try
        {
            var allVehicles = JsonConvert.DeserializeObject<IEnumerable<Vehicle>>(content,new JsonSerializerSettings { NullValueHandling= NullValueHandling.Include });

            if (allVehicles != null) return allVehicles;
            throw new Exception("no vehicles provided");
        }catch(Exception e)
        {
            throw new Exception(e.Message +"\n"+e.InnerException);
        }
    }

    
}

