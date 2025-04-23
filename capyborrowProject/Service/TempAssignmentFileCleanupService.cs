using capyborrowProject.Data;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Service
{
    public class TempAssignmentFileCleanupService(IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _expirationTime = TimeSpan.FromHours(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var blobService = scope.ServiceProvider.GetRequiredService<BlobStorageService>();

                var expirationThreshold = DateTime.UtcNow - _expirationTime;
                var staleFiles = await context.TempAssignmentFiles
                    .Where(f => f.CreatedAt < expirationThreshold)
                    .ToListAsync(stoppingToken);

                foreach (var file in staleFiles)
                {
                    try
                    {
                        await blobService.DeleteFileAsync(file.FileUrl, "assignment");
                        context.TempAssignmentFiles.Remove(file);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                await context.SaveChangesAsync(stoppingToken);

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }
    }
}