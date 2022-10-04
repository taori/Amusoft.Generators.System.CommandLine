using Amusoft.XUnit.NLog.Extensions;
using Amusoft.Generators.System.CommandLine.UnitTests.Toolkit;
using Xunit;
using Xunit.Abstractions;

namespace Amusoft.Generators.System.CommandLine.UnitTests
{
    public class UnitTest1 : TestBase
    {
        [Fact]
        public void Test1()
        {
        }

        public UnitTest1(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
        {
        }
    }
}
