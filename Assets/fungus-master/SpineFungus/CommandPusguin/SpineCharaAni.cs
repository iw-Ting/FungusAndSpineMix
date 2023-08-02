using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using Spine.Unity;





[CommandInfo("Narrative",
              "SpineCharaAni",
              "Controls a Spine Animation.")]
//flow command
public class SpineCharaAni : ControlWithDisplay<DisplayType>
{
    // Start is called before the first frame update
    public CharaSpine aTarget;

    public CharaSpine aReplacedTarget;//替換
    [SerializeField] protected Stage stage;
    [SerializeField] protected FacingDirection facing;

    [SerializeField] protected RectTransform fromPosition;

    [SerializeField] protected RectTransform toPosition;

    [SerializeField] protected string aAnimation;//要執行的動畫
    [SerializeField] protected string aInitialSkinName;//裝備

    [SerializeField] protected Vector3 offest;//位移差,部分角色的位置需要特別調整

    [SerializeField] protected bool move;//是否移動

    [SerializeField] protected bool loop;//是否移動

    [SerializeField] protected bool waitAnimationFinish = false;//等待動畫完成後,接著撥放
    [SerializeField] protected bool waitDialog = false;

    [SerializeField] protected bool fade = false;

    [SerializeField] protected TweenTime aTween=new TweenTime();

    public virtual RectTransform FromPosition { get { return fromPosition; } set { fromPosition = value; } }

    public virtual RectTransform ToPosition { get { return toPosition; } set { toPosition = value; } }

    public virtual Stage _Stage { get { return stage; } set { stage = value; } }

    public virtual bool Move { get { return move; } set { move = value; } }

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


        opt.aTween=aTween;
        opt._SpineCharaPrefab = aTarget.aSkeletonGraphic;
        opt._offest = offest + aTarget.mSet.Offest;

        opt._scale = aTarget.mSet.Scale;
        opt._OnComplete = Continue;
        opt._waitAnimationFinish = waitAnimationFinish;
        opt._waitDialog = waitDialog;

        opt._animation = aAnimation;
        opt._skin = aInitialSkinName;
        opt._display = display;
        opt._reverse = IsReverse();

        opt._move = Move;
        opt._loop = loop;
        opt._fade = fade;
        opt._fromPosition = FromPosition;
        opt._toPosition = ToPosition;


        stage.RunPortraitCommand(opt);

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
        if(aTarget.mSet.facing==FacingDirection.None){
            aTarget.mSet.facing=FacingDirection.Right;
        }

        if (facing != aTarget.mSet.facing)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // private IEnumerator Display
}
