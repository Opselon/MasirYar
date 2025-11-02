// مسیر: src/CoachingService/Infrastructure/Persistence/CoachingDbContext.cs

using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class CoachingDbContext : DbContext
{
    public CoachingDbContext(DbContextOptions<CoachingDbContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // پیکربندی Course
        modelBuilder.Entity<Course>()
            .HasIndex(c => c.AuthorId);
    }
}

