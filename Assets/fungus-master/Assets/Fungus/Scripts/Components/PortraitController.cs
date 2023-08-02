// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using MoonSharp.Interpreter;



namespace Fungus
{
    /// <summary>
    /// Types of display operations supported by portraits.
    /// </summary>
    public enum DisplayType//圖片顯示模式
    {
        /// <summary> Do nothing. </summary>
        None,
        /// <summary> Show the portrait. </summary>
        Show,
        /// <summary> Hide the portrait. </summary>
        Hide,
        /// <summary> Replace the existing portrait. </summary>
        Replace,
        /// <summary> Move portrait to the front. </summary>
        MoveToFront
    }

    /// <summary>
    /// Directions that character portraits can face.
    /// </summary>
    public enum FacingDirection
    {
        /// <summary> Unknown direction </summary>
        None,
        /// <summary> Facing left. </summary>
        Left,
        /// <summary> Facing right. </summary>
        Right
    }

    /// <summary>
    /// Offset direction for position.
    /// </summary>
    public enum PositionOffset
    {
        /// <summary> Unknown offset direction. </summary>
        None,
        /// <summary> Offset applies to the left. </summary>
        OffsetLeft,
        /// <summary> Offset applies to the right. </summary>
        OffsetRight
    }


    [AddComponentMenu("")]
    /// <summary>
    /// Controls the Portrait sprites on stage. 
    /// 
    /// Is only really used via Stage, it's child class. This class continues to exist to support existing API
    /// dependant code. All functionality is stage dependant.
    /// </summary>
    public class PortraitController : MonoBehaviour
    {
        // Timer for waitUntilFinished functionality
        protected float waitTimer;

        protected Stage stage;

        protected virtual void Awake()
        {
            stage = GetComponentInParent<Stage>();
        }

        protected virtual void FinishCommand(PortraitOptions options)
        {
            if (options.onComplete != null)
            {
                if (!options.waitUntilFinished)
                {
                    options.onComplete();
                }
                else
                {
                    StartCoroutine(WaitUntilFinished(options.fadeDuration, options.onComplete));
                }
            }
            else
            {
                StartCoroutine(WaitUntilFinished(options.fadeDuration));
            }
        }

        protected virtual void FinishCommand(SpineCharaAniOptions options)
        {
            if (options._OnComplete != null)
            {
                if (options.aTween.aFinishDuration < 0)
                {
                    options._OnComplete();
                }
                else
                {
                    StartCoroutine(WaitUntilFinished(options.aTween.aFinishDuration, options._OnComplete));
                }
            }
            else
            {
                StartCoroutine(WaitUntilFinished(options.aTween.aFinishDuration));
            }
        }

        /// <summary>
        /// Makes sure all options are set correctly so it won't break whatever command it's sent to
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        //*檢查設定並生成角色。
        protected virtual PortraitOptions CleanPortraitOptions(PortraitOptions options)
        {
            // Use default stage settings
            if (options.useDefaultSettings)
            {
                options.fadeDuration = stage.FadeDuration;
                options.moveDuration = stage.MoveDuration;
                options.shiftOffset = stage.ShiftOffset;
            }

            // if no previous portrait, use default portrait
            if (options.character.State.portrait == null)
            {
                options.character.State.SetPortraitImageBySprite(options.character.ProfileSprite);
            }

            // Selected "use previous portrait"
            if (options.portrait == null)
            {
                options.portrait = options.character.State.portrait;
            }

            // if no previous position, use default position
            if (options.character.State.position == null)
            {
                options.character.State.position = stage.DefaultPosition.rectTransform;
            }

            // Selected "use previous position"
            if (options.toPosition == null)
            {
                options.toPosition = options.character.State.position;
            }

            if (options.replacedCharacter != null)
            {
                // if no previous position, use default position
                if (options.replacedCharacter.State.position == null)
                {
                    options.replacedCharacter.State.position = stage.DefaultPosition.rectTransform;
                }
            }

            // If swapping, use replaced character's position
            if (options.display == DisplayType.Replace)
            {
                options.toPosition = options.replacedCharacter.State.position;
            }

            // Selected "use previous position"
            if (options.fromPosition == null)
            {
                options.fromPosition = options.character.State.position;
            }

            // if portrait not moving, use from position is same as to position
            if (!options.move)
            {
                options.fromPosition = options.toPosition;
            }

            if (options.display == DisplayType.Hide)
            {
                options.fromPosition = options.character.State.position;
            }

            // if no previous facing direction, use default facing direction
            if (options.character.State.facing == FacingDirection.None)
            {
                options.character.State.facing = options.character.PortraitsFace;
            }

            // Selected "use previous facing direction"
            if (options.facing == FacingDirection.None)
            {
                options.facing = options.character.State.facing;
            }

            if (options.character.State.portraitImage == null)
            {
                CreatePortraitObject(options.character);
            }

            return options;
        }


