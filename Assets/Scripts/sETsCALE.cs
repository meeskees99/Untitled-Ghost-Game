using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sETsCALE : MonoBehaviour
{
    void Update()
    {
        if (this.GetComponentInParent<RectTransform>().gameObject.name == "Players") 
        {
            this.transform.localScale = Vector3.one;
            print("AAS");
        }
    }
}
