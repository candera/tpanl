using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace TPanl.Net
{
    public class SimpleHttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }

    }
}
