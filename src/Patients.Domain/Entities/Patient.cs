namespace Patients.Domain.Entities
{
    public sealed class Patient
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; } = string.Empty;
        public DateOnly DateOfBirth { get; private set; }
        public List<string> Symptoms { get; private set; } = new();
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

        private Patient() { }

        public static Patient Create(string name, DateOnly dateOfBirth, IEnumerable<string>? symptoms)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            if (dateOfBirth == default)
                throw new ArgumentException("Date of birth is required.", nameof(dateOfBirth));

            var symptomList = (symptoms ?? Enumerable.Empty<string>())
                .Select(s => s?.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (symptomList.Count == 0)
                throw new ArgumentException("At least one symptom is required.", nameof(symptoms));

            return new Patient
            {
                Name = name.Trim(),
                DateOfBirth = dateOfBirth,
                Symptoms = symptomList,
                CreatedAtUtc = DateTime.UtcNow
            };
        }
    }
}
