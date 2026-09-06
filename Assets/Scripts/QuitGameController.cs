using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles application quit requests from UI events.
/// </summary>
public class QuitGameController : MonoBehaviour
{
    /// <summary>
    /// Quits Play Mode in the Unity Editor or returns to the start scene in built applications.
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
        // Application.Quit() is a no-op in WebGL and freezes the player loop, so go to the menu scene instead.
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
#endif
    }
}
