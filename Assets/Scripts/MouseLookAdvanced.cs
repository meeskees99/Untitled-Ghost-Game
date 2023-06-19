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

    [SerializeField] Camera cam;
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
    bool isLocked = false;
    bool startLock;
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

        print(currentScene.name);

        if (currentScene.name == "Game" || currentScene.name == "FallbackActiveScene")
        {
            print("SceneGame");
            if (!startLock)
            {
                startLock = true;
                print("lock glock on my cock");
                isLocked = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isLocked)
                {
                    print("Toggle Cursor To Lock");
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    isLocked = true;
                }
                else
                {
                    print("Toggle Cursor To Confined");
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    isLocked = false;
                }
            }
        }
        if (!isLocked)
        {
            return;
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
