using System.Linq;
using System.Threading.Tasks;
using Amusoft.XUnit.NLog.Extensions;
using Amusoft.Generators.System.CommandLine.UnitTests.Toolkit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Amusoft.Generators.System.CommandLine.UnitTests;

[UsesVerify]
public class CommandHandlerGeneratorTests : GeneratorTestBase
{
	public CommandHandlerGeneratorTests(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
	{
	}

	[Theory]
	[InlineData("TestResources|ClassWithAttribute.cs")]
	[InlineData("TestResources|ClassWithoutAttribute.cs")]
	[InlineData("TestResources|ClassTwoArguments.cs")]
	[InlineData("TestResources|ClassWrappedOption.cs")]
	[InlineData("TestResources|ClassWrappedArgument.cs")]
	[InlineData("TestResources|Sub1Command.cs")]
	public async Task CompareGeneration(string testFile)
	{
		var testContent = GetProjectFileContent(testFile);
		
		var results = GetSourceGeneratorResults<CommandHandlerGenerator>(testContent);

		await Verifier.Verify(results.runResult)
			.UseParameters(testFile);
	}
}