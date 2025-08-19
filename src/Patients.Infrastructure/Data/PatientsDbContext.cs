using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Patients.Domain.Entities;
using System.Text.Json;

namespace Patients.Infrastructure.Data
{
    public sealed class PatientsDbContext(DbContextOptions<PatientsDbContext> options) : DbContext(options)
    {
        public DbSet<Patient> Patients => Set<Patient>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = false };
            var symptomsConverter = new ValueConverter<List<string>, string>(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new()
            );

            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v)
            );

            modelBuilder.Entity<Patient>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Name).HasMaxLength(200).IsRequired();
                e.Property(p => p.Symptoms).HasConversion(symptomsConverter);
                e.Property(p => p.DateOfBirth).HasConversion(dateOnlyConverter);
                e.Property(p => p.CreatedAtUtc).IsRequired();
            });
        }
    }
}
