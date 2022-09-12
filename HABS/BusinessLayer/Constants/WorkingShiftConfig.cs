using BusinessLayer.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Constants
{
    public class WorkingShiftConfig
    {
        private readonly ConfigService _cfgService;
        public WorkingShiftConfig(ConfigService cfgService)
        {
            _cfgService = cfgService;
        }
        public int LoginTimeBeforeWorkingShift
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("LOGIN_TIME_BEFORE_WORKING_SHIFT"));
            }
        }
        //public const int LoginTimeBeforeWorkingShift = 15;
        public int BeginMorningShiftHour
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("BEGIN_MORNING_HOUR"));
            }
        }
        public int BeginMorningShiftMinute
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("BEGIN_MORNING_MINUTE"));
            }
        }

        //public const int BeginMorningShiftHour = 7;
        //public const int BeginMorningShiftMinute = 0;

        public int EndMorningShiftHour
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("END_MORNING_HOUR"));
            }
        }
        public int EndMorningShiftMinute
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("END_MORNING_MINUTE"));
            }
        }

        //public const int EndMorningShiftHour = 11;
        //public const int EndMorningShiftMinute = 0;
        public int BeginAfternoonShiftHour
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("BEGIN_AFTERNOON_HOUR"));
            }
        }
        public int BeginAfternoonShiftMinute
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("BEGIN_AFTERNOON_MINUTE"));
            }
        }
        //public const int BeginAfternoonShiftHour = 13;
        //public const int BeginAfternoonShiftMinute = 0;
        public int EndAfternoonShiftHour
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("END_AFTERNOON_HOUR"));
            }
        }
        public int EndAfternoonShiftMinute
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("END_AFTERNOON_MINUTE"));
            }
        }

        //public const int EndAfternoonShiftHour = 16;
        //public const int EndAfternoonShiftMinute = 0;

        public int BeginEveningShiftHour
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("BEGIN_EVENING_HOUR"));
            }
        }
        public int BeginEveningShiftMinute
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("BEGIN_EVENING_MINUTE"));
            }
        }
        //public const int BeginEveningShiftHour = 19;
        //public const int BeginEveningShiftMinute = 0;
        public int EndEveningShiftHour
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("END_EVENING_HOUR"));
            }
        }
        public int EndEveningShiftMinute
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("END_EVENING_MINUTE"));
            }
        }
        //public const int EndEveningShiftHour = 21;
        //public const int EndEveningShiftMinute = 0;

    }
}
