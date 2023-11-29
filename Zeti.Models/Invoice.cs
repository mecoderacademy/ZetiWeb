using System;
namespace Zeti.Models
{
	public class Invoice
	{
        public Invoice()
        {
            InvoiceNumber = new Random().Next(10000000, 999999999);
        }
        public string Make { get; set; }
        public string LisencePlate { get; set; }
        public double TotalMiles { get; set; }
        public double TotalCost { get; set; }
        public string InvoiceDate { get; set; }
        private int invoiceNumber;
        public int InvoiceNumber
        {
            get => invoiceNumber;
            set
            {
                // Ensure that the value is within the specified range
                invoiceNumber = value < 10000000 ? 10000000 : (value > 999999999 ? 999999999 : value);
            }
        }

        public string ReadingDateFrom { get; set; }
        public string ReadingDateTo { get; set; }



    }
}

