using Patients.Application.Abstractions;
using Patients.Application.DTOs.Patients;
using Patients.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Patients.Application.UseCases;

public interface IRegisterPatientUseCase
{
    Task<PatientDto> HandleAsync(PatientCreateDto input, Func<DateTime, DateTimeOffset> toUserTz);
}

public sealed class RegisterPatientUseCase(IPatientRepository repo) : IRegisterPatientUseCase
{
    public async Task<PatientDto> HandleAsync(PatientCreateDto input, Func<DateTime, DateTimeOffset> toUserTz)
    {
        var dob = input.DateOfBirth ?? throw new ValidationException("Date of birth is required.");

        var entity = Patient.Create(input.Name, dob, input.Symptoms);
        var saved = await repo.AddAsync(entity);

        return new PatientDto
        {
            Id = saved.Id,
            Name = saved.Name,
            DateOfBirth = saved.DateOfBirth,
            Symptoms = saved.Symptoms,
            CreatedAt = toUserTz(saved.CreatedAtUtc)
        };
    }
}
