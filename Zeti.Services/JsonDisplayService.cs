using System;
using Newtonsoft.Json;
using Zeti.Models;

namespace Zeti.Services
{
    public class JsonDisplayService : IDisplayService
    {
        public JsonDisplayService()
        {

        }
        public string ExportAsString(List<Invoice> invoices)
        {
            return JsonConvert.SerializeObject(invoices);
        }

        public void Upload(byte[] data, List<Invoice> invoices)
        {
            throw new NotImplementedException();
        }
    }
}

