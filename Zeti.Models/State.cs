using System;
namespace Zeti.Models
{
	public class State
	{
		public double OdometerInMeters { get; set; }
        public double SpeedInMph { get; set; }
        private DateTime _asAt;

        public DateTime AsAt
        {
            get => _asAt;
            set => _asAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}

