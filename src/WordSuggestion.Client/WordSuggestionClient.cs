using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using WordSuggestion.Service;

namespace WordSuggestion.Client
{
    public class WordSuggestionClient : IDisposable
    {
        private readonly string _hostname;
        private readonly int _port;
        private TcpClient _tcpClient;

        private readonly ILog _log = LogManager.GetLogger(typeof (WordSuggestionClient));

        private const int TimeOut = 60*1000;
        private const int WaitStepInterval = 10;

        public WordSuggestionClient(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;
        }

        public Task Connect()
        {
            if (_tcpClient != null)
                throw new InvalidOperationException();

            _tcpClient = new TcpClient(new IPEndPoint(IPAddress.Any, 0));
            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 10*60*1000);
            _tcpClient.ReceiveBufferSize = 1024;
            _tcpClient.SendBufferSize = 1024;
            _tcpClient.ReceiveTimeout = 1*1000;

            return _tcpClient.ConnectAsync(_hostname, _port);
        }

        public async Task<string[]> GetSuggestions(string token)
        {
            var suggestionSteam = new WordSuggestionStream(new StringNetworkStream(_tcpClient.GetStream(), Encoding.ASCII));

            await suggestionSteam.WriteAsync(token).ConfigureAwait(false);

      //      var waitTime = 0;
      //      while (!suggestionSteam.DataAvailable && waitTime < TimeOut)
      //      {
      //          waitTime += WaitStepInterval;
      //          await Task.Delay(WaitStepInterval);
      //      }
      //
      //      if (waitTime >= TimeOut)
      //      {
      //          throw new TimeoutException("server do not return suggestions");
      //      }
      //
            var rawSuggestions = await suggestionSteam.ReadAsync().ConfigureAwait(false);

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