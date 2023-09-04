using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Fungus
{

    public class SnsManager : MonoBehaviour
    {
        // Start is called before the first frame update
        //-------------------------------Parent--------------------
        public Transform MessageContentParent;

        public RectTransform OptionPanelParent;

        //-------------------------------List--------------------

        public  List<CharaSnsSetting> mCharaSetting = new List<CharaSnsSetting>();

        public List<SnsMessage> SnsMessages = new List<SnsMessage>();//之後會逐一顯示的對話

        public List<SnsMessage> HistorySnsMessages = new List<SnsMessage>();//之後會逐一顯示的對話

        private List<SnsMessage> DisplaySnsMessages = new List<SnsMessage>();//已經顯示出來的陣列

        private List<MessageCard> mMessageCards = new List<MessageCard>();


        //-------------------------------params--------------------

        private int curDisplaySnsCount = 0;//玩家觸碰後+1

        private bool CanAddMessage = true;

      //  private bool AutoSendMessage = false;

        [SerializeField] private Text DialogObjNameText;

        [SerializeField] private GameObject MessageCardPrefabs = null;

        [SerializeField] private GameObject ReplyOptionsPrefabs = null;

        [SerializeField] private GameObject SeparateLinePrefabs = null;

        private SnsManagerFunc aData;

        public  Action CloseWindowCallBack = null;//接續fungus劇情用

        //--------------------instance-------------------
        private static SnsManager instance=null;

        public static SnsManager GetInstance()
        {
            if (instance==null) {
                instance = GetSnsManager();
            }
            return instance;
        }
        private static SnsManager GetSnsManager()
        {
            return GameObject.FindFirstObjectByType<SnsManager>();
        }

        public void Start()
        {
            //StartCoroutine(Init(null));//
        }

        //有一個腳本要負責銜接block 分配點選哪些選項,就分配給哪些snsManager
        public IEnumerator Init(SnsManagerFunc _Data)
        {
            aData = _Data;

            DialogObjNameText.text = aData.DialogName;

            mCharaSetting = aData.DialogChara;
            HistorySnsMessages = aData.HistorySns;

            yield return SetOrigineMessage();
            aData.InitOnComplete();


           // yield return AutoStartDialogue();

        }
        private IEnumerator SetOrigineMessage()//先設置歷史對話
        {
            LeanTweenManager.SetCanvasGroupAlpha(gameObject, 0);
            //gameObject.transform.localScale = Vector3.zero;

            int finishCount = 0;

            foreach (SnsMessage sns in HistorySnsMessages)
            {
                SetMessageSetting(sns);
                StartCoroutine(SetMessage(sns, () => { finishCount++; },false));

            }

            yield return new WaitUntil(() => finishCount >= HistorySnsMessages.Count);
            StartCoroutine(LeanTweenManager.FadeIn(gameObject, 0.2f));
            yield return LeanTweenManager.RectTransScale(gameObject, new Vector3(1.1f,1.1f,1.1f), 0.15f);
             yield return LeanTweenManager.RectTransScale(gameObject, Vector3.one, 0.05f);

          //  yield return LeanTweenManager.FadeIn(gameObject, 0.2f);





        }

        private IEnumerator AutoStartDialogue()//自動對話點擊並進行(正常情況執行)
        {
            while (curDisplaySnsCount < SnsMessages.Count)
            {
                yield return SnsDialogue();
            }
                StartCoroutine(EndSnsWindow());

        }

        //onComolete for fungus func continue to setting
        public IEnumerator SetDialogue(SnsMessage addSns, Action onComplete = null)//fungus因應臨時對話系統(根據不同選項,給出不同回答)
        {
            SnsMessages.Add(addSns);
            while (!CanAddMessage)
            {
                yield return null;
            }

            yield return SnsDialogue(onComplete);

        }

        private IEnumerator SnsDialogue(Action onComplete=null)
        {
                if (CanAddMessage)
                {
                    SnsMessage sns = SnsMessages[curDisplaySnsCount];
                    sns.aFade = true;
                    SetMessageSetting(sns);

                    CanAddMessage = false;
                    switch (sns.mMessageType._snsType)
                    {
                        case SnsType.Message:
                            if (SnsMessages.Count > 0)
                            {
                            /* if (AutoSendMessage)
                             {
                                 AutoSendMessage = false;
                                 yield return SetMessage(sns);
                             }
                             else
                             {
                                 yield return CreateDialogArea(sns);
                             }*/
                            yield return AutoDialogForMessage(sns);
                        }
                            break;

                        case SnsType.Reply :

                                yield return CreateReplyArea(sns);
        

                            break;

                        case SnsType.Image:
                                 yield return AutoDialogForMessage(sns);
                        break;
                    }

                    curDisplaySnsCount++;
                }
                yield return new WaitUntil(() => CanAddMessage);
            if (onComplete!=null) {
                onComplete();
            }



            //對話完 關閉視窗
        }

        private IEnumerator AutoDialogForMessage(SnsMessage sns)
        {

            yield return new WaitForSeconds(sns.mMessageType._dialogWaitTime);

            StartCoroutine(SetMessage(sns));

            yield return new WaitUntil(() => CanAddMessage);

        }

        private IEnumerator CreateDialogArea(SnsMessage sns)//點擊 出現下一段對話
        {

             InputCallBack.InputOptions opt = new InputCallBack.InputOptions();
             opt.parentPos = OptionPanelParent;
             opt.touchSize = new Vector2(1080,350);

             yield return InputCallBack.GetInputCallBack().CreateDetectInputCB(
                 ClickMode.ClickOnButton,
                 () => { 
                     StartCoroutine(SetMessage(sns)); 
                 },
                 opt);

            StartCoroutine(SetMessage(sns));

            yield return new WaitUntil(() => CanAddMessage);
        }

        private IEnumerator CreateReplyArea(SnsMessage sns)
        {
            List<GameObject> optObj = new List<GameObject>();

            if (sns.mMessageType._replyMessage.Length>0) {

                foreach (var option in sns.mMessageType._replyMessage) {

                    GameObject sp = Instantiate(ReplyOptionsPrefabs);
                    LeanTweenManager.SetCanvasGroupAlpha(sp, 0);
                    sp.transform.Find("ContentText").GetComponent<Text>().text = option;
                    sp.transform.SetParent(OptionPanelParent, false);

                    sp.GetComponent<Button>().onClick.AddListener(() => {
                        sns.mMessageType._message = option;

                        StartCoroutine(SetMessage(sns));
                    });
                    optObj.Add(sp);

                }

            }
            else
            {
                GameObject sp = Instantiate(ReplyOptionsPrefabs);
                LeanTweenManager.SetCanvasGroupAlpha(sp, 0);
                sp.transform.Find("ContentText").GetComponent<Text>().text = sns.mMessageType._message;
                sp.transform.SetParent(OptionPanelParent, false);
                sp.GetComponent<Button>().onClick.AddListener(() => {

                    StartCoroutine(SetMessage(sns));
                });
                optObj.Add(sp);
            }


            foreach (var obj in optObj) {

           StartCoroutine( LeanTweenManager.FadeIn(obj));
            obj.transform.localScale = Vector3.zero;
            StartCoroutine(LeanTweenManager.RectTransScale(obj, new Vector3(1.1f,1.1f,1.1f), 0.25f, () => {
              StartCoroutine(  LeanTweenManager.RectTransScale(obj, Vector3.one,0.05f));
            }) );
                yield return new WaitForSeconds(0.1f);
            
            }


            yield return new WaitUntil(() => CanAddMessage);

            int finishCount = 0;
            foreach(var obj in optObj)
            {
                StartCoroutine( LeanTweenManager.FadeOut(obj, 0.3f, () => { 
                    finishCount++;
                }));
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitUntil(()=>finishCount>=sns.mMessageType._replyMessage.Length);
            foreach (var obj in optObj)
            {
                Destroy(obj);
            }

        }

        

        private IEnumerator SetMessage(SnsMessage sns,Action onComplete=null,bool needSeparateLine=true)
        {

            DisplaySnsMessages.Add(sns);//必須先抓對話 因為生成messageCard需要時間 如果晚加入,會讓後面的邏輯誤判

            if (IsLastOneOfSameCharacter(sns.mChara.mName)&&curDisplaySnsCount>0)
            {
                MessageCard mc = mMessageCards[(mMessageCards.Count - 1)];
                sns.NotHaveAvatar = true;
                yield return mc.Init(sns);

            }
            else
            {

                if (needSeparateLine && curDisplaySnsCount == 0)
                {

                    GameObject sepLine = Instantiate(SeparateLinePrefabs);
                    sepLine.transform.SetParent(MessageContentParent, false);

                    //訊息分隔線
                }

                sns.NotHaveAvatar = false;//文字段落連貫
                MessageCard Mcsp = Instantiate(MessageCardPrefabs).GetComponent<MessageCard>();
                mMessageCards.Add(Mcsp);
                Mcsp.transform.SetParent(MessageContentParent, false);
                yield return Mcsp.Init(sns);
            }


            Transform bg = transform.Find("Background");
            Transform sv = bg.transform.Find("ScrollView");

            RectTransform scrollViewRect = sv.GetComponent<RectTransform>();
            RectTransform BgRect = bg.GetComponent<RectTransform>();

            float limitWindowY = (Math.Abs( scrollViewRect.offsetMax.y) + Math.Abs(scrollViewRect.offsetMin.y))
                + (Math.Abs(BgRect.offsetMax.y) + Math.Abs(BgRect.offsetMin.y));  //計算訊息顯示的限制範圍 ex:1920-920


            if (MessageContentParent.GetComponent<RectTransform>().sizeDelta.y > (Screen.height - limitWindowY))
            {
                MessageContentParent.GetComponent<RectTransform>().pivot =new Vector2(0.5f,0);
                MessageContentParent.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-(Screen.height - limitWindowY));


            }
            if (onComplete!=null) {
                onComplete();
            }
            CanAddMessage = true;

        }


        private void SetMessageSetting( SnsMessage sns) {

            foreach (var chara in mCharaSetting)
            {//幫角色設置方向數值
                if (chara.mFungusChara.NameText == sns.mChara.mName)
                {
                    sns.mChara.mDirection = chara.mDirection;

                    if (sns.mMessageType._snsType==SnsType.None) {
                    }


                    switch (chara.mCharaRole)
                    {
                        case CharaRole.self :
                            if (sns.mMessageType._snsType == SnsType.None)
                            {
                                sns.mMessageType._snsType = SnsType.Reply;
                            }
                            break;
                        case CharaRole.otherSide:
                            if (sns.mMessageType._snsType == SnsType.None)
                            {
                                sns.mMessageType._snsType = SnsType.Message;
                            }
                            break;


                    }


                    if (chara.mFungusChara.charaAvatar != null)
                    {
                        sns.mChara.mAvatar = chara.mFungusChara.charaAvatar;
                    }
                }

            }
        }


        private bool IsLastOneOfSameCharacter(string judgeName)
        {

            if (DisplaySnsMessages.Count < 2)
            {
                return false;
            }

            if (DisplaySnsMessages[DisplaySnsMessages.Count - 2].mChara.mName == judgeName)
            {

                return true;
            }
            else
            {
                return false;
            }

        }

        public IEnumerator EndSnsWindow(Action endCB=null)
        {

            InputCallBack.InputOptions opt = new InputCallBack.InputOptions();
            opt.parentPos = OptionPanelParent;
            opt.touchSize = new Vector2(1080, 350);
            yield return InputCallBack.GetInputCallBack().CreateDetectInputCB(
                ClickMode.ClickOnButton,
                () => {
                    StartCoroutine(CloseSnsWindow(endCB));
                },
                opt);

        }

        private IEnumerator CloseSnsWindow(Action endCB = null)
        {

            StartCoroutine(LeanTweenManager.FadeOut(gameObject, 0.2f));

            yield return LeanTweenManager.RectTransScale(gameObject, new Vector3(1.1f,1.1f,1.1f),0.05f);

            yield return LeanTweenManager.RectTransScale(gameObject, Vector3.zero,0.15f);

            if (endCB!=null) {
                endCB();
            }
            Destroy(gameObject);
        }

        [ExecuteAlways]
        public  List<string> GetCharacterArray()
        {

            List<string> list = new List<string>();

            if (mCharaSetting.Count < 1)
            {
                return list;
            }

            foreach (var name in mCharaSetting) {
                if (name.mFungusChara==null) {
                    break;
                }
                if (name.mFungusChara.NameText!=null&& name.mFungusChara.NameText!="") {
                    list.Add(name.mFungusChara.NameText);
                }
            
            }

            return list;

        }


        [Serializable]
        public class SnsMessage
        {
            [HideInInspector]public bool NotHaveAvatar = false;

            [HideInInspector] public bool aFade = false;
            
            public SnsChara mChara;

            [SnsMessageProp]
            public MessageType mMessageType;


        }


        [Serializable]
        public class SnsChara
        {
            [HideInInspector] public Sprite mAvatar = null;

            [CharaDropOptions]
            public string mName = "";
            

           [HideInInspector]public Direction mDirection = Direction.Left;
        }
        [Serializable]
        public class MessageType
        {
            [HideInInspector] public  SnsType _snsType=SnsType.None;

            public string _message;

            public string[] _replyMessage;//不同回答,不同答案

            public Sprite _sprite;

            public float _dialogWaitTime = 0;
        }
        [Serializable]
        public class CharaSnsSetting
        {
            public Character mFungusChara;

            public CharaRole mCharaRole;


            public Direction mDirection;

        }

        public enum Direction
        {
            Right,
            Left
        }

        public enum CharaRole
        {
            self,
            otherSide
        }

        public enum SnsType // 
        {
            None,
            Message,
            Reply,
            Image,
            ReplyImage


        }

        public class SnsManagerFunc
        {
            public List<CharaSnsSetting> DialogChara;
            public List<SnsMessage> HistorySns;
            public string DialogName = "";
            public Action InitOnComplete = null;
            public SnsManagerFunc(string _dialogName, List<CharaSnsSetting> _dialogChara, List<SnsMessage> _historySns, Action _initOnComplete)
            {
                DialogName = _dialogName;
                DialogChara = _dialogChara;
                HistorySns = _historySns;
                InitOnComplete = _initOnComplete;
            }
        }

    }

    public class CharaDropOptions:PropertyAttribute
    {


    }

    public class SnsMessagePropAttribute : PropertyAttribute
    {


    }



}
