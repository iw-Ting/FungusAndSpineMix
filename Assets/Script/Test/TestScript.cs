using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fungus;

public class TestScript : MonoBehaviour
{


    public void Start()
    {
        StartCoroutine(test());
    }


    public IEnumerator test()
    {
        bool input = false;
        while (!input)
        {
            if (Input.GetKeyDown(KeyCode.A)) {
            
            input = true;
                Debug.Log("���\�F we did it");
            }
            else
            {
                yield return null;
            }

        }



    }


    


    public void Update()
    {



    }
}
