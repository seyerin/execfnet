namespace SanityCheck.Models;


public class SubGoal
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<ProductivityTask> KeyDrivers { get; set; } = new();
}

public class ProductivityTask
{
    public int Id { get; set; }
    public int SubGoalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsTopPriority { get; set; }
}

public class TeamCommitmentTask
{
    public int Id { get; set; }
    public int SourcePlanTaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = "To-Do";
    public bool IsTopPriority { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ReasonForNonAchievement { get; set; }
}

public class DailyProductivityTask
{
    public int Id { get; set; }
    public int WeeklyCommitmentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int EstimatedHours { get; set; }
    public string Status { get; set; } = "To-Do";
    public string? Bottleneck { get; set; }
}

public class CustomStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Contoh: "To-Do", "In Review", "Done"
    public string ColorCode { get; set; } = "#HEXCODE"; // Untuk kebutuhan UI di Figma
    // Penanda kategori dasar agar sistem tetap bisa menghitung analitik (Priority Analytics)
    public string CoreCategory { get; set; } = "Active"; // Nilainya: "Backlog", "Active", atau "Final"
    // Kepemilikan: Null berarti status bawaan sistem, jika terisi berarti status custom milik tim tertentu
    public string? TeamId { get; set; } 
}


public record CreateDailyTaskDto(int WeeklyCommitmentId, string Title, int EstimatedHours);
public record UpdateTaskStatusDto(string Status, string? Bottleneck);