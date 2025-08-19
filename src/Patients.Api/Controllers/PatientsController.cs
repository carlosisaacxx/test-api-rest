using Microsoft.AspNetCore.Mvc;
using Patients.Application.DTOs.Patients;
using Patients.Application.UseCases;

namespace Patients.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public sealed class PatientsController : ControllerBase
    {
        private readonly IRegisterPatientUseCase _register;
        private readonly IListPatientsUseCase _list;

        public PatientsController(IRegisterPatientUseCase register, IListPatientsUseCase list)
        {
            _register = register;
            _list = list;
        }

        [HttpPost]
        public async Task<ActionResult<Envelope<PatientDto>>> Create([FromBody] PatientCreateDto input, CancellationToken ct)
        {
            var dto = await _register.HandleAsync(input, ToUserTz);
            return CreatedAtAction(nameof(Get), new { }, new Envelope<PatientDto>(dto));
        }

        private DateTimeOffset ToUserTz(DateTime utc)
        {
            var tzId = Request.Headers["X-User-Timezone"].FirstOrDefault();
            tzId = string.IsNullOrWhiteSpace(tzId) ? "America/Merida" : tzId.Trim();

            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
                var local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), tz);
                return new DateTimeOffset(local, tz.GetUtcOffset(local));
            }
            catch
            {
                var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["America/Merida"] = "Central Standard Time (Mexico)",
                    ["America/Mexico_City"] = "Central Standard Time (Mexico)",
                    ["UTC"] = "UTC"
                };
                if (map.TryGetValue(tzId!, out var winId))
                {
                    try
                    {
                        var tz = TimeZoneInfo.FindSystemTimeZoneById(winId);
                        var local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), tz);
                        return new DateTimeOffset(local, tz.GetUtcOffset(local));
                    }
                    catch { }
                }
                return new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc));
            }
        }

        [HttpGet(Name = "Get")]
        public async Task<ActionResult<Envelope<IEnumerable<PatientDto>>>> Get(CancellationToken ct)
        {
            var tz = ResolveTz();

            var items = await _list.HandleAsync(
                toUserTz: utc =>
                {
                    var local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), tz);
                    return new DateTimeOffset(local, tz.GetUtcOffset(local));
                },
                todayProvider: () =>
                {
                    // "Hoy" según la zona horaria del usuario (IANA o fallback)
                    var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                    return DateOnly.FromDateTime(localNow);
                }
            );

            return Ok(new Envelope<IEnumerable<PatientDto>>(items, new { version = "v1", count = items.Count }));
        }

        private TimeZoneInfo ResolveTz()
        {
            var header = Request.Headers["X-User-Timezone"].FirstOrDefault();
            var tzId = string.IsNullOrWhiteSpace(header) ? "America/Merida" : header!.Trim();

            try { return TimeZoneInfo.FindSystemTimeZoneById(tzId); }
            catch
            {
                var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["America/Merida"] = "Central Standard Time (Mexico)",
                    ["America/Mexico_City"] = "Central Standard Time (Mexico)",
                    ["UTC"] = "UTC"
                };
                if (map.TryGetValue(tzId, out var winId))
                {
                    try { return TimeZoneInfo.FindSystemTimeZoneById(winId); } catch { }
                }
                return TimeZoneInfo.Utc;
            }
        }
    }

    public sealed class Envelope<T>
    {
        public Envelope(T data, object? meta = null)
        {
            Data = data;
            Meta = meta ?? new { version = "v1" };
        }

        public T Data { get; }
        public object Meta { get; }
    }
}