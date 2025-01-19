//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//namespace SpaceTrack.Services;

//public class TLEPayloadsScheduler : BackgroundService
//{
//    private readonly IServiceProvider _serviceProvider;

//    public TLEPayloadsScheduler(IServiceProvider serviceProvider)
//    {
//        _serviceProvider = serviceProvider;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        while (!stoppingToken.IsCancellationRequested)
//        {
//            var now = DateTime.UtcNow; // Use UTC for consistency
//            var targetTime = now.AddMinutes(3); // Run 5 minutes from now

//            Console.WriteLine($"Scheduler will run at: {targetTime}");

//            var delay = targetTime - now;
//            await Task.Delay(delay, stoppingToken); // Wait until the scheduled time

//            try
//            {
//                using var scope = _serviceProvider.CreateScope();
//                var tleService = scope.ServiceProvider.GetRequiredService<TLEService>();

//                Console.WriteLine($"Running TLE update at {DateTime.UtcNow}");
//                await tleService.SaveOrUpdateAllPayloadsTLEsAsync(); // Call the method
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error in scheduled task: {ex.Message}");
//            }
//        }
//    }
//}


using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SpaceTrack.Services
{
public class TLEPayloadsScheduler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public TLEPayloadsScheduler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow; // Use UTC for consistency
            var targetTime = new DateTime(now.Year, now.Month, now.Day, 12, 25, 0, DateTimeKind.Utc); // 7:30 AM UTC

            if (now > targetTime)
            {
                targetTime = targetTime.AddDays(1); // Schedule for the next day if the time has passed
            }

            var delay = targetTime - now;
            await Task.Delay(delay, stoppingToken); // Wait until the scheduled time

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var tleService = scope.ServiceProvider.GetRequiredService<TLEService>();

                await tleService.SaveOrUpdateAllPayloadsTLEsAsync(); // Call the method
            }
            catch (Exception ex)
            {
                // Log errors (optional: integrate a logging framework)
                Console.WriteLine($"Error in scheduled task: {ex.Message}");
            }
        }
    }
}
}
