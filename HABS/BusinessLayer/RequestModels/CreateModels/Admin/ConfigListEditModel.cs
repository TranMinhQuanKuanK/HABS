using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.CreateModels.Admin
{
    public class ConfigInListEditModel
    {
        public long Id { get; set; }
        public string Value { get; set; }
    }
    public class ConfigListEditModel
    {
        public List<ConfigInListEditModel> List { get; set; }
    }
}
