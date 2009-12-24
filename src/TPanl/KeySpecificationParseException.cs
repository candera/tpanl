using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPanl
{
    public class KeySpecificationParseException : Exception
    {
        public KeySpecificationParseException(string message)
            : base(message)
        {
        }
    }
}
