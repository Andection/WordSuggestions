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

        [SetUp]
        public void SetUp()
        {
            _server = new WordSuggestionServer(new WordSuggestionHandlingService("input.txt"), Port);
          
            _clientInput = new MemoryStream();
            _clientOutput = new MemoryStream();
            _client = new WordSuggestionClient("localhost", Port, new StreamReader(_clientInput), new StreamWriter(_clientOutput));

            _server.Start();
            _client.Start();

            Thread.Sleep(1000);
        }

        [Test]
        public void should_suggest_by_one_token()
        {
            const string simpleToken = "a";
            var bytes = Encoding.ASCII.GetBytes("get " + simpleToken + Environment.NewLine);
            _clientInput.Write(bytes, 0, bytes.Length);

            Thread.Sleep(10000);

            var buffer = new byte[1024];

            var readByteCount = _clientOutput.Read(buffer, 0, buffer.Length);

            var readTex = Encoding.ASCII.GetString(buffer, 0, readByteCount);

            readTex.Should().NotBeNull();

            readTex.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).Should().HaveCount(10);
        }
    }
}