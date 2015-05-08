using System;
using System.IO;
using System.Text;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using WordSuggestion.Client;
using WordSuggestion.Server;

namespace WordSuggestion.IntegrationTests
{
    [TestFixture]
    public class TcpServerIntegrationTests
    {
        private const int Port = 1212;
        private WordSuggestionServer _server;
        private WordSuggestionClient _client;
        private MemoryStream _clientInput;
        private MemoryStream _clientOutput;
        private StreamReader _streamReader;

        [SetUp]
        public void SetUp()
        {
            _server = new WordSuggestionServer(new WordSuggestionHandlingService("input.txt"), Port);
          
            _clientInput = new MemoryStream();
            _clientOutput = new MemoryStream();
            _streamReader = new StreamReader(_clientInput);
            var streamWriter = new StreamWriter(_clientOutput) {AutoFlush = true};
            _client = new WordSuggestionClient("localhost", Port, _streamReader, streamWriter);

            _server.Start();
            _client.Run();

            Thread.Sleep(1000);
        }

        [TearDown]
        public void TearDown()
        {
            _clientInput.Dispose();
            _clientOutput.Dispose();
            _client.Dispose();
            _server.Stop();
        }

        [Test]
        public void should_suggest_by_one_token()
        {
            const string simpleToken = "a";
            var bytes = Encoding.ASCII.GetBytes("get " + simpleToken + Environment.NewLine);
            _clientInput.Write(bytes, 0, bytes.Length);
            _clientInput.Flush();
            _clientInput.Seek(0, SeekOrigin.Begin);

            Thread.Sleep(3000);

            var buffer = new byte[1024];

            _clientOutput.Position = 0;
            var readByteCount = _clientOutput.Read(buffer, 0, buffer.Length);

            var readTex = Encoding.ASCII.GetString(buffer, 0, readByteCount);

            readTex.Should().NotBeNull();

            readTex.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).Should().HaveCount(10);
        }
    }
}