using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{

    public class StoryControl : MonoBehaviour
    {
        // Start is called before the first frame update

        public Button autoButton;
        public Button logButton;
        public Button skipButton;
        [HideInInspector] public Flowchart Fc;


        private List<DialogInfo> recordDialogList = new List<DialogInfo>();//記錄對話

        public GameObject LogWindowPopupParent = null;

        private GameObject LogWindowPopupPrefab = null;

        private GameObject mLogWindowPopup = null;

        private bool PlayAni = false;

        private bool recordDialogOrigineDisplayStatus=false;





        void Start()
        {
            StartCoroutine(Init());
        }

        public IEnumerator Init()
        {

            yield return LoadPrefab();
            SetButtonSetting();

        }

        public void SetButtonSetting()
        {
            autoButton.onClick.AddListener(ClickAutoButton);
            logButton.onClick.AddListener(() => { StartCoroutine(ClickLogButton()); });
            skipButton.onClick.AddListener(ClickSkipButton);


        }
        public IEnumerator LoadPrefab()
        {
            if (LogWindowPopupPrefab == null)
            {
                ResourceRequest resRe = Resources.LoadAsync<GameObject>("Prefabs/PenguinPrefab/LogPopup");
                yield return new WaitUntil(() => resRe.isDone);
                LogWindowPopupPrefab = resRe.asset as GameObject;
            }

        }


        public void ClickAutoButton()
        {
            Debug.Log("點擊自動對話按鈕");
            if (!SayDialog.GetSayDialog().GetWriter().AutoPlay)
            {
                SayDialog.GetSayDialog().GetWriter().AutoPlay = true;
            }
            else
            {
                SayDialog.GetSayDialog().GetWriter().AutoPlay = false;
            }

            Stage.GetActiveStage().SetAutoPlay();




        }

        public IEnumerator ClickLogButton()
        {

            if (!PlayAni)
            {
                PlayAni = true;
                if (mLogWindowPopup == null)
                {

                    mLogWindowPopup = Instantiate(LogWindowPopupPrefab);
                    mLogWindowPopup.transform.SetParent(LogWindowPopupParent.transform, false);

                    mLogWindowPopup.GetComponent<CanvasGroup>().alpha = 0;
                    yield return mLogWindowPopup.GetComponent<StoryLogPopup>().Init(recordDialogList);
                    SayDialog sd = SayDialog.GetSayDialog();
                    if (sd.NowAlphaStatus())
                    {
                        recordDialogOrigineDisplayStatus = true;
                        StartCoroutine(sd.ReactionAlpha(false));
                    }
                    yield return LeanTweenManager.FadeIn(mLogWindowPopup);
                    PlayAni = false;

                }
                else
                {
                    if (recordDialogOrigineDisplayStatus) {
                        recordDialogOrigineDisplayStatus = false;
                        StartCoroutine(SayDialog.GetSayDialog().ReactionAlpha(true));
                    }
                    
                    yield return LeanTweenManager.FadeOut(mLogWindowPopup);
                    Destroy(mLogWindowPopup);
                    mLogWindowPopup = null;
                    PlayAni = false;
                }
            }




        }

        public void ClickSkipButton()
        {
            Debug.Log("點擊跳過按鈕");
            Fc._storyEnabled = false;
            GameObject sp = new GameObject("FadeMask", typeof(RectTransform), typeof(Image));
            sp.GetComponent<Image>().color = Color.black;
            sp.transform.SetParent(LogWindowPopupParent.transform, false);
            sp.transform.position = new Vector3(0, 0, 0);
            RectTransform spRT = sp.GetComponent<RectTransform>();
            spRT.anchorMin = Vector2.zero;
            spRT.anchorMax = Vector2.one;
            spRT.offsetMax = Vector2.zero;
            spRT.offsetMin = Vector2.zero;
            StartCoroutine(LeanTweenManager.FadeIn(sp));

            //淡出螢幕



        }

        public void SaveDialogRecord(DialogInfo dia)//儲存對話
        {
            recordDialogList.Add(dia);
        }



    }
        public class DialogInfo
        {
            public string CharaName;
            public string DialogContent;
            public AudioClip aAudioClip=null;
            public DialogInfo(string charaName, string dialogContent, AudioClip audioClip=null)
        {
            CharaName = charaName;
            DialogContent = dialogContent;
            aAudioClip = audioClip;
        }
    }

    }

