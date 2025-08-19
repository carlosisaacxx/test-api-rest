using FluentAssertions;
using NSubstitute;
using Patients.Application.Abstractions;
using Patients.Application.DTOs.Patients;
using Patients.Application.UseCases;
using Patients.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Patients.Tests.Unit
{
    public class RegisterUseCaseTests
    {
        private sealed class FakeRepo : IPatientRepository
        {
            public Task<Patient> AddAsync(Patient patient, CancellationToken ct = default) => Task.FromResult(patient);
            public Task<IReadOnlyList<Patient>> ListAsync(CancellationToken ct = default) => Task.FromResult((IReadOnlyList<Patient>)new List<Patient>());
        }


        [Fact]
        public async Task HandleAsync_Should_Return_Age_Calculated_From_User_TZ_Today()
        {
            var input = new PatientCreateDto
            {
                Name = "Ana",
                DateOfBirth = new DateOnly(2000, 8, 20),
                Symptoms = new() { "tos" }
            };


            // Fijamos "hoy" = 2025-08-19 en la TZ del usuario (para que la prueba sea determinista)
            var fixedToday = new DateTime(2025, 8, 19, 10, 0, 0, DateTimeKind.Unspecified);
            DateTimeOffset ToUserTz(DateTime _) => new DateTimeOffset(fixedToday, TimeSpan.Zero);


            var uc = new RegisterPatientUseCase(new FakeRepo());
            var dto = await uc.HandleAsync(input, ToUserTz);


            dto.Name.Should().Be("Ana");
            dto.Age.Should().Be(24); // porque aún no llega su cumple 2000-08-20
            dto.Symptoms.Should().Contain("tos");
        }


        [Fact]
        public async Task HandleAsync_Should_Throw_When_Dob_Missing()
        {
            var input = new PatientCreateDto { Name = "Ana", Symptoms = new() { "tos" } };
            var uc = new RegisterPatientUseCase(new FakeRepo());


            Func<Task> act = () => uc.HandleAsync(input, _ => DateTimeOffset.UtcNow);
            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
