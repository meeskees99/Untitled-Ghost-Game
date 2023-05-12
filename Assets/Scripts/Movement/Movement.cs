using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float sprintMultiplier = 1;
    [SerializeField] int maxJumps = 1;
    [SerializeField] float jumpHeight = 50;
    Vector3 pos;
    float moveHor;
    float moveVert;
    Rigidbody rb;
    int jumpCount = 1;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveHor = Input.GetAxis("Horizontal");
        pos.x = moveHor;
        moveVert = Input.GetAxis("Vertical");
        pos.z = moveVert;
        transform.Translate(pos * movementSpeed * sprintMultiplier * Time.deltaTime);

        if(jumpCount > 0)
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.AddForce(0, jumpHeight, 0, ForceMode.VelocityChange);
                jumpCount--;
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Floor")) {
            jumpCount = maxJumps;
        }
    }
}
