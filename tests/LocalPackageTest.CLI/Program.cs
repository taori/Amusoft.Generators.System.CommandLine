using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Amusoft.Generators.System.CommandLine.Attributes;
using LocalPackageTest.CLI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace LocalPackageTest.CLI
{
	internal class Program
	{
		static Task<int> Main(string[] args)
		{
			var command = new ApplicationRootCommand();
			var commandLineBuilder = new CommandLineBuilder(command);

#if DEBUG
			var input = string.Empty;
			do
			{
				Console.WriteLine("Waiting for command:");
				args = Console.ReadLine().Split(" ");
				RunApplicationAsync(args, commandLineBuilder);
			} while (!string.IsNullOrEmpty(input));

			return Task.FromResult(0);
#else
			return RunApplicationAsync(args, commandLineBuilder);
#endif
		}

		private static Task<int> RunApplicationAsync(string[] args, CommandLineBuilder commandLineBuilder)
		{
			return commandLineBuilder
				.UseHost(_ => Host.CreateDefaultBuilder(args), builder =>
				{
					builder.UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location));
					builder.UseConsoleLifetime();
					builder.ConfigureServices((context, services) => { services.AddLogging(logging => logging.AddConsole()); });
				})
				.UseDefaults()
				.UseRuntimeLogLevel("LocalPackageTest.CLI")
				.Build()
				.InvokeAsync(args);
		}
	}

	public class ApplicationRootCommand : System.CommandLine.RootCommand
	{

		public static readonly LogLevelOption LogLevelOption = new(LogLevel.Information);

		public ApplicationRootCommand()
		{
			AddCommand(new Sub1Command());
			AddGlobalOption(LogLevelOption);
		}
	}
	public class LogLevelOption : Option<LogLevel>
	{
		public LogLevelOption(LogLevel logLevelDefault) : base("--logLevel", () => logLevelDefault, "log level for custom application code")
		{
		}
	}

	internal partial class Sub1Command : Command
	{
		public Option<string> TestProperty { get; set; } = new Option<string>("--test", "testparameter");

		public Option<int> Number { get; set; } = new("--number", "Some number");

		public Sub1Command() : base("sub1", "sub1 description")
		{
			AddOption(TestProperty);
			AddOption(Number);
			BindHandler();
		}

		[GenerateCommandHandler]
		internal partial class HandlerCommand
		{
			private readonly ILogger<HandlerCommand> _logger;

			public HandlerCommand(ILogger<HandlerCommand> logger)
			{
				_logger = logger;
			}

			public override Task ExecuteAsync(InvocationContext context, string testProperty, int number)
			{
				_logger.LogError("Printing parameter {test} {Number}", testProperty, number);
				return base.ExecuteAsync(context, testProperty, number);
			}
		}
	}
}