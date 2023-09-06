using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/*public enum SnsType // 
{
    None,
    Message,
    Reply,
    Image,
    ReplyImage


}*/

namespace Fungus
{

    [CommandInfo("Sns",
              "StartSns",
              "Open Sns Setting")]
    public class StartSns : Command
    {
        public string DialogWindowName;

        public List<SnsManager.CharaSnsSetting> DialogChara = new List<SnsManager.CharaSnsSetting>();

        public List<string> str = new List<string>();
        public List<StrList> strArr = new List<StrList>();



        public List< SnsManager.SnsMessage> HistorySns=new List<SnsManager.SnsMessage>();

        private static StartSns instance = null;
        public static StartSns GetInstance()
        {
            if (instance == null)
            {
                instance = GetStartSns();
            }
            return instance;
        }

        private static StartSns GetStartSns()
        {
            return GameObject.FindFirstObjectByType<StartSns>();
        }
        [ExecuteAlways]
        public List<string> GetCharacterArray()
        {

            List<string> list = new List<string>();

            if (DialogChara.Count < 1)
            {
                return list;
            }

            foreach (var name in DialogChara)
            {
                if (name.mFungusChara == null)
                {
                    break;
                }
                if (name.mFungusChara.NameText != null && name.mFungusChara.NameText != "")
                {
                    list.Add(name.mFungusChara.NameText);
                }

            }

            return list;

        }


        public override void OnEnter()
        {
          StartCoroutine(  StartSnsDialog());

            //Continue();

        }

        private IEnumerator StartSnsDialog() {
            SnsManager sns = null;
          /*  foreach (var hisSns in HistorySns) {
                hisSns.mMessageType._snsType = SnsManager.SnsType.Message;
            }*/
            yield return CreateSnsWindow(_sns => { sns = _sns; });
            yield return sns.Init(new SnsManager.SnsManagerFunc(
                DialogWindowName,
                DialogChara,
                HistorySns,
                Continue
                ) );
        }


        private IEnumerator CreateSnsWindow(Action<SnsManager> cb)
        {
            ResourceRequest  resReq = Resources.LoadAsync<GameObject>(FungusResourcesPath.PrefabsPath + "SnsSystem");
            yield return resReq;

            GameObject sp = Instantiate((GameObject)resReq.asset);
            sp.name = "SnsSystem"; 
            


            sp.transform.SetParent( Flowchart.GetInstance().mStoryControl.LogWindowPopupParent.transform,false);
            StartCoroutine(Flowchart.GetInstance().mStoryControl.HideDialogAndTopUI());

            cb(sp.GetComponent<SnsManager>());
        }


        public void SetCharaValueForAllHistory() {
            //Debug.Log("顯示的數量=>" + DialogChara.Count);
            List<SnsManager.CharaSnsSetting> test = new List<SnsManager.CharaSnsSetting>();
            foreach (var chara in DialogChara)
            {
                SnsManager.CharaSnsSetting ch = new SnsManager.CharaSnsSetting() {
                    mFungusChara = chara.mFungusChara,
                    mCharaRole = chara.mCharaRole,
                    mDirection = chara.mDirection
                };

                test.Add(chara);
            }


            if (DialogChara.Count>0&&HistorySns.Count>0) {

                for (int i=0; i<HistorySns.Count ;i++) {

                    HistorySns[i].mChara.Charas = test;
                }

            }

        }

       /* public void SetCharaValueForAllHistory()
        {
            Debug.Log("執行測試");
            if (str.Count<=0||strArr.Count<=0) {
                return;
            }
            List<string> test = new List<string>(str);


                for (int i=0;i<strArr.Count;i++) {

                    strArr[i].strList = test;
                
                }

        }*/

        [Serializable]
        public class StrList
        {
          public  List<string> strList = new List<string>();



        }



    }


}
