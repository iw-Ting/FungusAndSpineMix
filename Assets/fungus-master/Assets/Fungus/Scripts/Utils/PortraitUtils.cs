// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using Spine.Unity;
using System;




// using Spine.Unity;

namespace Fungus
{
    /// <summary>
    /// Contains all options to run a portrait command.
    /// </summary>
    public class PortraitOptions
    {
        public Character character;
        public Character replacedCharacter;
        public Sprite portrait;
        public DisplayType display;
        public PositionOffset offset;
        public RectTransform fromPosition;
        public RectTransform toPosition;
        public FacingDirection facing;
        public bool useDefaultSettings;
        public float fadeDuration;
        public float moveDuration;
        public Vector2 shiftOffset;
        public bool move; //sets to position to be the same as from
        public bool shiftIntoPlace;
        public bool waitUntilFinished;
        public System.Action onComplete;

        public PortraitOptions(bool useDefaultSettings = true)
        {
            character = null;
            replacedCharacter = null;
            portrait = null;
            display = DisplayType.None;
            offset = PositionOffset.None;
            fromPosition = null;
            toPosition = null;
            facing = FacingDirection.None;
            shiftOffset = new Vector2(0, 0);
            move = false;
            shiftIntoPlace = false;
            waitUntilFinished = false;
            onComplete = null;

            // Special values that can be overridden
            fadeDuration = 0.5f;
            moveDuration = 1f;
            this.useDefaultSettings = useDefaultSettings;
        }
    }
    [Serializable]
    public class TweenTime
    {

        public float aMoveAniDuration = 0;
        public float aFadeAniDuration = 0;

        public float aAnimationPlayRoundTime=0;
        public float aFinishDuration = 0;//自動完成時間
    }

    public class SpineCharaAniOptions
    {
        public string _charaName="";
        public DisplayType _display;
        public SkeletonGraphic _SpineCharaPrefab;
        public SkeletonGraphic _ReplacedCharacter;
        public RectTransform _fromPosition = null;
        public RectTransform _toPosition = null;
        public RectTransform _clickPosition = null;
        public Vector2 _clickButtonSize;
        public System.Action _OnComplete;

        // public Color _color;

        public string _skin="";

        public string _animation="";

        public bool _reverse;//看的方向

        public bool _move=false; //sets to position to be the same as from
        public bool _loop=false;

        public bool _fade=false;

        public bool _waitForButton = false;
        public bool _waitAnimationFinish=false;//等待動畫完成後,接著撥放
        public ClickMode _clickMode;


        public bool _waitDialog=false;//等待對話完成
       // public bool _waitForClick = false;//點集才可進入下一段動畫
        public Vector2 _scale=Vector2.zero;
        public Vector3 _offest=Vector3.zero;

        ///////////////////////////  Chara   //////////////
        public GameObject CharaObj = null;

        public TweenTime aTween = new TweenTime();


        public void SetCharaSetting(bool setSkin=true)
        {
            if (CharaObj == null)
            {
                Debug.Log("No Chara ");
                return;
            }
            SkeletonGraphic skeleG = CharaObj.GetComponent<SkeletonGraphic>();
             if (_skin != skeleG.initialSkinName&&setSkin)//造型
            {
                skeleG.initialSkinName = _skin;
                skeleG.SetSkinToChara();
            }
            skeleG.startingLoop = _loop;
            skeleG.transform.localScale = _scale;
            // skeleG.startingAnimation = "<None>";
            //   skeleG.;



        }

        public void SetMyAnimation()//設置動畫，在執行時
        {
            if (CharaObj == null)
            {
                Debug.Log("No Chara ");
                return;
            }
            SkeletonGraphic skeleG = CharaObj.GetComponent<SkeletonGraphic>();

            skeleG.startingAnimation = _animation;
            skeleG.SetPlayAnimation();
            aTween.aAnimationPlayRoundTime=skeleG.GetPlayAnimationTime();

        }

    }

