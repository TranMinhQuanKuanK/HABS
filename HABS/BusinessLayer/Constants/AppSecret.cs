using BusinessLayer.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Constants
{
    public class AppSecret
    {
        private readonly ConfigService _cfgService;
        public AppSecret(ConfigService cfgService)
        {
            _cfgService = cfgService;
        }
        public string AdminUsername
        {
            get
            {
                return _cfgService.GetValueFromConfig("ADMIN_USERNAME");
            }
        }
        public string AdminPassword
        {
            get
            {
                return _cfgService.GetValueFromConfig("ADMIN_PASSWORD");
            }
        }
        //public string SecretForCashierApp = "Secretttttt%123123123123!@#!@#";
        //public string SecretForDoctorApp = "Secretttttt%123123123123!@#!@#";
        //public string SecretForUserApp = "Secretttttt%123123123123!@#!@#";
        //public string SecretForScreenApp = "Secretttttt%123123123123!@#!@#";
        //public string SecretForAdminApp = "Secretttttt%123123123123!@#!@#";
    }
}
