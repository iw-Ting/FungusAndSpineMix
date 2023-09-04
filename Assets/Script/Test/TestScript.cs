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
        Debug.Log("ด๚ธี=>" +(int) asd);
        Debug.Log("ด๚ธี=>"+(int)Asd.dd);
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
