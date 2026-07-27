using UnityEngine;

/// <summary>
/// Handles application quit requests from UI events.
/// </summary>
public class QuitGameController : MonoBehaviour
{
    /// <summary>
    /// Quits Play Mode in the Unity Editor or exits the built application.
    /// Attach this method to a UI Button OnClick event.
    /// </summary>
    public void QuitGame()
    {
        // Log the quit action so it is visible in the Console.
        Debug.Log("QuitGameController: Quit action triggered.");

#if UNITY_EDITOR
        // Stop Play Mode when running inside the Unity Editor.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit the standalone or mobile application build.
        Application.Quit();
#endif
    }
}
