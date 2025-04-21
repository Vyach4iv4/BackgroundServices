using Microsoft.EntityFrameworkCore;
using BackgroundStudentApp.Models;
using System.Collections.Generic;

namespace BackgroundStudentApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students => Set<Student>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }
}
