using System;
using Zeti.Models;

namespace Zeti.Services
{
	public class JorneyService:IJourneyService
	{
		public JorneyService()
		{
		}

        public  List<Invoice> CalculateJorneyCosts(
            List<(double odometerBefore,
        double odometerAfter, double milesPerHour, string liscensePlate,
        string make, double costPerMile, string dateFrom, string dateTo)> calculationValues)
        {
            var costs =new List<Invoice>();
            calculationValues.ForEach(value =>
            {
                const double milesInMeters= 1609.34;
                const double costInMiles = 0.207;


                var odometerDiffrence = value.odometerAfter-value.odometerBefore;
                if (odometerDiffrence < 0) throw new Exception("Odometer cannot be less than 0");
                var totalMiles = odometerDiffrence / milesInMeters;

                var totalCost = Math.Round(totalMiles * costInMiles,2);
                costs.Add(new Invoice{ Make=value.make, LisencePlate=value.liscensePlate, TotalMiles=Math.Round(totalMiles, 1), TotalCost=totalCost, InvoiceDate=DateTime.Today.ToString(), ReadingDateFrom=value.dateFrom, ReadingDateTo= value.dateTo });
            });

            return costs.Distinct().ToList();
        }
    }
}

