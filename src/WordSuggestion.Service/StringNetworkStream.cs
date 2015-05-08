using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WordSuggestion.Service
{
    public class StringNetworkStream
    {
        private readonly NetworkStream _baseStream;
        private readonly Encoding _encoding;

        public StringNetworkStream(NetworkStream baseStream, Encoding encoding)
        {
            if (baseStream == null)
                throw new ArgumentNullException("baseStream");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            _baseStream = baseStream;
            _encoding = encoding;
        }

        public bool DataAvailable
        {
            get { return _baseStream.DataAvailable; }
        }

        public Task WriteAsync(string message)
        {
            var buffer = _encoding.GetBytes(message);

            return _baseStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public async Task<string> ReadAsync()
        {
            var buffer = new byte[1024];

            var result = new StringBuilder();

            while (_baseStream.DataAvailable)
            {
                var readCount = await _baseStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                var textMessage = _encoding.GetString(buffer, 0, readCount);
                result.Append(textMessage);
            }

            return result.ToString();
        }
    }
}