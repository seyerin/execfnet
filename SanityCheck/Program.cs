// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using SanityCheck.Models;
using SanityCheck.Repositories;
using SanityCheck.Endpoints;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if(app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ===================================================================
// DATA REPOSITORY IN-MEMORY (Simulasi Database)
// ===================================================================

// 1. Master Data "Istana Lego" (Goal -> Sub-Goal -> Key Driver)
var mainGoal = new MainGoal { Id = 1, Title = "Performance Empowerment" };

var subGoals = new List<SubGoal>
{
    new() { Id = 1, MainGoalId = 1, Title = "Meningkatkan Otonomi & Kecepatan Eksekusi Tim" },
    new() { Id = 2, MainGoalId = 1, Title = "Membangun Sistem Tracking Transparan" }
};

var keyDriversPlan = new List<KeyDriverTask>
{
    new() { Id = 1, SubGoalId = 1, Title = "Rancang Arsitektur Database di .NET", IsTopPriority = true },
    new() { Id = 2, SubGoalId = 1, Title = "Optimasi Query PostgreSQL untuk Dashboard", IsTopPriority = false },
    new() { Id = 3, SubGoalId = 2, Title = "Integrasi Dual-View Calendar", IsTopPriority = true }
};

// 2. Data Operasional Saringan (Weekly Commitment & Daily Execution)
var weeklyTeamCommitments = new List<WeeklyCommitment>();
var dailyProductivityPlans = new List<DailyProductivityTask>();


// ===================================================================
// ENDPOINTS API (Menggunakan Repositori via Dependency Injection)
// ===================================================================

app.MapExecutionEndpoints();
app.MapAnalyticsEndpoints(); // Tinggal nambah gini kalau app makin besar
app.MapUserEndpoints();

// 1. Lihat Struktur Hierarki Rencana Besar
app.MapGet("/api/blueprint/hierarchy", (IExecutionRepository repo) => 
    Results.Ok(repo.GetBlueprintHierarchy()));

// 2. Lihat Komitmen Mingguan Publik Tim
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

// 4. Fitur Granular Daily Breakdown: Pecah Komitmen Mingguan jadi Aksi Jam-Jaman Hari Ini
app.MapPost("/api/productivity-plan/daily-breakdown", (CreateDailyTaskDto input, IExecutionRepository repo) => {
    var newDailyTask = new DailyProductivityTask
    {
        Id = repo.GetDailyTasks().Count() + 1,
        WeeklyCommitmentId = input.WeeklyCommitmentId,
        Title = input.Title,
        EstimatedHours = input.EstimatedHours
    };
    
    repo.AddDailyTask(newDailyTask);
    return Results.Created($"/api/productivity-plan/{newDailyTask.Id}", newDailyTask);
});

// 5. Lihat Tugas Hari Ini
app.MapGet("/api/productivity-plan/today", (IExecutionRepository repo) => 
    Results.Ok(repo.GetDailyTasks()));

// 6. Sesi Sore: Check-Out Tugas Harian & Kunci Pengisian Bottleneck jika belum Done
app.MapPut("/api/productivity-plan/tasks/{id}/checkout", (int id, UpdateTaskStatusDto input, IExecutionRepository repo) => {
    var task = repo.GetDailyTaskById(id);
    if (task == null) return Results.NotFound("Tugas harian tidak ditemukan!");

    if (!input.Status.Equals("Done", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(input.Bottleneck))
    {
        return Results.BadRequest("Tugas BELUM SELESAI. Anda WAJIB mengisi 'Bottleneck' (Kendala/Alasan tidak tercapai) untuk evaluasi harian!");
    }

    task.Status = input.Status;
    task.Bottleneck = input.Bottleneck;
    if (input.Status.Equals("Done", StringComparison.OrdinalIgnoreCase)) task.CompletedAt = DateTime.Now;
    
    repo.UpdateDailyTask(task);
    return Results.Ok(new { Message = "Berhasil memperbarui progres harian!", Data = task });
});

// Redirect otomatis ke Swagger
app.MapGet("/", async context => {
    context.Response.Redirect("/swagger");
    await Task.CompletedTask;
});

app.Run();