        protected virtual SpineCharaAniOptions CleanPortraitOptions(SpineCharaAniOptions options)
        {
            // Use default stage settings
            if (options.aTween.aMoveAniDuration <= 0)
            {
                options.aTween.aMoveAniDuration = 0.5f;
            }

            if (options.aTween.aFadeAniDuration <= 0)
            {
                options.aTween.aFadeAniDuration = 0.4f;
            }

            if (options.CharaObj == null)
            {
                CreatePortraitObject(options);
            }

            return options;
        }

        /// <summary> 
        /// Creates and sets the portrait image for a character
        /// </summary>
        /// <param name="character"></param>
        /// <param name="fadeDuration"></param>
        protected virtual void CreatePortraitObject(Character character)
        {
            if (character.State.holder == null)//生成角色
            {
                character.State.holder = new GameObject(character.name + " holder",
                                                   typeof(RectTransform)
                                                   //typeof(CanvasRenderer),
                                                   //typeof(Image)
                                                   ).GetComponent<RectTransform>();

                // Set it to be a child of the stage
                character.State.holder.transform.SetParent(stage.PortraitCanvas.transform, false);

                SetRectTransform(character.State.holder, stage.DefaultPosition.GetComponent<RectTransform>());
            }

            if (character.State.allPortraits.Count == 0)
            {
                foreach (var item in character.Portraits)//生成該角色圖像
                {
                    if (item == null)
                    {
                        Debug.LogError("null in portrait list on character " + character.name);
                        continue;
                    }
                    // Create a new portrait object
                    GameObject po = new GameObject(item.name,
                                                            typeof(RectTransform),
                                                            typeof(CanvasRenderer),
                                                            typeof(Image));

                    // Set it to be a child of the stage
                    po.transform.SetParent(character.State.holder, false);

                    // Configure the portrait image
                    Image pi = po.GetComponent<Image>();
                    pi.preserveAspect = true;
                    pi.sprite = item;
                    pi.color = new Color(1f, 1f, 1f, 0f);

                    if (item == character.ProfileSprite)
                    {
                        character.State.portraitImage = pi;
                    }

                    //expand to fit parent
                    RectTransform rt = po.GetComponent<RectTransform>();
                    rt.sizeDelta = Vector2.zero;
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.pivot = Vector2.one * 0.5f;
                    rt.ForceUpdateRectTransforms();

                    po.SetActive(false);

                    character.State.allPortraits.Add(pi);
                }
            }
        }

        protected virtual void CreatePortraitObject(SpineCharaAniOptions opt)
        {
            if (opt.CharaObj == null)//生成角色
            {
                bool listIsHave = false;
                foreach (var chara in stage.SpineCharaOnStageList)
                {
                    if (opt._SpineCharaPrefab.name == chara.name)
                    {
                        opt.CharaObj = chara;
                        listIsHave = true;
                    }
                }

                if (opt._display == DisplayType.Hide && opt.CharaObj == null)
                {
                    Debug.Log("抓不到角色");
                    return;
                }

                if (!listIsHave)
                {
                    opt.CharaObj = Instantiate(opt._SpineCharaPrefab.gameObject);
                    opt.CharaObj.name = opt._SpineCharaPrefab.gameObject.name;

                    // Set it to be a child of the stage
                    opt.CharaObj.transform.SetParent(stage.PortraitCanvas.transform, false);

                    stage.SpineCharaOnStageList.Add(opt.CharaObj);

                }

                // opt._SpineChara.color = new Color(1f, 1f, 1f, 1f);
                if (opt._move)
                {
                    SetRectTransform(opt.CharaObj.GetComponent<RectTransform>(), opt._fromPosition);
                }
                else if (opt._toPosition != null)
                {

                    SetRectTransform(opt.CharaObj.GetComponent<RectTransform>(), opt._toPosition);
                }
                else
                {
                    SetRectTransform(opt.CharaObj.GetComponent<RectTransform>(), stage.DefaultPosition.GetComponent<RectTransform>());
                }

                opt.CharaObj.GetComponent<RectTransform>().localPosition = opt.CharaObj.GetComponent<RectTransform>().localPosition + opt._offest;

            }

        }

