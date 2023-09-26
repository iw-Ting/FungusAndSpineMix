using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Unity.EditorCoroutines.Editor;
using System;
using UnityEditor;

public class testPro : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(StartCountDown());
    }

    public void TouchButton()
    {
        // EditorCoroutineUtility.StartCoroutineOwnerless(StartTest());
        //   EditorCoroutineUtility.StartCoroutine(StartTest(), this);
        //StartCoroutine(StartTest());


       // EditorCoroutineUtility.StartCoroutineOwnerless(StartCountDown());
           //EditorCoroutineUtility.StartCoroutine(StartCountDown(), this);
        StartCoroutine(StartCountDown());

    }

    public IEnumerator StartCountDown()
    {
        int num = 0;
        int finish = 0;

        while (num<5) {
        num++;

            EditorCoroutineUtility.StartCoroutineOwnerless(StartTest(num.ToString(), () => { finish++; }));
          //  StartCoroutine(StartTest(num.ToString(), () => { finish++; }));
        }

        yield return new WaitUntil(() =>finish>=5);
        Debug.Log("全部結束");

    }


    public IEnumerator StartTest(string str,Action cb)
    {
        int calcDown=0;
        while(calcDown<5)
        {
            calcDown++;
            Debug.Log("倒數計時機器"+str+"號=>"+calcDown);
            yield return new WaitForSeconds(2);
        }
        cb();

    }

}

[CustomEditor ( typeof(testPro) ) ] 
public class testProEditor:Editor
{
    public testPro tar;

    public void OnEnable()
    {
        tar = target as testPro;
    }
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("touch")) {

            tar.TouchButton();
        
        
        }
    }








}
