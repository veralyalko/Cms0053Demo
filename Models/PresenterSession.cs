namespace Cms0053Demo.Models;

public class PresenterSession
{
    public int Id { get; set; }
    public int ActiveScenarioId { get; set; }
    public DemoScenario ActiveScenario { get; set; } = null!;
    public int CurrentScreen { get; set; } = 1;
    public DateTime StartedAt { get; set; }
    public bool IsActive { get; set; }
}