    /// <summary>
    /// Represents the current state of a character portrait on the stage.
    /// </summary>
    public class PortraitState
    {
        public bool onScreen;
        public bool dimmed;
        public DisplayType display;
        public RectTransform position, holder;
        public FacingDirection facing;
        public Image portraitImage;
        public Sprite portrait { get { return portraitImage != null ? portraitImage.sprite : null; } }
        public List<Image> allPortraits = new List<Image>();

        public void SetPortraitImageBySprite(Sprite portrait)
        {
            portraitImage = allPortraits.Find(x => x.sprite == portrait);
        }
    }

    /// <summary>
    /// Util functions for working with portraits.
    /// </summary>
    public static class PortraitUtil
    {
        #region Public members

        /// <summary>
        /// Convert a Moonsharp table to portrait options
        /// If the table returns a null for any of the parameters, it should keep the defaults
        /// </summary>
        /// <param name="table">Moonsharp Table</param>
        /// <param name="stage">Stage</param>
        /// <returns></returns>
        public static PortraitOptions ConvertTableToPortraitOptions(Table table, Stage stage)
        {
            PortraitOptions options = new PortraitOptions(true);

            // If the table supplies a nil, keep the default
            options.character = table.Get("character").ToObject<Character>()
                ?? options.character;

            options.replacedCharacter = table.Get("replacedCharacter").ToObject<Character>()
                ?? options.replacedCharacter;

            if (!table.Get("portrait").IsNil())
            {
                options.portrait = options.character.GetPortrait(table.Get("portrait").CastToString());
            }

            if (!table.Get("display").IsNil())
            {
                options.display = table.Get("display").ToObject<DisplayType>();
            }

            if (!table.Get("offset").IsNil())
            {
                options.offset = table.Get("offset").ToObject<PositionOffset>();
            }

            if (!table.Get("fromPosition").IsNil())
            {
                options.fromPosition = stage.GetPosition(table.Get("fromPosition").CastToString());
            }

            if (!table.Get("toPosition").IsNil())
            {
                options.toPosition = stage.GetPosition(table.Get("toPosition").CastToString());
            }

            if (!table.Get("facing").IsNil())
            {
                var facingDirection = FacingDirection.None;
                DynValue v = table.Get("facing");
                if (v.Type == DataType.String)
                {
                    if (string.Compare(v.String, "left", true) == 0)
                    {
                        facingDirection = FacingDirection.Left;
                    }
                    else if (string.Compare(v.String, "right", true) == 0)
                    {
                        facingDirection = FacingDirection.Right;
                    }
                }
                else
                {
                    facingDirection = table.Get("facing").ToObject<FacingDirection>();
                }

                options.facing = facingDirection;
            }

            if (!table.Get("useDefaultSettings").IsNil())
            {
                options.useDefaultSettings = table.Get("useDefaultSettings").CastToBool();
            }

            if (!table.Get("fadeDuration").IsNil())
            {
                options.fadeDuration = table.Get("fadeDuration").ToObject<float>();
            }

            if (!table.Get("moveDuration").IsNil())
            {
                options.moveDuration = table.Get("moveDuration").ToObject<float>();
            }

            if (!table.Get("move").IsNil())
            {
                options.move = table.Get("move").CastToBool();
            }
            else if (options.fromPosition != options.toPosition)
            {
                options.move = true;
            }

            if (!table.Get("shiftIntoPlace").IsNil())
            {
                options.shiftIntoPlace = table.Get("shiftIntoPlace").CastToBool();
            }

            if (!table.Get("waitUntilFinished").IsNil())
            {
                options.waitUntilFinished = table.Get("waitUntilFinished").CastToBool();
            }

            return options;
        }


        static public int PortraitCompareTo(Sprite x, Sprite y)
        {
            if (x == y)
                return 0;
            if (y == null)
                return -1;
            if (x == null)
                return 1;

            return x.name.CompareTo(y.name);
        }

        #endregion
    }
}
