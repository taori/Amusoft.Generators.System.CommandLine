using System.CommandLine;
using Amusoft.Generators.System.CommandLine.Attributes;

namespace Amusoft.Generators.System.CommandLine.UnitTests.TestResources;

public partial class ClassWithoutAttribute : Command
{
	public ClassWithoutAttribute(string name, string description = null) : base(name, description)
	{
	}
}