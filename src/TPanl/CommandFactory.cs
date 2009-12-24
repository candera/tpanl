using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPanl
{
    public abstract class CommandFactory
    {
        public abstract Command Create(string[] parameters); 
    }
}
