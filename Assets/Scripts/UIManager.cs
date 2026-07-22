using TMPro;
using UnityEngine;

/// <summary>
/// Updates score/time UI and controls the Game Over panel.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("UI text that displays current score.")]
    public TextMeshProUGUI scoreText;

    [Tooltip("UI text that displays remaining time.")]
    public TextMeshProUGUI timerText;

    [Tooltip("Panel that appears when the game ends.")]
    public GameObject gameOverPanel;

    [Tooltip("UI text that displays final score on the Game Over panel.")]
    public TextMeshProUGUI finalScoreText;

    [Header("Manager References")]
    [Tooltip("Optional direct reference. If empty, this script will try to find ScoreManager automatically.")]
    public ScoreManager scoreManager;

    [Tooltip("Optional direct reference. If empty, this script will try to find TimerManager automatically.")]
    public TimerManager timerManager;

    [Tooltip("Optional direct reference. If empty, this script will try to find GameManager automatically.")]
    public GameManager gameManager;

    private bool hasShownGameOver;

    private void Awake()
    {
        // Auto-find manager references if they are not assigned.
        if (scoreManager == null)
        {
            scoreManager = FindAnyObjectByType<ScoreManager>();
        }

        if (timerManager == null)
        {
            timerManager = FindAnyObjectByType<TimerManager>();
        }

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        // Auto-find common UI references if they are not assigned.
        if (scoreText == null)
        {
            scoreText = FindTextByName("ScoreText");
        }

        if (timerText == null)
        {
            timerText = FindTextByName("TimerText");
        }

        if (finalScoreText == null)
        {
            finalScoreText = FindTextByName("FinalScoreText");
        }

        if (gameOverPanel == null)
        {
            GameObject panelObject = GameObject.Find("GameOverPanel");
            if (panelObject != null)
            {
                gameOverPanel = panelObject;
            }
        }

        if (scoreManager != null)
        {
            Debug.Log("UIManager: ScoreManager found.");
        }
        else
        {
            Debug.LogWarning("UIManager: ScoreManager not found.");
        }

        if (timerManager != null)
        {
            Debug.Log("UIManager: TimerManager found.");
        }
        else
        {
            Debug.LogWarning("UIManager: TimerManager not found.");
        }

        if (gameManager != null)
        {
            Debug.Log("UIManager: GameManager found.");
        }
        else
        {
            Debug.LogWarning("UIManager: GameManager not found.");
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Debug.Log("UIManager: GameOver panel hidden at startup.");
        }
        else
        {
            Debug.LogWarning("UIManager: GameOver panel reference is missing.");
        }

        RefreshScoreText();
        RefreshTimerText();
    }

    private void OnEnable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged += HandleScoreChanged;
        }
    }

    private void OnDisable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= HandleScoreChanged;
        }
    }

    private void Update()
    {
        // Keep timer text current while the scene is running.
        RefreshTimerText();

        // If the game has ended, show Game Over UI once.
        if (!hasShownGameOver && gameManager != null && gameManager.CurrentState == GameManager.GameState.GameOver)
        {
            ShowGameOver();
        }

        // Keep panel hidden while game is not over.
        if (gameManager != null && gameManager.CurrentState != GameManager.GameState.GameOver && gameOverPanel != null && gameOverPanel.activeSelf)
        {
            gameOverPanel.SetActive(false);
            hasShownGameOver = false;
            Debug.Log("UIManager: GameOver panel hidden (game is not over).");
        }
    }

    /// <summary>
    /// Called when ScoreManager reports a score change.
    /// </summary>
    private void HandleScoreChanged(int newScore)
    {
        RefreshScoreText();
        Debug.Log($"UIManager: Score changed to {newScore}.");
    }

    /// <summary>
    /// Updates the score label text.
    /// </summary>
    private void RefreshScoreText()
    {
        if (scoreText == null || scoreManager == null)
        {
            return;
        }

        scoreText.text = $"Score: {scoreManager.CurrentScore}";
    }

    /// <summary>
    /// Updates the timer label text using TimerManager formatted output.
    /// </summary>
    private void RefreshTimerText()
    {
        if (timerText == null || timerManager == null)
        {
            return;
        }

        timerText.text = $"Time: {timerManager.GetFormattedTime()}";
    }

    /// <summary>
    /// Shows Game Over panel and writes final score.
    /// </summary>
    private void ShowGameOver()
    {
        hasShownGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null && scoreManager != null)
        {
            finalScoreText.text = $"Final Score: {scoreManager.CurrentScore}";
        }

        Debug.Log("UIManager: Game Over UI shown.");
    }

    /// <summary>
    /// Finds a TextMeshProUGUI element in the scene by object name.
    /// </summary>
    private TextMeshProUGUI FindTextByName(string objectName)
    {
        GameObject textObject = GameObject.Find(objectName);
        if (textObject == null)
        {
            return null;
        }

        return textObject.GetComponent<TextMeshProUGUI>();
    }
}
