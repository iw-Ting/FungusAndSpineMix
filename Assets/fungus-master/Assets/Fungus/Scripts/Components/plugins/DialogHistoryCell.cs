using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




namespace Fungus {

    public class DialogHistoryCell : MonoBehaviour
    {

        public Text ContentText;
        public Text CharaNameText;
        public Button PlayAudioButton;
        public DialogInfo mData;




        public IEnumerator Init(DialogInfo aData)
        {
            mData = aData;
            UpdateUI();
            SetButton();
            yield return null;


        }


        private void UpdateUI()
        {
            CharaNameText.text = mData.CharaName;
            ContentText.text = mData.DialogContent;

        }

        private void SetButton() { }

        // Start is called before the first frame update
     
    }
}