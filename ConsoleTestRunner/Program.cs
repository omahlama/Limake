using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LimakeTest;

namespace RunTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var st = new SituationTest();

            st.TestGeneratingMovesDoesNotBreakGroupings();
        }
    }
}
