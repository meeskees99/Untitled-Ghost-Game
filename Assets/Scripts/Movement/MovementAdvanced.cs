using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementAdvanced : NetworkBehaviour
{
    [Header("Movement")]
    float moveSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    //[SerializeField] float crouchSpeed;
    [SerializeField] float groundDrag;
    [SerializeField] Animator animator;

    [Header("Jumping")]
    [Tooltip("How high the player will jump")]
    [SerializeField] float jumpForce;
    [Tooltip("The time a player has to wait before being able to jump again")]
    [SerializeField] float jumpCooldown;
    [Tooltip("Speed will multiply by this whilst airborn")]
    [SerializeField] float airMultiplier;
    bool readyToJump;

    //[Header("Crouching")]
    //[SerializeField] float crouchYScale;

    //float startCrouchYScale;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] bool grounded;

    [Header("Slope Check")]
    [SerializeField] float maxSlopeAngle;
    RaycastHit slopeHit;

    [SerializeField] Transform orientation;
    [SerializeField] TMP_Text speedTxt;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDir;

    Rigidbody rb;


    public MovementState state;
    public enum MovementState
    {
        walk, run, air
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.freezeRotation = true;
        readyToJump = true;
        //animator = GetComponent<Animator>();
        //startCrouchYScale = transform.localScale.y;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsHost && gameObject.GetComponent<PlayerData>().playerId != 0)
        {
            this.enabled = false;
        }
        else if (!IsOwner)
        {
            this.enabled = false;
        }
    }

    bool GroundBool
    {
        get { return grounded; }
        set
        {
            if (value == grounded)
            {
                return;
            }
            grounded = value;
            if (grounded)
            {
                print("Ik kom");
                DoAnimation("HasLanded");
            }
        }
    }
    private void Update()
    {
        //speedTxt.text = "Speed: " + rb.velocity.magnitude.ToString("0.##");

        // Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, 0.01f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // Handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        DoBlendTree();

        //When To Jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // //When To Crouch
        // if (Input.GetKeyDown(crouchKey))
        // {
        //     //Placeholder
        //     transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        //     //animator.SetTrigger("Crouch");
        //     //rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        // }

        // if (Input.GetKeyUp(crouchKey))
        // {
        //     transform.localScale = new Vector3(transform.localScale.x, startCrouchYScale, transform.localScale.z);
        // }
    }

    void StateHandler()
    {
        //Mode Crouching
        // if (Input.GetKey(crouchKey))
        // {
        //     state = MovementState.crouch;
        //     moveSpeed = crouchSpeed;
        // }
        //Mode - Running
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.run;
            moveSpeed = sprintSpeed;
        }
        //Mode - Walking
        else if (grounded)
        {
            state = MovementState.walk;
            moveSpeed = walkSpeed;
        }
        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }
    void MovePlayer()
    {
        // Calculalte move direction
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // On slope
        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 10f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 40f, ForceMode.Force);
        }

        //On ground
        else if (grounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10, ForceMode.Force);
        //In air
        else if (!grounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10 * airMultiplier, ForceMode.Force);

        // Turn off gravity while on slope
        rb.useGravity = !OnSlope();
    }

    void SpeedControl()
    {
        //Limit speed on slope
        if (OnSlope())
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        //Limit speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Limit the velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

    }

    void Jump()
    {
        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        DoAnimation("Jump");
    }
    void ResetJump()
    {
        readyToJump = true;
    }

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }

    [ServerRpc(RequireOwnership = true)]
    public void DoAnimation(string Name)
    {
        animator.SetTrigger(Name);
        print("Ik doe nu trigger " + Name);
        ObserverAnim(Name);
    }
    [ObserversRpc]
    public void ObserverAnim(string Name)
    {
        animator.SetTrigger(Name);
        print("Ik doe nu trigger " + Name);
    }

    [ServerRpc(RequireOwnership = true)]
    public void DoBlendTree()
    {
        animator.SetFloat("X", horizontalInput);
        animator.SetFloat("Y", verticalInput);
        ObserverTree();
    }
    [ObserversRpc]
    public void ObserverTree()
    {
        animator.SetFloat("X", horizontalInput);
        animator.SetFloat("Y", verticalInput);
    }

}
