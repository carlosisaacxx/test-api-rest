using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Patients.Application.Abstractions;
using Patients.Infrastructure.Data;
using Patients.Infrastructure.Repositories;

namespace Patients.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            var cs = config.GetConnectionString("Sqlite") ?? "Data Source=patients.db";

            services.AddDbContext<PatientsDbContext>(opt => opt.UseSqlite(cs));
            services.AddScoped<IPatientRepository, EfPatientRepository>();

            return services;
        }
    }
}
