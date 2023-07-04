using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSpawner : MonoBehaviour
{
    [SerializeField] GameObject Loader;

    private void Start()
    {
        Instantiate(Loader);

    }
}
