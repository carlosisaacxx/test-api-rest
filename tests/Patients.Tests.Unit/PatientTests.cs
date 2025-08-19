using FluentAssertions;
using Patients.Domain.Entities;

namespace Patients.Tests.Unit
{
    [Trait("Category", "Unit")]
    public class PatientTests
    {
        [Fact]
        public void Create_Should_Set_Fields_And_Utc()
        {
            var dob = new DateOnly(1990, 4, 23);
            var p = Patient.Create(" Ana ", dob, new[] { "tos", "fiebre" });

            p.Name.Should().Be("Ana");
            p.DateOfBirth.Should().Be(dob);
            p.Symptoms.Should().Contain(new[] { "tos", "fiebre" });
            p.CreatedAtUtc.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public void Create_Should_Throw_When_Name_Empty()
        {
            var act = () => Patient.Create("  ", new DateOnly(1990, 1, 1), null);
            act.Should().Throw<ArgumentException>();
        }
    }
}
