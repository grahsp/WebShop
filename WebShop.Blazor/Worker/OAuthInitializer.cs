using WebShop.Blazor.Services;

namespace WebShop.Blazor.Worker
{
    public class OAuthInitializer : IHostedService
    {
        private readonly OAuthService _service;

        public OAuthInitializer(OAuthService service)
        {
            _service = service;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await _service.GetTokenAsync();
                    await Task.Delay(TimeSpan.FromSeconds(_service.ExpiresIn), cancellationToken);
                }
            }
            catch(OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch OAuth token!", ex);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
