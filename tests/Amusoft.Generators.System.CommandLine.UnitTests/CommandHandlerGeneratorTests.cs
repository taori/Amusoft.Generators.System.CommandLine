using Amusoft.XUnit.NLog.Extensions;
using Amusoft.Generators.System.CommandLine.UnitTests.Toolkit;
using Amusoft.Toolkit.Generators.System.CommandLine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Amusoft.Generators.System.CommandLine.UnitTests;

public class CommandHandlerGeneratorTests : TestBase
{
	public CommandHandlerGeneratorTests(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
	{
	}

	[Theory]
	[InlineData("TestResources|ClassWithAttribute.cs", true)]
	[InlineData("TestResources|ClassWithoutAttribute.cs", false)]
	public void CheckHandlerPresent(string testFile, bool expected)
	{
		var testContent = GetProjectFileContent(testFile);
		var found = CommandHandlerGenerator.ContainsCandidate(CSharpSyntaxTree.ParseText(testContent));
		found.ShouldBe(expected);
	}
}