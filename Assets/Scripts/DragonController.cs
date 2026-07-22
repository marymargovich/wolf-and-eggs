using UnityEngine;

/// <summary>
/// Controls the dragon's horizontal movement for Little Dragon Treasure Hunt.
/// Movement is allowed only while the game is active.
/// </summary>
public class DragonController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Horizontal movement speed in units per second.")]
    public float speed = 8f;

    [Tooltip("How far the dragon can move left/right from its start X position.")]
    public float xClamp = 7f;

    [Header("References (Optional)")]
    [Tooltip("If left empty, the script will try to find GameManager automatically.")]
    public GameManager gameManager;

    [Tooltip("Optional SpriteRenderer for left/right visual flip.")]
    public SpriteRenderer spriteRenderer;

    [Tooltip("Optional Animator for movement animation.")]
    public Animator animator;

    [Header("Animator Parameters")]
    [Tooltip("Animator float parameter name for movement speed.")]
    public string speedParameter = "Speed";

    [Tooltip("Animator bool parameter name for movement state.")]
    public string isMovingParameter = "IsMoving";

    private float startX;
    private bool lastMovingState;
    private int lastFacingDirection;
    private bool hasSpeedParameter;
    private bool hasIsMovingParameter;

    private void Awake()
    {
        startX = transform.position.x;

        // Auto-find GameManager if it was not assigned in the Inspector.
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (gameManager != null)
        {
            Debug.Log("DragonController: GameManager found.");
        }
        else
        {
            Debug.LogWarning("DragonController: GameManager not found. Dragon movement will stay locked until a GameManager exists.");
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator != null)
        {
            hasSpeedParameter = HasAnimatorParameter(speedParameter, AnimatorControllerParameterType.Float);
            hasIsMovingParameter = HasAnimatorParameter(isMovingParameter, AnimatorControllerParameterType.Bool);
            Debug.Log("DragonController: Animator detected.");
        }
    }

    private void Update()
    {
        // Move only while the game is active.
        if (gameManager == null || !gameManager.IsGameActive)
        {
            UpdateAnimation(0f, false);
            return;
        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Translate along X-axis only.
        float moveStep = horizontalInput * speed * Time.deltaTime;
        transform.Translate(moveStep, 0f, 0f);

        // Clamp position so the dragon stays within horizontal bounds.
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, startX - xClamp, startX + xClamp);
        transform.position = position;

        UpdateDirection(horizontalInput);

        bool isMoving = Mathf.Abs(horizontalInput) > 0.01f;
        UpdateAnimation(Mathf.Abs(horizontalInput), isMoving);

        // Log only when movement state changes to avoid console spam.
        if (isMoving != lastMovingState)
        {
            Debug.Log(isMoving ? "DragonController: Dragon started moving." : "DragonController: Dragon stopped moving.");
            lastMovingState = isMoving;
        }
    }

    /// <summary>
    /// Flips the dragon to face left or right based on movement input.
    /// </summary>
    private void UpdateDirection(float horizontalInput)
    {
        if (horizontalInput > 0.01f)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x);
                transform.localScale = scale;
            }

            if (lastFacingDirection != 1)
            {
                Debug.Log("DragonController: Facing right.");
                lastFacingDirection = 1;
            }
        }
        else if (horizontalInput < -0.01f)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                Vector3 scale = transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                transform.localScale = scale;
            }

            if (lastFacingDirection != -1)
            {
                Debug.Log("DragonController: Facing left.");
                lastFacingDirection = -1;
            }
        }
    }

    /// <summary>
    /// Updates animator parameters if an Animator is present and parameters exist.
    /// </summary>
    private void UpdateAnimation(float speedValue, bool isMoving)
    {
        if (animator == null)
        {
            return;
        }

        if (hasSpeedParameter)
        {
            animator.SetFloat(speedParameter, speedValue);
        }

        if (hasIsMovingParameter)
        {
            animator.SetBool(isMovingParameter, isMoving);
        }
    }

    /// <summary>
    /// Checks whether the Animator contains a parameter with the expected name and type.
    /// </summary>
    private bool HasAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == parameterType)
            {
                return true;
            }
        }

        Debug.LogWarning($"DragonController: Animator parameter '{parameterName}' ({parameterType}) was not found.");
        return false;
    }
}
