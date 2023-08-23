using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{

    public static class LeanTweenManager
    {
        // Start is called before the first frame update
        public static IEnumerator FadeIn(GameObject obj, float TweenTime = 0.25f)
        {
            CanvasGroup cg = obj.GetComponent<CanvasGroup>();

            if (cg == null)
            {
                cg = obj.AddComponent<CanvasGroup>();

            }
            cg.alpha = 0;
            LeanTween.alphaCanvas(cg, 1, TweenTime);
            yield return new WaitForSeconds(TweenTime);
        }

        public static IEnumerator FadeOut(GameObject obj, float TweenTime = 0.25f)
        {
            CanvasGroup cg = obj.GetComponent<CanvasGroup>();

            if (cg == null)
            {
                cg = obj.AddComponent<CanvasGroup>();

            }
            cg.alpha = 1;
            LeanTween.alphaCanvas(cg, 0, TweenTime);
            yield return new WaitForSeconds(TweenTime);


        }

    }
}

