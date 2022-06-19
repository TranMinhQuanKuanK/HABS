using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Constants
{
    public class WorkingShiftConstant
    {
        public readonly DateTime BeginMorningShift = new DateTime(0, 0, 0, 7, 0, 0);
        public readonly DateTime EndMorningShift = new DateTime(0, 0, 0, 11, 0, 0);
        public readonly DateTime BeginAfternoonShift = new DateTime(0, 0, 0, 13, 0, 0);
        public readonly DateTime EndAfternoonShift = new DateTime(0, 0, 0, 16, 0, 0);
        public readonly DateTime BeginEveningShift = new DateTime(0, 0, 0, 19, 0, 0);
        public readonly DateTime EndEveningShift = new DateTime(0, 0, 0, 21, 0, 0);
    }
}
