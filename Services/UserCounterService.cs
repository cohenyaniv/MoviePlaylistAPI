using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;

/// <summary>
/// This class run as a background service and hold for each user his running duration
/// </summary>
public class UserCounterService : BackgroundService
{
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _userCounters = new ConcurrentDictionary<string, CancellationTokenSource>();
    private readonly ConcurrentDictionary<string, int> _userCounterValues = new ConcurrentDictionary<string, int>();

    /// <summary>
    /// Starting point
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken); // Adjust the delay as necessary
        }
    }

    /// <summary>
    /// Start the counter for a specific user
    /// </summary>
    /// <param name="userId"></param>
    public void StartCounter(string userId)
    {
        var cts = new CancellationTokenSource();
        _userCounters[userId] = cts;
        _userCounterValues[userId] = 0; // Initialize the counter for the user

        Task.Run(async () =>
        {
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    // Increment the counter or perform any operation
                    _userCounterValues[userId]++; // Update the counter value
                    Console.WriteLine($"Counting for user: {userId}, Current Count: {_userCounterValues[userId]}");
                    await Task.Delay(1000); // Counting interval
                }
            }
            catch (TaskCanceledException)
            {
                // Update the DB when canceled
            }
        });
    }

    public void StopCounter(string userId)
    {
        if (_userCounters.TryRemove(userId, out var cts))
        {
            cts.Cancel(); // Stop the counting operation
        }
    }

    /// <summary>
    /// Get the user counter from the dictionary
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public int GetCounterValue(string userId)
    {
        // Return the current counter value for the user, defaulting to 0 if not found
        _userCounterValues.TryGetValue(userId, out int value);
        return value;
    }
}
