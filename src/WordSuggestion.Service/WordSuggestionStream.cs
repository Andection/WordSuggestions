﻿using System.Text;
using System.Threading.Tasks;

namespace WordSuggestion.Service
{
    public class WordSuggestionStream
    {
        private readonly StringNetworkStream _stringStream;

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

        public async Task<string> ReadAsync()
        {
            var result = new StringBuilder();

            result.Append(_notCompletedMessage);
            //дополнительных выделений памяти быть не должно для небольших (<64 кб) строк
            while (result.ToString().IndexOf(EndMessage) == -1)
            {
                var receivedSegment = await _stringStream.ReadAsync().ConfigureAwait(false);
                result.Append(receivedSegment);
            }

            var receivedText = result.ToString();

            var startIndex = receivedText.IndexOf(StartMessage);
            //startIndex==-1????
            var endIndex = receivedText.IndexOf(EndMessage);
            var message = receivedText.Substring(startIndex + 1, endIndex - startIndex - 1);

            _notCompletedMessage = _notCompletedMessage.Length > endIndex + 1 ? message.Substring(endIndex + 1) : string.Empty;

            return message;
        }
    }
}