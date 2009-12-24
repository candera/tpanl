using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPanl.Net
{
    public class Url
    {
        private readonly string _url; 

        public Url(string url)
        {
            _url = url; 
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false; 
            }

            string value; 

            if (obj is Url)
            {
                Url that = obj as Url;
                value = that._url;               
            }
            else if (obj is string)
            {
                value = obj as string;
            }
            else
            {
                return false; 
            }

            return value.Equals(this._url); 
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException(); 
        }

        public static implicit operator string(Url url)
        {
            return url._url; 
        }

        public static implicit operator Url(string url)
        {
            return new Url(url); 
        }

        public override string ToString()
        {
            return _url; 
        }
    }
}
