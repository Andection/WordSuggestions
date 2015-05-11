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
        private readonly byte[] _readBuffer = new byte[1024];
        private readonly byte[] _writeBuffer = new byte[1024];

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
            var bytesCount = _encoding.GetBytes(message, 0, message.Length, _writeBuffer, 0);
            return _baseStream.WriteAsync(_writeBuffer, 0, bytesCount);
        }

        public async Task<string> ReadAsync()
        {
            var readCount = await _baseStream.ReadAsync(_readBuffer, 0, _readBuffer.Length).ConfigureAwait(false);

            return _encoding.GetString(_readBuffer, 0, readCount);
        }
    }
}