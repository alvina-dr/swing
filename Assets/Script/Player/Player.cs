using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerControls inputManager;
    public PlayerGrappling playerGrappling;
    public PlayerMovement playerMovement;
    public Rigidbody rigibody;
    public MeshRenderer playerMesh;
    public Transform rightHand;
    public Transform leftHand;

    [Header("STATS")]
    public float walkSpeed;
    public float swingSpeed;
    public float swingMaxSpeed;
    public float swingAcceleration;

    [Header("INPUTS")]
    [HideInInspector] public bool leftShoulder;
    [HideInInspector] public bool rightShoulder;

    private void Awake()
    {
        inputManager = new PlayerControls();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        inputManager.Enable();
        inputManager.Gameplay.Swing.started += function => { playerGrappling.trySwing = true; };
        inputManager.Gameplay.Swing.canceled += function => { playerGrappling.trySwing = false; };
        inputManager.Gameplay.LeftShoulder.started += function => { leftShoulder = true; };
        inputManager.Gameplay.LeftShoulder.canceled += function => { leftShoulder = false; };
        inputManager.Gameplay.RightShoulder.started += function => { rightShoulder = true; };
        inputManager.Gameplay.RightShoulder.canceled += function => { rightShoulder = false; };
        inputManager.Gameplay.Jump.started += playerMovement.Jump;
    }

    private void OnDisable()
    {
        inputManager.Disable();
    }
}
