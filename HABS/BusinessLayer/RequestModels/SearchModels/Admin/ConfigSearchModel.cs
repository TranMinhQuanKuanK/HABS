﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.SearchModels.Admin

{
    public class ConfigSearchModel
    {
        public string SearchTerm { get; set; }
        public int? Type { get; set; }
        public long? Id { get; set; }
    }
}