using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;
using UnityEngine.Serialization;

namespace Fungus
{

    public enum DisplayEffect
    {
        None,
        Fade,
        Move
    }
    [Serializable]
    public class MenuImageOption: PropertyAttribute
    {
        public Sprite _image=null;
        public Vector2 imageSize = Vector2.zero;
        [FormerlySerializedAs("targetSequence")]
        public Block targetBlock;
        public InputCallBack.InputOptions _options;


    }
    [CommandInfo("Narrative",
             "Menu Image",
             "Displays a button in a multiple Image choice menu")]
    [AddComponentMenu("")]

    public class MenuImage : Command
    {

        //private static List<MenuImageOption>

        [SerializeField ] private MenuImageOption Options;

        public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
        {
            if (Options.targetBlock != null)
            {
                connectedBlocks.Add(Options.targetBlock);
            }
        }


        public override void OnEnter()//Cammand執行邏輯
        {

            if (Options._options.pos == null)
            {
                return;
            }

            if (Options._options.touchSize == Vector2.zero)
            {
                Options._options.touchSize = new Vector2(300, 300);
            }



           // Block block=Block.FindBlockByName()
         InputCallBack.GetInputCallBack().CreateMenuImage(Options);

           Continue();
        
        }


        }

    /*[CustomPropertyDrawer(typeof(MenuImageOption))]
    public class MenuImageOptionAttribute : PropertyDrawer
    {

        private ReorderableList _list;

        private ReorderableList GetReorderableList(SerializedProperty property)
        {
            if (_list == null)
            {
                var listProperty = property.FindPropertyRelative("List");

                _list = new ReorderableList(property.serializedObject, listProperty, true, true, true, true);

                _list.drawHeaderCallback += delegate (Rect rect)
                {
                    EditorGUI.LabelField(rect, property.displayName);
                };

                _list.drawElementCallback = delegate (Rect rect, int index, bool isActive, bool isFocused)
                {
                    EditorGUI.PropertyField(rect, listProperty.GetArrayElementAtIndex(index), true);
                };
            }

            return _list;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetReorderableList(property).GetHeight();
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var list = GetReorderableList(property);

            var listProperty = property.FindPropertyRelative("List");
            var height = 0f;
            for (var i = 0; i < listProperty.arraySize; i++)
            {
                height = Mathf.Max(height, EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
            }

            list.elementHeight = height;
            list.DoList(position);
        }
     




        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

                
            Rect rect = new Rect(position.position.x, position.position.y, position.width,position.height+30);


            Debug.Log("現在屬性=>" + property.type);

            EditorGUI.PropertyField(rect, property);


            //EditorGUI.PropertyField(rect, property.FindPropertyRelative("_options").FindPropertyRelative("pos"));

            // EditorGUILayout.PropertyField( property.FindPropertyRelative("_options"));


            EditorGUI.EndProperty();
            // base.OnGUI(position, property, label);
            
        }



    }*/



}
