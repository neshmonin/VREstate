using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreClasses
{
    public interface ICSV
    {
        string ToCSV();
        bool FromCSV(string csv);
    }
}
