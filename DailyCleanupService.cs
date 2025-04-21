using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using BackgroundStudentApp.Models;
using BackgroundStudentApp.Data;

namespace BackgroundStudentApp.Services;

public class DailyCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public DailyCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        if (DateTime.Now.DayOfWeek != DayOfWeek.Monday)
        {
            Console.WriteLine($"[Пропуск] Сегодня не понедельник: {DateTime.Now.DayOfWeek}, сервис не запускается");
            return;
        }

        Console.WriteLine($"[Запуск] Сегодня понедельник. Сервис запускается...");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var targetTime = DateTime.Today.AddHours(12).AddMinutes(45);

            if (now > targetTime)
                targetTime = targetTime.AddDays(1);

            var delay = targetTime - now;
            await Task.Delay(delay, stoppingToken);

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var usersToRemove = context.Students
                    .Where(s => s.Name.StartsWith("А"))
                    .ToList();

                if (usersToRemove.Count > 0)
                {
                    context.Students.RemoveRange(usersToRemove);
                    await context.SaveChangesAsync();

                    Console.WriteLine($"[Удаление] В {DateTime.Now} удалено {usersToRemove.Count} студент(ов), имя которых начинается на 'А'");
                }
                else
                {
                    Console.WriteLine($"[Удаление] В {DateTime.Now} студентов с именем на 'А' не найдено");
                }
            }
        }
    }
}
