using Patients.Domain.Entities;

namespace Patients.Application.Abstractions
{
    public interface IPatientRepository
    {
        Task<Patient> AddAsync(Patient patient, CancellationToken ct = default);
        Task<IReadOnlyList<Patient>> ListAsync(CancellationToken ct = default);
    }
}
