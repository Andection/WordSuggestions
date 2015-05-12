using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordSuggestion.Service
{
    public class WordSuggestionStream
    {
        private readonly StringNetworkStream _stringStream;

        //Сообщения обрамляются специальными символами.
        private const char StartMessage = (char) 0x02;
        private const char EndMessage = (char) 0x03;
        private string _notCompletedMessage = string.Empty;

        public WordSuggestionStream(StringNetworkStream stringStream)
        {
            _stringStream = stringStream;
        }

        public bool DataAvailable
        {
            get { return _stringStream.DataAvailable || !string.IsNullOrEmpty(_notCompletedMessage); }
        }

        public Task WriteAsync(string message)
        {
            return _stringStream.WriteAsync(StartMessage + message + EndMessage);
        }

        public Task<string> ReadAsync()
        {
            return ReadAsync(new CancellationTokenSource().Token);
        }

        public async Task<string> ReadAsync(CancellationToken cancellationToken)
        {
            var result = new StringBuilder();

            var lastSegment = _notCompletedMessage;

            result.Append(lastSegment);
            while (lastSegment.IndexOf(EndMessage) == -1)
            {
                lastSegment = await _stringStream.ReadAsync(cancellationToken).ConfigureAwait(false);
                result.Append(lastSegment);
            }

            var receivedText = result.ToString();

            var startIndex = receivedText.IndexOf(StartMessage);
            var endIndex = receivedText.IndexOf(EndMessage);
            var message = receivedText.Substring(startIndex + 1, endIndex - startIndex - 1);

            _notCompletedMessage = _notCompletedMessage.Length > endIndex + 1 ? message.Substring(endIndex + 1) : string.Empty;

            return message;
        }
    }
}