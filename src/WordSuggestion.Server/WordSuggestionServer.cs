using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WordSuggestion.Server
{
    public class WordSuggestionServer
    {
        private readonly WordSuggestionHandlingService _handlingService;
        private readonly TcpListener _tcpListener;

        public WordSuggestionServer(WordSuggestionHandlingService handlingService, int port)
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

        public void Stop()
        {
            if (_tcpListener != null)
            {
                _tcpListener.Stop();
            }
        }

        private void OnNewConnection(object state)
        {
            OnNewConnection((TcpClient) state);
        }

        private async void OnNewConnection(TcpClient client)
        {
            using (client)
            {
                await _handlingService.Handle(client.GetStream());
            }
        }
    }
}