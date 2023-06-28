using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Object.Synchronizing;

public class MovementAdvanced : NetworkBehaviour
{
    [Header("Movement")]
    float moveSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float groundDrag;
    [SerializeField] StofZuiger stofZuiger;
    [SerializeField] Animator animator;

    [Header("Jumping")]
    [Tooltip("How high the player will jump")]
    [SerializeField] float jumpForce;
    [Tooltip("The time a player has to wait before being able to jump again")]
    [SerializeField] float jumpCooldown;
    [Tooltip("Speed will multiply by this whilst airborn")]
    [SerializeField] float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] bool grounded;

    [Header("Slope Check")]
    [SerializeField] float maxSlopeAngle;
    RaycastHit slopeHit;

    [Header("Character")]
    public GameObject[] character;
    [SyncVar] public int characterIndex;


    [SerializeField] Transform orientation;
    [SerializeField] TMP_Text speedTxt;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDir;

    Rigidbody rb;

    [SyncVar][SerializeField] bool isStunned;
    public bool IsStunned
    {
        get
        {
            return isStunned;
        }
    }
    [SerializeField] float stunTime = 5f;

    [SerializeField] float raycastLenght = 0.5f;

    [SerializeField] bool wallWalk;


    public MovementState state;
    public enum MovementState
    {
        walk, run, air
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        readyToJump = true;
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
        // else if (IsOwner)
        // {
        // }
        CharInt(PlayerPrefs.GetInt("Character"));

    }
    [ServerRpc(RequireOwnership = false)]
    void CharInt(int charInt)
    {
        characterIndex = charInt;
        charSet = true;
    }
    [SyncVar] public bool charSet;
    [ServerRpc(RequireOwnership = false)]
    void CharSet(bool value)
    {
        charSet = value;
    }
    private void Update()
    {
        if (charSet)
        {
            character[characterIndex].SetActive(true);
            animator = character[characterIndex].GetComponent<Animator>();
            stofZuiger.animator = character[characterIndex].GetComponent<Animator>();
            CharSet(false);
        }
        if (animator == null)
        {
            return;
        }
        if (isStunned)
            return;
        //speedTxt.text = "Speed: " + rb.velocity.magnitude.ToString("0.##");

        grounded = Physics.Raycast(transform.position, Vector3.down, 0.01f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        SetBoolAnim("OnGround", grounded);

        // Handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (isStunned)
            return;
        MovePlayer();
    }
    [ServerRpc(RequireOwnership = false)]
    public void Stun()
    {
        if (isStunned)
            return;
        isStunned = true;
        print("stun");
        StartCoroutine(ResetStun());
    }
    public IEnumerator ResetStun()
    {
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        DoBlendTree(horizontalInput, verticalInput);

        if (horizontalInput != 0 || verticalInput != 0)
        {
            SetBoolAnim("IsWalking", true);
        }
        else
        {
            SetBoolAnim("IsWalking", false);
        }

        //When To Jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            SetBoolAnim("Jump", true);
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        else if (!Input.GetKey(jumpKey))
        {
            SetBoolAnim("Jump", false);
        }
    }

    void StateHandler()
    {
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
    [SerializeField] Transform shootPos;
    void MovePlayer()
    {
        // Calculalte move direction
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        RaycastHit hit;
        Debug.DrawRay(shootPos.position, moveDir);
        if (Physics.Raycast(shootPos.position, moveDir, out hit, raycastLenght) && hit.transform.tag != "SuckBox")
        {
            wallWalk = true;
        }
        else
        {
            wallWalk = false;
        }
        if (wallWalk)
        {
            rb.AddForce(Vector3.down * 10f, ForceMode.Force);
            return;
        }
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
        if (IsHost)
            return;
        animator.SetTrigger(Name);
        print("Ik doe nu trigger " + Name);
    }

    [ServerRpc(RequireOwnership = true)]
    public void DoBlendTree(float hor, float ver)
    {
        ObserverTree(hor, ver);
    }
    [ObserversRpc]
    public void ObserverTree(float hor, float ver)
    {
        animator.SetFloat("X", hor);
        animator.SetFloat("Y", ver);
    }
    [ServerRpc(RequireOwnership = true)]
    public void SetBoolAnim(string s, bool b)
    {
        animator.SetBool(s, b);
        SetBoolObserver(s, b);
    }
    [ObserversRpc]
    public void SetBoolObserver(string s, bool b)
    {
        if (IsHost)
            return;

        animator.SetBool(s, b);
    }

}
