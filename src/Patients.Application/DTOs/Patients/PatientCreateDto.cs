using System.ComponentModel.DataAnnotations;

namespace Patients.Application.DTOs.Patients
{
    public class PatientCreateDto : IValidatableObject
    {
        [Required, StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateOnly? DateOfBirth { get; set; }

        [MinLength(1, ErrorMessage = "At least one symptom is required.")]
        public List<string> Symptoms { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            var valid = (Symptoms ?? new())
                .Select(s => s?.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Any();

            if (!valid)
                yield return new ValidationResult(
                    "At least one symptom is required.",
                    new[] { nameof(Symptoms) });
        }
    }
}
