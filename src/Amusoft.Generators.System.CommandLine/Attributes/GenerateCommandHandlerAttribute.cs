using System;

namespace Amusoft.Generators.System.CommandLine.Attributes;

/// <summary>
/// Marker attribute to cause a command to generate a handler
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class GenerateCommandHandlerAttribute : Attribute
{
	
}