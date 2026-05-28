namespace SanityCheck.Endpoints;

public static class BlueprintEndpoints
{
    public static void MapCommitmentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/weekly-commitment", (IExecutionRepository repo) => 
            Results.Ok(repo.GetWeeklyCommitments()));
            
        // Endpoint commitment lainnya tinggal ditaruh di sini...
    }
}