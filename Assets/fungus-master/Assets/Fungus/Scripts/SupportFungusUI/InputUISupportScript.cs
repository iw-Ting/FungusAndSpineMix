using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


namespace Fungus{

    public static class InputUISupportScript
    {

        public static GameObject CreateButtonArea(RectTransform createPos, Action clickAction)
        {
            Debug.Log("生成觸控區");
            RectTransform sp = new GameObject("Touch Area", typeof(RectTransform), typeof(Button),typeof(Image)).GetComponent<RectTransform>();
            sp.SetParent(createPos);
            sp.sizeDelta = createPos.sizeDelta;
            sp.anchoredPosition = Vector3.zero;
            sp.rotation = createPos.rotation;
            sp.localScale = Vector3.one;
            
            sp.GetComponent<Button>().onClick.AddListener(() => {
                Debug.Log("觸控成功!!!");
                clickAction();
            });


            return sp.gameObject;
        }












    }
}
