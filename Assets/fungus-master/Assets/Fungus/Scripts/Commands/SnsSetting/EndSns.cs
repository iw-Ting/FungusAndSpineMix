using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{


    [CommandInfo("Sns",
          "EndSns",
          "Close Sns Setting")]
    public class EndSns : Command
    {

        public override void OnEnter()
        {
            Transform snsWindow = Flowchart.GetInstance().mStoryControl.LogWindowPopupParent.transform.Find("SnsSystem");

            if (snsWindow!=null) {
                StartCoroutine(snsWindow.GetComponent<SnsManager>().EndSnsWindow(()=> {
                    Continue();
                    StartCoroutine(Flowchart.GetInstance().mStoryControl.ShowTopUI());
                }));
            } else
            {
                Continue();
            }

        }




    }


     class eng
    {

        public string aa;   
        private string bb;
        protected internal string cc;



    }
         class qwe {

        private eng ee = new eng(); 
         public void asd()
            {

            ee.cc = "123";

          
        }
    
    
    }



}
