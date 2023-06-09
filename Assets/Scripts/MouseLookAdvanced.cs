using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class MouseLookAdvanced : NetworkBehaviour
{
    [Header("Settings")]
    [Tooltip("Range you can suck up ghosts from")]
    [SerializeField] float suckRange;

    [SerializeField] float sens;
    [SerializeField] PlayerData playerData;

    [SerializeField] Transform orientation;
    float xRotation;
    float yRotation;

    bool mouseLocked = true;

    public Camera cam;

    [SerializeField] LayerMask mask;
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

    // Update is called once per frame
    void Update()
    {
        sens = PlayerPrefs.GetFloat("Mouse Sensitivity");
        if (mouseLocked)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                mouseLocked = !mouseLocked;
                Cursor.visible = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.Locked;
                mouseLocked = !mouseLocked;
                Cursor.visible = false;
            }
        }

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime* sens;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}