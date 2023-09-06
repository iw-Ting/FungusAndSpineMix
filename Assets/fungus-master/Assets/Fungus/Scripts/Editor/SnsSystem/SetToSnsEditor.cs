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
        private SetToSns sns = null;
        private string origineCharaName=null;



        public override void OnEnable()
        {


            // base.OnEnable();
        }

        public override void DrawCommandGUI()
        {

            sns = (SetToSns)target;
            SnsProp = serializedObject.FindProperty("sns");
            SerializedProperty messageInfo = SnsProp.FindPropertyRelative("mMessageType");

            //SnsManager snsWindow = Flowchart.GetInstance().mStoryControl.LogWindowPopupParent.transform.Find("SnsSystem").GetComponent<SnsManager>();
            //  List < SnsManager.CharaSnsSetting > charaList= StartSns.GetInstance().DialogChara;


            //StartSns startSns = sns.ParentBlock.GetComponent<StartSns>();
            StartSns startSns =GetDialogStartSns();

            if (startSns&& startSns.DialogChara!=null&& startSns.DialogChara.Count>0) {
                sns.sns.mChara.Charas = startSns.DialogChara;
               // SnsProp = serializedObject.FindProperty("sns");
            }
            else
            {
                Debug.Log("Not Setting CharaSetting");
                return;
            }

            int availableChara = startSns.DialogChara.Count;

            foreach (var chara in sns.sns.mChara.Charas) {
                if (chara.mFungusChara==null) {
                    availableChara--;
                }
            }
            if (availableChara<=0) {
                return;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(SnsProp.FindPropertyRelative("mChara"));
            if (EditorGUI.EndChangeCheck()) {

                Debug.Log("�H��ܨ���");
                serializedObject.ApplyModifiedProperties();
            }

            // EditorGUILayout.PropertyField(messageInfo.FindPropertyRelative("_snsType"));//�ھڤ��P����

            foreach (var chara in sns.sns.mChara.Charas)
            {

                if (chara.mFungusChara.NameText==sns.sns.mChara.mName) {
    
                    List<string> displaySnsType = new List<string>();

  

                    switch (chara.mCharaRole)
                    {

                        case SnsManager.CharaRole.self:

                            displaySnsType.Add("Message");
                            displaySnsType.Add("Reply");
                            displaySnsType.Add("Image");

                            if (origineCharaName!= sns.sns.mChara.mName) {
                                origineCharaName = sns.sns.mChara.mName;
                                sns.sns.mMessageType._snsType = SnsManager.SnsType.Reply;
                            }

                            CommandEditor.EnumField<SnsManager.SnsType>(
                                messageInfo.FindPropertyRelative("_snsType"),
                                new GUIContent("SnsType", "Change Sns Type"),
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
                                new GUIContent("SnsType", "Change Sns Type"),
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
                //�ھڤ��P������w��,�������P���ﶵ
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

        }

       private StartSns GetDialogStartSns()//������T��startSns  �Ҧ���command���}�����|���b flowchart �o�Ӫ��󩳤U
        {
            List<CalcStartSnsIndex> Snss = new List<CalcStartSnsIndex>();
            int mIndex = 0;

            for (int i=0;i< sns.ParentBlock.CommandList.Count;i++) {
                Command childSns = sns.ParentBlock.CommandList[i];


                if (childSns.GetType() == typeof(StartSns)&&childSns!=null)
                {
                    Snss.Add(new CalcStartSnsIndex( i , (StartSns)childSns));
                }
                if (childSns==sns) {
                    mIndex = i;
                
                }
            }


            List<CalcStartSnsIndex> SnssCopy = new List<CalcStartSnsIndex>(Snss);
          //  Debug.Log("1.�z��X��sns�ƶq=>" + Snss.Count);

            foreach (var s in Snss)
            {
                if (s.index>mIndex) {
                    SnssCopy.Remove(s);
                }
                
            }
            //Debug.Log("2.�L�o�X��sns�ƶq=>" +SnssCopy.Count);

            //sns.CommandIndex

            if (SnssCopy.Count<=0) {
            return null;
            }

            return SnssCopy[(SnssCopy.Count-1)].mSns;

        }

        private class CalcStartSnsIndex
        {
            public int index = 0;
            public StartSns mSns=null;
            public CalcStartSnsIndex(int _index,StartSns _mSns) {
                index = _index;
                mSns=_mSns;
            
            }



        }



    }





}
