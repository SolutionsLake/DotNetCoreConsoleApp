using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleDemo.Core;
using Microsoft.Extensions.Hosting;

namespace ConsoleDemo.App
{
    internal class WorkerService : BackgroundService
    {
        private readonly IProcessor _processor;

        public WorkerService(IProcessor processor)
        {
            _processor = processor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Do your preparation (e.g. Start code) here
            //while (!stoppingToken.IsCancellationRequested)  // this will run forever
            //{
            //    await _processor.RunAsync();
            //    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            //}
            await _processor.RunAsync();
            //Do your cleanup (e.g. Stop code) here
        }
    }
}
