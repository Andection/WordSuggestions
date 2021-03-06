﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WordSuggestion.Service;

namespace WordSuggestion.Client
{
    public class WordSuggestionClient : IDisposable
    {
        private readonly string _hostname;
        private readonly int _port;
        private TcpClient _tcpClient;

        public WordSuggestionClient(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;
        }

        public Task Connect()
        {
            if (_tcpClient != null)
                throw new InvalidOperationException();

            _tcpClient = new TcpClient(new IPEndPoint(IPAddress.Any, 0)) {ReceiveBufferSize = 1024, SendBufferSize = 1024};
            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            return _tcpClient.ConnectAsync(_hostname, _port);
        }

        public async Task<string[]> GetSuggestions(string token)
        {
            var suggestionSteam = new WordSuggestionStream(new StringNetworkStream(_tcpClient.GetStream(), Encoding.ASCII));

            await suggestionSteam.WriteAsync(token).ConfigureAwait(false);

            var rawSuggestions = await suggestionSteam.ReadAsync(new CancellationTokenSource(TimeSpan.FromMinutes(1)).Token).ConfigureAwait(false);

            return rawSuggestions.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        }

        public void Dispose()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                _tcpClient.Close();
                _tcpClient = null;
            }
        }
    }
}