using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using WordSuggestion.Service;

namespace WordSuggestion.Server
{
    public class WordSuggestionHandlingService
    {
        private readonly ILog _log = LogManager.GetLogger(typeof (WordSuggestionHandlingService));
        private readonly SuggestionService _suggestionService;

        public WordSuggestionHandlingService(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                var dictionary = DictionaryLoader.Load(new StreamReader(stream));

                _suggestionService = new SuggestionService(dictionary);
            }
        }

        public async Task Handle(TcpClient client)
        {
            var wordSuggestionStream = new WordSuggestionStream(new StringNetworkStream(client.GetStream(), Encoding.ASCII));

            while (true)
            {
                var suggestionToken = await wordSuggestionStream.ReadAsync().ConfigureAwait(false) ?? string.Empty;

                if (_log.IsInfoEnabled)
                {
                    _log.Info(m => m("recieved {0}", suggestionToken));
                }

                var suggestions = _suggestionService.Suggest(suggestionToken);

                var suggestionsMessage = String.Join(Environment.NewLine, suggestions);
                await wordSuggestionStream.WriteAsync(suggestionsMessage).ConfigureAwait(false);
                if (_log.IsInfoEnabled)
                {
                    _log.Info(m => m("sent {0}", suggestionsMessage));
                }
            }
        }
    }
}