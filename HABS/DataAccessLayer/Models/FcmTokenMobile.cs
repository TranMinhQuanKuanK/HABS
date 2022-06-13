using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class FcmTokenMobile
    {
        public long Id { get; set; }
        public string TokenId { get; set; }
        public long AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}
