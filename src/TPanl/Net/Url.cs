using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPanl.Net
{
    public class Url
    {
        private string _path;
        private readonly Dictionary<string, IEnumerable<string>> _queryParameters = new Dictionary<string, IEnumerable<string>>();
        private readonly string _rawUrl;

        public string Path
        {
            get
            {
                return _path; 
            }
        }

        public IDictionary<string, IEnumerable<string>> QueryParameters
        {
            get { return _queryParameters; }
        }

        public Url(string url)
        {
            _rawUrl = url;
            ParseQuery();
            ParsePath(); 
        }

        private void ParsePath()
        {
            int start = 0; 
            int end = _rawUrl.Length;
            
            int queryStringIndex = _rawUrl.IndexOf('?');
            if (queryStringIndex != -1)
            {
                end = queryStringIndex; 
            }

            int separatorIndex = _rawUrl.IndexOf("://"); 
            if (separatorIndex != -1)
            {
                start = _rawUrl.IndexOf('/', separatorIndex + "://".Length); 
            }

            _path = _rawUrl.Substring(start, end - start); 
            
        }

        private void ParseQuery()
        {
            int queryStringIndex = _rawUrl.IndexOf('?');

            if (queryStringIndex != -1)
            {
                var rawQuery = _rawUrl.Substring(queryStringIndex + 1);

                if (rawQuery.Length == 0)
                {
                    return; 
                }

                var pairs = rawQuery.Split('&');

                foreach (var pair in pairs)
                {
                    var nameValue = pair.Split(new char[] { '=' }, 2);

                    var name = nameValue[0];
                    string value = string.Empty;

                    if (nameValue.Length > 1)
                    {
                        value = nameValue[1];
                    }

                    if (!_queryParameters.ContainsKey(name))
                    {
                        _queryParameters.Add(name, new List<string>());
                    }

                    ((List<string>)_queryParameters[name]).Add(value);
                }
            }
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
                value = that._rawUrl;               
            }
            else if (obj is string)
            {
                value = obj as string;
            }
            else
            {
                return false; 
            }

            return value.Equals(this._rawUrl); 
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException(); 
        }

        public static implicit operator string(Url url)
        {
            return url._rawUrl; 
        }

        public static implicit operator Url(string url)
        {
            return new Url(url); 
        }

        public override string ToString()
        {
            return _rawUrl; 
        }
    }
}
