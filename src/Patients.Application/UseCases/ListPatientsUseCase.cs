using Patients.Application.Abstractions;
using Patients.Application.DTOs.Patients;

namespace Patients.Application.UseCases;

public interface IListPatientsUseCase
{
    Task<IReadOnlyList<PatientDto>> HandleAsync(
        Func<DateTime, DateTimeOffset> toUserTz,
        Func<DateOnly> todayProvider);
}

public sealed class ListPatientsUseCase(IPatientRepository repo) : IListPatientsUseCase
{
    public async Task<IReadOnlyList<PatientDto>> HandleAsync(
        Func<DateTime, DateTimeOffset> toUserTz,
        Func<DateOnly> todayProvider)
    {
        var today = todayProvider();
        var list = await repo.ListAsync();

        return list.Select(p => new PatientDto
        {
            Id = p.Id,
            Name = p.Name,
            Age = CalculateAge(p.DateOfBirth, today),
            Symptoms = p.Symptoms,
            CreatedAt = toUserTz(p.CreatedAtUtc)
        }).ToList();
    }

    private static int CalculateAge(DateOnly dob, DateOnly today)
    {
        var age = today.Year - dob.Year;
        if (today.Month < dob.Month || (today.Month == dob.Month && today.Day < dob.Day))
            age--;
        return age;
    }
}