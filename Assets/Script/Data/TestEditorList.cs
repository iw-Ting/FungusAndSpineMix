using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(TestScript))]
public class TestEditorList : Editor
{

    // public ReorderableList list;
    // public SerializedProperty mData;
    // // public SerializedObject target;

    // private void OnEnable()
    // {

    //     mData = serializedObject.FindProperty("mData");//陣列
    //     list = new ReorderableList(serializedObject, mData, true, true, true, true);
    //     list.drawElementCallback = DrawItem;
    //     list.drawHeaderCallback = DrawHeader;


    // }


    // void DrawHeader(Rect rect)
    // {
    //     EditorGUI.LabelField(rect, "newTest");
    // }


    // void DrawItem(Rect rect, int index, bool isActive, bool isFocused)
    // {
    //     //Screen.height * EditorGUIUtility.pixelsPerPoint

    //     // EditorGUI EditorGUILayout GUILayout

    //     // Debug.Log("顯示結果==>"+(target as TestScript).mData[index].Name);

    //     Debug.Log("顯示位置==>" + Event.current.mousePosition);
    //     Debug.Log("顯示比例==>" + EditorGUIUtility.pixelsPerPoint);
    //     Debug.Log("顯示高度==>" + EditorGUIUtility.fieldWidth);

    //     //獲取陣列裡的值          
    //     var element = list.serializedProperty.GetArrayElementAtIndex(index);

    //     EditorGUI.PropertyField(
    //         new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight),
    //         element.FindPropertyRelative("role"),
    //         GUIContent.none
    //     );

    //     // Debug.Log(index+"測試==>rect.x==>"+rect.x+"測試==>rect.y==>"+rect.y);

    //     // EditorGUI.LabelField(new Rect(rect.x+120,rect.y,100,EditorGUIUtility.singleLineHeight),"Name");
    //     EditorGUI.LabelField(new Rect(rect.x + 120, rect.y, 100, EditorGUIUtility.singleLineHeight), "Name");



    //     // EditorGUI.PropertyField(new Rect(rect.x+160,rect.y,100,EditorGUIUtility.singleLineHeight),
    //     //     element.FindPropertyRelative("Name"),
    //     //     GUIContent.none);

    //     var name = (target as TestScript).mData[index].Name;

    //     (target as TestScript).mData[index].Name = EditorGUI.TextField(new Rect(rect.x + 160, rect.y, 100, EditorGUIUtility.singleLineHeight), name);

    //     if ((target as TestScript).mData[index].Name == "22")
    //     {
    //         //Debug.Log("成功更改==>" + name);
    //         (target as TestScript).mData[index].Name = "123";
    //     }

    //     EditorGUI.LabelField(new Rect(rect.x + 200, rect.y, 100, EditorGUIUtility.singleLineHeight), "Att");

    //     EditorGUI.PropertyField(
    //         new Rect(rect.x + 250, rect.y, 100, EditorGUIUtility.singleLineHeight),
    //         element.FindPropertyRelative("attackValue"),
    //         GUIContent.none);


    // }

    // public override void OnInspectorGUI()
    // {

    //     base.OnInspectorGUI();
    //     serializedObject.Update();
    //     list.DoLayoutList();//將list的方法應用在inspector上面
    //     serializedObject.ApplyModifiedProperties();//在inspector輸入的數據直接應用在腳本(會儲存更改的數據)

    // }





}
