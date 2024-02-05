using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player player;
    [Header("Movement")]
    public float currentMoveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
        // ground check
        if (player.playerGrappling.isGrappling) return;
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        if (player.playerGrappling.isSwinging)
        {
            if (currentMoveSpeed < player.swingSpeed) currentMoveSpeed = player.swingSpeed;
            currentMoveSpeed += player.swingAcceleration * Time.deltaTime;
            if (currentMoveSpeed >= player.swingMaxSpeed) currentMoveSpeed = player.swingMaxSpeed;
        }
        else if (grounded) currentMoveSpeed = player.walkSpeed; //Mathf.Lerp(currentMoveSpeed, GPCtrl.Instance.player.walkSpeed, Time.deltaTime * 3);

        // handle drag
        if (grounded && !player.playerGrappling.isGrappling)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if (player.playerGrappling.isSwinging)
        {
            player.playerMesh.transform.rotation = Quaternion.Slerp(GPCtrl.Instance.player.playerMesh.transform.rotation, Quaternion.LookRotation(rb.velocity), Time.deltaTime);
        }

        //else
        //{
        //    Quaternion.Slerp(GPCtrl.Instance.player.playerMesh.transform.rotation, Quaternion.identity, Time.deltaTime);
        //}
    }

    private void FixedUpdate()
    {
        if (player.playerGrappling.isGrappling) return;
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > currentMoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentMoveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (readyToJump && grounded)
        {
            readyToJump = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
