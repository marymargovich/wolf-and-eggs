using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 0.4f;

    private void Start()
    {
        // Automatically destroy this GameObject after the configured delay.
        Destroy(gameObject, destroyDelay);
    }
}
