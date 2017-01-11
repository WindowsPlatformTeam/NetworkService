using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Impl.Models
{
    public class RetryModel
    {
        public TimeSpan SleepPeriod { get; set; }
        public int RetryCount { get; set; }
    }
}
