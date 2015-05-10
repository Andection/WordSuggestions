using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using WordSuggestion.Client;

namespace WordSuggestion.Server.Tests
{
    [TestFixture]
    public class TcpServerIntegrationTests
    {
        private const int Port = 1212;
        private WordSuggestionServer _server;
        private WordSuggestionClient _client;

        [SetUp]
        public void SetUp()
        {
            _server = new WordSuggestionServer(new WordSuggestionHandlingService("input.txt"), Port);

            _client = new WordSuggestionClient("localhost", Port);

            _server.Start();
            _client.Connect();

            Thread.Sleep(1000);
        }

        [TearDown]
        public void TearDown()
        {

            _client.Dispose();
            _server.Stop();
        }

        [Test]
        public async void should_suggest_by_one_token()
        {
            const string simpleToken = "a";

            var suggestions = await _client.GetSuggestions(simpleToken);

            suggestions.Should().NotBeNull();

            suggestions.Should().HaveCount(10);
        }
    }
}