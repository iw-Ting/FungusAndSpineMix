using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testPro : MonoBehaviour
{
    // Start is called before the first frame update


    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnGUI(){


        Debug.Log("測試==>"+Event.current.mousePosition);
    }
}
