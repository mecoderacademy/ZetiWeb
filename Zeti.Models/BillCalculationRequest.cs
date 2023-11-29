using System;
namespace Zeti.Models
{
    public class BillCalculationRequest
    {
        public string StartInterval { get; set; }
        public string EndInterval { get; set; }
        public double Cost { get; set; }
        public List<string> LisencePlates { get; set; }
        public string UploadType { get; set; }

    }
}

