// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Define a set of screen positions where character sprites can be displayed and controls portraits.
    /// </summary>
    [ExecuteInEditMode]
    public class Stage : PortraitController  //hierarchy裡面的 script
    {
        [Tooltip("Canvas object containing the stage positions.")]
        [SerializeField] protected Canvas portraitCanvas;

        public Transform CharaParent=null;


       /* public Transform ImageParent
        { 
            get {
                if (!imageParent) {
                    SetImageParent();
                }
                return imageParent;
            }
        }*/

        protected Transform audiosParent = null;

        public Transform AudiosParent
        {
            get
            {
                if (!audiosParent)
                {
                    SetAudiosParent();
                }
                return audiosParent;
            }
        }

        public Transform ImageParent = null;

        public Transform PositionsParent = null;

        public Transform ViewParent = null;


        [Tooltip("Dim portraits when a character is not speaking.")]
        [SerializeField] protected bool dimPortraits;

        [Tooltip("Choose a dimColor")]
        [SerializeField] protected Color dimColor =new Color(0.5f, 0.5f, 0.5f, 1f);

        [Tooltip("Duration for fading character portraits in / out.")]
        [SerializeField] protected float fadeDuration = 0.5f;

        [Tooltip("Duration for moving characters to a new position")]
        [SerializeField] protected float moveDuration = 1f;

        [Tooltip("Ease type for the fade tween.")]
        [SerializeField] protected LeanTweenType fadeEaseType;

        [Tooltip("Constant offset to apply to portrait position.")]
        [SerializeField] protected Vector2 shiftOffset;

        [Tooltip("The position object where characters appear by default.")]
        [SerializeField] protected RectTransform defaultPosition;

        [Tooltip("List of stage position rect transforms in the stage.")]
        [SerializeField] protected List<RectTransform> positions;

        protected List<Character> charactersOnStage = new List<Character>();

        protected List<GameObject> spineCharaOnStageList = new List<GameObject>();

        protected static List<Stage> activeStages = new List<Stage>();

        private bool exe = true;


        protected virtual void OnEnable()
        {
            if (!ConfirmParentComponent()) {
                return;
            }
            Debug.Log("執行stage的父母物件=>" + gameObject.transform.parent.name);
            Debug.Log("執行stage=>" + gameObject.name);


            if (gameObject.name == "_CommandCopyBuffer"|| transform.parent.name == "_CommandCopyBuffer")
            {
                return;
            }

            if (!activeStages.Contains(this))
            {
                activeStages.Add(this);
            }
            if (portraitCanvas==null) {
                if (transform.Find("Canvas").GetComponent<Canvas>()) {
                    portraitCanvas = transform.Find("Canvas").GetComponent<Canvas>();
                }
                else
                {
                    CreateCanavas();
                }
            }
            SetCnavasCamera();
            Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(DetectPosStatus());

        }

        public IEnumerator DetectPosStatus()
        {
            while (exe) {
                if (PositionsParent.childCount!=positions.Count) {
                    positions.Clear();

                    for (int i=0;i<PositionsParent.childCount;i++) {
                          var pos = PositionsParent.GetChild(i);

                        RectTransform getRect = null;

                        if (pos.TryGetComponent<RectTransform>(out getRect)) {
                            positions.Add(getRect);
                        }
                    
                    }
                
                }

                yield return null;
            }
        }


        protected virtual void OnDisable()
        {
            exe = false;
            StopAllCoroutines();
            activeStages.Remove(this);

        }

        protected virtual void Start()
        {
            if (Application.isPlaying &&
                portraitCanvas != null)
            {
                // Ensure the stage canvas is active
                portraitCanvas.gameObject.SetActive(true);
            }
        }

        #region Public members

        /// <summary>
        /// Gets the list of active stages.
        /// </summary>
        public static List<Stage> ActiveStages { get { return activeStages; } }

        /// <summary>
        /// Returns the currently active stage.
        /// </summary>
        public static Stage GetActiveStage()
        {
            if (Stage.activeStages == null ||
                Stage.activeStages.Count == 0)
            {
                return null;
            }

            return Stage.activeStages[0];
        }

        /// <summary>
        /// Canvas object containing the stage positions.
        /// </summary>
        public virtual Canvas PortraitCanvas { get { return portraitCanvas; } }

        /// <summary>
        /// Dim portraits when a character is not speaking.
        /// </summary>
        public virtual bool DimPortraits { get { return dimPortraits; } set { dimPortraits = value; } }

        /// <summary>
        /// Choose a dimColor.
        /// </summary>
        public virtual Color DimColor { get { return dimColor; } set { dimColor = value; } }

        /// <summary>
        /// Duration for fading character portraits in / out.
        /// </summary>
        public virtual float FadeDuration { get { return fadeDuration; } set { fadeDuration = value; } }

        /// <summary>
        /// Duration for moving characters to a new position.
        /// </summary>
        public virtual float MoveDuration { get { return moveDuration; } set { moveDuration = value; } }

        /// <summary>
        /// Ease type for the fade tween.
        /// </summary>
        public virtual LeanTweenType FadeEaseType { get { return fadeEaseType; }set { fadeEaseType = value; } }

        /// <summary>
        /// Constant offset to apply to portrait position.
        /// </summary>
        public virtual Vector2 ShiftOffset { get { return shiftOffset; }set { shiftOffset = value; } }

        /// <summary>
        /// The position object where characters appear by default.
        /// </summary>
        public virtual RectTransform DefaultPosition { get { return defaultPosition; }set { defaultPosition = value; } }

        /// <summary>
        /// List of stage position rect transforms in the stage.
        /// </summary>
        public virtual List<RectTransform> Positions { get { return positions; } }

        /// <summary>
        /// List of currently active characters on the stage.
        /// </summary>
        public virtual List<Character> CharactersOnStage { get { return charactersOnStage; } }

        public virtual List<GameObject> SpineCharaOnStageList { get { return spineCharaOnStageList; } }

        /// <summary>
        /// Searches the stage's named positions
        /// If none matches the string provided, give a warning and return a new RectTransform
        /// </summary>
        public RectTransform GetPosition(string positionString)
        {
            if (string.IsNullOrEmpty(positionString))
            {
                return null;
            }

            for (int i = 0; i < positions.Count; i++)
            {
                if ( String.Compare(positions[i].name, positionString, true) == 0 )
                {
                    return positions[i];
                }
            }
            return null;
        }
        public View GetView(string rectName)
        {

            if (rectName.Equals(null))
            {
                return null;
            }

            for (int i=0;i<ViewParent.childCount;i++) {
                View view = ViewParent.GetChild(i).GetComponent<View>();
                if (String.Compare(view.name,rectName,true)==0) {
                    return view;
                }
            }
            return null;
        }

        public Image GetImage(string imageName)
        {
            if (imageName.Equals(null)) {
                return null;
            }
            for (int i = 0; i < ImageParent.childCount; i++)
            {
                Image img = ImageParent.GetChild(i).GetComponent<Image>();
                if (String.Compare(img.name, imageName, true) == 0)
                {
                    return img;
                }
            }
            return null;


        }

        public SpriteRenderer GetSpriteRenderer(string imageName)
        {
            if (imageName.Equals(null))
            {
                return null;
            }
            for (int i = 0; i < ImageParent.childCount; i++)
            {
                SpriteRenderer sprite = ImageParent.GetChild(i).GetComponent<SpriteRenderer>();
                if (String.Compare(sprite.name, imageName, true) == 0)
                {
                    return sprite;
                }
            }
            return null;
        }

        public void CloseOtherRaycastTarget(RectTransform tarRect) {

            foreach (var rect in positions) {

                if (rect==tarRect) {

                }
                else
                {
                    if (rect.GetComponent<Image>())
                    {
                        rect.GetComponent<Image>().raycastTarget = false;
                    }
                }
            
            }

        }

        public void OpenAllPositionRaycastTarget()
        {

            foreach (var rect in positions)
            {
                if (rect.GetComponent<Image>()) 
                {
                    rect.GetComponent<Image>().raycastTarget = true;
                }
            }


        }

        public void ClearData()
        {

            foreach (var pos in positions) { 
            DestroyImmediate(pos.gameObject);
            }
            positions.Clear();

            List<GameObject> childs = new List<GameObject>();

            for (int i=0;i<ViewParent.childCount;i++) {
            var child = ViewParent.GetChild(i);
                childs.Add(child.gameObject);
            }

            for (int i = 0; i < ImageParent.childCount; i++)
            {
                var child = ImageParent.GetChild(i);
                childs.Add(child.gameObject);
            }

            for (int i = 0; i <AudiosParent.childCount; i++)
            {
                var child = AudiosParent.GetChild(i);
                childs.Add(child.gameObject);
            }
            foreach (GameObject obj in childs) {
                DestroyImmediate(obj);
            }
            childs.Clear();





        }

        public void SetCnavasCamera()
        {
            portraitCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            portraitCanvas.worldCamera = Camera.main;
            portraitCanvas.renderMode = RenderMode.WorldSpace;
              //  LayoutRebuilder.ForceRebuildLayoutImmediate
        }

        public void SetImageParent()
        {
            if (!ImageParent) {
                var imgPar = gameObject.transform.parent.Find("Images");
                if (imgPar) {
                    ImageParent=imgPar;
                }
                else
                {
                    GameObject sp = new GameObject("Images");
                    sp.transform.SetParent(transform.parent);
                    sp.transform.position = Vector3.zero;
                    ImageParent = sp.transform;
                }
            }

        }

        public void SetAudiosParent()
        {
            if (!audiosParent)
            {
                var AudPar = gameObject.transform.parent.Find("Audios");
                if (AudPar)
                {
                    audiosParent = AudPar;
                }
                else
                {
                    GameObject sp = new GameObject("Audios");
                    sp.transform.SetParent(transform.parent);
                    sp.transform.position = Vector3.zero;
                    audiosParent = sp.transform;
                }
            }

        }

        public void CreateCanavas()
        {

            GameObject sp = new GameObject("Canvas", 
                typeof(RectTransform), typeof(Canvas),typeof(GraphicRaycaster),
                typeof(CanvasScaler),typeof(CanvasGroup)
                );
            portraitCanvas=sp.GetComponent<Canvas>();



        }

        public bool ConfirmParentComponent()
        {
            if (gameObject.transform.parent==null) {
                return false;
            }
            else
             {
                Flowchart flow = null;

                if (transform.parent.TryGetComponent<Flowchart>(out flow)) {

                        if (flow.name!= "_CommandCopyBuffer") {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                }
                else
                {
                    return false;
                }
            }

        }




        #endregion
    }
}

