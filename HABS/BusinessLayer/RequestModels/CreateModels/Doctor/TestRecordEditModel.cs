using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.CreateModels.Doctor
{
    public class TestRecordEditModel
    {
        public long Id { get; set; }
        public int? Status { get; set; }
        public IFormFile ResultFile { get; set; }
        public string ResultDescription { get; set; }

    }
}
