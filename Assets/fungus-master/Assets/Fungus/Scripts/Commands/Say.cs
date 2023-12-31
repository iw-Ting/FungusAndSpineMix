// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Writes text in a dialog box.
    /// </summary>
    [CommandInfo("Narrative", 
                 "Say", 
                 "Writes text in a dialog box.")]
    [AddComponentMenu("")]
    public class Say : Command, ILocalizable
    {
        // Removed this tooltip as users's reported it obscures the text box
        [TextArea(5,10)]
        [SerializeField] protected string storyText = "";

        [Tooltip("Notes about this story text for other authors, localization, etc.")]
        [SerializeField] protected string description = "";

        [Tooltip("Character that is speaking")]
        [SerializeField] protected Character character;

        [Tooltip("Portrait that represents speaking character")]
        [SerializeField] protected Sprite portrait;//說話人的頭像

        [Tooltip("Voiceover audio to play when writing the text")]
        [SerializeField] protected AudioClip voiceOverClip;

        [Tooltip("Always show this Say text when the command is executed multiple times")]
        [SerializeField] protected bool showAlways = true;

        [Tooltip("Number of times to show this Say text when the command is executed multiple times")]
        [SerializeField] protected int showCount = 1;

        [Tooltip("Type this text in the previous dialog box.")]
        [SerializeField] protected bool extendPrevious = false;

        [Tooltip("Fade out the dialog box when writing has finished and not waiting for input.")]
        [SerializeField] protected bool fadeWhenDone = false;

        [Tooltip("Wait for player to click before continuing.")]
        [SerializeField] protected bool waitForClick = true;

        [Tooltip("Stop playing voiceover when text finishes writing.")]
        [SerializeField] protected bool stopVoiceover = true;

        [Tooltip("Wait for the Voice Over to complete before continuing")]
        [SerializeField] protected bool waitForVO = false;

        //add wait for vo that overrides stopvo

        [Tooltip("Sets the active Say dialog with a reference to a Say Dialog object in the scene. All story text will now display using this Say Dialog.")]
        [SerializeField] protected SayDialog setSayDialog;

        protected int executionCount;

        #region Public members

        /// <summary>
        /// Character that is speaking.
        /// </summary>
        public virtual Character _Character { get { return character; } }

        /// <summary>
        /// Portrait that represents speaking character.
        /// </summary>
        public virtual Sprite Portrait { get { return portrait; } set { portrait = value; } }

        /// <summary>
        /// Type this text in the previous dialog box.
        /// </summary>
        public virtual bool ExtendPrevious { get { return extendPrevious; } }

        public override void OnEnter()
        {
            if (!showAlways && executionCount >= showCount)
            {
                Continue();
                return;
            }

            executionCount++;

            // Override the active say dialog if needed
            if (character != null && character.SetSayDialog != null)
            {
                SayDialog.ActiveSayDialog = character.SetSayDialog;
            }

            if (setSayDialog != null)
            {
                SayDialog.ActiveSayDialog = setSayDialog;
            }

            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                Continue();
                return;
            }
    
            var flowchart = GetFlowchart();

            sayDialog.SetActive(true);
            
            sayDialog.SetCharacter(character);//設置對話框角色
            sayDialog.SetCharacterImage(portrait);//設置角色頭像

            string displayText = storyText;


            var activeCustomTags = CustomTag.activeCustomTags;
            for (int i = 0; i < activeCustomTags.Count; i++)
            {
                var ct = activeCustomTags[i];
                displayText = displayText.Replace(ct.TagStartSymbol, ct.ReplaceTagStartWith);
                if (ct.TagEndSymbol != "" && ct.ReplaceTagEndWith != "")
                {
                    displayText = displayText.Replace(ct.TagEndSymbol, ct.ReplaceTagEndWith);
                }
            }


            string content = flowchart.SubstituteVariables(displayText);

            //sayDialog.GetWriter().SetAutoPlay

            if (GetNextCommand()==typeof(Say)) {
                fadeWhenDone = true;
            }
            

           StartCoroutine(  sayDialog.ReactionAlpha(true));

            /* sayDialog.Say(subbedText, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip
                 , ()=> {
                 Continue();
             });*/
            Sayinfo sayinfo = new Sayinfo();

            sayinfo.content = content;
            sayinfo.clearPrevious = !extendPrevious;
            sayinfo.waitForClick = waitForClick;
            sayinfo.fadeWhenDone = fadeWhenDone;
            sayinfo.stopVoiceover = stopVoiceover;
            sayinfo.waitForVO = waitForVO;
            sayinfo.voiceOverClip = voiceOverClip;
            sayinfo.onComplete= () =>
            {
                Continue();
            };

            sayinfo.setLogDialog = dialog => {
                StoryControl sc = ParentBlock.GetFlowchart().mStoryControl;
                string charaName = "None";
                if (character!=null) {
                    charaName = character.NameText;
                }
                Debug.Log("記錄對話");
                sc.SaveDialogRecord(new DialogInfo(charaName, dialog, voiceOverClip));
            };

            sayDialog.Say(sayinfo);

        }

        public override void OnExit()
        {
         //   SayDialog sd= SayDialog.GetSayDialog();
         //   StoryControl sc = ParentBlock.GetFlowchart().mStoryControl;
        //    sc.SaveDialogRecord(new DialogInfo(sd.NameText,InputUISupportScript.RemoveAllRichText( sd.StoryText),voiceOverClip));
        }



        public override string GetSummary()
        {
            string namePrefix = "";
            if (character != null) 
            {
                namePrefix = character.NameText + ": ";
            }
            if (extendPrevious)
            {
                namePrefix = "EXTEND" + ": ";
            }
            return namePrefix + "\"" + storyText + "\"";
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override void OnReset()
        {
            executionCount = 0;
        }

        public override void OnStopExecuting()
        {
            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                return;
            }

            sayDialog.Stop();
        }

        #endregion

        #region ILocalizable implementation

        public virtual string GetStandardText()
        {
            return storyText;
        }

        public virtual void SetStandardText(string standardText)
        {
            storyText = standardText;
        }

        public virtual string GetDescription()
        {
            return description;
        }
        
        public virtual string GetStringId()
        {
            // String id for Say commands is SAY.<Localization Id>.<Command id>.[Character Name]
            string stringId = "SAY." + GetFlowchartLocalizationId() + "." + itemId + ".";
            if (character != null)
            {
                stringId += character.NameText;
            }

            return stringId;
        }

        #endregion
    }
    public struct Sayinfo
    {
        public string content;
        public bool clearPrevious;
        public bool waitForClick;
        public bool fadeWhenDone;
        public bool stopVoiceover;
        public bool waitForVO;
        public AudioClip voiceOverClip;
        public Action<string> setLogDialog;
        public Action onComplete;
        

        public Sayinfo(string _content,bool _clearPrevious,bool _waitForClick,bool _fadeWhenDone,bool _stopVoiceover, bool _waitForVO, AudioClip _voiceOverClip, Action<string> _setLogDialog, Action _onComplete)
        {
            content = _content;
            clearPrevious= _clearPrevious;
            waitForClick= _waitForClick;
            fadeWhenDone= _fadeWhenDone;
            stopVoiceover= _stopVoiceover;
            waitForVO= _waitForVO;
            voiceOverClip= _voiceOverClip;
            setLogDialog=_setLogDialog;
            onComplete=_onComplete;
        }


    }




}