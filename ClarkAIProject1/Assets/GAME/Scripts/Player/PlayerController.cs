using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform camerae;
    public float moveSpeed = 25f, speedCap = 50f;
    public Rigidbody rb;
    public float decelerationFactor = 5f;
    bool isDecelerating;
    public Vector3 moveInput;
    public Vector3 moveForce;
    public Vector3 negativeMoveDirection;
    public float drag;
    public void Start()
    {
        GlobalPlayerInput.instance.OnMoveInput += GetMoveInput;
        GlobalPlayerInput.instance.OnMoveInputCancelled += Halt;
    }

    public void GetMoveInput(Vector2 vec)
    {
        if (vec.magnitude > 0.05f)
        {
            isDecelerating = false;
        }
        float moveX = Mathf.Abs(vec.x) > 0 ? Mathf.Sign(vec.x) : 0;
        float moveY = Mathf.Abs(vec.y) > 0 ? Mathf.Sign(vec.y) : 0;

        Vector3 moveDir = camerae.forward * moveY + camerae.right * moveX;
        moveDir = new Vector3(moveDir.x, 0, moveDir.z);
        if (moveDir != Vector3.zero)
            negativeMoveDirection = -moveDir;

        //rb.AddForce(moveDir.normalized * moveSpeed, ForceMode.Force);
        moveForce = moveDir.normalized * moveSpeed;
        moveInput = new Vector3(moveX, 0, moveY);
    }

    public void Halt(Vector2 useless)
    {
        Debug.Log("You've violated the law");
        isDecelerating = true;
        moveForce = Vector2.zero;
        moveInput = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        moveInput = Vector3.zero;
    }

    public void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.y);

        if (flatVel.magnitude > speedCap)
        {
            Vector3 limitedvel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedvel.x, rb.linearVelocity.y, limitedvel.z);
        }

    }
    public void Update()
    {
        rb.linearDamping = drag;
    }
    public void FixedUpdate()
    {
        SpeedControl(); 
        if (!isDecelerating)
        {

            rb.AddForce(moveForce, ForceMode.Force);
        }
        else
        {
            //rb.AddForce(negativeMoveDirection, ForceMode.Force);
            //negativeMoveDirection = Vector3.Lerp(negativeMoveDirection, Vector3.zero, 25);
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * decelerationFactor);
        }
    }
}
