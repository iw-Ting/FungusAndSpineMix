using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Spine.Unity;



public static class SpineTween
{


    public static IEnumerator SpineSkeletonGraphicFadeIn(GameObject obj, float dur)
    {
        SkeletonGraphic data = null;
        if (obj.TryGetComponent<SkeletonGraphic>(out data))
        {

            Color origineC = data.color;
            float addColorA = 0;
            float offValue = data.color.a / dur * Time.deltaTime;

            data.color = new Color(origineC.r, origineC.g, origineC.b, 0);

            while (addColorA < origineC.a)
            {

                if (data != null)
                {
                    addColorA = addColorA + offValue > 1 ? 1 : addColorA + offValue;

                    data.color = new Color(origineC.r, origineC.g, origineC.b, addColorA);

                    yield return null;
                }
                else
                {
                    yield break;
                }

            }
            data.color = origineC;


        }
        else
        {
            Debug.Log("Get Script On Fail");
            yield break;
        }
    }

    public static IEnumerator SpineSkeletonGraphicFadeOut(GameObject obj, float dur)
    {
        SkeletonGraphic data = null;

        if (obj.TryGetComponent<SkeletonGraphic>(out data))
        {

            Color origineC = data.color;
            float addColorA = origineC.a;
            float offValue = data.color.a / dur * Time.fixedDeltaTime;

            while (addColorA > 0)
            {

                if (data != null)
                {
                    addColorA = addColorA - offValue < 0 ? 0 : addColorA - offValue;

                    data.color = new Color(origineC.r, origineC.g, origineC.b, addColorA);

                    yield return null;
                }
                else
                {
                    yield break;
                }
            }
            data.color = new Color(origineC.r, origineC.g, origineC.b, 0);


        }
        else
        {
            Debug.Log("Get Script On Fail");
            yield break;
        }
    }

}







