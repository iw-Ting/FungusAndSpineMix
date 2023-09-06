using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fungus;
using UnityEditor;
using System;


[Serializable]
public class Test 
{

    public string aaa = "";

    public string bbb = "";

    public string ccc = "";

}


[Serializable]
public class TestScript 
{

    public string aaa = "";

    public string bbb = "";

    public string ccc= "";

}

[CustomEditor(typeof( TestScript))]
public class TestScriptEditor : Editor
{
    SerializedProperty aaaProp,bbbProp,cccProp;


    public  void OnEnable()
    {

        Debug.Log("­º¥ý±Ò°Ê");
        aaaProp = serializedObject.FindProperty("aaa");
        bbbProp = serializedObject.FindProperty("bbb");
        cccProp = serializedObject.FindProperty("ccc");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(aaaProp);

    }



}