using BackgroundStudentApp.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using BackgroundStudentApp.Data;

namespace BackgroundStudentApp.Services
{
    public class StudentBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly string[] _names = new[] { "Антон", "Руслан", "Денис" };
        private int _index = 0;

        public StudentBackgroundService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var student = new Student
                    {
                        Name = _names[_index % _names.Length],
                        CreatedDate = DateTime.Now
                    };

                    db.Students.Add(student);
                    await db.SaveChangesAsync();

                    Console.WriteLine($"Добавлен студент: {student.Name} в {student.CreatedDate}");

                    _index++;
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
