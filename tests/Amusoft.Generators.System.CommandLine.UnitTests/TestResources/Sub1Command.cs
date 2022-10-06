using Amusoft.Generators.System.CommandLine.Attributes;
using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.CommandLine;
using System.Threading.Tasks;

namespace Amusoft.Generators.System.CommandLine.UnitTests.TestResources;


internal partial class Sub1Command : Command
{
	public Option<string> TestProperty { get; set; } = new Option<string>("--test", "testparameter");

	public Sub1Command() : base("sub1", "sub1 description")
	{
		AddOption(TestProperty);
		// BindHandler();
	}

	[GenerateCommandHandler]
	internal partial class HandlerCommand
	{
		private readonly ILogger<HandlerCommand> _logger;

		public HandlerCommand(ILogger<HandlerCommand> logger)
		{
			_logger = logger;
		}

		// public override Task ExecuteAsync(InvocationContext context)
		// {
		// 	_logger.LogError("test");
		// 	return base.ExecuteAsync(context);
		// }
	}
}