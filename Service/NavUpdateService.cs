using Investment1.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

// Service for periodically updating the Net Asset Value (NAV) of mutual funds
public class NavUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NavUpdateService> _logger; // Add logger field

    // Constructor to initialize the service with an IServiceProvider and ILogger
    public NavUpdateService(IServiceProvider serviceProvider, ILogger<NavUpdateService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger; // Initialize the logger
    }

    // Override of the ExecuteAsync method from BackgroundService
    // This method is executed in the background and runs in a loop
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Continuously runs until cancellation is requested
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Update NAV values
                await UpdateNavValuesAsync();

                // Delay for 1 minute before the next update
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating NAV values.");
            }
        }
    }

    // Method to update NAV values for all mutual funds
    private async Task UpdateNavValuesAsync()
    {
        // Create a new scope to get a scoped service provider
        using (var scope = _serviceProvider.CreateScope())
        {
            // Resolve the InvestmentDbContext from the service provider
            var context = scope.ServiceProvider.GetRequiredService<InvestmentDbContext>();

            // Retrieve all mutual funds from the database
            var funds = await context.MutualFunds.ToListAsync();

            // Update the NAV for each mutual fund
            foreach (var fund in funds)
            {
                fund.NAV += 0.10m; // Increment NAV by 0.10
            }

            // Save changes to the database
            await context.SaveChangesAsync();
        }
    }
}
