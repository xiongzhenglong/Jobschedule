using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Core.Common.Extension
{
    public static class DateExtension
    {
        public static string ToDateStr(this DateTime input)
        {
            return input.ToString("yyyy-MM-dd");
        }
    }
}
