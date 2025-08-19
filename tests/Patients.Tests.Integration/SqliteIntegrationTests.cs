using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Patients.Domain.Entities;
using Patients.Infrastructure.Data;
using Patients.Infrastructure.Repositories;

namespace Patients.Tests.Integration;

[Trait("Category", "Integration")]
public class SqliteIntegrationTests
{
    [Fact]
    public async Task EfRepository_Should_Persist_And_List()
    {
        var csb = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
        using var conn = new SqliteConnection(csb.ToString());
        await conn.OpenAsync();

        var opts = new DbContextOptionsBuilder<PatientsDbContext>()
            .UseSqlite(conn)
            .Options;

        using (var ctx = new PatientsDbContext(opts))
            await ctx.Database.EnsureCreatedAsync();

        using var db = new PatientsDbContext(opts);
        var repo = new EfPatientRepository(db);

        var p = Patient.Create("Ana", new DateOnly(1991, 4, 23), new[] { "tos" });
        await repo.AddAsync(p);

        var list = await repo.ListAsync();
        list.Should().HaveCount(1);
        list[0].Name.Should().Be("Ana");
    }
}