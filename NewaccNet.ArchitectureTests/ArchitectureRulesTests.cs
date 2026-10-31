using System;
using NetArchTest.Rules;
using Xunit;

namespace NewaccNet.ArchitectureTests
{
    public class ArchitectureRulesTests
    {
        [Fact]
        public void Agent_ShouldNotReference_Core_Or_DataAccess()
        {
            // Rule 3.2: NewaccNet.Agent CẤM tham chiếu Core, Tools, Office, Wpf, LLBLGen, dataaccess
            var result = Types.InAssembly(typeof(NewaccNet.Agent.AgentMarker).Assembly)
                .ShouldNot()
                .HaveDependencyOnAny(
                    "NewaccNet.Core",
                    "NewaccNet.Tools",
                    "NewaccNet.Office",
                    "NewaccNet.Wpf",
                    "DataAccess"
                )
                .GetResult();

            Assert.True(result.IsSuccessful, "NewaccNet.Agent violated architecture dependency boundaries.");
        }

        [Fact]
        public void Contracts_ShouldNotReference_AnyOtherProject()
        {
            // Rule 3.2: NewaccNet.Contracts chỉ dùng BCL & System.Text.Json, CẤM tham chiếu project khác
            var result = Types.InAssembly(typeof(NewaccNet.Contracts.VoucherDraft).Assembly)
                .ShouldNot()
                .HaveDependencyOnAny(
                    "NewaccNet.Core",
                    "NewaccNet.Agent",
                    "NewaccNet.Tools",
                    "NewaccNet.Office",
                    "NewaccNet.Wpf",
                    "DataAccess"
                )
                .GetResult();

            Assert.True(result.IsSuccessful, "NewaccNet.Contracts violated dependency isolation.");
        }
    }
}
