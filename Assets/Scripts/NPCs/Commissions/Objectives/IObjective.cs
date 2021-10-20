public interface IObjective
{
    public bool IsCompleted { get; }
    public string Status { get; }

    public void Initialize(IObjective objectiveData);
    public void UpdateStatus();
    public void Complete();
}
