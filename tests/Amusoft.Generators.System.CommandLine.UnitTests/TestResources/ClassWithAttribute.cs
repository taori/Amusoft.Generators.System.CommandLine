using System.CommandLine;
using Amusoft.Generators.System.CommandLine.Attributes;

namespace Amusoft.Generators.System.CommandLine.UnitTests.TestResources;

[GenerateCommandHandler]
public partial class ClassWithAttribute : Command
{
	public ClassWithAttribute(string name, string description = null) : base(name, description)
	{
	}
}