namespace Patients.Application.DTOs.Patients
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public List<string> Symptoms { get; set; } = new();
        public DateTimeOffset CreatedAt { get; set; }
    }
}
