using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;

namespace WordSuggestion.Server
{
    public class WordSuggestionServer
    {
        private readonly WordSuggestionHandlingService _handlingService;
        private readonly int _port;
        private readonly TcpListener _tcpListener;
        private static readonly ILog Log = LogManager.GetLogger(typeof (WordSuggestionServer));

        public WordSuggestionServer(WordSuggestionHandlingService handlingService, int port)
        {
            _handlingService = handlingService;
            _port = port;
            _tcpListener = new TcpListener(IPAddress.Any, port);
        }

        public async Task Start()
        {
            _tcpListener.Start();
            if (Log.IsInfoEnabled)
            {
                Log.Info(m => m("start listen {0} port", _port));
            }

            while (true)
            {
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                if (Log.IsInfoEnabled)
                {
                    Log.Info(m => m("client {0} has been connected", tcpClient.Client.RemoteEndPoint));
                }
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
            try
            {
                using (client)
                {
                    client.ReceiveBufferSize = 1024;
                    client.SendBufferSize = 1024;
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 10 * 60 * 1000);

                    await _handlingService.Handle(client.GetStream());
                }
            }
            catch (Exception ex)
            {
                Log.Warn("exception occured during connection", ex);
            }
        }
    }
}