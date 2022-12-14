using System;
using System.IO;
using System.Linq;
using Amusoft.XUnit.NLog.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Amusoft.Generators.System.CommandLine.UnitTests.Toolkit
{
	public class TestBase : LoggedTestBase, IClassFixture<GlobalSetupFixture>
	{
		private readonly GlobalSetupFixture _data;

		public TestBase(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper)
		{
			_data = data;
		}

		protected string GetProjectFileContent(string pipedPath, int pathSkip = 3)
		{
			var dir = Path.GetDirectoryName(typeof(TestBase).Assembly.Location);
			var fileSections = pipedPath.Split('|', StringSplitOptions.RemoveEmptyEntries);
			var filePath = Path.Combine(dir, Path.Combine(Enumerable.Repeat("..", pathSkip).ToArray()), Path.Combine(fileSections.ToArray()));
			filePath = new Uri(filePath, UriKind.Absolute).AbsolutePath;

			if (!File.Exists(filePath))
				throw new FileNotFoundException($"File not found ({filePath})", filePath);

			return File.ReadAllText(filePath);
		}
	}
}