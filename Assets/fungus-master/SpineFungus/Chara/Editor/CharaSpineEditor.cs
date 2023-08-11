using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fungus.EditorUtils;
using Spine.Unity;



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

        }
        public override void OnInspectorGUI()
        {
            // EditorGUI.PropertyField(MainSkeletonData);
            EditorGUILayout.PropertyField(SkeletonGraphicPro);
            mCharaSpine = target as CharaSpine;

            if (SkeletonGraphicPro.objectReferenceValue==null) {
                EditorGUILayout.HelpBox(new GUIContent("Can Not Have a SkeletonGraphic"));
                serializedObject.ApplyModifiedProperties();
                return;
            }

            SkeletonGraphic aSkele = SkeletonGraphicPro.objectReferenceValue as SkeletonGraphic;


            if (aSkele.skeletonDataAsset==null) {
                EditorGUILayout.HelpBox(new GUIContent("Can Not Have a skeletonDataAsset"));
                serializedObject.ApplyModifiedProperties();
                return;
            }
            else
            {


                EditorGUILayout.PropertyField(SpineSettingPro);

  

                CommandEditor.StringField(SkeletonSkin,
                                        new GUIContent("Skin", "Change representing Skin"),
                                        new GUIContent("<None>"),
                                        aSkele.GetSkinStrings());

                CommandEditor.StringField(SkeletonAni,
                                        new GUIContent("Ani", "Change representing Ani"),
                                        new GUIContent("<None>"),
                                        aSkele.GetSkeletonStrings());
            }


           

            serializedObject.ApplyModifiedProperties();




        }





    }




































}
