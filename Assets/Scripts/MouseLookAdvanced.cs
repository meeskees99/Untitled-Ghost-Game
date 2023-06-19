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
    [SerializeField] GameManager gameManager;
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
        gameManager = FindObjectOfType<GameManager>();
    }
    bool isLocked = false;
    // Update is called once per frame
    void Update()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

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
        
        if (currentScene.name == "Game")
        {
            if (!isLocked)
            {
                isLocked = true;
                gameManager.MouseLocked = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!gameManager.MouseLocked)
                {
                    print("Toggle Cursor To Lock");
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    gameManager.MouseLocked = true;
                }
                else
                {
                    print("Toggle Cursor To Confined");
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    gameManager.MouseLocked = false;
                }
            }
            if (gameManager.MouseLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                gameManager.MouseLocked = false;
            }
            if (gameManager.settingsUI.activeSelf)
            {
                return;
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
