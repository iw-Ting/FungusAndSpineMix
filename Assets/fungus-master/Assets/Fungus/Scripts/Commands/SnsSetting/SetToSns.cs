using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Fungus
{

    [CommandInfo("Sns",
       "SetToSns",
       "Set Sns Setting")]
    public class SetToSns : Command
    {

        [SerializeField]public SnsManager.SnsMessage sns;



        public override void OnEnter()
        {

            Transform snsWindow = Flowchart.GetInstance().mStoryControl.LogWindowPopupParent.transform.Find("SnsSystem");

            if (snsWindow != null)
            {
                StartCoroutine(snsWindow.GetComponent<SnsManager>().SetDialogue(sns,Continue));
            }
            else
            {
                Continue();
            }


        }

        }

}
