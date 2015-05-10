using System;
using System.Linq;
using Common.Logging;

namespace WordSuggestion.Client
{
    internal class Program
    {
        private const string GetToken = "get ";
        private const string ExitToken = "exit";
        private const string Help = "cannot recognize command. Type \"get <prefix>\" for suggestion or \"exit\" for exit ";
        private const string HaveNoSuggestions = "Have no suggestions";
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            var arguments = Parse(args);

            using (var client = new WordSuggestionClient(arguments.Hostname, arguments.Port))
            {
                client.Connect().Wait();

                var line = Console.ReadLine() ?? string.Empty;

                while (line != ExitToken)
                {
                    var suggestionToken = TryExtractSuggestion(line);
                    if (string.IsNullOrEmpty(suggestionToken))
                    {
                        Console.WriteLine(Help);
                    }
                    else
                    {
                        var suggestions = client.GetSuggestions(suggestionToken).Result;
                        if (suggestions.Any())
                        {
                            foreach (var suggestion in suggestions)
                            {
                                Console.WriteLine(suggestion);
                            }
                        }
                        else
                        {
                            Console.WriteLine(HaveNoSuggestions);
                        }
                    }

                    line = Console.ReadLine() ?? string.Empty;
                }
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Fatal("unhandled exception occured", (Exception) e.ExceptionObject);
        }

        private static string TryExtractSuggestion(string line)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            var startGetTokenIndex = line.IndexOf(GetToken, StringComparison.OrdinalIgnoreCase);
            if (startGetTokenIndex == -1)
                return string.Empty;

            var startTokenIndex = startGetTokenIndex + GetToken.Length;
            if (line.Length <= startTokenIndex)
                return string.Empty;

            var endTokenIndex = line.IndexOf(" ", startTokenIndex, StringComparison.OrdinalIgnoreCase);
            return endTokenIndex == -1 ? line.Substring(startTokenIndex) : line.Substring(startTokenIndex, endTokenIndex - startTokenIndex);
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