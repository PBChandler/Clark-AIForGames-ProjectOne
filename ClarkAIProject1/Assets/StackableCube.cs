using UnityEngine;

public class StackableCube : MonoBehaviour
{
    private void OnEnable()
    {
        
        StackManager.Instance.RegisterCube(transform);
    }

    private void OnDisable()
    {
        if (StackManager.Instance != null)
        {
            StackManager.Instance.DeregisterCube(transform);
        }
    }
}