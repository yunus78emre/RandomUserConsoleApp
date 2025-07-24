using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomUserConsoleApp
{
    public class Log
    {
        public string LogLevel { get; set; }  
        public string Message { get; set; }
        public string? Exception { get; set; }
        public string? StackTrace { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
