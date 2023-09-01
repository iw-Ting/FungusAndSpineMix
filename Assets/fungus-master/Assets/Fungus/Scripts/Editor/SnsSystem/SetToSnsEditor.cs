using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Fungus.EditorUtils
{




    [CustomEditor(typeof(SetToSns))]
    public class SetToSnsEditor : CommandEditor
    {
        private SerializedProperty SnsProp;

        public override void OnEnable()
        {
            SnsProp = serializedObject.FindProperty("sns");
           // base.OnEnable();
        }

        public override void DrawCommandGUI()
        {
           
            SetToSns sns= (SetToSns)target;

            EditorGUILayout.PropertyField(SnsProp.FindPropertyRelative("mChara"));

            SerializedProperty messageInfo = SnsProp.FindPropertyRelative("mMessageType");

            EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_snsType"));
            serializedObject.ApplyModifiedProperties();

            switch (sns.sns.mMessageType._snsType)
            {
                case SnsManager.SnsType.Message:
                    EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_message"));
                    break;
                case SnsManager.SnsType.Reply:
                    EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_replyMessage"));
                    break;
                case SnsManager.SnsType.Image:
                    EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_image"));
                    break;
            }
        }

    }





}
