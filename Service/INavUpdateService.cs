using System.Threading;
using System.Threading.Tasks;

namespace Investment1.Service
{
    // Interface for the service responsible for updating NAV (Net Asset Value)
    public interface INavUpdateService
    {
        // Asynchronous method to update NAVs
        // The method accepts a CancellationToken to support task cancellation
        Task UpdateNavsAsync(CancellationToken stoppingToken);
    }
}
