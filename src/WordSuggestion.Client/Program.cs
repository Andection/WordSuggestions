using System;
using System.IO;
using System.Linq;

namespace WordSuggestion.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var arguments = Parse(args);

            var input = new StreamReader(Console.OpenStandardInput());
            var output = new StreamWriter(Console.OpenStandardOutput());
            output.AutoFlush = true;
            var client = new WordSuggestionClient(arguments.Hostname, arguments.Port, input, output);

            client.Run().Wait();
        }

        private static Arguments Parse(string[] args)
        {
            var hostname = args.ElementAtOrDefault(0);
            if (string.IsNullOrEmpty(hostname))
                throw new ArgumentException("can not extract hostname from arguments");

            int port;
            if (!int.TryParse(args.ElementAtOrDefault(1), out port))
                throw new ArgumentException("can not extract port from arguments");

            return new Arguments()
            {
                Hostname = hostname,
                Port = port
            };
        }

        private class Arguments
        {
            public string Hostname { get; set; }
            public int Port { get; set; }
        }
    }
}