using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Updates score/time UI and controls result/rules panels.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("UI text that displays current score.")]
    public TextMeshProUGUI scoreText;

    [Tooltip("UI text that displays remaining time.")]
    public TMP_Text timerText;

    [Tooltip("Exit button shown while the game is active.")]
    public GameObject exitButton;

    [Tooltip("Bottom UI container shown while the game is active.")]
    public GameObject bottomContainer;

    public GameObject touchControlBar;

    public Image musicButtonImage;
    public Image sfxButtonImage;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;

    public Button infoButton;

    [Tooltip("Heart UI elements that represent remaining player lives (up to 5).")]
    public GameObject[] heartImages;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text winScoreText;
    [SerializeField] private GameObject fullRulesPanel;
    [SerializeField] private GameObject miniRulesPanel;
    [SerializeField] private GameObject comboPopUpPanel;
    [SerializeField] private float comboPopUpDuration = 0.8f;

    [Header("Manager References")]
    [Tooltip("Optional direct reference. If empty, this script will try to find ScoreManager automatically.")]
    public ScoreManager scoreManager;

    [Tooltip("Optional direct reference. If empty, this script will try to find TimerManager automatically.")]
    public TimerManager timerManager;

    [Tooltip("Optional direct reference. If empty, this script will try to find GameManager automatically.")]
    public GameManager gameManager;

    private bool miniRulesPausedGameplay;
    private bool hasShownGameOver;
    private Coroutine comboPopUpRoutine;
    private GameManager.GameState? lastKnownState;

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

        // Keep custom result panels hidden at startup.
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (fullRulesPanel != null)
        {
            fullRulesPanel.SetActive(false);
        }

        if (miniRulesPanel != null)
        {
            miniRulesPanel.SetActive(false);
        }

        if (comboPopUpPanel != null)
        {
            comboPopUpPanel.SetActive(false);
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

        if (touchControlBar != null)
        {
            touchControlBar.SetActive(false);
        }

        RefreshScoreText();
        if (timerManager != null)
        {
            UpdateTimerUI(timerManager.GetFormattedTime());
        }

        // Initialize lives UI in Awake
        if (gameManager != null)
        {
            Debug.Log("UIManager.Awake: Initializing lives UI. Current lives: " + gameManager.CurrentLives + ", Max lives: " + gameManager.maxLives);
            
            // Log initial state of heart images
            if (heartImages != null)
            {
                Debug.Log("UIManager.Awake: heartImages array has " + heartImages.Length + " elements");
                for (int i = 0; i < heartImages.Length; i++)
                {
                    if (heartImages[i] != null)
                    {
                        Debug.Log("  heartImages[" + i + "]: " + heartImages[i].name + " = " + (heartImages[i].activeSelf ? "ACTIVE" : "INACTIVE"));
                    }
                    else
                    {
                        Debug.LogWarning("  heartImages[" + i + "] is NULL!");
                    }
                }
            }
            else
            {
                Debug.LogWarning("UIManager.Awake: heartImages is NULL!");
            }
            
            UpdateLivesUI(gameManager.CurrentLives);
        }
        else
        {
            Debug.LogWarning("UIManager.Awake: GameManager not found yet. Lives UI will be initialized in OnEnable.");
        }

        UpdateAudioUI();
    }

    private void Start()
    {
        UpdateAudioUI();
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
        if (gameManager == null)
        {
            return;
        }

        // Show gameplay UI only while the game is active.
        bool isGameActive = gameManager.IsGameActive;

        if (exitButton != null)
        {
            exitButton.SetActive(isGameActive);
        }

        if (bottomContainer != null)
        {
            bottomContainer.SetActive(isGameActive);
        }

        if (touchControlBar != null)
        {
            touchControlBar.SetActive(isGameActive);
        }

        // If the game has ended, show default Game Over UI once when no custom result panel is active.
        if (!hasShownGameOver && gameManager.CurrentState == GameManager.GameState.GameOver && !IsCustomResultPanelActive())
        {
            // If player still has lives, show win screen (timer expired). Otherwise show game over.
            if (gameManager.CurrentLives > 0)
            {
                ShowWinScreen();
            }
            else
            {
                ShowGameOver();
            }
        }

        // Keep panels hidden while game is not over.
        if (gameManager.CurrentState != GameManager.GameState.GameOver && winPanel != null && winPanel.activeSelf)
        {
            winPanel.SetActive(false);
            hasShownGameOver = false;
        }
    }

    private bool IsCustomResultPanelActive()
    {
        bool winVisible = winPanel != null && winPanel.activeSelf;
        return winVisible;
    }

    private void ShowGameOver()
    {
        hasShownGameOver = true;
        Debug.Log("UIManager: Game Over - All lives lost. Showing loss screen.");
        
        // For now, show the same win panel but with a different message
        // In a full implementation, you'd have a separate loss panel
        if (winPanel != null)
        {
            DisableGameplayForResult();
            
            // Stop BGM and play win music to signal game end
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayWinMusic();
                Debug.Log("UIManager: ShowGameOver() - Playing win music");
            }
            
            winPanel.SetActive(true);
        }
        
        if (winScoreText != null && scoreManager != null)
        {
            winScoreText.text = scoreManager.CurrentScore.ToString();
        }
    }

    private void OnEnable()
    {
        hasShownGameOver = false;  // Reset when UIManager is enabled
        
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged += HandleScoreChanged;
            scoreManager.OnComboTriggered += HandleComboTriggered;
        }

        if (gameManager != null)
        {
            gameManager.OnGameStateChanged += HandleGameStateChanged;
            gameManager.OnLivesChanged += UpdateLivesUI;
            Debug.Log("UIManager.OnEnable: Subscribed to GameManager events. Current lives: " + gameManager.CurrentLives);
            HandleGameStateChanged(gameManager.CurrentState);
            UpdateLivesUI(gameManager.CurrentLives);
        }
        else
        {
            Debug.LogError("UIManager.OnEnable: gameManager is NULL!");
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
            gameManager.OnGameStateChanged -= HandleGameStateChanged;
            gameManager.OnLivesChanged -= UpdateLivesUI;
        }

        if (timerManager != null)
        {
            timerManager.OnTimeChanged -= UpdateTimerUI;
        }

        if (scoreManager != null)
        {
            scoreManager.OnComboTriggered -= HandleComboTriggered;
        }

        if (comboPopUpRoutine != null)
        {
            StopCoroutine(comboPopUpRoutine);
            comboPopUpRoutine = null;
        }

        if (comboPopUpPanel != null)
        {
            comboPopUpPanel.SetActive(false);
        }

        if (miniRulesPausedGameplay)
        {
            Time.timeScale = 1f;
            miniRulesPausedGameplay = false;
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
    /// Updates the heart UI elements to show remaining lives.
    /// Deactivates hearts from right to left as lives decrease.
    /// </summary>
    private void UpdateLivesUI(int currentLives)
    {
        Debug.Log("UIManager.UpdateLivesUI called with " + currentLives + " lives. heartImages = " + (heartImages == null ? "NULL" : heartImages.Length.ToString()));
        
        if (heartImages == null || heartImages.Length == 0)
        {
            Debug.LogError("UIManager: heartImages array is null or empty! Cannot update lives UI.");
            return;
        }

        Debug.Log("UIManager: Updating " + heartImages.Length + " heart images. Showing " + currentLives + " hearts.");
        
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null)
            {
                Debug.LogWarning("UIManager: heartImages[" + i + "] is NULL!");
                continue;
            }

            bool shouldBeActive = i < currentLives;
            bool wasActive = heartImages[i].activeSelf;
            
            // First try SetActive (if hearts are separate GameObjects)
            heartImages[i].SetActive(shouldBeActive);
            
            // Also update Image/CanvasGroup alpha as fallback (if hearts are UI elements)
            Image heartImage = heartImages[i].GetComponent<Image>();
            if (heartImage != null)
            {
                Color color = heartImage.color;
                color.a = shouldBeActive ? 1f : 0.3f;
                heartImage.color = color;
                Debug.Log("UIManager: heartImages[" + i + "] (" + heartImages[i].name + ") Image alpha set to " + color.a);
            }
            
            CanvasGroup canvasGroup = heartImages[i].GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = shouldBeActive ? 1f : 0.3f;
                Debug.Log("UIManager: heartImages[" + i + "] (" + heartImages[i].name + ") CanvasGroup alpha set to " + canvasGroup.alpha);
            }
            
            Debug.Log("UIManager: heartImages[" + i + "] (" + heartImages[i].name + ") was " + wasActive + ", now " + shouldBeActive);
        }

        Debug.Log("UIManager: Lives UI updated to show " + currentLives + "/" + heartImages.Length + " hearts.");
    }

    /// <summary>
    /// Shows the win screen and final score, then stops active gameplay.
    /// </summary>
    public void ShowWinScreen()
    {
        Debug.Log("UIManager.ShowWinScreen() called!");
        DisableGameplayForResult();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayWinMusic();
        }

        CloseMiniRulesPanel();

        if (winPanel != null)
        {
            Debug.Log("UIManager: Showing winPanel");
            winPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("UIManager: winPanel is NULL!");
        }

        if (winScoreText != null && scoreManager != null)
        {
            winScoreText.text = scoreManager.CurrentScore.ToString();
        }

        Debug.Log("UIManager: Win screen shown.");
    }

    /// <summary>
    /// Toggles global game audio mute state.
    /// Bind this method to the Mute button.
    /// </summary>
    public void ToggleMuteFromButton()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("UIManager: AudioManager not found. Cannot toggle mute.");
            return;
        }

        bool isMuted = AudioManager.Instance.ToggleMute();
        Debug.Log($"UIManager: Audio mute toggled. Muted = {isMuted}.");
        UpdateAudioUI();
    }

    /// <summary>
    /// Toggles music mute state and updates the button sprite.
    /// Bind this method to the Music button.
    /// </summary>
    public void ToggleMusic()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("UIManager: AudioManager not found. Cannot toggle music.");
            return;
        }

        bool nextMuteState = !AudioManager.Instance.IsMusicMuted;
        AudioManager.Instance.SetMusicMute(nextMuteState);
        UpdateAudioUI();
    }

    /// <summary>
    /// Toggles SFX mute state and updates the button sprite.
    /// Bind this method to the SFX button.
    /// </summary>
    public void ToggleSFX()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("UIManager: AudioManager not found. Cannot toggle SFX.");
            return;
        }

        bool nextMuteState = !AudioManager.Instance.IsSFXMuted;
        AudioManager.Instance.SetSFXMute(nextMuteState);
        UpdateAudioUI();
    }

    /// <summary>
    /// Opens the appropriate rules panel for the current game state.
    /// Bind this method to the Info button.
    /// </summary>
    public void OpenInfoPanel()
    {
        if (gameManager == null)
        {
            return;
        }

        bool isWinPanelActive = winPanel != null && winPanel.activeSelf;

        if (gameManager.CurrentState == GameManager.GameState.Playing || isWinPanelActive)
        {
            OpenMiniRulesPanel();
            return;
        }

        OpenFullRulesPanel();
    }

    /// <summary>
    /// Closes any open rules panel and resumes gameplay time when needed.
    /// </summary>
    public void CloseRulesPanels()
    {
        if (fullRulesPanel != null)
        {
            fullRulesPanel.SetActive(false);
        }

        if (miniRulesPanel != null)
        {
            miniRulesPanel.SetActive(false);
        }

        if (gameManager != null && gameManager.CurrentState == GameManager.GameState.Playing)
        {
            Time.timeScale = 1f;
        }

        miniRulesPausedGameplay = false;
    }

    /// <summary>
    /// Reloads the currently active scene and restarts gameplay.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        hasShownGameOver = false;
        
        // Reset audio to ensure BGM starts fresh on restart
        if (AudioManager.Instance != null)
        {
            Debug.Log("UIManager: RestartGame() - Resetting audio for fresh start");
            AudioManager.Instance.PlayBGM();
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Exits active gameplay and loads the initial scene (Main Menu).
    /// Uses async loading for WebGL compatibility.
    /// </summary>
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        hasShownGameOver = false;
        
        // Reset audio state for menu
        if (AudioManager.Instance != null)
        {
            Debug.Log("UIManager: ExitToMainMenu() - Stopping game music");
            AudioManager.Instance.PlayBGM();  // Will stop current and reset to BGM
        }
        
        SceneManager.LoadSceneAsync(0);
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

        SetGameplayUIActive(false);
    }

    private void SetGameplayUIActive(bool isActive)
    {
        if (exitButton != null && exitButton.activeSelf != isActive)
        {
            exitButton.SetActive(isActive);
        }

        if (bottomContainer != null && bottomContainer.activeSelf != isActive)
        {
            bottomContainer.SetActive(isActive);
        }

        if (touchControlBar != null && touchControlBar.activeSelf != isActive)
        {
            touchControlBar.SetActive(isActive);
        }
    }

    /// <summary>
    /// Opens the full rules panel.
    /// </summary>
    private void OpenFullRulesPanel()
    {
        if (fullRulesPanel != null)
        {
            fullRulesPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Opens the mini rules panel and pauses gameplay.
    /// </summary>
    private void OpenMiniRulesPanel()
    {
        if (miniRulesPanel == null)
        {
            return;
        }

        miniRulesPanel.SetActive(true);
        Time.timeScale = 0f;
        miniRulesPausedGameplay = true;
    }

    /// <summary>
    /// Closes the mini rules panel and restores gameplay time.
    /// </summary>
    private void CloseMiniRulesPanel()
    {
        if (miniRulesPanel != null)
        {
            miniRulesPanel.SetActive(false);
        }

        if (miniRulesPausedGameplay)
        {
            Time.timeScale = 1f;
            miniRulesPausedGameplay = false;
        }
    }

    /// <summary>
    /// Applies panel rules for Start, Playing, and GameOver states.
    /// </summary>
    private void HandleStateSpecificPanels(GameManager.GameState currentState)
    {
        if (!lastKnownState.HasValue || lastKnownState.Value != currentState)
        {
            if (currentState == GameManager.GameState.Start)
            {
                OpenFullRulesPanel();
            }

            if (currentState == GameManager.GameState.Playing)
            {
                if (fullRulesPanel != null)
                {
                    fullRulesPanel.SetActive(false);
                }

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayBGM();
                }
            }

            if (currentState == GameManager.GameState.GameOver)
            {
                CloseMiniRulesPanel();
            }

            if (infoButton != null)
            {
                bool showInfoButton = currentState == GameManager.GameState.Playing;
                infoButton.gameObject.SetActive(showInfoButton);
            }

            SetGameplayUIActive(currentState == GameManager.GameState.Playing);

            lastKnownState = currentState;
        }
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        HandleStateSpecificPanels(newState);
    }

    /// <summary>
    /// Shows combo popup for a short duration when combo bonus is triggered.
    /// </summary>
    private void HandleComboTriggered()
    {
        if (comboPopUpPanel == null)
        {
            return;
        }

        if (comboPopUpRoutine != null)
        {
            StopCoroutine(comboPopUpRoutine);
        }

        comboPopUpRoutine = StartCoroutine(ShowComboPopUpRoutine());
    }

    /// <summary>
    /// Displays combo popup then hides it automatically.
    /// </summary>
    private IEnumerator ShowComboPopUpRoutine()
    {
        comboPopUpPanel.SetActive(true);
        float displayDuration = Mathf.Max(0.1f, comboPopUpDuration);
        yield return new WaitForSecondsRealtime(displayDuration);
        comboPopUpPanel.SetActive(false);
        comboPopUpRoutine = null;
    }

    /// <summary>
    /// Syncs audio button sprites with current mute states.
    /// </summary>
    private void UpdateAudioUI()
    {
        bool isMusicMuted = AudioManager.Instance != null && AudioManager.Instance.IsMusicMuted;
        bool isSfxMuted = AudioManager.Instance != null && AudioManager.Instance.IsSFXMuted;

        if (musicButtonImage != null)
        {
            Sprite musicSprite = isMusicMuted ? musicOffSprite : musicOnSprite;
            if (musicSprite != null)
            {
                musicButtonImage.sprite = musicSprite;
            }
        }

        if (sfxButtonImage != null)
        {
            Sprite sfxSprite = isSfxMuted ? sfxOffSprite : sfxOnSprite;
            if (sfxSprite != null)
            {
                sfxButtonImage.sprite = sfxSprite;
            }
        }
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
