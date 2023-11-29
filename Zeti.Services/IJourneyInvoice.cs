using Zeti.Models;

namespace Zeti.Services;

public interface IJourneyService
{
    public List<Invoice> CalculateJorneyCosts(List<(double odometerBefore,
        double odometerAfter, double milesPerHour, string liscensePlate,
        string make, double costPerMile, string dateFrom, string dateTo)> calculationValues);


}