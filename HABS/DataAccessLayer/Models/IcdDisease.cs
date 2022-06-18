using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class IcdDisease
    {
        public IcdDisease()
        {
            CheckupRecords = new HashSet<CheckupRecord>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CheckupRecord> CheckupRecords { get; set; }
    }
}
