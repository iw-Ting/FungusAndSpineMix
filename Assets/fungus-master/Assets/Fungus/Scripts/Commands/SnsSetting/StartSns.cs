using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fungus
{

    [CommandInfo("Sns",
              "StartSns",
              "Open Sns Setting")]
    public class StartSns : Command
    {
        public string DialogWindowName;

        public List<SnsManager.CharaSnsSetting> DialogChara;

        public List< SnsManager.SnsMessage> HistorySns;

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







    }


}
