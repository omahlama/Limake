using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Limake
{
    public class LimakeException : Exception
    {
        public String Errors { get; set; }
        public LimakeException(string Errors) { this.Errors = Errors; }
    }
}
