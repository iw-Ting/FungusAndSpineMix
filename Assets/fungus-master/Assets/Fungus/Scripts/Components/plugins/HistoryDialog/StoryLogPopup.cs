using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Fungus {
    public class StoryLogPopup : MonoBehaviour
    {

        public GameObject LogCellContentParent = null;

        public GameObject LogCellPrefab = null;

        public Button CloseButton;

        private Action mCloseButtonCb;




        public IEnumerator Init(List<DialogInfo> dia,Action closeButtonCb)
        {
            mCloseButtonCb = closeButtonCb;
            yield return LoadPrefabs();
            SetPrefabsPivot(dia);
            CreateCell (dia);
            SetButtonSetting();

            //LayoutRebuilder.ForceRebuildLayoutImmediate(ChildTextRect);

        }

        private void SetButtonSetting()
        {
            CloseButton.onClick.AddListener(()=> {
                Debug.Log("Ãö³¬¾ú¥v¹ï¸Ü");
                mCloseButtonCb();
            });


        }

        private void SetPrefabsPivot(List<DialogInfo> dia)
        {
            if (dia.Count > 5)
            {
                LogCellContentParent.GetComponent<RectTransform>().pivot = Vector3.zero;
            }


        }


        public void CreateCell(List<DialogInfo> dia)
        {

            if (LogCellPrefab==null) {
                return;
            }
            GameObject sp = null;
            foreach (DialogInfo info in dia) {
                 sp = Instantiate(LogCellPrefab);
                sp.transform.SetParent(LogCellContentParent.transform,false);

                StartCoroutine( sp.GetComponent<DialogHistoryCell>().Init(info));
            
            }

            if (dia.Count == 1)
            {
                sp.gameObject.SetActive(false);
                sp.gameObject.SetActive(true);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(LogCellContentParent.GetComponent<RectTransform>());

        }

        private IEnumerator LoadPrefabs()
        {
            ResourceRequest resRe = Resources.LoadAsync<GameObject>("Prefabs/PenguinPrefab/DialogHistoryCell");
            yield return new WaitUntil(() => resRe.isDone);
            LogCellPrefab = resRe.asset as GameObject;


        }



    }
}