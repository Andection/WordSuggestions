using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using WordSuggestion.Service;

namespace WordSuggestion.Server
{
    public class WordSuggestionHandlingService
    {
        private readonly ILog _log = LogManager.GetLogger(typeof (WordSuggestionHandlingService));

        public WordSuggestionHandlingService(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                var dictionary = DictionaryLoader.Load(new StreamReader(stream));
                SuggestionManager.Init(dictionary);
            }
        }

        public async Task Handle(NetworkStream stream)
        {
            var stringStream = new WordSuggestionStream(new StringNetworkStream(stream, Encoding.ASCII));

            while (true)
            {
                if (!stringStream.DataAvailable)
                {
                    await Task.Delay(10);
                    continue;
                }
                var suggestionToken = await stringStream.ReadAsync().ConfigureAwait(false)??string.Empty;

                if (_log.IsInfoEnabled)
                {
                    _log.Info(m => m("recieved {0}", suggestionToken));
                }

                var suggestions = SuggestionManager.Suggest(suggestionToken);

                var suggestionsMessage = String.Join(Environment.NewLine, suggestions);
                await stringStream.WriteAsync(suggestionsMessage).ConfigureAwait(false);
                if (_log.IsInfoEnabled)
                {
                    _log.Info(m => m("sent {0}", suggestionsMessage));
                }
            }
        }
    }
}