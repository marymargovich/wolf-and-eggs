using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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

    [Tooltip("How much the dragon can overlap past the visible screen edge.")]
    public float edgeOverlapMargin = 0.25f;

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

    private bool lastMovingState;
    private int lastFacingDirection;
    private bool hasSpeedParameter;
    private bool hasIsMovingParameter;
    private Camera cachedMainCamera;

    private void Awake()
    {
        cachedMainCamera = Camera.main;

        // Auto-find GameManager if it was not assigned in the Inspector.
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
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

        // Prefer pointer/touch hold input; fall back to legacy keyboard input otherwise.
        if (TryGetPointerTargetWorldX(out float pointerTargetWorldX))
        {
            Vector3 currentPos = transform.position;
            float clampedTargetX = GetClampedX(pointerTargetWorldX);
            float newX = Mathf.MoveTowards(currentPos.x, clampedTargetX, speed * Time.deltaTime);
            float deltaX = newX - currentPos.x;

            horizontalInput = Mathf.Abs(deltaX) > 0.0001f ? Mathf.Sign(deltaX) : 0f;
            currentPos.x = newX;
            transform.position = currentPos;
        }
        else
        {
            // Translate along X-axis only.
            float moveStep = horizontalInput * speed * Time.deltaTime;
            transform.Translate(moveStep, 0f, 0f);
        }

        // Clamp position using camera screen bounds so the dragon stays visible.
        Vector3 position = transform.position;
        float clampedX = GetClampedX(position.x);
        position.x = clampedX;
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
    /// Returns true when pointer/touch or left mouse button is held and provides target world X.
    /// </summary>
    private bool TryGetPointerTargetWorldX(out float targetWorldX)
    {
        targetWorldX = 0f;

        Camera mainCamera = cachedMainCamera;
        if (mainCamera == null)
        {
            cachedMainCamera = Camera.main;
            mainCamera = cachedMainCamera;
        }

        if (mainCamera == null)
        {
            return false;
        }

        Vector2 screenPosition;
        if (!TryGetHeldPointerScreenPosition(out screenPosition))
        {
            return false;
        }

        float distanceToCamera = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 pointerScreenPoint = new Vector3(screenPosition.x, screenPosition.y, distanceToCamera);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(pointerScreenPoint);
        targetWorldX = worldPoint.x;
        return true;
    }

    /// <summary>
    /// Gets held pointer screen position from Input System first, then legacy mouse fallback.
    /// </summary>
    private bool TryGetHeldPointerScreenPosition(out Vector2 screenPosition)
    {
        screenPosition = default;

#if ENABLE_INPUT_SYSTEM
        if (Pointer.current != null && Pointer.current.press.isPressed)
        {
            screenPosition = Pointer.current.position.ReadValue();
            return true;
        }
#endif

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            screenPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }

        return false;
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

    /// <summary>
    /// Returns X position clamped to the camera view with a small overlap margin.
    /// Falls back to legacy xClamp when a camera is not available.
    /// </summary>
    private float GetClampedX(float targetX)
    {
        Camera mainCamera = cachedMainCamera;
        if (mainCamera == null)
        {
            cachedMainCamera = Camera.main;
            mainCamera = cachedMainCamera;
        }

        if (mainCamera == null)
        {
            return Mathf.Clamp(targetX, -xClamp, xClamp);
        }

        float distanceToCamera = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, distanceToCamera));
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, distanceToCamera));

        float minX = leftEdge.x - edgeOverlapMargin;
        float maxX = rightEdge.x + edgeOverlapMargin;
        return Mathf.Clamp(targetX, minX, maxX);
    }
}
