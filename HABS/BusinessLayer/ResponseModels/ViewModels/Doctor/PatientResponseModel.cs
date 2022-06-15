using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    public class PatientResponseModel
    {
        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Bhyt { get; set; }
    }
}
