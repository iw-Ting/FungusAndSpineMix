using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fungus;

public class TestScript : MonoBehaviour
{

    public Asd asd=Asd.bb;


    public void Start()
    {
        Debug.Log("����=>" +(int) asd);
        Debug.Log("����=>"+(int)Asd.dd);
    }

    public void Update()
    {

    }

}

public enum Asd
{
    aa,
    bb,
    cc=5,
    dd


}
