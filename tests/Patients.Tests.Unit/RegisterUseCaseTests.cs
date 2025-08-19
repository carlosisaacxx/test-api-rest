using FluentAssertions;
using NSubstitute;
using Patients.Application.Abstractions;
using Patients.Application.DTOs.Patients;
using Patients.Application.UseCases;
using Patients.Domain.Entities;

namespace Patients.Tests.Unit
{
    public class RegisterUseCaseTests
    {
        [Fact]
        public async Task Register_Should_Map_And_Return_ReadDto()
        {
            var repo = Substitute.For<IPatientRepository>();
            repo.AddAsync(Arg.Any<Patient>()).Returns(ci => ci.Arg<Patient>());

            var uc = new RegisterPatientUseCase(repo);
            var dto = await uc.HandleAsync(new PatientCreateDto
            {
                Name = "Ana",
                DateOfBirth = new DateOnly(1991, 4, 23),
                Symptoms = new() { "tos" }
            }, utc => new DateTimeOffset(utc));

            dto.Name.Should().Be("Ana");
            dto.Symptoms.Should().Contain("tos");
        }
    }
}
