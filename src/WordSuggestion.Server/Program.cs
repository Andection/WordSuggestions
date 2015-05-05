using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WordSuggestion.Server
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public class TcpServer
    {
        private readonly WordSuggestionHandlingService _handlingService;
        private readonly TcpListener _tcpListener;

        public TcpServer(WordSuggestionHandlingService handlingService,int port)
        {
            _handlingService = handlingService;
            _tcpListener = new TcpListener(IPAddress.Any, port);
        }

        public async void Start()
        {
            _tcpListener.Start();

            while (true)
            {
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                ThreadPool.QueueUserWorkItem(OnNewConnection, tcpClient);
            }
        }

        private void OnNewConnection(object state)
        {
            OnNewConnection((TcpClient) state);
        }

        private void OnNewConnection(TcpClient client)
        {
            using (client)
            {
                _handlingService.Handle(client.GetStream());
            }
        }
    }

    public class WordSuggestionHandlingService
    {
        public void Handle(NetworkStream stream)
        {
            while (true)
            {
                if (stream.DataAvailable)
                {
                    Thread.Sleep(10);
                    continue;
                }

            //    var token=stream.re
            }
        }
    }
}
