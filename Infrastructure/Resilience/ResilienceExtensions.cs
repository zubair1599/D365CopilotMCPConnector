using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace McpD365Server.Infrastructure.Resilience;

/// <summary>
/// Extension methods for configuring Polly v8 resilience pipelines.
/// </summary>
public static class ResilienceExtensions
{
    /// <summary>
    /// Adds a standard resilience pipeline to the named/typed HttpClient:
    /// - Retry: 3 attempts with exponential backoff (1s, 2s, 4s)
    /// - Circuit Breaker: opens after 5 failures, 30s break duration
    /// - Timeout: 30s per-request timeout
    /// </summary>
    public static IHttpClientBuilder AddD365ResiliencePipeline(this IHttpClientBuilder builder)
    {
        builder.AddResilienceHandler("D365Pipeline", pipelineBuilder =>
        {
            // Retry with exponential backoff
            pipelineBuilder.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = static args => ValueTask.FromResult(
                    args.Outcome.Result is { IsSuccessStatusCode: false } ||
                    args.Outcome.Exception is not null)
            });

            // Circuit breaker
            pipelineBuilder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                SamplingDuration = TimeSpan.FromSeconds(60),
                FailureRatio = 0.5,
                MinimumThroughput = 5,
                BreakDuration = TimeSpan.FromSeconds(30),
                ShouldHandle = static args => ValueTask.FromResult(
                    args.Outcome.Result is { IsSuccessStatusCode: false } ||
                    args.Outcome.Exception is not null)
            });

            // Per-request timeout
            pipelineBuilder.AddTimeout(TimeSpan.FromSeconds(30));
        });

        return builder;
    }
}
