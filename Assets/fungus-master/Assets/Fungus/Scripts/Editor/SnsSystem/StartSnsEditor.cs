using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Fungus.EditorUtils
{

    [CustomEditor(typeof(StartSns))]
    public class StartSnsEditor : CommandEditor
    {

        private SerializedProperty DialogNameProp;
        private SerializedProperty DialogCharaProp;
        private SerializedProperty HistorySnsProp;
        private ReorderableList _roleList;


        public override void OnEnable()
        {
            DialogNameProp = serializedObject.FindProperty("DialogWindowName");
            DialogCharaProp= serializedObject.FindProperty("DialogChara");
            HistorySnsProp= serializedObject.FindProperty("HistorySns");

            _roleList = new ReorderableList(serializedObject, HistorySnsProp
            , true, true, true, true);

        }

        public override void DrawCommandGUI()
        {

            EditorGUILayout.PropertyField(DialogNameProp);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(DialogCharaProp);
            if (EditorGUI.EndChangeCheck()) { 
            
            
            }

            //DialogNameProp.arr


            EditorGUILayout.PropertyField(_roleList.serializedProperty);
            //_roleList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();


        }

















    }
}
