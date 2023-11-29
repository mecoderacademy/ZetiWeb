using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zeti.Services;
using Zeti.Test.Web.Controllers;

namespace Zeti.Tests;

[TestClass]
public class ControllerTest
{
    private  IVehicleService _vehicleService;
    private  IDisplayService _displayService;
    private  IJourneyService _journeyService;
    private  InvoiceController _invoiceController;

    [ClassInitialize]
    public void Setup()
    {
        _vehicleService = new VehicleService(new HttpClient());
        _journeyService = new JorneyService();
        _invoiceController = new InvoiceController(_vehicleService,_journeyService,_displayService);
    }

    [TestMethod]
    public async Task InvoiceController_IsNull_ReturnsBadRequestMessege()
    {
        var result =await _invoiceController.CalculateBill(null) as BadRequestObjectResult;

        Assert.AreEqual(result?.Value, "Request is null");
    }
}
