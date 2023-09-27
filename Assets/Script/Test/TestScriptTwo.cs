using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TestScriptTwo : MonoBehaviour
{

    public GameObject obj = null;

    public void Start()
    {
        obj = GameObject.Find("_CommandCopyBuffer");
        Destroy(obj);

    }


}
