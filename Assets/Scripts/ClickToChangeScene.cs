using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToChangeScene : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load on screen click.")]
    public string nextSceneName = "GameScene";

    private void Update()
    {
        // Detects left mouse click or touch on screen
        if (Input.GetMouseButtonDown(0))
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        Debug.Log($"ClickToChangeScene: Loading scene '{nextSceneName}'.");
        SceneManager.LoadScene(nextSceneName);
    }
}