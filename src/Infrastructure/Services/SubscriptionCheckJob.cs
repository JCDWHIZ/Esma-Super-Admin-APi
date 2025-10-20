using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;
public class SubscriptionCheckJob
{
    private readonly ApplicationDbContext _dbContext;
    //private readonly IEmailService _emailService;
    private readonly ILogger<SubscriptionCheckJob> _logger;

    public SubscriptionCheckJob(
        ApplicationDbContext dbContext,
        //IEmailService emailService,
        ILogger<SubscriptionCheckJob> logger)
    {
        _dbContext = dbContext;
        //_emailService = emailService;
        _logger = logger;
    }

    public async Task CheckSubscriptionsAsync()
    {
        _logger.LogInformation("Starting subscription check job at {Time}", DateTime.UtcNow);

        try
        {
            // Query schools with their subscriptions
            List<Domain.Schools.Schools> schools = await _dbContext.Schools
                .Include(s => s.Subscriptions)
                .Where(s => s.Subscribed)
                .ToListAsync();

            DateTime now = DateTime.UtcNow;
            var notificationIntervals = new List<int> { 50, 30, 15, 5, 4, 3, 2, 1 };

            foreach (Domain.Schools.Schools? school in schools)
            {
                if (school.Subscriptions == null || !school.Subscriptions.EndDate.HasValue)
                {
                    _logger.LogWarning("School {SchoolName} (ID: {SchoolId}) has no valid subscription",
                        school.SchoolName, school.Id);
                    continue;
                }

                DateTime endDate = school.Subscriptions.EndDate.Value;
                double daysRemaining = (endDate - now).TotalDays;

                // Check if subscription has expired
                if (daysRemaining <= 0)
                {
                    //school.Subscribed = false;

                    _logger.LogInformation("Subscription for school {SchoolName} (ID: {SchoolId}) has expired",
                        school.SchoolName, school.Id);
                    // send email message
                }
                // Check for notification intervals
                else if (notificationIntervals.Any(interval => Math.Abs(daysRemaining - interval) < 0.5))
                {
                    int days = (int)Math.Round(daysRemaining);
                    _logger.LogInformation("Sending reminder for school {SchoolName} (ID: {SchoolId}): {Days} days remaining",
                        school.SchoolName, school.Id, days);
                    // send email message
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Completed subscription check job at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during subscription check job");
        }
    }
}
