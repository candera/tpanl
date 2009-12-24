using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPanl
{
    internal class TimerAction
    {
        public Action Action { get; set; }
        public DateTime When { get; set; }
    }
}
