using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFov : MonoBehaviour
{
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = PlayerPrefs.GetInt("fov");
    }
}
