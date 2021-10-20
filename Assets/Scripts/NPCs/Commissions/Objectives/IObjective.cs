public interface IObjective
{
    public bool IsCompleted { get; }
    public string Status { get; }

    public void Initialize(IObjective objectiveData);

    public void Accepted();
    public void Removed();
    public void Complete();

    public void UpdateStatus();
}
