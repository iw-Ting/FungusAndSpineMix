using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fungus;

public class TestScript : MonoBehaviour
{

    public float updateTime,WhileTime;


    public void Start()
    {
        StartCoroutine(CalcTimeFromWhile());
    }

    public void Update()
    {
        if (updateTime<5) {
            updateTime += Time.deltaTime;
        }
    }


    public IEnumerator CalcTimeFromWhile()
    {
        while (updateTime<5) {

            WhileTime+=Time.deltaTime;
        yield return null;
        
        }


    }


    public IEnumerator test()
    {
        bool input = false;
        while (!input)
        {
            if (Input.GetKeyDown(KeyCode.A)) {
            
            input = true;
                Debug.Log("жие\дF we did it");
            }
            else
            {
                yield return null;
            }

        }



    }


    



}
