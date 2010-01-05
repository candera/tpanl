using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace TPanl.Net
{
    public class SimpleHttpContext : IDisposable
    {
        private readonly Action<string> _logger;
        private readonly SimpleHttpRequest _request;
        private readonly StreamReader _requestReader; 
        private readonly SimpleHttpResponse _response;
        private readonly StreamWriter _responseWriter; 
        private readonly TcpClient _socket; 

        public SimpleHttpRequest Request { get { return _request; } }
        public SimpleHttpResponse Response { get { return _response; } }

        public SimpleHttpContext(TcpClient socket, Action<string> logger)
        {
            _logger = logger; 
            _request = new SimpleHttpRequest();
            _response = new SimpleHttpResponse();
            _socket = socket;

            _requestReader = new StreamReader(socket.GetStream());
            _responseWriter = new StreamWriter(socket.GetStream()); 

            bool firstLine = true;
            string line = null;
            while ((line = _requestReader.ReadLine()) != null)
            {
                Log("Received " + line);
                if (firstLine)
                {
                    var parts = line.Split(' ');
                    _request.Method = parts[0];
                    _request.RawUrl = parts[1];
                    Log("method is " + _request.Method + " url is " + _request.RawUrl);
                    firstLine = false; 
                }
                else if (line.Trim().Length == 0)
                {
                    break; 
                }
            }
            Log("Connection closed");
        }

        public void Write(string body)
        {
            _responseWriter.Write(body); 
        }

        private void Log(string message)
        {
            _logger(message); 
        }

        public void Dispose()
        {
            _responseWriter.Flush();
            _requestReader.Dispose();
            _responseWriter.Dispose();
        }
    }
}
