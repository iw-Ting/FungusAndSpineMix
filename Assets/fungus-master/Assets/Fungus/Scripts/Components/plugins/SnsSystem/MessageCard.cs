using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Fungus
{
    public class MessageCard : MonoBehaviour
    {

        [SerializeField] private Image AvatarImage;

        [SerializeField] private GameObject ConversationCloud;

        [SerializeField] private Text PlayerName;

        [SerializeField] private Transform MessageContentListParent;

        [SerializeField] private Transform ViewPortParent;

        [SerializeField] private GameObject MessagePrefab = null;

        private SnsManager.SnsMessage aData;


        public IEnumerator Init(SnsManager.SnsMessage message)
        {
          
            aData = message;
            ViewPortParent = transform.parent.parent;

            StartCoroutine(SendMessage());

            yield return null;
        }

        private IEnumerator SendMessage()
        {

            switch (aData.mChara.mDirection)
            {//預設為左

                case SnsManager.Direction.Right:
                    PlayerName.alignment = TextAnchor.MiddleRight;
                    AvatarImage.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(425, -125);
                    MessageContentListParent.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperRight;
                    MessageContentListParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(300, 0);
                    ConversationCloud.GetComponent<RectTransform>().anchoredPosition = new Vector2(300, -120);
                    ConversationCloud.transform.eulerAngles = new Vector3(0, 180, 0);
                    break;

                case SnsManager.Direction.Left:
                    PlayerName.alignment = TextAnchor.MiddleLeft;
                    AvatarImage.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-425, -125);
                    MessageContentListParent.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
                    MessageContentListParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, 0);
                    ConversationCloud.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, -120);
                    break;
            }

            if (!aData.NotHaveAvatar)
            {
                AvatarImage.sprite = aData.mChara.mAvatar;
                PlayerName.text = aData.mChara.mName;
            }



            GameObject sp = Instantiate(MessagePrefab);

            sp.transform.SetParent(MessageContentListParent, false);

            RectTransform BgRect = sp.GetComponent<RectTransform>();
            Transform child = null;

            if (aData.mMessageType._snsType==SnsManager.SnsType.Image) {

                child = sp.transform.Find("ContentImage");
                child.gameObject.SetActive(true);
                sp.transform.Find("ContentText").gameObject.SetActive(false);
            }
            else
            {
                child = sp.transform.Find("ContentText");
                child.GetComponent<Text>().text = aData.mMessageType._message;
            }

            RectTransform ChildTextRect = child.GetComponent<RectTransform>();

            CanvasGroup cg = null;

            if (!aData.NotHaveAvatar)
            {
                cg = transform.GetComponent<CanvasGroup>();
                if (!cg)
                {
                    cg = gameObject.AddComponent<CanvasGroup>();
                }
            }
            else
            {
                cg = sp.transform.GetComponent<CanvasGroup>();

                if (!cg)
                {
                    cg = sp.gameObject.AddComponent<CanvasGroup>();
                }
            }
            cg.alpha = 0;

            LayoutRebuilder.ForceRebuildLayoutImmediate(ChildTextRect);
            //yield return new WaitForEndOfFrame();
            if (aData.mMessageType._snsType == SnsManager.SnsType.Image)
            {
                Image img = child.GetComponent<Image>();
                img.preserveAspect = true;

                img.sprite = aData.mMessageType._sprite;



            }
            else
            {
                if (ChildTextRect.sizeDelta.x > 550) //鎖住文字框寬度
                {
                    child.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                    ChildTextRect.sizeDelta = new Vector2(550, ChildTextRect.sizeDelta.y);
                    //yield return new WaitForEndOfFrame();
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(ChildTextRect);
            BgRect.sizeDelta = new Vector2(ChildTextRect.sizeDelta.x + 40, ChildTextRect.sizeDelta.y + 40);

            LayoutRebuilder.ForceRebuildLayoutImmediate(BgRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(MessageContentListParent.GetComponent<RectTransform>());
            // yield return new WaitForEndOfFrame();

            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1050, MessageContentListParent.GetComponent<RectTransform>().sizeDelta.y + 70);

            Transform OrigineTransform = null;
            OrigineTransform = gameObject.transform.parent;

            gameObject.transform.SetParent(OrigineTransform.parent);

            gameObject.transform.SetParent(OrigineTransform);


            if (aData.aFade) {
                if (!aData.NotHaveAvatar )
                 {
                    yield return LeanTweenManager.FadeIn(gameObject, 0.2f, () => { });
                 }
                 else
                 {
                    yield return LeanTweenManager.FadeIn(sp, 0.2f, () => { });
                 }
             }

        
            cg.alpha = 1;

        }



    }
}