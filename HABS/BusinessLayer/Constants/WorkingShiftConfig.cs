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
            RefreshAll();
        }
        public string[] ConfigKeyList
        {
            get
            {
                return new string[] {
            "LOGIN_TIME_BEFORE_WORKING_SHIFT",
            "BEGIN_MORNING_HOUR",
            "BEGIN_MORNING_MINUTE",
            "END_MORNING_HOUR",
            "END_MORNING_MINUTE",
            "BEGIN_AFTERNOON_HOUR",
            "BEGIN_AFTERNOON_MINUTE",
            "END_AFTERNOON_HOUR",
            "END_AFTERNOON_MINUTE",
            "BEGIN_EVENING_HOUR",
            "BEGIN_EVENING_MINUTE",
            };
            }
        }
        public void RefreshAll()
        {
            RefreshLoginTimeBeforeWorkingShift();

            RefreshBeginMorningShiftHour();
            RefreshBeginMorningShiftMinute();
            RefreshEndMorningShiftHour();
            RefreshEndMorningShiftMinute();
            RefreshBeginAfternoonShiftHour();
            RefreshBeginAfternoonShiftMinute();
            RefreshEndAfternoonShiftHour();
            RefreshEndAfternoonShiftMinute();
            RefreshBeginEveningShiftHour();
            RefreshBeginEveningShiftMinute();
            RefreshEndEveningShiftHour();
            RefreshEndEveningShiftMinute();
        }
        public void RefreshSpecific(string cfgKey)
        {
            switch (cfgKey)
            {
                case "LOGIN_TIME_BEFORE_WORKING_SHIFT":
                    LoginTimeBeforeWorkingShift = int.Parse(_cfgService.GetValueFromConfig("LOGIN_TIME_BEFORE_WORKING_SHIFT"));
                    break;
                case "BEGIN_MORNING_HOUR":
                    BeginMorningShiftHour = int.Parse(_cfgService.GetValueFromConfig("BEGIN_MORNING_HOUR"));
                    break;
                case "BEGIN_MORNING_MINUTE":
                    BeginMorningShiftMinute = int.Parse(_cfgService.GetValueFromConfig("BEGIN_MORNING_MINUTE"));
                    break;
                case "END_MORNING_HOUR":
                    EndMorningShiftHour = int.Parse(_cfgService.GetValueFromConfig("END_MORNING_HOUR"));
                    break;
                case "END_MORNING_MINUTE":
                    EndMorningShiftMinute = int.Parse(_cfgService.GetValueFromConfig("END_MORNING_MINUTE"));
                    break;
                case "BEGIN_AFTERNOON_HOUR":
                    BeginAfternoonShiftHour = int.Parse(_cfgService.GetValueFromConfig("BEGIN_AFTERNOON_HOUR"));
                    break;
                case "BEGIN_AFTERNOON_MINUTE":
                    BeginAfternoonShiftMinute = int.Parse(_cfgService.GetValueFromConfig("BEGIN_AFTERNOON_MINUTE"));
                    break;
                case "END_AFTERNOON_HOUR":
                    EndAfternoonShiftHour = int.Parse(_cfgService.GetValueFromConfig("END_AFTERNOON_HOUR"));
                    break;
                case "END_AFTERNOON_MINUTE":
                    EndAfternoonShiftMinute = int.Parse(_cfgService.GetValueFromConfig("END_AFTERNOON_MINUTE"));
                    break;
                case "BEGIN_EVENING_HOUR":
                    BeginEveningShiftHour = int.Parse(_cfgService.GetValueFromConfig("BEGIN_EVENING_HOUR"));
                    break;
                case "BEGIN_EVENING_MINUTE":
                    BeginEveningShiftMinute = int.Parse(_cfgService.GetValueFromConfig("BEGIN_EVENING_MINUTE"));
                    break;
            }
        }
        #region Refresh function
        public int RefreshLoginTimeBeforeWorkingShift()
        {
            RefreshSpecific("LOGIN_TIME_BEFORE_WORKING_SHIFT");
            return LoginTimeBeforeWorkingShift;
        }
        public int RefreshBeginMorningShiftHour()
        {
            RefreshSpecific("BEGIN_MORNING_HOUR");
            return BeginMorningShiftHour;
        }
        public int RefreshBeginMorningShiftMinute()
        {
            RefreshSpecific("BEGIN_MORNING_MINUTE");
            return BeginMorningShiftMinute;
        }
        public int RefreshEndMorningShiftHour()
        {
            RefreshSpecific("END_MORNING_HOUR");
            return EndMorningShiftHour;
        }
        public int RefreshEndMorningShiftMinute()
        {
            RefreshSpecific("END_MORNING_MINUTE");
            return EndMorningShiftMinute;
        }
        public int RefreshBeginAfternoonShiftHour()
        {
            RefreshSpecific("BEGIN_AFTERNOON_HOUR");
            return BeginAfternoonShiftHour;
        }
        public int RefreshBeginAfternoonShiftMinute()
        {
            RefreshSpecific("BEGIN_AFTERNOON_MINUTE");
            return BeginAfternoonShiftMinute;
        }
        public int RefreshEndAfternoonShiftHour()
        {
            RefreshSpecific("END_AFTERNOON_HOUR");
            return EndAfternoonShiftHour;
        }
        public int RefreshEndAfternoonShiftMinute()
        {
            RefreshSpecific("END_AFTERNOON_MINUTE");
            return EndAfternoonShiftMinute;
        }
        public int RefreshBeginEveningShiftHour()
        {
            RefreshSpecific("BEGIN_EVENING_HOUR");
            return BeginEveningShiftHour;
        }
        public int RefreshBeginEveningShiftMinute()
        {
            RefreshSpecific("BEGIN_EVENING_MINUTE");
            return BeginEveningShiftMinute;
        }
        public int RefreshEndEveningShiftHour()
        {
            RefreshSpecific("END_EVENING_HOUR");
            return EndEveningShiftHour;
        }
        public int RefreshEndEveningShiftMinute()
        {
            RefreshSpecific("END_EVENING_MINUTE");
            return EndEveningShiftMinute;
        }
        #endregion

        #region Properties
        public int LoginTimeBeforeWorkingShift { get; private set; }
        //public const int LoginTimeBeforeWorkingShift = 15;
        public int BeginMorningShiftHour { get; private set; }
        public int BeginMorningShiftMinute { get; private set; }

        //public const int BeginMorningShiftHour = 7;
        //public const int BeginMorningShiftMinute = 0;

        public int EndMorningShiftHour { get; private set; }
        public int EndMorningShiftMinute { get; private set; }

        //public const int EndMorningShiftHour = 11;
        //public const int EndMorningShiftMinute = 0;
        public int BeginAfternoonShiftHour { get; private set; }
        public int BeginAfternoonShiftMinute { get; private set; }
        //public const int BeginAfternoonShiftHour = 13;
        //public const int BeginAfternoonShiftMinute = 0;
        public int EndAfternoonShiftHour { get; private set; }
        public int EndAfternoonShiftMinute { get; private set; }

        //public const int EndAfternoonShiftHour = 16;
        //public const int EndAfternoonShiftMinute = 0;

        public int BeginEveningShiftHour { get; private set; }
        public int BeginEveningShiftMinute { get; private set; }
        //public const int BeginEveningShiftHour = 19;
        //public const int BeginEveningShiftMinute = 0;
        public int EndEveningShiftHour { get; private set; }
        public int EndEveningShiftMinute { get; private set; }
        //public const int EndEveningShiftHour = 21;
        //public const int EndEveningShiftMinute = 0;
        #endregion

    }
}
