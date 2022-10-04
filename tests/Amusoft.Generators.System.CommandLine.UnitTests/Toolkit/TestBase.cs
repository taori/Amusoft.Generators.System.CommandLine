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
	}
}