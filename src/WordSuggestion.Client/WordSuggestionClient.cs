using System;
using System.IO;
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
        private readonly StreamReader _textReader;
        private readonly StreamWriter _textWriter;
        private TcpClient _tcpClient;

        private readonly ILog Log = LogManager.GetLogger(typeof (WordSuggestionClient));

        public WordSuggestionClient(string hostname, int port, StreamReader textReader, StreamWriter textWriter)
        {
            _hostname = hostname;
            _port = port;
            _textReader = textReader;
            _textWriter = textWriter;
        }

        public async void Run()
        {
            using (_tcpClient = new TcpClient(new IPEndPoint(IPAddress.Any, 1000)))
            {
                await _tcpClient.ConnectAsync(_hostname, _port).ConfigureAwait(false);

                var stream = new StringNetworkStream(_tcpClient.GetStream(), Encoding.ASCII);

                var readLine = await _textReader.ReadLineAsync() ?? string.Empty;

                while (readLine.ToLower() != "exit")
                {
                    if (!string.IsNullOrEmpty(readLine))
                    {
                        await stream.WriteAsync(readLine).ConfigureAwait(false);
                        if (Log.IsInfoEnabled)
                        {
                            var line = readLine;
                            Log.Info(m => m("client: {0} has been sent", line));
                        }
                    }

                    while (stream.DataAvailable)
                    {
                        var message = await stream.ReadAsync().ConfigureAwait(false);
                        await _textWriter.WriteAsync(message).ConfigureAwait(false);
                        if (Log.IsInfoEnabled)
                        {
                            Log.Info(m => m("client: {0} has been received", message));
                        }
                    }

                    readLine = await _textReader.ReadLineAsync().ConfigureAwait(false) ?? string.Empty;
                }
            }
        }

        public void Dispose()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                _tcpClient.Close();
            }
        }
    }
}