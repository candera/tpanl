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
        private readonly int _port;
        private readonly Action<SimpleHttpContext> _requestHandler;

        private SimpleHttpListener(int port, Action<SimpleHttpContext> requestHandler)
        {
            _port = port;
            _requestHandler = requestHandler;
        }

        public static SimpleHttpListener Start(int port, Action<SimpleHttpContext> requestHandler)
        {
            var listener = new SimpleHttpListener(port, requestHandler);

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
                    TcpClient client = server.AcceptTcpClient();
                    var context = new SimpleHttpContext(client);
                    _requestHandler(context);
                    context.Close(); 
                }
            }
            finally
            {
                server.Stop();
            }

        }
    }
}
