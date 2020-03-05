using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using DashBoard.Data.Entities;
using DashBoard.Data.Data;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;


namespace DashBoard.Business.Services
{
    public class BackgroundMailService: IHostedService, IDisposable 
    {
        private Timer _timer;
        private readonly IServiceScopeFactory scopeFactory;

        public BackgroundMailService(IServiceScopeFactory scopeFactory)    
        {
            this.scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(10));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                if (_dbContext != null)
                {
                    try
                    {
                        var _requestService = scope.ServiceProvider.GetRequiredService<IRequestService>();
                        var domainModels = _dbContext.Domains.Where(x => x.Deleted == false && x.Active == true).ToList();

                        foreach (DomainModel model in domainModels)
                        {
                            await _requestService.GetService(model.Id, null, "-1", "background", model);
                        }
                    }
                    finally
                    {
                        _dbContext.Dispose();
                    }
                }
            }                
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

