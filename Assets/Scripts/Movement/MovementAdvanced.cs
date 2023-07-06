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
    public List<GameObject> gunLights = new();
    public List<GameObject> tankLights = new();

    [SyncVar] bool canSteal;

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
        set
        {
            isStunned = value;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetCanSteal(bool value)
    {
        canSteal = value;
    }
    public bool GetCanSteal()
    {
        return canSteal;
    }


    [SerializeField] float stunTime = 5f;

    [SerializeField] float raycastLenght = 0.5f;

    [SerializeField] bool wallWalk;

    [SerializeField] StofZuiger stofZuiger;

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
            return;
        }
        else if (!IsOwner)
        {
            this.enabled = false;
            return;
        }

        int babaoooo = PlayerPrefs.GetInt("Character");
        CharInt(babaoooo);
    }
    [SyncVar] bool charIntSet;
    [ServerRpc(RequireOwnership = false)]
    void CharInt(int charint)
    {
        characterIndex = charint;
        charIntSet = true;
    }

    [ServerRpc(RequireOwnership = false)]
    void SetChar(int index)
    {
        for (int i = 0; i < character.Length; i++)
        {
            if (i == index)
            {
                character[i].SetActive(true);
            }
            else
            {
                character[i].SetActive(false);
            }
        }


        animator = character[index].GetComponent<Animator>();
        stofZuiger.animator = character[index].GetComponent<Animator>();

        gunLights.Add(character[index].transform.GetChild(1).gameObject);
        gunLights.Add(character[index].transform.GetChild(2).gameObject);
        gunLights.Add(character[index].transform.GetChild(3).gameObject);

        tankLights.Add(character[index].transform.GetChild(5).gameObject);
        tankLights.Add(character[index].transform.GetChild(6).gameObject);
        tankLights.Add(character[index].transform.GetChild(7).gameObject);
        SetCharObserver(index);
    }
    [ObserversRpc(BufferLast = true)]
    void SetCharObserver(int index)
    {
        for (int i = 0; i < character.Length; i++)
        {
            if (i == index)
            {
                character[i].SetActive(true);
            }
            else
            {
                character[i].SetActive(false);
            }
        }
        animator = character[index].GetComponent<Animator>();
        stofZuiger.animator = character[index].GetComponent<Animator>();
        if (IsHost)
            return;
        gunLights.Add(character[index].transform.GetChild(1).gameObject);
        gunLights.Add(character[index].transform.GetChild(2).gameObject);
        gunLights.Add(character[index].transform.GetChild(3).gameObject);

        tankLights.Add(character[index].transform.GetChild(5).gameObject);
        tankLights.Add(character[index].transform.GetChild(6).gameObject);
        tankLights.Add(character[index].transform.GetChild(7).gameObject);
    }

    bool charSet;
    private void Update()
    {
        if (!charSet && charIntSet)
        {
            charSet = true;
            SetChar(characterIndex);
        }
        if (animator == null)
        {
            return;
        }
        if (isStunned)
            return;
        //speedTxt.text = "Speed: " + rb.velocity.magnitude.ToString("0.##");
        // Ground Check
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

        SetTankValue(stofZuiger.GhostPoints);
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
        SetCanSteal(false);
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
        if (animator == null)
        {
            return;
        }
        animator.SetTrigger(Name);
        print("Ik doe nu trigger " + Name);
        ObserverAnim(Name);
    }
    [ObserversRpc]
    public void ObserverAnim(string Name)
    {
        if (animator == null)
        {
            return;
        }
        if (IsHost)
            return;
        animator.SetTrigger(Name);
        print("Ik doe nu trigger " + Name);
    }

    [ServerRpc(RequireOwnership = true)]
    public void DoBlendTree(float hor, float ver)
    {
        if (animator == null)
        {
            return;
        }
        ObserverTree(hor, ver);
    }
    [ObserversRpc]
    public void ObserverTree(float hor, float ver)
    {
        if (animator == null)
        {
            return;
        }
        animator.SetFloat("X", hor);
        animator.SetFloat("Y", ver);
    }
    [ServerRpc(RequireOwnership = true)]
    public void SetBoolAnim(string s, bool b)
    {
        if (animator == null)
        {
            return;
        }
        animator.SetBool(s, b);
        SetBoolObserver(s, b);
    }
    [ObserversRpc]
    public void SetBoolObserver(string s, bool b)
    {
        if (animator == null)
        {
            return;
        }
        if (IsHost)
            return;

        animator.SetBool(s, b);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetTankValue(int value)
    {
        for (int i = 0; i < value; i++)
        {
            gunLights[i].SetActive(true);
            tankLights[i].SetActive(true);
        }
        for (int y = 2; y > value - 1; y--)
        {
            gunLights[y].SetActive(false);
            tankLights[y].SetActive(false);
        }
    }

}
