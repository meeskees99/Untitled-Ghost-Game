using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sETsCALE : MonoBehaviour
{
    RectTransform parent;
    RectTransform me;
    void Update()
    {
        parent = GetComponentInParent<RectTransform>();
        if (parent.gameObject.name == "Players") 
        {
            me = GetComponent<RectTransform>();
            me.sizeDelta = Vector2.one;
            print("AAS");
        }
    }
}
