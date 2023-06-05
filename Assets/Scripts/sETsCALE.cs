using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sETsCALE : MonoBehaviour
{
    void Update()
    {
        if (this.GetComponentInParent<GameObject>().name == "Players") 
        {
            transform.localScale = Vector3.one;
        }
    }
}
