using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class ToggleInteraction : IInputInteraction
{
    private InputInteractionContext _context;

    static ToggleInteraction()
    {
        InputSystem.RegisterInteraction<ToggleInteraction>();
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize() { }

    public void Process(ref InputInteractionContext context)
    {
        _context = context;

        switch (context.phase)
        {
            case InputActionPhase.Waiting:
                if (context.action.IsPressed())
                {
                    context.Started();
                }
                break;
            case InputActionPhase.Started:
                if (context.action.IsPressed())
                {
                    context.Canceled();
                }
                break;
        }
    }

    public void Reset()
    {

    }
}
