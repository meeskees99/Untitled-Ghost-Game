using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using FishNet.Object;

public class MouseLookAdvanced : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] float sens;

    [SerializeField] Transform orientation;
    float xRotation;
    float yRotation;

    bool mouseLocked;

    public Camera cam;

    [SerializeField] StofZuiger stofZuiger;

    [SerializeField] KeyCode use;

    RaycastHit hit;
    [SerializeField] float useRange;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            this.enabled = false;
        }
        else
        {
            cam = FindObjectOfType<Camera>();
            cam.transform.SetParent(transform);
            cam.transform.position = this.transform.position;
            cam.transform.rotation = new Quaternion();
        }
    }
    private void Start()
    {
        //Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(use))
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, useRange))
            {
                if (hit.transform.tag == "Canister")
                {
                    stofZuiger.StorePoints();
                }
            }
        }
        if (PlayerPrefs.HasKey("Mouse Sensitivity"))
        {
            sens = PlayerPrefs.GetFloat("Mouse Sensitivity");
        }
        

        Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        if (currentScene.name == "Game")
        {
            if (mouseLocked)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    mouseLocked = !mouseLocked;
                }
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    mouseLocked = !mouseLocked;
                }
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        


        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}