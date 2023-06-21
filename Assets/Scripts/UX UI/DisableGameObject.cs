using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGameObject : MonoBehaviour
{
    public void DisableObject(){
        gameObject.SetActive(false);
    }
}
