using System;
using System.IO;
using System.Linq;

namespace WordSuggestion.Server
{
    class Program
    {
        private static void Main(string[] args)
        {
            var arguments = Parse(args);

            var server = new WordSuggestionServer(new WordSuggestionHandlingService(arguments.SourceFileName), arguments.Port);
            server.Start().Wait();
        }

        private static Arguments Parse(string[] args)
        {
            var filename = args.ElementAtOrDefault(0);
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("can not extract filename");

            if (!File.Exists(filename))
                throw new ArgumentException("file do not exists");

            int port;
            if (!int.TryParse(args.ElementAtOrDefault(1), out port))
                throw new ArgumentException("can not extract port");

            return new Arguments
            {
                Port = port,
                SourceFileName = filename
            };
        }

        private class Arguments
        {
            public string SourceFileName { get; set; }
            public int Port { get; set; }
        }
    }
}
