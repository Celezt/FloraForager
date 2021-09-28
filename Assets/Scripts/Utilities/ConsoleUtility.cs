using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IngameDebugConsole;

public static class ConsoleUtility
{
    [ConsoleMethod("quit", "Quit the game.")]
    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
