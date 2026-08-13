namespace BuildingBlocks.Resilience;

public static class ResilienceExecutor
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    public static async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken,
        int maxAttempts = 3,
        TimeSpan? timeout = null,
        Func<Exception, bool>? shouldRetry = null,
        TimeSpan? firstDelay = null)
    {
        ArgumentNullException.ThrowIfNull(operation);

        var operationTimeout = timeout ?? DefaultTimeout;
        var delay = firstDelay ?? TimeSpan.FromMilliseconds(250);
        var attempt = 0;
        var lastException = default(Exception);

        while (attempt < maxAttempts && !cancellationToken.IsCancellationRequested)
        {
            attempt++;

            using var attemptCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            attemptCts.CancelAfter(operationTimeout);

            try
            {
                return await operation(attemptCts.Token);
            }
            catch (Exception exception) when (ShouldRetry(exception, shouldRetry, cancellationToken, attempt, maxAttempts, attemptCts))
            {
                lastException = exception;

                if (attempt >= maxAttempts)
                {
                    break;
                }

                await Task.Delay(delay, cancellationToken);
                delay = TimeSpan.FromMilliseconds(Math.Min(delay.TotalMilliseconds * 2, 2000));
            }
        }

        throw lastException ?? new TimeoutException("The operation timed out.");
    }

    private static bool ShouldRetry(
        Exception exception,
        Func<Exception, bool>? shouldRetry,
        CancellationToken cancellationToken,
        int attempt,
        int maxAttempts,
        CancellationTokenSource attemptCts)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        if (attempt >= maxAttempts)
        {
            return false;
        }

        if (exception is OperationCanceledException)
        {
            return attemptCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested;
        }

        return shouldRetry?.Invoke(exception) ?? exception is HttpRequestException or TimeoutException;
    }
}
