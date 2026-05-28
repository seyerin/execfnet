using SanityCheck.Models;

namespace SanityCheck.Repositories;

public interface IExecutionRepository
{
    // --- Area 1: Strategic Blueprint (Hierarki Goal) ---
    IEnumerable<SubGoal> GetBlueprintHierarchy();
    ProductivityTask? GetProductivityTaskById(int id);

    // --- Area 2: Weekly Team Commitment (WTC) ---
    IEnumerable<TeamCommitmentTask> GetWeeklyCommitments();
    void AddWeeklyCommitment(TeamCommitmentTask commitment);
    bool IsAlreadyCommitted(int planTaskId);

    // --- Area 3: Productivity Plan (Daily Execution - THE HOW) ---
    IEnumerable<DailyProductivityTask> GetDailyTasks();
    DailyProductivityTask? GetDailyTaskById(int id);
    void AddDailyTask(DailyProductivityTask task);
    void UpdateDailyTask(DailyProductivityTask task);
}