        protected virtual IEnumerator WaitUntilFinished(float duration, Action onComplete = null)
        {
            // Wait until the timer has expired
            // Any method can modify this timer variable to delay continuing.

            waitTimer = duration;
            while (waitTimer > 0f)
            {
                waitTimer -= Time.deltaTime;
                yield return null;
            }

            // Wait until next frame just to be safe
            yield return new WaitForEndOfFrame();

            if (onComplete != null)
            {
                onComplete();
            }
        }

        protected virtual void SetupPortrait(PortraitOptions options)//處理鏡像
        {
            if (options.character.State.holder == null)
                return;

            SetRectTransform(options.character.State.holder, options.fromPosition);

            if (options.character.State.facing != options.character.PortraitsFace)
            {
                options.character.State.holder.localScale = new Vector3(-1f, 1f, 1f);
            }
            else
            {
                options.character.State.holder.localScale = new Vector3(1f, 1f, 1f);
            }

            if (options.facing != options.character.PortraitsFace)
            {
                options.character.State.holder.localScale = new Vector3(-1f, 1f, 1f);
            }
            else
            {
                options.character.State.holder.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        protected virtual void SetupPortrait(SpineCharaAniOptions options)//處理鏡像
        {
            if (options._SpineCharaPrefab == null)
            {
                return;
            }
            if (options.CharaObj == null)
            {
                Debug.Log("***Not Have Instance");//無生產實例
                return;
            }

            SetRectTransform(options._SpineCharaPrefab.GetComponent<RectTransform>(), options._fromPosition);
            if(options._reverse){

                options.CharaObj.transform.localEulerAngles = new Vector3(0, options.CharaObj.transform.localEulerAngles.y+180, 0);
            }

        }

        protected virtual void DoMoveTween(PortraitOptions options)
        {
            CleanPortraitOptions(options);

            LeanTween.cancel(options.character.State.holder.gameObject);

            // LeanTween doesn't handle 0 duration properly
            float duration = (options.moveDuration > 0f) ? options.moveDuration : float.Epsilon;

            // LeanTween.move uses the anchoredPosition, so all position images must have the same anchor position
            LeanTween.move(options.character.State.holder.gameObject, options.toPosition.position, duration)
                .setEase(stage.FadeEaseType);

            if (options.waitUntilFinished)
            {
                waitTimer = duration;
            }
        }

        protected virtual void DoMoveTween(SpineCharaAniOptions options)
        {
            // CleanPortraitOptions(options);

            LeanTween.cancel(options.CharaObj);

            // LeanTween doesn't handle 0 duration properly

            // LeanTween.move uses the anchoredPosition, so all position images must have the same anchor position
            options._toPosition.localPosition = options._toPosition.localPosition + options._offest;
            LeanTween.move(options.CharaObj, options._toPosition.position, options.aTween.aMoveAniDuration)
                .setEase(stage.FadeEaseType);

        }

        /// <summary>
        /// Performs a deep copy of all values from one RectTransform to another.
        /// </summary>
        public static void SetRectTransform(RectTransform target, RectTransform from)
        {
            target.eulerAngles = from.eulerAngles;
            target.position = from.position;
            target.rotation = from.rotation;
            target.anchoredPosition = from.anchoredPosition;
            target.sizeDelta = from.sizeDelta;
            target.anchorMax = from.anchorMax;
            target.anchorMin = from.anchorMin;
            target.pivot = from.pivot;
            target.localScale = from.localScale;
        }

        /// <summary>
        /// Using all portrait options available, run any portrait command.
        /// </summary>
        /// <param name="options">Portrait Options</param>
        /// <param name="onComplete">The function that will run once the portrait command finishes</param>
        public virtual void RunPortraitCommand(PortraitOptions options, Action onComplete)//設定頭像的zoom //判斷是否有誤
        {
            waitTimer = 0f;

            // If no character specified, do nothing
            if (options.character == null)
            {
                onComplete();
                return;
            }

            // If Replace and no replaced character specified, do nothing
            if (options.display == DisplayType.Replace && options.replacedCharacter == null)
            {
                onComplete();
                return;
            }

            // Early out if hiding a character that's already hidden
            if (options.display == DisplayType.Hide &&
                !options.character.State.onScreen)
            {
                onComplete();
                return;
            }

            options = CleanPortraitOptions(options);
            options.onComplete = onComplete;

            switch (options.display)
            {
                case (DisplayType.Show):
                    Show(options);
                    break;

                case (DisplayType.Hide):
                    Hide(options);
                    break;

                case (DisplayType.Replace):
                    Show(options);
                    Hide(options.replacedCharacter, options.replacedCharacter.State.position.name);
                    break;

                case (DisplayType.MoveToFront):
                    MoveToFront(options);
                    break;
            }
        }

        public virtual void RunPortraitCommand(SpineCharaAniOptions options)
        {
            waitTimer = 0f;
            // If no character specified, do nothing
            if (options._SpineCharaPrefab == null)
            {
                options._OnComplete();
                return;
            }

            // If Replace and no replaced character specified, do nothing
            if (options._display == DisplayType.Replace && options._ReplacedCharacter == null)
            {
                options._OnComplete();
                return;
            }

            options = CleanPortraitOptions(options);

            if (options._display == DisplayType.Hide && options.CharaObj == null)
            {
                options._OnComplete();
                return;
            }

            switch (options._display)
            {
                case (DisplayType.Show):
                    StartCoroutine(Show(options));
                    break;

                case (DisplayType.Hide):
                    StartCoroutine(Hide(options));
                    break;

                case (DisplayType.Replace):
                    // Show(options);
                    // Hide(options.replacedCharacter, options.replacedCharacter.State.position.name);
                    break;

                case (DisplayType.MoveToFront):
                    // MoveToFront(options);
                    break;
            }




        }
        /// <summary>
        /// Moves Character in front of other characters on stage
        /// </summary>
        public virtual void MoveToFront(PortraitOptions options)
        {
            options.character.State.holder.SetSiblingIndex(options.character.State.holder.parent.childCount);
            options.character.State.display = DisplayType.MoveToFront;
            FinishCommand(options);
        }

        public virtual void MoveToFront(SpineCharaAniOptions options)
        {
            options.CharaObj.transform.SetSiblingIndex(options.CharaObj.transform.parent.childCount);
            // options.character.State.display = DisplayType.MoveToFront;
            // FinishCommand(options);
        }
        /// <summary>
        /// Show portrait with the supplied portrait options
        /// </summary>
        /// <param name="options"></param>
        public virtual void Show(PortraitOptions options)//展現隱藏的角色
        {
            options = CleanPortraitOptions(options);

            if (options.shiftIntoPlace)
            {
                options.fromPosition = Instantiate(options.toPosition) as RectTransform;
                if (options.offset == PositionOffset.OffsetLeft)
                {
                    options.fromPosition.anchoredPosition =
                        new Vector2(options.fromPosition.anchoredPosition.x - Mathf.Abs(options.shiftOffset.x),
                            options.fromPosition.anchoredPosition.y - Mathf.Abs(options.shiftOffset.y));
                }
                else if (options.offset == PositionOffset.OffsetRight)
                {
                    options.fromPosition.anchoredPosition =
                        new Vector2(options.fromPosition.anchoredPosition.x + Mathf.Abs(options.shiftOffset.x),
                            options.fromPosition.anchoredPosition.y + Mathf.Abs(options.shiftOffset.y));
                }
                else
                {
                    options.fromPosition.anchoredPosition = new Vector2(options.fromPosition.anchoredPosition.x, options.fromPosition.anchoredPosition.y);
                }
            }

            SetupPortrait(options);

            // LeanTween doesn't handle 0 duration properly
            float duration = (options.fadeDuration > 0f) ? options.fadeDuration : float.Epsilon;

            var prevPortrait = options.character.State.portrait;

            if (options.character.State.portrait != null && options.character.State.portrait != options.portrait)
            {
                HidePortrait(options.character.State.portraitImage.rectTransform, duration);
            }

            options.character.State.SetPortraitImageBySprite(options.portrait);
            options.character.State.portraitImage.rectTransform.gameObject.SetActive(true);

            if (options.character.State.portraitImage.color != Color.white)
            {

                LeanTween.color(options.character.State.portraitImage.rectTransform, Color.white, duration)
                    .setEase(stage.FadeEaseType)
                    .setRecursive(false);
            }

            var a = LeanTween.alpha(options.character.State.portraitImage.rectTransform, 1f, duration)
                 .setEase(stage.FadeEaseType)
                 .setRecursive(false);


            DoMoveTween(options);

            FinishCommand(options);

            if (!stage.CharactersOnStage.Contains(options.character))
            {
                stage.CharactersOnStage.Add(options.character);
            }

            MoveToFront(options);

            // Update character state after showing
            options.character.State.onScreen = true;
            options.character.State.display = DisplayType.Show;
            options.character.State.facing = options.facing;
            options.character.State.position = options.toPosition;
        }

        public virtual IEnumerator Show(SpineCharaAniOptions options)//展現隱藏的角色(SpineChara)確認角色存在,展示角色
        {
            options = CleanPortraitOptions(options);//生成

            SetupPortrait(options);//鏡像設置

            // LeanTween doesn't handle 0 duration properly
            MoveToFront(options);//將目標設置在前面
            options.SetCharaSetting();
            if (options._move)
            {
                DoMoveTween(options);
            }
            if (options._fade)
            {
                // StartCoroutine(SpineTween.SpineSkeletonGraphicFadeIn(options.CharaObj,options.aTween.aMoveAniDuration));
                StartCoroutine(SpineTween.SpineSkeletonGraphicFadeIn(options.CharaObj, options.aTween.aFadeAniDuration));
                // LeanTween.color(options.CharaObj, Color.white, options.aTween.aMoveAniDuration)
                //                     .setEase(stage.FadeEaseType)
                //                     .setRecursive(false);


            }


            if (options._move || options._fade)
            {
                float waitTime = 0;
                if (options._move && options._fade)
                {
                    // waitTime=options.aTween.aFadeAniDuration>options.aTween.aMoveAniDuration?options.aTween.aFadeAniDuration:options.aTween.aMoveAniDuration;
                    waitTime = Mathf.Max(options.aTween.aFadeAniDuration, options.aTween.aMoveAniDuration);
                }
                else if (options._move && !options._fade)
                {
                    waitTime = options.aTween.aMoveAniDuration;
                }
                else
                {
                    waitTime = options.aTween.aFadeAniDuration;
                }

                yield return new WaitForSeconds(waitTime);//位移動畫或淡入
            }

            options.SetMyAnimation();

            if (options._waitAnimationFinish)
            {
                yield return new WaitForSeconds(options.aTween.aAnimationPlayRoundTime);//等待一輪動畫結束
            }

            FinishCommand(options);

        }

        protected virtual void HidePortrait(RectTransform rectTransform, float duration)
        {
            LeanTween.alpha(rectTransform, 0f, duration)
                .setEase(stage.FadeEaseType)
                .setRecursive(false)
                .setOnComplete(() => rectTransform.gameObject.SetActive(false));
        }

        /// <summary>
        /// Hide portrait with provided options
        /// </summary>
        public virtual void Hide(PortraitOptions options)
        {
            CleanPortraitOptions(options);

            if (options.character.State.display == DisplayType.None)
            {
                return;
            }

            SetupPortrait(options);

            // LeanTween doesn't handle 0 duration properly
            float duration = (options.fadeDuration > 0f) ? options.fadeDuration : float.Epsilon;

            HidePortrait(options.character.State.portraitImage.rectTransform, duration);

            DoMoveTween(options);

            //update character state after hiding
            options.character.State.onScreen = false;
            options.character.State.facing = options.facing;
            options.character.State.position = options.toPosition;
            options.character.State.display = DisplayType.Hide;

            if (stage.CharactersOnStage.Remove(options.character))
            {
            }

            FinishCommand(options);
        }

        public virtual IEnumerator Hide(SpineCharaAniOptions options)
        {
            CleanPortraitOptions(options);

            if (options._display == DisplayType.None)
            {
                yield break;
            }

            options.SetCharaSetting(false);
            SetupPortrait(options);//處理鏡像

            if (options._move)
            {
                DoMoveTween(options);
            }

            if (options._fade)
            {

                StartCoroutine(SpineTween.SpineSkeletonGraphicFadeOut(options.CharaObj, options.aTween.aFadeAniDuration));
            }

            if (options._move || options._fade)
            {
                float waitTime = 0;
                if (options._move && options._fade)
                {
                    // waitTime=options.aTween.aFadeAniDuration>options.aTween.aMoveAniDuration?options.aTween.aFadeAniDuration:options.aTween.aMoveAniDuration;
                    waitTime = Mathf.Max(options.aTween.aFadeAniDuration, options.aTween.aMoveAniDuration);
                }
                else if (options._move && !options._fade)
                {
                    waitTime = options.aTween.aMoveAniDuration;
                }
                else
                {
                    waitTime = options.aTween.aFadeAniDuration;
                }
                yield return new WaitForSeconds(waitTime);//位移動畫或淡入
            }

            Destroy(options.CharaObj);
            stage.SpineCharaOnStageList.Remove(options.CharaObj);
            FinishCommand(options);
        }

        /// <summary>
        /// Sets the dimmed state of a character on the stage.
        /// </summary>
        public virtual void SetDimmed(Character character, bool dimmedState)
        {
            if (character.State.dimmed == dimmedState)
            {
                return;
            }

            character.State.dimmed = dimmedState;

            Color targetColor = dimmedState ? stage.DimColor : Color.white;

            // LeanTween doesn't handle 0 duration properly
            float duration = (stage.FadeDuration > 0f) ? stage.FadeDuration : float.Epsilon;

            LeanTween.color(character.State.portraitImage.rectTransform, targetColor, duration).setEase(stage.FadeEaseType).setRecursive(false);
        }

        #region Overloads and Helpers

        /// <summary>
        /// Shows character at a named position in the stage
        /// </summary>
        /// <param name="character"></param>
        /// <param name="position">Named position on stage</param>
        public virtual void Show(Character character, string position)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.fromPosition = options.toPosition = stage.GetPosition(position);

            Show(options);
        }

        /// <summary>
        /// Shows character moving from a position to a position
        /// </summary>
        /// <param name="character"></param>
        /// <param name="portrait"></param>
        /// <param name="fromPosition">Where the character will appear</param>
        /// <param name="toPosition">Where the character will move to</param>
        public virtual void Show(Character character, string portrait, string fromPosition, string toPosition)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.portrait = character.GetPortrait(portrait);
            options.fromPosition = stage.GetPosition(fromPosition);
            options.toPosition = stage.GetPosition(toPosition);
            options.move = true;

            Show(options);
        }

