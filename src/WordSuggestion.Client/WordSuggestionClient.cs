using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WordSuggestion.Client
{
    public class WordSuggestionClient:IDisposable
    {
        private readonly string _hostname;
        private readonly int _port;
        private readonly StreamReader _textReader;
        private readonly StreamWriter _textWriter;
        private TcpClient _tcpClient;

        public WordSuggestionClient(string hostname, int port, StreamReader textReader, StreamWriter textWriter)
        {
            _hostname = hostname;
            _port = port;
            _textReader = textReader;
            _textWriter = textWriter;
        }

        public async void Start()
        {
            _tcpClient = new TcpClient(new IPEndPoint(IPAddress.Any, 1000));
            await _tcpClient.ConnectAsync(_hostname, _port).ConfigureAwait(false);

            var readLine = await _textReader.ReadLineAsync() ?? string.Empty;

            var buffer = new Byte[1024];

            while (readLine.ToLower() != "exit")
            {
                if (!string.IsNullOrEmpty(readLine))
                {
                    var bytes = Encoding.ASCII.GetBytes(readLine);

                    await _tcpClient.GetStream().WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);

                    var textData = await Read(buffer);
                    await _textWriter.WriteAsync(textData).ConfigureAwait(false);

                    while (_tcpClient.Available != 0)
                    {
                        textData = await Read(buffer);
                        await _textWriter.WriteAsync(textData).ConfigureAwait(false);
                    }
                }
                readLine = await _textReader.ReadLineAsync().ConfigureAwait(false) ?? string.Empty;
            }
        }

        private async Task<string> Read(byte[] buffer)
        {
            var readData = await _tcpClient.GetStream().ReadAsync(buffer, 0, buffer.Length);

            return Encoding.ASCII.GetString(buffer, 0, readData);
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