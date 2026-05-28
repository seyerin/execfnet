using SanityCheck.Models;
using SanityCheck.Repositories;


namespace SanityCheck.Endpoints;

public static class CommitmentEndpoints
{
    public static void MapCommitmentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/weekly-commitment", (IExecutionRepository repo) => 
            Results.Ok(repo.GetWeeklyCommitments()));
            
        // 3. Fitur Auto-Pull: Tarik Key Driver dari Blueprint menjadi Komitmen Kerja Mingguan
        app.MapPost("/api/weekly-commitment/auto-pull/{keyDriverId}", (int keyDriverId, IExecutionRepository repo) => {
            var driver = repo.GetProductivityTaskById(keyDriverId);
            if (driver == null) return Results.NotFound("Key Driver tidak ditemukan di master plan!");

            if (repo.IsAlreadyCommitted(keyDriverId))
                return Results.BadRequest("Key Driver ini sudah dimasukkan ke komitmen minggu ini!");

            var newCommitment = new TeamCommitmentTask
            {
                Id = repo.GetWeeklyCommitments().Count() + 1,
                SourcePlanTaskId = driver.Id,
                Title = driver.Title,
                IsTopPriority = driver.IsTopPriority
            };

            repo.AddWeeklyCommitment(newCommitment);
            return Results.Ok(new { Message = "Berhasil berkomitmen ke tim untuk minggu ini!", Data = newCommitment });
        });
    }
}