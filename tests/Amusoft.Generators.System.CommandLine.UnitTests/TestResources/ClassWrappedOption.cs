using System.CommandLine;
using Amusoft.Generators.System.CommandLine.Attributes;

namespace Amusoft.Generators.System.CommandLine.UnitTests.TestResources;

public partial class ClassWrappedOption : Command
{
	public CustomOptionOne OptionOne { get; set; }
	public CustomOptionTwo OptionTwo { get; set; }

	public ClassWrappedOption() : base("sample", "just a sample description")
	{
		AddOption(OptionOne);
		AddOption(OptionTwo);
	}

	[GenerateCommandHandler]
	public partial class ClassWithHandler
	{
	}

	public class CustomOptionOne : Option<int>
	{
		public CustomOptionOne() : base("one", "one")
		{
		}
	}

	public class CustomOptionTwo : Option<string>
	{
		public CustomOptionTwo() : base("two", "two")
		{
		}
	}
}