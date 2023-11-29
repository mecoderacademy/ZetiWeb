using System;
using Zeti.Models;

namespace Zeti.Services
{
	public interface IDisplayService
    {
		public void Upload(byte[] data, List<Invoice> invoices);
		public string ExportAsString(List<Invoice> invoices);
	}
}

