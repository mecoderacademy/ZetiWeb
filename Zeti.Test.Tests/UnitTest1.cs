using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Zeti.Models;
using Zeti.Services;
using Zeti.Test.Web.Controllers;

namespace Zeti.Test.Tests;

public class InvoiceControllerTest
{
    private IVehicleService _vehicleService;
    private JsonDisplayService _displayService;
    private IJourneyService _journeyService;
    private InvoiceController _invoiceController;
    private HttpClient _client;
    
    public void Setup()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        _client = mockHttpMessageHandler.ToHttpClient();
        _client.BaseAddress = new Uri("http://localhost");
        _vehicleService = new VehicleService(_client, getConfig());
        _journeyService = new JorneyService();
        _displayService = new JsonDisplayService();
        _invoiceController = new InvoiceController(_vehicleService, _journeyService, _displayService);

        SetUpRequests(mockHttpMessageHandler);
    }


    [Test]
    public async Task InvoiceController_IsNull_ReturnsBadRequestMessege()
    {
        var result = await _invoiceController.CalculateBill(null) as BadRequestObjectResult;

        Assert.That(result?.Value, Is.EqualTo("Request is null"));
    }

    [Test]
    public async Task InvoiceController_InvalidCalculationRequest_ReturnsBadRequestMessege()
    {
        var result = await _invoiceController.CalculateBill(new BillCalculationRequest()) as BadRequestObjectResult;

        Assert.That(result?.Value, Is.EqualTo("Invalid calculation request"));
    }

    [Test]
    public async Task InvoiceController_InvalidUrl_ReturnsBadRequestMessege()
    {
     
        var result = await _invoiceController.CalculateBill(new BillCalculationRequest()) as BadRequestObjectResult;

        Assert.That(result?.Value, Is.EqualTo("Invalid calculation request"));
    }


    [Test]
    public async Task InvoiceController_InvalidUr_ReturnsBadRequestMessege()
    {
        var billRequest = new BillCalculationRequest()
        {
            Cost = 0.207,
            StartInterval = "2021-02-01T00:00:00Z",
            EndInterval = "2021-02-28T23:59:00Z",
            LisencePlates = new List<string> { "CBDH 789", "86532 AZE" },
            UploadType = "json"
        };
        
        var result = await _invoiceController.CalculateBill(billRequest) as ContentResult;

        Assert.That(result?.Content, Is.EqualTo(JsonInvoiceResultContent()));
    }
    private IConfiguration getConfig()
    {
        return new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string>
    {
        // Add your configuration key-value pairs here
        {"apiBaseAddress", "http://localhost/"}
    })
    .Build();
}

    private void SetUpRequests(MockHttpMessageHandler mockHttpMessageHandler)
    {
        mockHttpMessageHandler.When("http://localhost/api/vehicles")
        .Respond(HttpStatusCode.OK, "application/json", VehiclesResponseSuccessfulResponse());

        mockHttpMessageHandler.When("http://localhost/api/vehicles/history/2021-02-01T00:00:00Z")
        .Respond(HttpStatusCode.OK, "application/json", VehiclesHistorysResponseSuccessfulResponseBefore());

        mockHttpMessageHandler.When("http://localhost/api/vehicles/history/2021-02-28T23:59:00Z")
        .Respond(HttpStatusCode.OK, "application/json", VehicleHistoryResponseSuccessAfter());

        mockHttpMessageHandler.When("").Respond(HttpStatusCode.BadRequest);

    }


    private string VehiclesResponseSuccessfulResponse()
    {
        return "[\n {\n \"vin\": \"abcd123\",\n \"licensePlate\": \"CBDH 789\",\n \"make\": \"Jaguar\",\n \"model\": \"IPace\",\n \"state\": null\n },\n {\n \"vin\": \"xyz12345\",\n \"licensePlate\": \"86532 AZE\",\n \"make\": \"Tesla\",\n \"model\": \"3\",\n \"state\": null\n },\n {\n \"vin\": \"173hvidvde\",\n \"licensePlate\": \"987 XWE)\",\n \"make\": \"Tesla\",\n \"model\": \"3\",\n \"state\": null\n }\n]";
    }

    private string VehiclesHistorysResponseSuccessfulResponseBefore()
    {
        return "[\n {\n \"vin\": \"abcd123\",\n \"licensePlate\": \"CBDH 789\",\n \"make\": \"Jaguar\",\n \"model\": \"IPace\",\n \"state\": {\n \"odometerInMeters\": 789000,\n \"speedInMph\": 100,\n \"asAt\": \"2021-02-01T00:00:00+00:00\"\n }\n },\n {\n \"vin\": \"xyz12345\",\n \"licensePlate\": \"86532 AZE\",\n \"make\": \"Tesla\",\n \"model\": \"3\",\n \"state\": {\n \"odometerInMeters\": 48280320,\n \"speedInMph\": 10.1,\n \"asAt\": \"2021-02-01T00:00:00+00:00\"\n }\n },\n {\n \"vin\": \"173hvidvde\",\n \"licensePlate\": \"987 XWE\",\n \"make\": \"Tesla\",\n \"model\": \"3\",\n \"state\": {\n \"odometerInMeters\": 43280320,\n \"speedInMph\": 10.1,\n \"asAt\": \"2021-02-01T00:00:00+00:00\"\n }\n }\n]\n";
    }
    private string VehicleHistoryResponseSuccessAfter()
    {
        return "[\n {\n \"vin\": \"abcd123\",\n \"licensePlate\": \"CBDH 789\",\n \"make\": \"Jaguar\",\n \"model\": \"IPace\",\n \"state\": {\n \"odometerInMeters\": 949934,\n \"speedInMph\": 30.7,\n \"asAt\": \"2021-02-28T23:59:00+00:00\"\n }\n },\n {\n \"vin\": \"xyz12345\",\n \"licensePlate\": \"86532 AZE\",\n \"make\": \"Tesla\",\n \"model\": \"3\",\n \"state\": {\n \"odometerInMeters\": 51499008,\n \"speedInMph\": 89,\n \"asAt\": \"2021-02-28T23:59:00+00:00\"\n }\n },\n {\n \"vin\": \"173hvidvde\",\n \"licensePlate\": \"987 XWE\",\n \"make\": \"Tesla\",\n \"model\": \"3\",\n \"state\": {\n \"odometerInMeters\": 47199008,\n \"speedInMph\": 89,\n \"asAt\": \"2021-02-28T23:59:00+00:00\"\n }\n }\n]";
    }

    private string JsonInvoiceResultContent()
    {
        return "[{\"Make\":\"Jaguar\",\"LisencePlate\":\"CBDH 789\",\"TotalMiles\":100.0,\"TotalCost\":20.7,\"InvoiceDate\":\"11/28/2023 12:00:00AM\",\"InvoiceNumber\":0.0,\"ReadingDateFrom\":null,\"ReadingDateTo\":null},{\"Make\":\"Tesla\",\"LisencePlate\":\"86532 AZE\",\"TotalMiles\":2000.0,\"TotalCost\":414.0,\"InvoiceDate\":\"11/28/2023 12:00:00AM\",\"InvoiceNumber\":0.0,\"ReadingDateFrom\":null,\"ReadingDateTo\":null}]";
    }
}
