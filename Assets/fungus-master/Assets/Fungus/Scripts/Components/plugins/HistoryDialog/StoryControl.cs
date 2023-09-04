using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{

    [ExecuteInEditMode]
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

        [SerializeField]private GameObject TopFuncListParent=null;

        private bool PlayAni = false;



        private void OnEnable()
        {
            FindCameraRender();
        }
        private void FindCameraRender()
        {
            if (GetComponent<Canvas>().worldCamera == null)
            {
                GetComponent<Canvas>().worldCamera = Camera.main;
            }


        }


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


        private void ClickAutoButton()
        {
            if (PlayAni) {
                return;
            }
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

        private IEnumerator ClickLogButton()
        {

            if (!PlayAni)
            {
                PlayAni = true;
                if (mLogWindowPopup == null)
                {

                    mLogWindowPopup = Instantiate(LogWindowPopupPrefab);
                    mLogWindowPopup.transform.SetParent(LogWindowPopupParent.transform, false);

                    mLogWindowPopup.GetComponent<CanvasGroup>().alpha = 0;

                    yield return mLogWindowPopup.GetComponent<StoryLogPopup>().Init(recordDialogList,
                        ()=> { StartCoroutine(ClickCloseLogButton()); });
                    StartCoroutine(HideDialogAndTopUI());
                    yield return LeanTweenManager.FadeIn(mLogWindowPopup);
                    PlayAni = false;

                }
            }
        }

        private IEnumerator ClickCloseLogButton()
        {
            if (mLogWindowPopup!=null&&!PlayAni) {
                PlayAni = true;

                StartCoroutine(ShowTopUI());
                
                yield return LeanTweenManager.FadeOut(mLogWindowPopup);
                Destroy(mLogWindowPopup);
                mLogWindowPopup = null;
                PlayAni = false;
            }
        }



        private void ClickSkipButton()
        {
            if (PlayAni)
            {
                return;
            }
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

        public IEnumerator ShowTopUI()
        {

            TopFuncListParent.SetActive(true);
            yield return LeanTweenManager.FadeIn(TopFuncListParent);
        }

        public IEnumerator HideDialogAndTopUI()
        {
            SayDialog sd = SayDialog.GetSayDialog();

            if (sd.NowAlphaStatus())
            {
                StartCoroutine(sd.ReactionAlpha(false));
                yield return LeanTweenManager.FadeOut(TopFuncListParent, () => { TopFuncListParent.SetActive(false); });
            }
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

