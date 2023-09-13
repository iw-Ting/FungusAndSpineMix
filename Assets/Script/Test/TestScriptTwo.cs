using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TestScriptTwo : MonoBehaviour
{


    public RectTransform testObj;


    private void OnDrawGizmos()
    {
        if (testObj==null) {
            return;
        
        }

        Debug.Log("ด๚ธี1=>"+testObj.position);
        Debug.Log("ด๚ธี2=>" + testObj.anchoredPosition);
        //Debug.Log("ด๚ธี2=>" + RectTransformUtility.screen);


    }


}
