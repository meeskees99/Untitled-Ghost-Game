using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sETsCALE : MonoBehaviour
{
    public RectTransform parent;
    RectTransform me;
    private void Update()
    {
        parent = this.GetComponent<RectTransform>();
        parent.localScale = new Vector3(1, 1, 1);
        parent.rotation = Quaternion.identity;
    }
}
