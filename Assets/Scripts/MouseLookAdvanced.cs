using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class MouseLookAdvanced : NetworkBehaviour
{
    [SerializeField] float sens;

    [SerializeField] Transform orientation;

    float xRotation;
    float yRotation;

    bool mouseLocked = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsHost)
        {
            if (this.gameObject.GetComponent<PlayerData>().playerId != 0)
            {
                this.gameObject.SetActive(false);
            }
        }
        else if(!base.IsOwner)
        {
            this.gameObject.SetActive(false);
        }
       
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
        //if (!base.IsOwner)
        //{
        //    print("print");
        //    this.gameObject.SetActive(false);
        //}

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime* sens;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
