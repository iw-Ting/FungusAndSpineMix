using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using System;



namespace Fungus
{



    [Serializable]
    public class SpineSetting
    {

        public string CharaName;//顯示在對話框上的名字

        // public Sprite DefaultSayDialogSprite;//名字底下的圖片

        // public Color ColorName;
        public Vector2 Scale=Vector2.one;

        public Vector3 Offest=Vector3.zero;

        public FacingDirection Facing;

        

    }





    [ExecuteInEditMode]
    public class CharaSpine : MonoBehaviour, ILocalizable, IComparer<CharaSpine>//角色外框
    {

        public SpineSetting mSet;
        // [SerializeField] private string SkeletonAnimationPath = "";//加載資源的路徑

        public SkeletonGraphic aSkeletonGraphic;//skins 因為spine裡面已經有造型選擇 造理說只會有一個


        public string DefaultSkin=null;

        public string DefaultAni=null;
        // public IEnumerator LoadSkeletonAnimationPath()//確認加載
        // {
        //     if (aSkeletonGraphic == null)
        //     {
        //         ResourceRequest resRequest = Resources.LoadAsync<GameObject>(SkeletonAnimationPath);

        //         yield return new WaitUntil(() => resRequest.isDone);

        //         // var a = (resRequest.asset as GameObject).GetComponent<SkeletonAnimation>();

        //         SkeletonGraphic skele = null;

        //         if ((resRequest.asset as GameObject).TryGetComponent<SkeletonGraphic>(out skele))
        //         {
        //             aSkeletonGraphic = skele;
        //         }
        //         else
        //         {
        //             Debug.LogError("無效的路徑" + SkeletonAnimationPath);
        //         }



        //     }


        // }

        // public void Update(){

        //     // Debug.Log("偵測是否執行111");




        // }





        public int Compare(CharaSpine a, CharaSpine b)
        {
            return 1;
        }


        public string GetStandardText()
        {

            return "";
        }

        public void SetStandardText(string standardText)
        {

        }

        public string GetDescription()
        {
            return "";
        }

        public string GetStringId()
        {
            return "";
        }


    }
}

