using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using Spine.Unity;
using System;


public enum ClickAeraSetting
{
    Default,
    Customize
}


[CommandInfo("Narrative",
              "SpineCharaAni",//顯示名稱 類別是抓繼承command
              "Controls a Spine Animation.")]
//flow command
public class SpineCharaAni : ControlWithDisplay<DisplayType>
{
    // Start is called before the first frame update
    public CharaSpine aTarget;

    public CharaSpine aReplacedTarget;//替換
    [SerializeField] protected Stage stage;
    [SerializeField] protected FacingDirection facing;

    [SerializeField] protected RectTransform fromPosition=null;

    [SerializeField] protected RectTransform toPosition=null;

    [SerializeField] protected RectTransform ClickPos=null;

    [SerializeField] protected  Vector2 ClickButtonSize=new Vector2(300,300);

    [SerializeField] protected ClickAeraSetting aClickButtonSizeSetting = ClickAeraSetting.Default;

    [SerializeField] protected int spineOrder = 0;
    [SerializeField] protected string aAnimation;//要執行的動畫
    [SerializeField] protected string aInitialSkinName;//裝備

    [SerializeField] protected Vector3 offest;//位移差,部分角色的位置需要特別調整

    [SerializeField] protected bool move;//是否移動

    [SerializeField] protected bool loop;//是否移動

    [SerializeField] protected bool waitAnimationFinish = false;//等待動畫完成後,接著撥放
    [SerializeField] protected bool waitDialog = false;

    //[SerializeField] protected bool waitForClick = false;//點集才可進入下一段動畫

   // [SerializeField] protected bool waitForButton = false;//必須點擊某處才會進入下一階段

    [SerializeField] protected ClickMode clickMode = ClickMode.Disabled;//必須點擊某處才會進入下一階段

    [SerializeField] protected bool fade = false;

    public bool StartDraw = false;

    [SerializeField] protected TweenTime aTween = new TweenTime();

    public virtual RectTransform FromPosition { get { return fromPosition; } set { fromPosition = value; } }

    public virtual RectTransform ToPosition { get { return toPosition; } set { toPosition = value; } }

    public virtual ClickMode mClickMode { get { return clickMode; } set { clickMode = value; } }

    public virtual Stage _Stage { get { return stage; } set { stage = value; } }

    public virtual bool Move { get { return move; } set { move = value; } }

    public virtual bool Fade { get { return fade; } set { fade = value; } }

    public override void OnEnter()//Cammand執行邏輯
    {
        
        if (stage == null)
        {
            // If no default specified, try to get any portrait stage in the scene

            stage = Stage.GetActiveStage();
            // If portrait stage does not exist, do nothing
            if (stage == null)
            {
                Continue();
                return;
            }
        }

        if (IsDisplayNone(display))
        {
            Continue();
            return;
        }

        SpineCharaAniOptions opt = new SpineCharaAniOptions();

        opt._charaName = aTarget.mSet.CharaName;
        opt.aTween = aTween;
        opt._SpineCharaPrefab = aTarget.aSkeletonGraphic;
        opt._offest = offest + aTarget.mSet.Offest;
        opt._scale = aTarget.mSet.Scale;
        opt._OnComplete = Continue;
        opt._waitAnimationFinish = waitAnimationFinish;
        opt._spineOrder = spineOrder;
       
        opt._clickButtonSize = ClickButtonSize;

        if (string.IsNullOrEmpty(aAnimation))//沒指定動畫
        {
            if (!string.IsNullOrEmpty(aTarget.DefaultAni))//有預設動畫
            {
                opt._animation=aTarget.DefaultAni;//使用預設動畫
            }
        }
        else
        {
            opt._animation = aAnimation;//使用指定動畫
        }

        if (string.IsNullOrEmpty(aInitialSkinName))//沒指定造型
        {
            if (!string.IsNullOrEmpty(aTarget.DefaultSkin))//有預設造型
            {
                opt._skin=aTarget.DefaultSkin;//使用預設造型
            }
        }
        else
        {
            opt._skin = aInitialSkinName;//使用指定造型
        }

        opt._display = display;
        opt._reverse = IsReverse();

        opt._move = Move;
        opt._loop = loop;
        opt._fade = fade;


        opt._clickMode = clickMode;

        // opt._waitForButton = waitForButton;
        // opt._waitForClick = waitForClick;

        if (toPosition==null) {
            toPosition = stage.DefaultPosition;
        }
        if (fromPosition==null) {
            fromPosition = stage.DefaultPosition;
        }
        opt._fromPosition = FromPosition;
        opt._toPosition = ToPosition;

        switch (clickMode)
        {
            case ClickMode.Disabled:
                break;
            case ClickMode.ClickAnywhere:
                break;
            case ClickMode.ClickOnDialog:
                break;
            case ClickMode.ClickOnButton:
                if (ClickPos==null) {
                ClickPos= stage.DefaultPosition;
                }
                opt._clickPosition = ClickPos;

                switch (aClickButtonSizeSetting)
                {
                    case ClickAeraSetting.Default:
                        opt._clickButtonSize = ClickPos.sizeDelta;
                        break;
                    case ClickAeraSetting.Customize:
                        opt._clickButtonSize = ClickButtonSize;
                        break;
                }
                break;
        }



        stage.RunSpineCommand(opt);

    }

    public void SetMyAnimation()//設置動畫，在執行時
    {
        if (aInitialSkinName != aTarget.aSkeletonGraphic.initialSkinName)//造型
        {
            aTarget.aSkeletonGraphic.initialSkinName = aInitialSkinName;
        }
        aTarget.aSkeletonGraphic.startingAnimation = aAnimation;

    }

    public bool IsReverse()//偵測角色是否需要反轉
    {
        if (aTarget.mSet.Facing == FacingDirection.None)
        {
            aTarget.mSet.Facing = FacingDirection.Right;
        }

        if (facing != aTarget.mSet.Facing)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void  DrawTexture()
    {
        if (!StartDraw) {
            return;
        }
        bool isThis = false;
        foreach (var com in Flowchart.GetInstance().SelectedCommands)
        {
            if (com == this)
            {
                isThis = true;
            }

        }

        if (!isThis) {
            return;
        }


          GameObject sp = new GameObject("displayRect", typeof(RectTransform));
           sp.transform.SetParent(_Stage.DefaultPosition.parent,false);
            
          RectTransform newRect = sp.GetComponent<RectTransform>();

    //    RectTransform newRect = new RectTransform();

        if (ClickPos!=null) {

            newRect.position = ClickPos.position;
            newRect.pivot = ClickPos.pivot;
        }
        else
        {

            newRect.position = _Stage.DefaultPosition.position;
            newRect.pivot = _Stage.DefaultPosition.pivot;
        }

        switch (aClickButtonSizeSetting)
        {

            case ClickAeraSetting.Default:
                if (ClickPos != null)
                {
                    newRect.sizeDelta = new Vector2(ClickPos.sizeDelta.x, ClickPos.sizeDelta.y);
                }
                else
                {
                    newRect.sizeDelta = new Vector2(_Stage.DefaultPosition.sizeDelta.x, _Stage.DefaultPosition.sizeDelta.y);
                }
                break;
            case ClickAeraSetting.Customize:
                newRect.sizeDelta = ClickButtonSize;
                break;

        }
        DrawGizmoLine.DrawTexture(newRect);
        DestroyImmediate(sp);
        

    }

    public void OnDrawGizmos()
    {
        DrawTexture();


    }




}
