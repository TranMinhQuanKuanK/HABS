using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Admin
{
    public class ConfigViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public int? Type { get; set; }
    }
}
