﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WMM.Data
{
    public enum ForecastType
    {
        Exception = 0,  // exceptional expenses, not forcastable
        Monthly = 1,    // to be forcasted by a monthly mean value
        Daily = 2       // to be forcasted by extrapolating a daily mean value 
    }
}
