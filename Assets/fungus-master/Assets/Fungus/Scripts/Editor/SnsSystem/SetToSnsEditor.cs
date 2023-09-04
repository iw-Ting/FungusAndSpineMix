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
            SerializedProperty messageInfo = SnsProp.FindPropertyRelative("mMessageType");

            //SnsManager snsWindow = Flowchart.GetInstance().mStoryControl.LogWindowPopupParent.transform.Find("SnsSystem").GetComponent<SnsManager>();
            List < SnsManager.CharaSnsSetting > charaList= StartSns.GetInstance().DialogChara;

            EditorGUILayout.PropertyField(SnsProp.FindPropertyRelative("mChara"));


           // EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_snsType"));//根據不同角色





            foreach (var chara in charaList)
            {
                if (chara.mFungusChara.NameText==sns.sns.mChara.mName) {
                    List<string> displaySnsType = new List<string>();

  

                    switch (chara.mCharaRole)
                    {

                        case SnsManager.CharaRole.self:

                            displaySnsType.Add("Message");
                            displaySnsType.Add("Reply");
                            displaySnsType.Add("Image");
                            CommandEditor.EnumField<SnsManager.SnsType>(
                                messageInfo.FindPropertyRelative("_snsType"),
                                new GUIContent("Skin", "Change representing Skin"),
                                SnsManager.SnsType.Message,
                                 displaySnsType); 

                                    serializedObject.ApplyModifiedProperties();
                            switch (sns.sns.mMessageType._snsType)
                            {
                                case SnsManager.SnsType.Message:
                                     EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_message"));
                                    EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_dialogWaitTime"));
                                    break;
                                case SnsManager.SnsType.Reply:

                                    EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_replyMessage"));
                                    break;
                                case SnsManager.SnsType.Image:

                                    EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_sprite"));
                                    break;
                            }
                            break;
                        case SnsManager.CharaRole.otherSide:

                            displaySnsType.Add("Message");
                            displaySnsType.Add("Image");
                            CommandEditor.EnumField<SnsManager.SnsType>(
                                messageInfo.FindPropertyRelative("_snsType"),
                                new GUIContent("Skin", "Change representing Skin"),
                                SnsManager.SnsType.Message,
                                 displaySnsType); 

                              serializedObject.ApplyModifiedProperties();
                            switch (sns.sns.mMessageType._snsType)
                            {
                                case SnsManager.SnsType.Message:
                                    EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_message"));
                                    EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_dialogWaitTime"));
                                    break;
                                case SnsManager.SnsType.Image:
                                    EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_sprite"));
                                    EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_dialogWaitTime"));
                                    break;
                            }
                            break;

                    }
                }
                //根據不同的角色定位,給予不同的選項
            }

          /*  switch (sns.sns.mMessageType._snsType)
            {
                case SnsManager.SnsType.Message:
                    EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_message"));
                    break;
                case SnsManager.SnsType.Reply:
                    EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_replyMessage"));
                    break;
                case SnsManager.SnsType.Image:
                    EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_sprite"));
                    break;
            }*/


        }

    }





}
