using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fungus.EditorUtils;



namespace Fungus
{

    [CustomEditor(typeof(CharaSpine))]
    public class CharaSpineEditor : Editor
    {

        public CharaSpine mCharaSpine;
        public SerializedProperty SpineSettingPro, SkeletonGraphicPro;//骨架陣列 
        public SerializedProperty SkeletonSkin, SkeletonAni;

        public void OnEnable()
        {

            Debug.Log("OnEnable CharaSpineEditor");
            SpineSettingPro = serializedObject.FindProperty("mSet");
            SkeletonGraphicPro = serializedObject.FindProperty("aSkeletonGraphic");
            SkeletonSkin = serializedObject.FindProperty("DefaultSkin");
            SkeletonAni = serializedObject.FindProperty("DefaultAni");
            mCharaSpine = target as CharaSpine;

        }
        public override void OnInspectorGUI()
        {
            // EditorGUI.PropertyField(MainSkeletonData);


            EditorGUILayout.PropertyField(SkeletonGraphicPro);

            EditorGUILayout.PropertyField(SpineSettingPro);

            CommandEditor.StringField(SkeletonSkin,
                                    new GUIContent("Skin", "Change representing Skin"),
                                    new GUIContent("<None>"),
                                    mCharaSpine.aSkeletonGraphic.GetSkinStrings());

            CommandEditor.StringField(SkeletonAni,
                                    new GUIContent("Ani", "Change representing Ani"),
                                    new GUIContent("<None>"),
                                    mCharaSpine.aSkeletonGraphic.GetSkeletonStrings());
            




            serializedObject.ApplyModifiedProperties();




        }





    }




































}
