using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.SearchModels.Doctor
{
    public class MedicineSearchModel
    {
        public string Name { get; set; }
        public long? CategoryId { get; set; }
    }
}
