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
        private StartSns mTarget; 
        private SerializedProperty DialogNameProp;
        private SerializedProperty DialogCharaProp;
        private SerializedProperty HistorySnsProp;

        private SerializedProperty test1;
        private SerializedProperty test2;
        // private ReorderableList _roleList;
        private Vector2 scrollVec2;

        public override void OnEnable()
        {

            DialogNameProp = serializedObject.FindProperty("DialogWindowName");
            DialogCharaProp = serializedObject.FindProperty("DialogChara");
            HistorySnsProp = serializedObject.FindProperty("HistorySns");

            test1 = serializedObject.FindProperty("str");
            test2 = serializedObject.FindProperty("strArr");

            mTarget = (StartSns)target;
            //  DialogNameProp = serializedObject.FindProperty("DialogWindowName");
            // DialogCharaProp= serializedObject.FindProperty("DialogChara");
            // HistorySnsProp= serializedObject.FindProperty("HistorySns");
            // mTarget= (StartSns)target;

            //  _roleList = new ReorderableList(serializedObject, HistorySnsProp
            //  , true, true, true, true);

        }

        public override void DrawCommandGUI()
        {
            /*      serializedObject.Update();
                  EditorGUILayout.PropertyField(DialogNameProp);
                  EditorGUI.BeginChangeCheck();

                  EditorGUILayout.PropertyField(DialogCharaProp);
                  if (EditorGUI.EndChangeCheck()) {
                      serializedObject.ApplyModifiedProperties();
                  }


                 foreach (var hisChara in mTarget.HistorySns) {
                      if (mTarget.DialogChara != null)
                      {
                          hisChara.mChara.Charas = new List<SnsManager.CharaSnsSetting>(mTarget.DialogChara);
                      }
                      Debug.Log("歷史對話數量1=>" + hisChara.mChara.Charas.Count);
                  }

                  serializedObject.ApplyModifiedProperties();

                  HistorySnsProp.serializedObject.Update();

                  foreach (var hisChara in mTarget.HistorySns)
                  {
                      Debug.Log("歷史對話數量2=>" + hisChara.mChara.Charas.Count);
                  }

                  EditorGUILayout.PropertyField(HistorySnsProp);*/

            EditorGUILayout.PropertyField(DialogNameProp);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.PropertyField(DialogCharaProp);

            mTarget.SetCharaValueForAllHistory();

            //  EditorGUILayout.PropertyField(HistorySnsProp);

            HistorySnsProp.arraySize=EditorGUILayout.IntField(HistorySnsProp.arraySize);

           EditorGUILayout.BeginVertical();
            scrollVec2=EditorGUILayout.BeginScrollView(scrollVec2, GUILayout.Width(400), GUILayout.Height(300));
            for (int i=0;i<HistorySnsProp.arraySize;i++) {
                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField(("element------" + i));



                SerializedProperty hisProp = HistorySnsProp.GetArrayElementAtIndex(i);

                EditorGUILayout.PropertyField(hisProp.FindPropertyRelative("mChara"));
                List<string> strList = new List<string>();
                strList.Add("Message");
                strList.Add("Image");
                EditorGUI.BeginChangeCheck();
                EnumField<SnsManager.SnsType>(
                    hisProp.FindPropertyRelative("mMessageType").FindPropertyRelative("_snsType"),
                    new GUIContent("SnsType", "SetSnsType"),
                    strList
                    ) ;
                if (EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
                }

                switch (hisProp.FindPropertyRelative("mMessageType").FindPropertyRelative("_snsType").enumValueIndex) {
                    case 1:
                        EditorGUILayout.PropertyField(hisProp.FindPropertyRelative("mMessageType").FindPropertyRelative("_message"));
                        break;

                    case 3:
                        EditorGUILayout.PropertyField(hisProp.FindPropertyRelative("mMessageType").FindPropertyRelative("_sprite"));
                        break;
                
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
         //    EditorGUILayout.PropertyField(HistorySnsProp);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            /*  EditorGUILayout.PropertyField(test1);
    mTarget.SetCharaValueForAllHistory();

    EditorGUILayout.PropertyField(test2);*/

        }

















    }
}
