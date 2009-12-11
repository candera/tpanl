using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TPanl
{
    public class ClickCommandMap : Dictionary<string, KeyEventSequence>
    {
        public ClickCommandMap() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }
    }
}
