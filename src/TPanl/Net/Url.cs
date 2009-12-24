using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPanl.Net
{
    public class Url
    {
        public override bool Equals(object obj)
        {
            throw new NotImplementedException(); 
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException(); 
        }

        public static implicit operator string(Url url)
        {
            throw new NotImplementedException(); 
        }

        public static implicit operator Url(string url)
        {
            throw new NotImplementedException(); 
        }
    }
}
