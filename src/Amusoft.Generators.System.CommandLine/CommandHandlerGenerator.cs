﻿using System;
using System.Collections.Generic;
using System.Linq;
using Amusoft.Generators.System.CommandLine.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NotImplementedException = System.NotImplementedException;

namespace Amusoft.Generators.System.CommandLine;

[Generator]
internal class CommandHandlerGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context)
	{
	}

	public void Execute(GeneratorExecutionContext context)
	{
		foreach (var syntaxTree in context.Compilation.SyntaxTrees)
		{
			var root = syntaxTree.GetRoot(context.CancellationToken);
			foreach (var outerClass in ClassesWhichImplementCommand(root))
			{
				var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
				if (TryGetCandidate(semanticModel, outerClass, out var innerClass))
				{
					AppendGeneratorCode(context, semanticModel, outerClass, innerClass);
				}
			}
		}

		// // Find the main method
		// var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);
		// 		// Build up the source code
		// 		string source = $@"// <auto-generated/>
		// using System;
		//
		// namespace {mainMethod.ContainingNamespace.ToDisplayString()}
		// {{
		//     public partial class {mainMethod.ContainingType.Name}
		//     {{
		//         static partial void HelloFrom(string name) =>
		//             Console.WriteLine($""Generator says this: Hi from '{{name}}'"");
		//     }}
		// }}
		// ";
		// 		var typeName = mainMethod.ContainingType.Name;
		//
		// 		// Add the source code to the compilation
		// 		context.AddSource($"{typeName}.g.cs", source);
	}

	private static IEnumerable<ClassDeclarationSyntax> ClassesWhichImplementCommand(SyntaxNode root)
	{
		return root.DescendantNodes(_ => true).OfType<ClassDeclarationSyntax>();
	}

	private void AppendGeneratorCode(GeneratorExecutionContext context, SemanticModel semanticModel, ClassDeclarationSyntax outerClass, ClassDeclarationSyntax innerClass)
	{
		if (outerClass == null) throw new ArgumentNullException(nameof(outerClass));
		if (innerClass == null) throw new ArgumentNullException(nameof(innerClass));

		var outerClassSymbol = semanticModel.GetDeclaredSymbol(outerClass);
		var targetNamespace = outerClassSymbol.ContainingNamespace.ToDisplayString();
		var source = 
		$$"""
		// <auto-generated/>
		using System.CommandLine;
		using System.CommandLine.Invocation;
		using System.Threading.Tasks;
		using Amusoft.Generators.System.CommandLine.Attributes;
		using Microsoft.Extensions.DependencyInjection;
		using Microsoft.Extensions.Hosting;

		namespace {{targetNamespace}};	
		
		{{outerClass.Modifiers}} {{outerClass.Identifier}}
		{
			private {{innerClass.Identifier.Text}} _handler;

			private void BindHandler()
			{
				if (_handler is null)
					return;

				this.SetHandler(async (context) =>
				{
					var host = context.BindingContext.GetRequiredService<IHost>();
					_handler = new {{innerClass.Identifier.Text}}(host.Services.GetRequiredService<ILogger>());

					var p1 = context.ParseResult.GetValueForOption(OptionOne);
					var p2 = context.ParseResult.GetValueForOption(OptionTwo);
					var p3 = context.ParseResult.GetValueForArgument(ArgumentOne);

					await _handler.ExecuteAsync(context, p1, p2, p3);
				});
			}

			{{innerClass.Modifiers}} {{innerClass.Identifier.Text}} : InvokerBase
			{
			}
		
			public abstract class InvokerBase
			{
				public virtual Task ExecuteAsync(InvocationContext context, string optionOne, int optionTwo, int argumentOne) => Task.CompletedTask;				
			}
		}
		""";

		context.AddSource($"{outerClass.Identifier.Text}.g.cs", source);
	}

	private static bool TryGetCandidate(SemanticModel semanticModel, ClassDeclarationSyntax parentClass, out ClassDeclarationSyntax implementorChildClass)
	{
		var parentSymbol = semanticModel.GetDeclaredSymbol(parentClass);
		var handlerAttributeTypeSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(GenerateCommandHandlerAttribute).FullName);
		// var commandSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(System.CommandLine.Command).FullName)

		implementorChildClass = parentClass.ChildNodes()
			.OfType<ClassDeclarationSyntax>()
			.Where(syntax =>
			{
				var classSymbol = semanticModel.GetDeclaredSymbol(syntax);
				return classSymbol.GetAttributes().Any(attributeData => attributeData.AttributeClass.Equals(handlerAttributeTypeSymbol, SymbolEqualityComparer.Default));
			})
			.FirstOrDefault();

		return implementorChildClass != null;
	}
}