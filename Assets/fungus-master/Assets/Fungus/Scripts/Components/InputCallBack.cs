using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace Fungus {

    public class InputCallBack : MonoBehaviour //�D�n�\��,detect mouse input 
    {
        public ClickMode eClickMode;

        public Action SetNextLineFlag = null;

        protected StandaloneInputModule currentStandaloneInputModule;

        [SerializeField] protected bool cancelEnabled = true;

        [SerializeField] protected bool ignoreMenuClicks = true;

        protected bool dialogClickedFlag;

        protected bool nextLineInputFlag;

        public List<GameObject> TriggerObjs = new List<GameObject>();

        public List<IEnumerator> WaitIEList = new List<IEnumerator>();

        private static InputCallBack instance=null;

        public static InputCallBack  Instance(){
            return instance;
        }

        public static bool IsHaveInstance()
        {
            if (instance==null) {
            return false;
            }
            else
            {
                return true;
            }
        }

        public static InputCallBack GetInputCallBack()
        {

            GameObject sp;

            if (!InputCallBack.IsHaveInstance())
            {
                sp = new GameObject("InputSetting", typeof(InputCallBack));
            }
            else
            {
                sp = InputCallBack.Instance().gameObject;
            }
            return sp.GetComponent<InputCallBack>();
        }

        public  IEnumerator CreateDetectInputCB(ClickMode clickMode, Action cb, InputCallBack.InputOptions inpOpt = null)
        {
            IEnumerator ie= WaitClickCallBack(clickMode, cb, inpOpt);
            WaitIEList.Add(ie);
            yield return ie;
        }

        public void Awake()
        {
            if (instance==null) {
                instance = this;
            }
            CheckEventSystem();
        }

        protected virtual void CheckEventSystem()
        {
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();

            if (eventSystem == null)
            {
                // Auto spawn an Event System from the prefab
                GameObject prefab = Resources.Load<GameObject>("Prefabs/EventSystem");

                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab) as GameObject;
                    go.name = "EventSystem";
                }
            }
        }


        public IEnumerator WaitClickCallBack(ClickMode clickMode,Action cb,InputOptions inpOpt=null) {

            eClickMode = clickMode;
            SetNextLineFlag = cb ;

            if (EventSystem.current == null )
            {
                Debug.Log("Not Have EventSystem");
               yield break;
            }

            if (currentStandaloneInputModule == null)
            {
                currentStandaloneInputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
            }

            bool isinput = false;

            

                if (Input.GetButtonDown(currentStandaloneInputModule.submitButton) ||//��L
                    (cancelEnabled && Input.GetButton(currentStandaloneInputModule.cancelButton)))
                {
                    isinput = true;
                    SetNextLineFlag();//�����I����block command����
                }

                //SayDialog.GetSayDialog().GetComponent<DialogInput>().CloseInputButtonArea();

                GameObject sp =null;

                switch (eClickMode)
                {
                    case ClickMode.Disabled:
                        break;
                    case ClickMode.ClickAnywhere:

                    inpOpt.touchSize = new Vector2(Screen.width, Screen.height);
                    inpOpt.SetLocalPos = true;

                    sp = InputUISupportScript.CreateButtonArea(inpOpt, () => {
                        isinput = true;
                        SetNextLineFlag();
                    });
                    //sp.GetComponent<Button>().onClick.AddListener(() => { Destroy(sp); });
                    break;
                    case ClickMode.ClickOnButton:

                        if (inpOpt!=null) {
                             sp = InputUISupportScript.CreateButtonArea(inpOpt, () => {
                                isinput = true;
                                SetNextLineFlag();
                            });
                           // sp.GetComponent<Button>().onClick.AddListener(() => { Destroy(sp); });


                        }
                        else
                        {
                            Debug.Log("not setting");
                            yield break;
                        }
                        break;

                }

            if (sp!=null) {
                TriggerObjs.Add(sp);
            }

            yield return new WaitUntil(() => isinput);
            //fade out�ʵe
            SetNextLineFlag = null;
            if (TriggerObjs.Count>0) {
                foreach (GameObject obj in TriggerObjs)
                {
                    Destroy(obj);
                }
                TriggerObjs.Clear();
            }

        }

        private IEnumerator WaitClickMenuCallBack( Action cb, MenuImageOption inpOpt = null)
        {

            eClickMode =ClickMode.ClickOnButton;

            if (EventSystem.current == null)
            {
                Debug.Log("Not Have EventSystem");
                yield break;
            }

            bool isinput = false;

            GameObject sp = null;

            if (inpOpt != null)
            {
                sp = InputUISupportScript.CreateButtonArea(inpOpt._options, () => {
                    isinput = true;
                    cb();
                });


                if (inpOpt._image!=null) {

                    GameObject imObj = new GameObject("DisplayImage", typeof(RectTransform), typeof(Image),typeof(CanvasGroup));
                    imObj.transform.SetParent(sp.transform);
                    imObj.transform.localScale = Vector3.one;
                    imObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    CanvasGroup cg=imObj.GetComponent<CanvasGroup>();
                    cg.alpha = 0;

                    Image im = imObj.GetComponent<Image>();
                    im.sprite = inpOpt._image;
                    im.raycastTarget = false;

                    if (inpOpt.imageSize != Vector2.zero)
                    {
                        imObj.GetComponent<RectTransform>().sizeDelta = inpOpt.imageSize;
                    }
                    else
                    {
                        im.preserveAspect = true;
                    }
                    LeanTween.alphaCanvas(cg, 1, 0.5f);
                    yield return new WaitForSeconds(0.5f);
                }



            }

            if (sp != null)
            {
                TriggerObjs.Add(sp);
            }
            yield return new WaitUntil(() => isinput);
            //fade out�ʵe
            if (TriggerObjs.Count > 0)
            {
                foreach (GameObject obj in TriggerObjs)
                {
                    Destroy(obj);
                }
                TriggerObjs.Clear();
            }

        }

        public void CreateMenuImage( MenuImageOption menuImageOptList)
        {
            �@
            // Block block = Block.FindBlockByName(menuImageOptList.targetBlock);
            Block block = menuImageOptList.targetBlock;
             Flowchart fc = block.GetFlowchart();

                    Action _OnComplete = () => {
                        fc.StartCoroutine(CallBlock(block));
                    };

                    StartCoroutine(WaitClickMenuCallBack( _OnComplete, menuImageOptList));
        }



        protected IEnumerator CallBlock(Block block) //������wblock
        {
            yield return new WaitForEndOfFrame();
            block.StartExecution();
        }


        public void InitInputList()//��l��Ĳ�o
        {
            if (WaitIEList.Count>0) {
                foreach (IEnumerator ie in WaitIEList)
                {
                    StopCoroutine(ie);
                }
                WaitIEList.Clear();
            }


            if (TriggerObjs.Count > 0)
            {
                foreach (GameObject obj in TriggerObjs)
                {
                    Destroy(obj);
                }
                TriggerObjs.Clear();
            }
            if (SetNextLineFlag != null) { 
                SetNextLineFlag();
            }

        }


        [Serializable]
        public class InputOptions {

            public RectTransform parentPos = null;
            public Vector2 touchSize = Vector2.zero;
            public bool SetLocalPos=false;

        }


    }
}
