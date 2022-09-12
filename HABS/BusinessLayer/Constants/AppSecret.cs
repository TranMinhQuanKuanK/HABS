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
        public int SecretForCashierApp
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("CASHIER_SECRET "));
            }
        }
        public int SecretForDoctorApp
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("DOCTOR_SECRET "));
            }
        }
        public int SecretForUserApp
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("USER_SECRET"));
            }
        }
        public int SecretForScreenApp
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("SCREEN_SECRET"));
            }
        }
        public int SecretForAdminApp
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("ADMIN_SECRET"));
            }
        }
        //public string SecretForCashierApp = "Secretttttt%123123123123!@#!@#";
        //public string SecretForDoctorApp = "Secretttttt%123123123123!@#!@#";
        //public string SecretForUserApp = "Secretttttt%123123123123!@#!@#";
        //public string SecretForScreenApp = "Secretttttt%123123123123!@#!@#";
        //public string SecretForAdminApp = "Secretttttt%123123123123!@#!@#";
    }
}
