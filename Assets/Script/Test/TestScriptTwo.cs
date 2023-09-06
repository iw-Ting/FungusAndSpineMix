using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TestScriptTwo : MonoBehaviour
{

    public List<string> aa = new List<string>();
    public List<string> bb = new List<string>();

    public void Start()
    {
        bb = new List<string>(aa);
    }


}