        /// <summary>
        /// From lua, you can pass an options table with named arguments
        /// example:
        ///     stage.show{character=jill, portrait="happy", fromPosition="right", toPosition="left"}
        /// Any option available in the PortraitOptions is available from Lua
        /// </summary>
        /// <param name="optionsTable">Moonsharp Table</param>
        public virtual void Show(Table optionsTable)
        {
            Show(PortraitUtil.ConvertTableToPortraitOptions(optionsTable, stage));
        }


        /// <summary>
        /// Simple show command that shows the character with an available named portrait
        /// </summary>
        /// <param name="character">Character to show</param>
        /// <param name="portrait">Named portrait to show for the character, i.e. "angry", "happy", etc</param>
        public virtual void ShowPortrait(Character character, string portrait)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.portrait = character.GetPortrait(portrait);

            if (character.State.position == null)
            {
                options.toPosition = options.fromPosition = stage.GetPosition("middle");
            }
            else
            {
                options.fromPosition = options.toPosition = character.State.position;
            }

            Show(options);
        }

        /// <summary>
        /// Simple character hide command
        /// </summary>
        /// <param name="character">Character to hide</param>
        public virtual void Hide(Character character)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;

            Hide(options);
        }

        /// <summary>
        /// Move the character to a position then hide it
        /// </summary>
        /// <param name="character">Character to hide</param>
        /// <param name="toPosition">Where the character will disapear to</param>
        public virtual void Hide(Character character, string toPosition)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.toPosition = stage.GetPosition(toPosition);
            options.move = true;

            Hide(options);
        }

        /// <summary>
        /// From lua, you can pass an options table with named arguments
        /// example:
        ///     stage.hide{character=jill, toPosition="left"}
        /// Any option available in the PortraitOptions is available from Lua
        /// </summary>
        /// <param name="optionsTable">Moonsharp Table</param>
        public virtual void Hide(Table optionsTable)
        {
            Hide(PortraitUtil.ConvertTableToPortraitOptions(optionsTable, stage));
        }

        /// <summary>
        /// Moves Character in front of other characters on stage
        /// </summary>
        public virtual void MoveToFront(Character character)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;

            MoveToFront(CleanPortraitOptions(options));
        }


        protected virtual void DoMoveTween(Character character, RectTransform fromPosition, RectTransform toPosition, float moveDuration, Boolean waitUntilFinished)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.fromPosition = fromPosition;
            options.toPosition = toPosition;
            options.moveDuration = moveDuration;
            options.waitUntilFinished = waitUntilFinished;

            DoMoveTween(options);
        }

        #endregion 
    }
}
