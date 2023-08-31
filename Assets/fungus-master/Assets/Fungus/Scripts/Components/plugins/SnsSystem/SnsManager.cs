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

        public Transform MessageContentParent;

        public RectTransform OptionPanelParent;

        //-------------------------------Parent--------------------

        public  List<CharaSnsSetting> mCharaSetting = new List<CharaSnsSetting>();

        public List<SnsMessage> SnsMessages = new List<SnsMessage>();//����|�v�@��ܪ����

        public List<SnsMessage> HistorySnsMessages = new List<SnsMessage>();//����|�v�@��ܪ����

        private int curDisplaySnsCount = 0;//���aĲ�I��+1

        private List<SnsMessage> DisplaySnsMessages = new List<SnsMessage>();//�w�g��ܥX�Ӫ��}�C

        private List<MessageCard> mMessageCards = new List<MessageCard>();

        private bool CanAddMessage = true;

        private bool AutoSendMessage = false;

        [SerializeField] private GameObject MessageCardPrefabs = null;

        [SerializeField] private GameObject ReplyOptionsPrefabs = null;

        public static SnsManager GetSnsManager()
        {
            return GameObject.FindFirstObjectByType<SnsManager>();
        }

        public void Start()
        {
            StartCoroutine(Init());
        }

        //���@�Ӹ}���n�t�d�α�block ���t�I����ǿﶵ,�N���t������snsManager
        public IEnumerator Init()
        {
            yield return SetOrigineMessage();
            yield return StartDialogue();

        }
        private IEnumerator SetOrigineMessage()//���]�m���v���
        {
            LeanTweenManager.SetCanvasGroupAlpha(gameObject, 0);

            int finishCount = 0;

            foreach (SnsMessage sns in HistorySnsMessages)
            {
                SetMessageSetting(sns);
                StartCoroutine(SetMessage(sns, () => { finishCount++; }));

            }

            yield return new WaitUntil(() => finishCount >= HistorySnsMessages.Count);

            LeanTweenManager.SetCanvasGroupAlpha(gameObject, 1);
            //  yield return null;

            //�x�s����messaage��m���a�� ���Υk

            //���P�_��ܬO�_�ݭn�Y�� ���δN���ݭn�ͦ�messageCard �������b�l����N�n �o�̤]�n���snsmessage���Ѽ�
        }

        private IEnumerator StartDialogue()
        {


            while (curDisplaySnsCount < SnsMessages.Count)
            {
                if (CanAddMessage)
                {
                    SnsMessage sns = SnsMessages[curDisplaySnsCount];
                    sns.aFade = true;
                    SetMessageSetting( sns);

                    CanAddMessage = false;
                    switch (sns.mMessageType._snsType)
                    {
                        case SnsType.Message:
                            if (SnsMessages.Count > 0)
                            {
                                if (AutoSendMessage) {
                                    AutoSendMessage = false;
                                    yield return new WaitForSeconds(0.5f);
                                    yield return SetMessage(sns);
                                }
                                else
                                {
                                    yield return CreateDialogArea(sns);
                                }

                                yield return CreateDialogArea(sns);

                            }
                            break;

                        case SnsType.Reply:
                            if (sns.mMessageType._ReplyMessage.Length>=1) {
                                yield return CreateReplyArea(sns);
                            }
                            else
                            {
                                yield return CreateDialogArea(sns);
                            }
                            break;

                        case SnsType.Image:

                            break;
                    }

                    curDisplaySnsCount++;

                }
                yield return new WaitUntil(() => CanAddMessage);
            }
            Debug.Log("��ܵ���");
            //��ܧ� ��������
        }

        private IEnumerator CreateDialogArea(SnsMessage sns)//�I�� �X�{�U�@�q���
        {
                InputCallBack.InputOptions opt = new InputCallBack.InputOptions();
                opt.parentPos = OptionPanelParent;
                opt.touchSize = OptionPanelParent.sizeDelta;

                yield return InputCallBack.GetInputCallBack().CreateDetectInputCB(
                    ClickMode.ClickOnButton,
                    () => { 
                        StartCoroutine(SetMessage(sns)); 
                    },
                    opt);
            yield return new WaitUntil(() => CanAddMessage);
        }

        private IEnumerator CreateReplyArea(SnsMessage sns)
        {
            List<GameObject> optObj = new List<GameObject>();

            foreach (var option in sns.mMessageType._ReplyMessage) {

                GameObject sp = Instantiate(ReplyOptionsPrefabs);
                LeanTweenManager.SetCanvasGroupAlpha(sp, 0);
                sp.transform.Find("ContentText").GetComponent<Text>().text = option;
                sp.transform.SetParent(OptionPanelParent,false);
                sp.GetComponent<Button>().onClick.AddListener(() => {
                    sns.mMessageType._message = option;
                    AutoSendMessage = true;
                    StartCoroutine(SetMessage(sns));
                });
                optObj.Add(sp);

            }
            foreach (var obj in optObj) {

           StartCoroutine( LeanTweenManager.FadeIn(obj));
            obj.transform.localScale = Vector3.zero;
            StartCoroutine(LeanTweenManager.RectTransScale(obj, new Vector3(1.2f,1.2f,1.2f), 0.25f, () => {
              StartCoroutine(  LeanTweenManager.RectTransScale(obj, Vector3.one,0.1f));
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

            yield return new WaitUntil(()=>finishCount>=sns.mMessageType._ReplyMessage.Length);
            foreach (var obj in optObj)
            {
                Destroy(obj);
            }

        }

        private IEnumerator SetMessage(SnsMessage sns,Action onComplete=null)//�]�m��� bool �P�_�O�_�������
        {
            /*  foreach (var chara in mCharaSetting) {//������]�m��V�ƭ�
                  if (chara.mFungusChara.NameText==sns.mChara.mName) {
                      sns.mChara.mDirection = chara.mDirection;
                      sns.mMessageType._snsType = chara.mSnsType;
                      if(chara.mFungusChara.charaAvatar!=null){
                          sns.mChara.mAvatar = chara.mFungusChara.charaAvatar;
                      }
                  }

              }*/

            DisplaySnsMessages.Add(sns);//���������� �]���ͦ�messageCard�ݭn�ɶ� �p�G�ߥ[�J,�|���᭱���޿�~�P
            if (!IsLastOneOfSameCharacter(sns.mChara.mName))
            {
                sns.NotHaveAvatar = false;//��r�q���s�e
                MessageCard Mcsp = Instantiate(MessageCardPrefabs).GetComponent<MessageCard>();
                mMessageCards.Add(Mcsp);
                Mcsp.transform.SetParent(MessageContentParent, false);
                yield return Mcsp.Init(sns);

            }
            else
            {
                MessageCard mc = mMessageCards[(mMessageCards.Count - 1)];
                sns.NotHaveAvatar = true;
                yield return mc.Init(sns);
            }


            Transform bg = transform.Find("Background");
            Transform sv = bg.transform.Find("ScrollView");

            RectTransform scrollViewRect = sv.GetComponent<RectTransform>();
            RectTransform BgRect = bg.GetComponent<RectTransform>();

            float limitWindowY = (Math.Abs( scrollViewRect.offsetMax.y) + Math.Abs(scrollViewRect.offsetMin.y))
                + (Math.Abs(BgRect.offsetMax.y) + Math.Abs(BgRect.offsetMin.y));  //�p��T����ܪ�����d�� ex:1920-920


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
            {//������]�m��V�ƭ�
                if (chara.mFungusChara.NameText == sns.mChara.mName)
                {
                    sns.mChara.mDirection = chara.mDirection;
                    sns.mMessageType._snsType = chara.mSnsType;
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
            [HideInInspector] public  SnsType _snsType;

            public string _message;

            public string[] _ReplyMessage;//���P�^��,���P����

            public Image _image;
        }
        [Serializable]
        public class CharaSnsSetting
        {
            public Character mFungusChara;

            public SnsType mSnsType;

            public Direction mDirection;

        }

        public enum Direction
        {
            Right,
            Left
        }

        public enum SnsType // 
        {
            Message,
            Reply,
            Image,


        }
    }

    public class CharaDropOptions:PropertyAttribute
    {


    }

    public class SnsMessagePropAttribute : PropertyAttribute
    {


    }



}
