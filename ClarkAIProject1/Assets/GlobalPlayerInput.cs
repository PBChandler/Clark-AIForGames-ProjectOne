using UnityEngine;
using UnityEngine.InputSystem;
public class GlobalPlayerInput : MonoBehaviour
{
    public static GlobalPlayerInput instance;
    public InputSystem_Actions main;
    public Vector2 look, move;
    public delegate void vector2Input(Vector2 info);
    public vector2Input OnLookInput;
    public vector2Input OnMoveInput;
    public vector2Input OnMoveInputCancelled;
    public void Awake()
    {
        if(GlobalPlayerInput.instance != null && GlobalPlayerInput.instance != this)
        {
            Destroy(this);
        }
        else
        {
            GlobalPlayerInput.instance = this;
        }
        main = new InputSystem_Actions();
        OnLookInput += vtwodummy;
        OnMoveInput += vtwodummy;
        OnMoveInputCancelled += vtwodummy;
        main.Player.Look.performed += ctx => look = ctx.ReadValue<Vector2>();
        main.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        main.Player.Move.canceled += ctx => move = ctx.ReadValue<Vector2>();
    }
    /// <summary>
    /// just exists to not have the delegate throw errors when it doesn't have listeners.
    /// </summary>
    /// <param name="woah"></param>
    public void vtwodummy(Vector2 woah) { }
    public void OnEnable()
    {
        main.Enable();
    }

    public void OnDisable()
    {
        main.Disable();
    }
    public void Update()
    {
        HandleLookInput();
        HandleMoveInput();
    }

    public void HandleMoveInput()
    {
        OnMoveInput.Invoke(move);
    }
    public void HandleLookInput()
    {
        Debug.Log(look);
        OnLookInput.Invoke(look);
    }
}
