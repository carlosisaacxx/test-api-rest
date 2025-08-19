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


        // "Hoy" según la TZ del usuario (se obtiene vía toUserTz)
        var today = DateOnly.FromDateTime(toUserTz(DateTime.UtcNow).DateTime);


        return new PatientDto
        {
            Id = saved.Id,
            Name = saved.Name,
            Age = CalculateAge(saved.DateOfBirth, today),
            Symptoms = saved.Symptoms,
            CreatedAt = toUserTz(saved.CreatedAtUtc)
        };
    }


    private static int CalculateAge(DateOnly dob, DateOnly today)
    {
        var age = today.Year - dob.Year;
        if (today.Month < dob.Month || (today.Month == dob.Month && today.Day < dob.Day))
            age--;
        return age;
    }
}