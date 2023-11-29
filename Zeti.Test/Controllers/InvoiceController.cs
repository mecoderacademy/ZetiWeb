using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Zeti.Models;
using Zeti.Services;

namespace Zeti.Test.Web.Controllers;

[ApiController]
[Route("api")]
public class InvoiceController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly IDisplayService _jsonDisplayService;
    private readonly IJourneyService _journeyService;

    public InvoiceController(
        IVehicleService vehicleService,
        IJourneyService journeyService,
        IDisplayService jsonDisplayService)
    {
        _vehicleService = vehicleService;
        _jsonDisplayService = jsonDisplayService;
        _journeyService = journeyService;

    }

    [HttpGet("Vehicles")]
    public async Task<IActionResult> GetVehicles()
    {
        return Ok(await _vehicleService.GetVehicles(l => l.LicensePlate == l.LicensePlate));
    }
    
        [HttpPost("billcalculation")]
    public async Task<IActionResult> CalculateBill([FromBody] BillCalculationRequest request)
    {
        if (request == null) return BadRequest("Request is null");
        if (request.EndInterval == default
            || request.LisencePlates == null
            || !request.LisencePlates.Any()
            || request.StartInterval == default
            || request.Cost <= 0)

            return BadRequest("Invalid calculation request");

        try
        {
            var existingVehicles = await _vehicleService.GetVehicles(l => request.LisencePlates.Contains(l.LicensePlate));
            var vehiclesBeforeDataInTimeFrame = await _vehicleService.GetHistory(l => request.LisencePlates.Contains(l.LicensePlate), DateTime.Parse(request.StartInterval));
            NotExistingPlates(vehiclesBeforeDataInTimeFrame, request);
            var vehiclesAftrerDataInTimeFrame = await _vehicleService.GetHistory(l => request.LisencePlates.Contains(l.LicensePlate), DateTime.Parse(request.EndInterval));
            NotExistingPlates(vehiclesAftrerDataInTimeFrame, request);
            var vehiclesDataInTimeFrame = vehiclesBeforeDataInTimeFrame.Concat(vehiclesAftrerDataInTimeFrame);
            var costInvoiceData = _journeyService.CalculateJorneyCosts(GenerateCalculationValues(vehiclesDataInTimeFrame,request));
         
                switch (request.UploadType.ToLower())
                {
                    case "upload":
                    //return ok if successful
                        break;
                    case "json":
                         return Content(_jsonDisplayService.ExportAsString(costInvoiceData), "application/json");
  
                }
           
        }catch(Exception e)
        {
            return BadRequest(e.Message);
        }

        return BadRequest();
    }

    private void NotExistingPlates(IEnumerable<Vehicle> vehiclesBeforeDataInTimeFrame, BillCalculationRequest request)
    {
        var nonExistentPlates = vehiclesBeforeDataInTimeFrame.Where(x => !request.LisencePlates.Contains(x.LicensePlate));
        if (nonExistentPlates.Count() > 0)
        {
            throw new Exception($"the following plates {string.Join(",", nonExistentPlates)} do not exist within this timeframe");
        }

        return;
    }

    private List<(double odometerBefore, double odometerAfter, double milesPerHour, string liscensePlate, string make, double costPerMile,string dateFrom,string dateTo)> GenerateCalculationValues
        (IEnumerable<Vehicle> vehiclesBeforeDataInTimeFrame, BillCalculationRequest request) 
    {
        return vehiclesBeforeDataInTimeFrame
    .OrderBy(x => x.State.AsAt)
    .GroupBy(x => x.LicensePlate)
    .Select(group =>
    {
        var firstItem = group.OrderBy(c => c.State.AsAt).First();
        var lastItem = group.OrderBy(c => c.State.AsAt).Last();

        return (
            odometerBefore: firstItem.State.OdometerInMeters,
            odometerAfter: lastItem.State.OdometerInMeters,
            milesPerHour: firstItem.State.SpeedInMph,
            licensePlate: group.Key,
            make: firstItem.Make,
            costPerMile: request.Cost,
            startInterval: request.StartInterval,
            endInterval: request.EndInterval
        );
    })
    .ToList();
    }
}

