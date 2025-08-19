using Patients.Application.Abstractions;
using Patients.Application.DTOs.Patients;

namespace Patients.Application.UseCases;

public interface IListPatientsUseCase
{
    Task<IReadOnlyList<PatientDto>> HandleAsync(Func<DateTime, DateTimeOffset> toUserTz);
}

public sealed class ListPatientsUseCase(IPatientRepository repo) : IListPatientsUseCase
{
    public async Task<IReadOnlyList<PatientDto>> HandleAsync(Func<DateTime, DateTimeOffset> toUserTz)
    {
        var list = await repo.ListAsync();
        return list.Select(p => new PatientDto
        {
            Id = p.Id,
            Name = p.Name,
            DateOfBirth = p.DateOfBirth,
            Symptoms = p.Symptoms,
            CreatedAt = toUserTz(p.CreatedAtUtc)
        }).ToList();
    }
}
