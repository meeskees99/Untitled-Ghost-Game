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
    [SerializeField] PlayerData pData;
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
    bool hasFov;
    void Update()
    {
        if (cam != null && !hasFov)
        {
            hasFov = true;
            cam.fieldOfView = PlayerPrefs.GetInt("fov");
        }
        if (Input.GetKeyDown(use))
        {
            if (pData.teamID == 0)
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, useRange))
                {
                    if (hit.transform.tag == "Canister")
                    {
                        stofZuiger.StorePoints();
                    }
                    else if (hit.transform.tag == "Player" && hit.transform.GetComponent<PlayerData>().teamID == 1)
                    {
                        print("hitPlayer");
                        if (hit.transform.GetComponent<MovementAdvanced>().IsStunned && hit.transform.GetComponent<MovementAdvanced>().GetCanSteal())
                        {
                            hit.transform.GetComponent<MovementAdvanced>().SetCanSteal(false);
                            print("isStunned");
                            print(hit.transform.GetChild(1).GetChild(0));
                            stofZuiger.StealPoints(hit.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<StofZuiger>().GhostPoints, hit.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<StofZuiger>());
                        }
                    }
                    else if (hit.transform.tag == "Door")
                    {
                        SetBoolAnim(hit.transform);
                    }
                }
            }
            else
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, useRange))
                {
                    if (hit.transform.tag == "Canister2")
                    {
                        stofZuiger.StorePoints();
                    }
                    else if (hit.transform.tag == "Player" && hit.transform.GetComponent<PlayerData>().teamID == 0)
                    {
                        print("hitPlayer");
                        if (hit.transform.GetComponent<MovementAdvanced>().IsStunned && hit.transform.GetComponent<MovementAdvanced>().GetCanSteal())
                        {
                            hit.transform.GetComponent<MovementAdvanced>().SetCanSteal(false);
                            print("isStunned");
                            print(hit.transform.GetChild(1).GetChild(0));
                            stofZuiger.StealPoints(hit.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<StofZuiger>().GhostPoints, hit.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<StofZuiger>());
                        }
                    }
                    else if (hit.transform.tag == "Door")
                    {
                        SetBoolAnim(hit.transform);
                    }
                }
            }

        }
        if (PlayerPrefs.HasKey("Mouse Sensitivity"))
        {
            sens = PlayerPrefs.GetFloat("Mouse Sensitivity");
        }
        if (!GameManager.MouseLocked)
        {
            print("AAAAAAAAAAAAAAAAAAAAAA");
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
    [ServerRpc(RequireOwnership = true)]
    public void SetBoolAnim(Transform hit)
    {
        print("server door");
        hit.GetComponent<Animator>().SetTrigger("Toggle");
        SetBoolObserver(hit);
    }
    [ObserversRpc]
    public void SetBoolObserver(Transform hit)
    {
        if (IsHost)
            return;
        print("observer door");

        hit.GetComponent<Animator>().SetTrigger("Toggle");
    }
}
