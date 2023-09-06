using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Fungus.EditorUtils {


    [CustomPropertyDrawer(typeof(CharaDropOptions))]
    public class SnsCharaEditor:PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 40f;

        }

       

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            //EditorUtility.SetDirty(property.serializedObject.targetObject); // Repaint
            //property.serializedObject.ApplyModifiedProperties();
            //�S��s���� property�|�@�������ª�value
            //  property.serializedObject.Update();


            SerializedProperty name = property.FindPropertyRelative("mName");
                SerializedProperty Charas = property.FindPropertyRelative("Charas");
                
                GUIContent nullLabel = new GUIContent("None");


                List<string> objectList = new List<string>();

               // Debug.Log("�p�⨤��ƶq=>"+Charas.arraySize);
               
                if (Charas.arraySize>0) {

                    for (int i=0;i< Charas.arraySize;i++) {
                    
                        SerializedProperty element = Charas.GetArrayElementAtIndex(i);
                      //  Debug.Log("������5?"+element.propertyType);
                        Character charaSetting = element.FindPropertyRelative("mFungusChara").objectReferenceValue as  Character;
                    if (charaSetting) {
                        objectList.Add(charaSetting.NameText);
                    }
                    else
                    {
                    //    Debug.Log("Dont Setting Chara Item");
                        return;
                    }
                    }
                }
                else
                {
                    Debug.Log("�䤣�쨤��");
                    return;
                }
                List<GUIContent> objectNames = new List<GUIContent>();

                string selectedObject = name.stringValue;

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
            //GUILayout.Width(400), GUILayout.Height(300)


            EditorGUI.LabelField(new Rect(position.x, position.y -10, position.width, position.height ), label);
            Rect optionRectPos=new Rect(position.x, position.y+18, position.width, position.height-18); 
                selectedIndex = EditorGUI.Popup(optionRectPos, selectedIndex, objectNames.ToArray());

                if (selectedIndex <= 0)
                {
                    // Currently selected object is not in list, but nothing else was selected so no change.

                    name.stringValue = null;
                    // return;
                }
                else
                {

                    name.stringValue = objectList[selectedIndex - 1];
                }

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        
    }



}
