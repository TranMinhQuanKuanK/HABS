using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels
{
    public class PagingRequestModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
