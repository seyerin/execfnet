using SanityCheck.Models;

namespace SanityCheck.Repositories;

public class InMemoryExecutionRepository : IExecutionRepository
{
    // Simulasi tabel database menggunakan List di dalam RAM
    private readonly List<SubGoal> _subGoals = new();
    private readonly List<ProductivityTask> _productivityTasks = new();
    private readonly List<TeamCommitmentTask> _weeklyCommitments = new();
    private readonly List<DailyProductivityTask> _dailyTasks = new();

    // Constructor: Otomatis mengisi data tiruan awal saat aplikasi menyala
    public InMemoryExecutionRepository()
    {
        // 1. Membuat Sub-Goals (Pilar Istana)
        _subGoals.Add(new SubGoal { Id = 1, Title = "Meningkatkan Otonomi & Kecepatan Eksekusi Tim" });
        _subGoals.Add(new SubGoal { Id = 2, Title = "Membangun Sistem Tracking Transparan" });

        // 2. Membuat Key Drivers yang nempel ke Sub-Goals di atas
        _productivityTasks.Add(new ProductivityTask { Id = 1, SubGoalId = 1, Title = "Rancang Arsitektur Database di .NET", IsTopPriority = true });
        _productivityTasks.Add(new ProductivityTask { Id = 2, SubGoalId = 1, Title = "Optimasi Query PostgreSQL untuk Dashboard", IsTopPriority = false });
        _productivityTasks.Add(new ProductivityTask { Id = 3, SubGoalId = 2, Title = "Integrasi Dual-View Calendar di Vue.js", IsTopPriority = true });
    }

    // --- IMPLEMENTASI FUNGSI KONTRAK ---

    // Mengambil hierarki utuh (SubGoal beserta daftar Key Drivers di dalamnya)
    public IEnumerable<SubGoal> GetBlueprintHierarchy()
    {
        foreach (var subGoal in _subGoals)
        {
            // Auto-pull relasi anak berdasarkan SubGoalId
            subGoal.KeyDrivers = _productivityTasks.Where(kd => kd.SubGoalId == subGoal.Id).ToList();
        }
        return _subGoals;
    }

    public ProductivityTask? GetProductivityTaskById(int id) => 
        _productivityTasks.FirstOrDefault(t => t.Id == id);

    public IEnumerable<TeamCommitmentTask> GetWeeklyCommitments() => _weeklyCommitments;

    public void AddWeeklyCommitment(TeamCommitmentTask commitment) => 
        _weeklyCommitments.Add(commitment);

    public bool IsAlreadyCommitted(int planTaskId) => 
        _weeklyCommitments.Any(w => w.SourcePlanTaskId == planTaskId);

    public IEnumerable<DailyProductivityTask> GetDailyTasks() => _dailyTasks;

    public DailyProductivityTask? GetDailyTaskById(int id) => 
        _dailyTasks.FirstOrDefault(t => t.Id == id);

    public void AddDailyTask(DailyProductivityTask task) => _dailyTasks.Add(task);

    public void UpdateDailyTask(DailyProductivityTask task)
    {
        var existing = GetDailyTaskById(task.Id);
        if (existing != null)
        {
            existing.Status = task.Status;
            existing.Bottleneck = task.Bottleneck;
            existing.CompletedAt = task.CompletedAt;
        }
    }
}