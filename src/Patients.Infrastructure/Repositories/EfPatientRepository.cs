using Microsoft.EntityFrameworkCore;
using Patients.Application.Abstractions;
using Patients.Domain.Entities;
using Patients.Infrastructure.Data;

namespace Patients.Infrastructure.Repositories;

public sealed class EfPatientRepository(PatientsDbContext db) : IPatientRepository
{
    public async Task<Patient> AddAsync(Patient patient, CancellationToken ct = default)
    {
        db.Patients.Add(patient);
        await db.SaveChangesAsync(ct);
        return patient;
    }

    public async Task<IReadOnlyList<Patient>> ListAsync(CancellationToken ct = default)
        => await db.Patients.AsNoTracking()
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(ct);
}
