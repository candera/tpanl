using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace TPanl.Net
{
    public class SimpleHttpContext
    {
        private readonly SimpleHttpResponse _response;
        private readonly SimpleHttpRequest _request;

        public SimpleHttpRequest Request { get { return _request; } }
        public SimpleHttpResponse Response { get { return _response; } }

        public SimpleHttpContext(TcpClient client)
        {
            _request = new SimpleHttpRequest();
            _response = new SimpleHttpResponse();

            using (StreamReader reader = new StreamReader(client.GetStream()))
            {
                bool firstLine = true;
                string resource = null;
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    Log("Received " + line);
                    if (firstLine)
                    {
                        var parts = line.Split(' ');
                        _request.Method = parts[0];
                        _request.RawUrl = parts[1];
                        Log("method is " + _request.Method + " url is " + _request.RawUrl);
                    }
                }
                reader.Close();
                Log("Connection closed");
                SetIcon();
            }
        }

        private void SetIcon()
        {
            //throw new NotImplementedException();
        }

        private void Log(string p)
        {
            //throw new NotImplementedException();
        }

        internal void Close()
        {
            //throw new NotImplementedException();
        }
    }
}
