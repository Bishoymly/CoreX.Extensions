using CoreX.Extensions.Metrics;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreX.Dashboard
{
    public class DevDashboardBackground : BackgroundService
    {
        private IMetricsService _metrics;
        private IHubContext<DevDashboardHub> _hub;

        public DevDashboardBackground(IMetricsService metrics, IHubContext<DevDashboardHub> hub)
        {
            _metrics = metrics;
            _metrics.RequestStarted += Metrics_RequestStarted;
            _metrics.RequestEnded += Metrics_RequestEnded;

            _hub = hub;
        }

        private async void Metrics_RequestEnded(object sender, Extensions.Metrics.Events.RequestEventArgs e)
        {
            await _hub.Clients.All.SendAsync("RequestEnded", e.Request);
        }

        private async void Metrics_RequestStarted(object sender, Extensions.Metrics.Events.RequestEventArgs e)
        {
            await _hub.Clients.All.SendAsync("RequestStarted", e.Request);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
