using System.IO;
using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyXunit;

namespace Amusoft.Generators.System.CommandLine.UnitTests.Toolkit;

internal class ModuleInitializer
{
	[ModuleInitializer]
	public static void Initialize()
	{
		VerifierSettings.DerivePathInfo(
			(sourceFile, projectDirectory, type, method) => new(
				directory: Path.Combine(projectDirectory, "Snapshots"),
				typeName: type.Name,
				methodName: method.Name));

		VerifySourceGenerators.Enable();
	}
}