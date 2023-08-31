using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Fungus.EditorUtils {

    [CustomEditor(typeof( SnsManager))]
    public class SnsManagerEditor : Editor
    {
        // Start is called before the first frame update
        public ReorderableList _rolelist;//°}¦Cªº¤¸¯À




    }
    /*[CustomPropertyDrawer(typeof(SnsMessagePropAttribute))]
    public class SnsMessageEditor : PropertyDrawer
    {
        // Start is called before the first frame update

        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {

            return 50f;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {


            // EditorGUI.PropertyField(position, property, label);

            //aaa=(SnsManager.SnsType)Enum.Parse(typeof( SnsManager.SnsType),Enum.GetValues) 


            var enumInt = property.FindPropertyRelative("_snsType").enumValueIndex;

            var enumArr = Enum.GetNames(typeof(SnsManager.SnsType));

            var enumValue =(SnsManager.SnsType) Enum.Parse(typeof(SnsManager.SnsType), enumArr[enumInt]);

           var EnumResult=EditorGUI.EnumPopup(position, enumValue);

            var selectInt = 0;

            foreach( var val in enumArr)
            {
                if (val==EnumResult.ToString()) {
                    break;
                }
                selectInt++;
            }

            property.FindPropertyRelative("_snsType").enumValueIndex =selectInt;

            Rect posTwo=new Rect(position.x, position.y+20, position.width, position.height);

            //property.FindPropertyRelative("_snsType").stringValue = EditorGUI.TextField(posTwo, property.FindPropertyRelative("_snsType").stringValue);

             EditorGUI.TextField(posTwo, "");



        }

        }*/




    [CustomPropertyDrawer(typeof(CharaDropOptions))]
    public class SnsCharaEditor : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                GUIContent nullLabel = new GUIContent("None");
                // List<string> objectList = SnsManager.GetSnsManager().GetCharacterArray();

                List<string> objectList=new List<string>();

                if (SnsManager.GetSnsManager()) {
                    objectList = SnsManager.GetSnsManager().GetCharacterArray();
                }



                List<GUIContent> objectNames = new List<GUIContent>();

                string selectedObject = property.stringValue;

                int selectedIndex = -1; // Invalid index

                // First option in list is <None>
                objectNames.Add(nullLabel);



                if (string.IsNullOrEmpty(selectedObject))
                {
                    selectedIndex = 0;
                }

                for (int i = 0; i < objectList.Count; ++i)
                {
                    if (objectList[i] == null) continue;
                    objectNames.Add(new GUIContent(objectList[i]));

                    if (selectedObject == objectList[i])
                    {
                        selectedIndex = i + 1;
                    }
                }

                selectedIndex = EditorGUI.Popup(position, selectedIndex, objectNames.ToArray());

                if (selectedIndex <= 0)
                {
                    // Currently selected object is not in list, but nothing else was selected so no change.

                    property.stringValue = null;
                    // return;
                }
                else
                {

                    property.stringValue = objectList[selectedIndex - 1];
                }

            }

        }
    }








}
