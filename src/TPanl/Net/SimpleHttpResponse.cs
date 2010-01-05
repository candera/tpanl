using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace TPanl.Net
{
    public class SimpleHttpResponse
    {
        public string ContentType { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public SimpleHttpResponse()
        {
            StatusCode = HttpStatusCode.OK;
            StatusDescription = "OK"; 
        }

        internal void WriteHeaders(TextWriter writer)
        {
            writer.WriteLine("HTTP/1.1 {0} {1}", (int) StatusCode, StatusDescription);

            if (ContentType != null)
            {
                writer.WriteLine("Content-Type: {0}", ContentType); 
            }

            writer.WriteLine(); 
        }
    }
}
