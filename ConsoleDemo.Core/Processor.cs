using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ConsoleDemo.Core
{
    public interface IProcessor
    {
        Task RunAsync();
    }
    public class Processor : IProcessor
    {
        private readonly ILogger<Processor> _logger;
        private readonly string[] _args;

        public Processor(ILogger<Processor> logger, string[] args)
        {
            _logger = logger;
            _args = args;
        }

        public async Task RunAsync()
        {
            foreach (var s in _args)
            {
                _logger.LogInformation(s);
            }
            await Task.CompletedTask;
        }
    }
}
