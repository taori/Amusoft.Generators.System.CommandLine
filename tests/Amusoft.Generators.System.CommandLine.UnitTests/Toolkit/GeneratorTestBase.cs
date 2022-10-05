using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Amusoft.Generators.System.CommandLine.Attributes;
using Shouldly;
using Xunit.Abstractions;
using System;
using System.CommandLine;
using NLog.Fluent;
using System.Collections.Generic;

namespace Amusoft.Generators.System.CommandLine.UnitTests.Toolkit;

public class GeneratorTestBase : TestBase
{
	public record GeneratorTestResult(Compilation outputCompilation, GeneratorDriverRunResult runResult, GeneratorRunResult generatorResult);

	public GeneratorTestBase(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
	{
	}
	
	protected GeneratorTestResult GetSourceGeneratorResults<TGenerator>(string inputSource, bool assertions = false) where TGenerator : ISourceGenerator, new()
	{
		var codeBases = typeof(GenerateCommandHandlerAttribute).Assembly.GetReferencedAssemblies().Select(d => d.CodeBase)
			.ToArray();
		// Create the 'input' compilation that the generator will act on

		Compilation inputCompilation = CreateCompilation(inputSource); 
		foreach (var d in inputCompilation.GetDiagnostics())
		{
			Log.Info(CSharpDiagnosticFormatter.Instance.Format(d));
		}

		var generator = new TGenerator();

		// Create the driver that will control the generation, passing in our generator
		GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

		// Run the generation pass
		// (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
		driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

		if (assertions)
		{
			diagnostics.IsEmpty.ShouldBeTrue();
			outputCompilation.SyntaxTrees.Count().ShouldBeGreaterThan(0);
			outputCompilation.GetDiagnostics().IsEmpty.ShouldBeTrue();
		}

		GeneratorDriverRunResult runResult = driver.GetRunResult();

		if (assertions)
		{
			runResult.GeneratedTrees.Length.ShouldBe(1);
			runResult.Diagnostics.IsEmpty.ShouldBeTrue();
		}

		// Or you can access the individual results on a by-generator basis
		var generatorResult = runResult.Results[0];
		// Debug.Assert(generatorResult.Generator == generator);
		if (assertions)
		{
			generatorResult.Diagnostics.IsEmpty.ShouldBeTrue();
			generatorResult.GeneratedSources.Length.ShouldBe(1);
			generatorResult.Exception.ShouldBeNull();
		}

		return new (outputCompilation, runResult, generatorResult);
	}

	static IEnumerable<string> GetReferenceAssemblyPaths(Assembly assembly)
	{
		var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
		var referencedAssemblies = assembly.GetReferencedAssemblies();
		return referencedAssemblies
			.Select(name => loadedAssemblies.SingleOrDefault(a => a.FullName == name.FullName)?.Location)
			.Where(location => location != null);
	}

	static IEnumerable<MetadataReference> GetMetadataReferences()
	{
		var referenceSources = new[] {typeof(GenerateCommandHandlerAttribute).Assembly};
		foreach (var path in referenceSources.SelectMany(GetReferenceAssemblyPaths) )
		{
			// yield return MetadataReference.CreateFromFile(path);
		}
		yield return MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location);
		yield return MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location);
		yield return MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51").Location);
		yield return MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location);
		yield return MetadataReference.CreateFromFile(typeof(Command).GetTypeInfo().Assembly.Location);
		yield return MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location);
		yield return MetadataReference.CreateFromFile(typeof(GenerateCommandHandlerAttribute).GetTypeInfo().Assembly.Location);
	}

	private static Compilation CreateCompilation(string source)
		=> CSharpCompilation.Create("compilation",
			new[] { CSharpSyntaxTree.ParseText(source) },
			GetMetadataReferences().ToArray(),
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
}