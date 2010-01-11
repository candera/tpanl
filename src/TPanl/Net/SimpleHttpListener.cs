using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace TPanl.Net
{
    public class SimpleHttpListener
    {
        private readonly Action<string> _logger; 
        private readonly int _port;
        private readonly Action<SimpleHttpContext> _requestHandler;

        private SimpleHttpListener(int port, Action<SimpleHttpContext> requestHandler, Action<string> logger)
        {
            _port = port;
            _requestHandler = requestHandler;
            _logger = logger; 
        }

        public static SimpleHttpListener Start(int port, Action<SimpleHttpContext> requestHandler, Action<string> logger)
        {
            var listener = new SimpleHttpListener(port, requestHandler, logger);

            var thread = new Thread(listener.Run);
            thread.IsBackground = true; 
            thread.Start(); 

            return listener; 
        }

        public void Run()
        {
            TcpListener server = new TcpListener(IPAddress.Any, _port);
            server.Start();

            try
            {
                while (true)
                {
                    TcpClient socket = server.AcceptTcpClient();

                    using (var context = new SimpleHttpContext(socket, _logger))
                    {
                        _requestHandler(context);
                    }
                }
            }
            finally
            {
                server.Stop();
            }

        }

    }
}
