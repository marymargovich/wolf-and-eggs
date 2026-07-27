using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Updates score/time UI and controls the Game Over panel.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("UI text that displays current score.")]
    public TextMeshProUGUI scoreText;

    [Tooltip("UI text that displays remaining time.")]
    public TMP_Text timerText;

    [Tooltip("Panel that appears when the game ends.")]
    public GameObject gameOverPanel;

    [Tooltip("Exit button shown while the game is active.")]
    public GameObject exitButton;

    [Tooltip("Bottom UI container shown while the game is active.")]
    public GameObject bottomContainer;

    [Tooltip("Heart UI elements that represent remaining player lives.")]
    public GameObject[] heartImages;

    [Tooltip("UI text that displays final score on the Game Over panel.")]
    public TextMeshProUGUI finalScoreText;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text winScoreText;

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

        // Keep custom result panels hidden at startup.
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        // Hide optional gameplay UI elements at startup.
        if (exitButton != null)
        {
            exitButton.SetActive(false);
        }

        if (bottomContainer != null)
        {
            bottomContainer.SetActive(false);
        }

        RefreshScoreText();
        if (timerManager != null)
        {
            UpdateTimerUI(timerManager.GetFormattedTime());
        }
    }

    private void OnEnable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged += HandleScoreChanged;
        }

        if (gameManager != null)
        {
            gameManager.OnLivesChanged += UpdateLivesUI;
            UpdateLivesUI(gameManager.CurrentLives);
        }

        if (timerManager != null)
        {
            timerManager.OnTimeChanged += UpdateTimerUI;
            UpdateTimerUI(timerManager.GetFormattedTime());
        }
    }

    private void OnDisable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= HandleScoreChanged;
        }

        if (gameManager != null)
        {
            gameManager.OnLivesChanged -= UpdateLivesUI;
        }

        if (timerManager != null)
        {
            timerManager.OnTimeChanged -= UpdateTimerUI;
        }
    }

    private void Update()
    {
        UpdateGameUI();
    }

    /// <summary>
    /// Updates runtime UI visibility and game-over presentation.
    /// </summary>
    private void UpdateGameUI()
    {
        // Show gameplay UI only while the game is active.
        bool isGameActive = gameManager != null && gameManager.IsGameActive;

        if (exitButton != null)
        {
            exitButton.SetActive(isGameActive);
        }

        if (bottomContainer != null)
        {
            bottomContainer.SetActive(isGameActive);
        }

        // If the game has ended, show default Game Over UI once when no custom result panel is active.
        if (!hasShownGameOver && gameManager != null && gameManager.CurrentState == GameManager.GameState.GameOver && !IsCustomResultPanelActive())
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

        scoreText.text = $"{scoreManager.CurrentScore}";
    }

    /// <summary>
    /// Updates the timer label text when TimerManager publishes a new time string.
    /// </summary>
    private void UpdateTimerUI(string timeString)
    {
        if (timerText == null)
        {
            return;
        }

        timerText.text = timeString;
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
    /// Shows the win screen and final score, then stops active gameplay.
    /// </summary>
    public void ShowWinScreen()
    {
        DisableGameplayForResult();
        hasShownGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if (winScoreText != null && scoreManager != null)
        {
            winScoreText.text = "You win " + scoreManager.CurrentScore;
        }

        Debug.Log("UIManager: Win screen shown.");
    }

    /// <summary>
    /// Reloads the currently active scene.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Updates heart icons to match current remaining lives.
    /// </summary>
    private void UpdateLivesUI(int currentLives)
    {
        if (heartImages == null || heartImages.Length == 0)
        {
            return;
        }

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null)
            {
                continue;
            }

            heartImages[i].SetActive(i < currentLives);
        }
    }

    /// <summary>
    /// Ends active gameplay and hides gameplay-only UI controls.
    /// </summary>
    private void DisableGameplayForResult()
    {
        if (gameManager != null && gameManager.IsGameActive)
        {
            gameManager.EndGame();
        }

        if (exitButton != null)
        {
            exitButton.SetActive(false);
        }

        if (bottomContainer != null)
        {
            bottomContainer.SetActive(false);
        }
    }

    /// <summary>
    /// Returns true when a custom result panel is visible.
    /// </summary>
    private bool IsCustomResultPanelActive()
    {
        bool winVisible = winPanel != null && winPanel.activeSelf;
        return winVisible;
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
