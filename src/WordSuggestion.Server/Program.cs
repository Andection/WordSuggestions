using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WordSuggestion.Service;

namespace WordSuggestion.Server
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

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
        public WordSuggestionHandlingService(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                var dictionary = DictionaryLoader.Load(new StreamReader(stream));
                SuggestionManager.Init(dictionary);
            }
        }

        public async void Handle(NetworkStream stream)
        {
            var buffer = new byte[1024];

            while (true)
            {
                if (!stream.DataAvailable)
                {
                    Thread.Sleep(10);
                    continue;
                }

                var readLenght = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                var message = Encoding.ASCII.GetString(buffer, 0, readLenght);

                var suggestTokens = message.Split(new[] {"get"}, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();

                foreach (var token in suggestTokens)
                {
                    var suggestions = SuggestionManager.Suggest(token);

                    foreach (var sendBytes in suggestions.Select(suggestion => Encoding.ASCII.GetBytes(suggestion + Environment.NewLine)))
                    {
                        await stream.WriteAsync(sendBytes, 0, sendBytes.Length).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
