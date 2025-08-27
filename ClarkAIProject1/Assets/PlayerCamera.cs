using Unity.Cinemachine;
using UnityEngine;
public class PlayerCamera : MonoBehaviour
{
    public CinemachineFreeLook cm;
    public Transform body;
    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public void HandleLookInput(Vector2 inp)
    {
        
    }
}
