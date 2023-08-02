using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fungus.EditorUtils;
using Spine.Unity;
using Fungus;
[CustomEditor(typeof(SpineCharaAni))]
//flow command editor
public class SpineCharaAniEditor : CommandEditor
{
    private SerializedProperty aSkeletonGraphic;
    private SerializedProperty StagePro;
    private SerializedProperty FacingPro;
    private SerializedProperty aAnimationPro;
    private SerializedProperty FromPosPro;
    private SerializedProperty ToPosPro;
    private SerializedProperty MovePro;
    private SerializedProperty LoopPro;
    private SerializedProperty DisplayProp;

    private SerializedProperty aInitialSkinName;
    private SerializedProperty WaitAnimationFinishPro;
    private SerializedProperty WaitDialogPro;
    private SerializedProperty OffestPro;
    private SerializedProperty FadePro;

    private SerializedProperty tweenTime;
    public override void OnEnable()
    {
        aSkeletonGraphic = serializedObject.FindProperty("aTarget");
        DisplayProp = serializedObject.FindProperty("display");
        StagePro = serializedObject.FindProperty("stage");
        FacingPro = serializedObject.FindProperty("facing");
        aAnimationPro = serializedObject.FindProperty("aAnimation");
        FromPosPro = serializedObject.FindProperty("fromPosition");
        ToPosPro = serializedObject.FindProperty("toPosition");
        MovePro = serializedObject.FindProperty("move");
        LoopPro = serializedObject.FindProperty("loop");
        aInitialSkinName = serializedObject.FindProperty("aInitialSkinName");
        WaitAnimationFinishPro = serializedObject.FindProperty("waitAnimationFinish");
        WaitDialogPro = serializedObject.FindProperty("waitDialog");
        OffestPro = serializedObject.FindProperty("offest");
        FadePro = serializedObject.FindProperty("fade");
        tweenTime = serializedObject.FindProperty("aTween");

    }


    public override void DrawCommandGUI()
    {
        // EditorGUILayout.PropertyField(aSkeletonGraphic);
        serializedObject.Update();
        SpineCharaAni tar = target as SpineCharaAni;

        if (Stage.ActiveStages.Count > 1)//有複數才會顯示
        {
            CommandEditor.ObjectField<Stage>(StagePro,
                                        new GUIContent("Portrait Stage", "Stage to display the character portraits on"),
                                        new GUIContent("<Default>"),
                                        Stage.ActiveStages);
        }
        else
        {
            tar._Stage = null;

        }

        EditorGUI.BeginChangeCheck();//選取造型

        // CommandEditor.ObjectField(aSkeletonGraphic,
        //                         new GUIContent("SkeletonGraphic", "SkeletonGraphic representing character"),
        //                         new GUIContent("<None>")
        //                         , SkeletonGraphic.SkeletonGraphicList);

        EditorGUILayout.PropertyField(aSkeletonGraphic, new GUIContent("SkeletonGraphic", "SkeletonGraphic representing character"));

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }


        if (aSkeletonGraphic.objectReferenceValue != null)
        {

            string[] displayLabels = StringFormatter.FormatEnumNames(tar.Display, "<None>");
            DisplayProp.enumValueIndex = EditorGUILayout.Popup("Display", (int)DisplayProp.enumValueIndex, displayLabels);
            EditorGUILayout.PropertyField(OffestPro);



            // Debug.Log("釋出==>"+aSkeletonGraphic.objectReferenceValue);

            // tar.aSkeletonGraphic=aSkeletonGraphic.objectReferenceValue as SkeletonGraphic;//將更改後的Value反應在script

            if (tar.Display != DisplayType.None)
            {
                SerializedProperty aFadeAni = tweenTime.FindPropertyRelative("aFadeAniDuration");

                if (tar.Display != DisplayType.Hide)
                {
                    EditorGUILayout.PropertyField(FadePro, new GUIContent("FadeIn", "淡入"));
                    EditorGUILayout.PropertyField(aFadeAni);

                    CommandEditor.StringField(aInitialSkinName,
                                new GUIContent("Skin", "Change representing Skin"),
                                new GUIContent("<None>"),
                                GetSkinStrings(tar));

                    CommandEditor.StringField(aAnimationPro,
                                            new GUIContent("Animation", "Animation representing character"),
                                            new GUIContent("<None>"),
                                             GetSkeletonStrings(tar));
                    EditorGUILayout.PropertyField(LoopPro);
                }
                else if (tar.Display == DisplayType.Hide)
                {
                    EditorGUILayout.PropertyField(FadePro, new GUIContent("FadeOut", "淡出"));
                    EditorGUILayout.PropertyField(aFadeAni);
                }
                aFadeAni.floatValue = aFadeAni.floatValue <= 0 ? 0 : aFadeAni.floatValue;



                if (DisplayProp.enumValueIndex > 0 && aAnimationPro.stringValue != null)
                {
                    EditorGUILayout.PropertyField(WaitAnimationFinishPro);
                    EditorGUILayout.PropertyField(WaitDialogPro);

                }

                string[] facingArrows = new string[]
                    {
                            "<Previous>",
                            "<--",
                            "-->",
                    };
                FacingPro.enumValueIndex = EditorGUILayout.Popup("Facing", FacingPro.enumValueIndex, facingArrows);
                SetMoveProperty(tar);

            }

        }


        serializedObject.ApplyModifiedProperties();

    }

    public void SetMoveProperty(SpineCharaAni tar)
    {
        Stage s = tar._Stage;

        if (tar._Stage == null)            // If default portrait stage selected
        {
            if (tar._Stage == null)        // If no default specified, try to get any portrait stage in the scene
            {
                s = GameObject.FindObjectOfType<Stage>();
            }
        }
        EditorGUILayout.PropertyField(MovePro);

        SerializedProperty aMoveAni = tweenTime.FindPropertyRelative("aMoveAniDuration");
        if (tar.Move)
        {
            CommandEditor.ObjectField<RectTransform>(FromPosPro,
                            new GUIContent("From Position", "Move the portrait to this position"),
                            new GUIContent("<Previous>"),
                            s.Positions);
            EditorGUILayout.PropertyField(aMoveAni);

        }
        aMoveAni.floatValue = aMoveAni.floatValue <= 0 ? 0 : aMoveAni.floatValue;

        CommandEditor.ObjectField<RectTransform>(ToPosPro,
        new GUIContent("To Position", "Move the portrait to this position"),
        new GUIContent("<Previous>"),
        s.Positions);


    }



    public List<string> GetSkeletonStrings(SpineCharaAni tar)
    {
        // Debug.Log("測試==>" + tar.aSkeletonGraphic);
        List<string> CollectAniName = new List<string>();
        // Debug.Log("偵測動畫=>" + tar.aSkeletonGraphic.skeletonDataAsset.toAnimation);
        foreach (var ani in tar.aTarget.aSkeletonGraphic.skeletonDataAsset.GetAnimationStateData().SkeletonData.Animations)
        {
            CollectAniName.Add(ani.Name);
        }

        return CollectAniName;
    }

    public List<string> GetSkinStrings(SpineCharaAni tar)
    {
        // Debug.Log("測試==>" + tar.aSkeletonGraphic);
        List<string> CollectAniName = new List<string>();
        // Debug.Log("偵測動畫=>" + tar.aSkeletonGraphic.skeletonDataAsset.toAnimation);
        foreach (var ani in tar.aTarget.aSkeletonGraphic.skeletonDataAsset.GetAnimationStateData().SkeletonData.Skins)
        {
            CollectAniName.Add(ani.Name);
        }

        return CollectAniName;
    }





}
