using System.CommandLine;
using Amusoft.Generators.System.CommandLine.Attributes;

namespace Amusoft.Generators.System.CommandLine.UnitTests.TestResources;

public partial class ClassWithAttribute : Command
{
	public Option<string> OptionOne { get; set; }

	public Option<int> OptionTwo { get; set; }

	public Argument<int> ArgumentOne { get; set; }

	public ClassWithAttribute() : base("sample", "just a sample description")
	{
		AddOption(OptionOne);
		AddOption(OptionTwo);
		AddArgument(ArgumentOne);
	}

	[GenerateCommandHandler]
	public partial class ClassWithHandler
	{
		private readonly string _logger;

		public ClassWithHandler(string logger)
		{
			_logger = logger;
		}
	}
}