using System;
using System.Threading.Tasks;

namespace NetworkService.TestRunner.Models
{
    public class Service
    {
        public string Name { get; set; }
        public Func<Task<object>> ServiceMethod { get; set; }
    }
}
