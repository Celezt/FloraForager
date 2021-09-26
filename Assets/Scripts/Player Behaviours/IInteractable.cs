using UnityEngine;
public interface IInteractable
{
    /// <summary>
    /// Higher priority will always be prioritized no matter how far away it is compared to lower priorities.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Called on interaction with object that inherit <c>IInteractable</c>.
    /// </summary>
    /// <param name="context"></param>
    public void OnInteract(InteractContext context);
}
