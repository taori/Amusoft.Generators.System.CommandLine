using System.CommandLine;
using Amusoft.Generators.System.CommandLine.Attributes;

namespace Amusoft.Generators.System.CommandLine.UnitTests.TestResources;

public partial class ClassTwoArguments : Command
{
	public Argument<int> ArgumentOne { get; set; }
	public Argument<int> ArgumentTwo { get; set; }

	public ClassTwoArguments() : base("sample", "just a sample description")
	{
		AddArgument(ArgumentOne);
		AddArgument(ArgumentTwo);
	}

	[GenerateCommandHandler]
	public partial class ClassWithHandler
	{
		private readonly string _logger;

		public ClassWithHandler(string logger, int someOtherService)
		{
			_logger = logger;
		}
	}
}