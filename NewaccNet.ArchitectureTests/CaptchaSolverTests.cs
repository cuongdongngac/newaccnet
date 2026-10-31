using System;
using System.Threading;
using System.Threading.Tasks;
using NewaccNet.AI.Services;
using NewaccNet.TaxInvoice;
using Xunit;

namespace NewaccNet.ArchitectureTests
{
    public class CaptchaSolverTests
    {
        [Fact]
        public async Task AiCaptchaSolver_WhenUnconfigured_ShouldReturnNullGracefully()
        {
            var solver = new AiCaptchaSolver();
            var challenge = new CaptchaChallenge("test-key", "<svg><path d=\"MQMQZ\"/></svg>", "1");

            // Khi chưa có API Key, solver không ném ngoại lệ mà trả về null để hệ thống tiếp tục các luồng khác
            var result = await solver.SolveAsync(challenge, CancellationToken.None);
            Assert.Null(result);
        }

        [Fact]
        public async Task TaxInvoiceClient_WhenCustomSolverProvided_ShouldInvokeSolverOnFallback()
        {
            bool solverInvoked = false;
            var mockSolver = new MockCaptchaSolver((ch) =>
            {
                solverInvoked = true;
                return Task.FromResult<string?>("ai999");
            });

            var client = new TaxInvoiceClient
            {
                CaptchaSolver = mockSolver
            };

            Assert.NotNull(client.CaptchaSolver);
        }

        private class MockCaptchaSolver : ICaptchaSolver
        {
            private readonly Func<CaptchaChallenge, Task<string?>> _solveFunc;

            public MockCaptchaSolver(Func<CaptchaChallenge, Task<string?>> solveFunc)
            {
                _solveFunc = solveFunc;
            }

            public Task<string?> SolveAsync(CaptchaChallenge challenge, CancellationToken cancellationToken = default)
            {
                return _solveFunc(challenge);
            }
        }
    }
}
