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

            Transform child = sp.transform.Find("ContentText");

            RectTransform ChildTextRect = child.GetComponent<RectTransform>();

            child.GetComponent<Text>().text = aData.mMessageType._message;

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
            
            if (ChildTextRect.sizeDelta.x > 550) //鎖住文字框寬度
            {
                child.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                ChildTextRect.sizeDelta = new Vector2(550, ChildTextRect.sizeDelta.y);

                LayoutRebuilder.ForceRebuildLayoutImmediate(ChildTextRect);
                //yield return new WaitForEndOfFrame();
            }

            BgRect.sizeDelta = new Vector2(ChildTextRect.sizeDelta.x + 50, ChildTextRect.sizeDelta.y + 50);

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