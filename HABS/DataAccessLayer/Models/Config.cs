    using System;
    using System.Collections.Generic;

    #nullable disable

    namespace DataAccessLayer.Models
    {
        public partial class Config
        {
            public enum ConfigType
            {
                DEVELOPER_CONFIG,
                USER_CONFIG,
                PASSWORD_CONFIG,
            }
            public long Id { get; set; }
            public string Name { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public string Description { get; set; }
            public ConfigType? Type { get; set; }
        }
    }
