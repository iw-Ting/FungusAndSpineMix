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

        Debug.Log("����1=>"+testObj.position);
        Debug.Log("����2=>" + testObj.anchoredPosition);
        //Debug.Log("����2=>" + RectTransformUtility.screen);


    }


}
