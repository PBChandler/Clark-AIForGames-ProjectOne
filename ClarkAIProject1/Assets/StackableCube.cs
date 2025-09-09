using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StackableCube : MonoBehaviour
{
    private Rigidbody rb;
    public bool hasLanded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        // Don't register immediately — wait until it has landed
        hasLanded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Consider landed if it touches the ground or another cube
        if (!hasLanded && (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Cube")))
        {
            hasLanded = true;

            // Register with the StackManager
            if (StackManager.Instance != null)
                StackManager.Instance.RegisterCube(transform);
        }
    }

    private void OnDisable()
    {
        if (StackManager.Instance != null)
        {
            StackManager.Instance.DeregisterCube(transform);
        }
    }
}
