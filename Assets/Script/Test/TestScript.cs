using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    //    public List<TestData> mData=new List<TestData>();

    // [HideInInspector]public TestData[] mData;
    public string myText = "";
    public string detect = "";

    public char qwe;

    public void Start()
    {

    }


    public void test()
    {
        // if (myText.StartsWith(detect))
        // {
        //     Debug.Log("start有該文字");
        // }

        // if (myText.EndsWith(detect))
        // {
        //     Debug.Log("end有該文字");
        // }

       myText=myText.Trim(qwe);

       myText.Split("asd");

        Debug.Log("測試==>"+myText);
        //   yield return null;
    }


    public void Update()
    {

        test();

    }
}
