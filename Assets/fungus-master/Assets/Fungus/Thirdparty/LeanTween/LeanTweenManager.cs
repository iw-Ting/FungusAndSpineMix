using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fungus
{

    public static class LeanTweenManager
    {
        // Start is called before the first frame update
        public static IEnumerator FadeIn(GameObject obj, float TweenTime = 0.25f,Action onComplete=null)
        {
            CanvasGroup cg = obj.GetComponent<CanvasGroup>();

            if (cg == null)
            {
                cg = obj.AddComponent<CanvasGroup>();

            }
            cg.alpha = 0;
            LeanTween.alphaCanvas(cg, 1, TweenTime);
            yield return new WaitForSeconds(TweenTime);
            if (onComplete!=null) {
            onComplete();
            }

        }
        public static IEnumerator FadeIn(GameObject obj, Action onComplete = null)
        {

            return FadeIn(obj, 0.25f, onComplete);
        
        }

        public static IEnumerator FadeIn(GameObject obj)
        {

            return FadeIn(obj, 0.25f, null);

        }

        public static IEnumerator FadeOut(GameObject obj, float TweenTime = 0.25f, Action onComplete = null)
        {
            CanvasGroup cg = obj.GetComponent<CanvasGroup>();

            if (cg == null)
            {
                cg = obj.AddComponent<CanvasGroup>();

            }
            cg.alpha = 1;
            LeanTween.alphaCanvas(cg, 0, TweenTime);
            yield return new WaitForSeconds(TweenTime);
            if (onComplete != null)
            {
                onComplete();
            }
        }
        public static IEnumerator FadeOut(GameObject obj,Action onComplete = null)
        {

            return FadeOut(obj, 0.25f, onComplete);


        }

        public static IEnumerator FadeOut(GameObject obj)
        {

            return FadeOut(obj, 0.25f, null);


        }




    }





}

