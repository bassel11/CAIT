using MeetingApplication.Interfaces.Committee;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace MeetingInfrastructure.Integrations.Committee
{
    public static class CommitteeServiceExtensions
    {
        public static IServiceCollection AddCommitteeServiceClient(
            this IServiceCollection services,
            IConfiguration config)
        {
            var baseUrl = config["Services:CommitteeBaseUrl"]
                          ?? throw new Exception("Committee Service URL not configured");

            services.AddHttpClient<ICommitteeService, CommitteeServiceClient>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(5);
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetTimeoutPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
            => HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retry => TimeSpan.FromMilliseconds(200));

        private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
            => Policy.TimeoutAsync<HttpResponseMessage>(2);

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
            => HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(20));
    }

}
