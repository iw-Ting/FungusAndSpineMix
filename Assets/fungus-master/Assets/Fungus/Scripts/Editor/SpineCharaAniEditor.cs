using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fungus.EditorUtils;
using Spine.Unity;
using Fungus;
using System;
[CustomEditor(typeof(SpineCharaAni))]


//flow command editor
public class SpineCharaAniEditor : CommandEditor
{
    
    private SerializedProperty aSkeletonGraphicPro;
    private SerializedProperty StagePro;
    private SerializedProperty FacingPro;
    private SerializedProperty aAnimationPro;
    private SerializedProperty FromPosPro;
    private SerializedProperty ToPosPro;
    private SerializedProperty clickPositionPro;
    private SerializedProperty MovePro;
    private SerializedProperty LoopPro;
    private SerializedProperty DisplayProp;
    private SerializedProperty aClickButtonSizeProp;
    private SerializedProperty aClickButtonSizeSettingProp;
    private SerializedProperty aInitialSkinName;
    private SerializedProperty aSpineOrder;

    /////////////////////////// condition bool /////////////////
    private SerializedProperty WaitAnimationFinishPro;
    //  private SerializedProperty WaitForClick;
    private SerializedProperty clickModlePro;

    private SerializedProperty OffestPro;
    private SerializedProperty FadePro;
    
    private SerializedProperty tweenTime;


    private bool SetSpineOrder = false;
    public override void OnEnable()
    {
        aSkeletonGraphicPro = serializedObject.FindProperty("aTarget");
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
        clickModlePro = serializedObject.FindProperty("clickMode");
        clickPositionPro = serializedObject.FindProperty("ClickPos");
        aClickButtonSizeProp = serializedObject.FindProperty("ClickButtonSize");
        aClickButtonSizeSettingProp = serializedObject.FindProperty("aClickButtonSizeSetting");
        //aClickButtonSizeSetting
        // WaitForClick=serializedObject.FindProperty("waitForClick");
        OffestPro = serializedObject.FindProperty("offest");
        FadePro = serializedObject.FindProperty("fade");
        tweenTime = serializedObject.FindProperty("aTween");
        aSpineOrder = serializedObject.FindProperty("spineOrder");



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

        EditorGUILayout.PropertyField(aSkeletonGraphicPro, new GUIContent("SkeletonGraphic", "SkeletonGraphic representing character"));

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }


        if (aSkeletonGraphicPro.objectReferenceValue != null)
        {
            CharaSpine cs=aSkeletonGraphicPro.objectReferenceValue as CharaSpine;

            if (cs.aSkeletonGraphic==null) {
                EditorGUILayout.HelpBox(new GUIContent("Can Not Have a Chara"));
                return;
            }


            string[] displayLabels = StringFormatter.FormatEnumNames(tar.Display, "<None>");
            DisplayProp.enumValueIndex = EditorGUILayout.Popup("Display", (int)DisplayProp.enumValueIndex, displayLabels);
            EditorGUILayout.PropertyField(OffestPro);

            

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Order",GUILayout.Width(150));
            SetSpineOrder = EditorGUILayout.Toggle(SetSpineOrder);
            EditorGUILayout.EndHorizontal();

            if (SetSpineOrder)
            {
                EditorGUILayout.PropertyField(aSpineOrder);
            }


            // Debug.Log("釋出==>"+aSkeletonGraphic.objectReferenceValue);

            // tar.aSkeletonGraphic=aSkeletonGraphic.objectReferenceValue as SkeletonGraphic;//將更改後的Value反應在script

            if (tar.Display != DisplayType.None)
            {
                SerializedProperty aFadeAni = tweenTime.FindPropertyRelative("aFadeAniDuration");

                if (tar.Display != DisplayType.Hide)
                {
                    EditorGUILayout.PropertyField(FadePro, new GUIContent("FadeIn", "淡入"));

                    if (tar.Fade) {
                        EditorGUILayout.PropertyField(aFadeAni);
                    }

                    CommandEditor.StringField(aInitialSkinName,
                                new GUIContent("Skin", "Change representing Skin"),
                                new GUIContent("<None>"),
                                tar.aTarget.aSkeletonGraphic.GetSkinStrings());

                    CommandEditor.StringField(aAnimationPro,
                                            new GUIContent("Animation", "Animation representing character"),
                                            new GUIContent("<None>"),
                                             tar.aTarget.aSkeletonGraphic.GetSkeletonStrings());
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
                    EditorGUILayout.LabelField("Animation  Judge", EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(clickModlePro);

                    EditorGUILayout.PropertyField(WaitAnimationFinishPro);
                    

                    
                    //  EditorGUILayout.PropertyField(WaitForClick);

                }

                if (tar.mClickMode == ClickMode.ClickOnButton)
                {
                    if (tar._Stage == null)
                    {

                        tar._Stage = GameObject.FindObjectOfType<Stage>();

                    }


                    if (tar._Stage != null)
                    {
                        CommandEditor.ObjectField<RectTransform>(clickPositionPro,
                            new GUIContent("ButtonCreatePos", "CreateTouchButton"),
                            new GUIContent("<Previous>"),
                            tar._Stage.Positions);
                    }

                    EditorGUILayout.PropertyField(aClickButtonSizeSettingProp);

                    string[] enumName = aClickButtonSizeSettingProp.enumNames;
                    string clickSetting = enumName[ aClickButtonSizeSettingProp.enumValueIndex]; 

                    var enumStatus=(ClickAeraSetting)Enum.Parse(typeof(ClickAeraSetting), clickSetting);

                    switch (enumStatus)
                    {
                        case ClickAeraSetting.Default:

                            break;
                        case ClickAeraSetting.Customize:
                            EditorGUILayout.PropertyField(aClickButtonSizeProp);
                            break;
                    }
                    tar.StartDraw = true;
                }
                else
                {
                    tar.StartDraw = false;
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
        serializedObject.Update();

    }

    public void SetMoveProperty(SpineCharaAni tar)
    {
        if (tar._Stage == null)
        {

            tar._Stage = GameObject.FindObjectOfType<Stage>();

        }
        Stage s = tar._Stage;


            if (s == null)        // If no default specified, try to get any portrait stage in the scene
            {
                EditorGUILayout.HelpBox("No portrait stage has been set.", MessageType.Error);
                return;
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

    



    // public List<string> GetSkeletonStrings(SpineCharaAni tar)//獲得骨架數據的動作陣列
    // {
    //     // Debug.Log("測試==>" + tar.aSkeletonGraphic);
    //     List<string> CollectAniName = new List<string>();
    //     // Debug.Log("偵測動畫=>" + tar.aSkeletonGraphic.skeletonDataAsset.toAnimation);


    //     foreach (var ani in tar.aTarget.aSkeletonGraphic.SkeletonDataAsset.GetAnimationStateData().SkeletonData.Animations)
    //     {

    //         CollectAniName.Add(ani.Name);
    //     }

    //     return CollectAniName;
    // }

    // public List<string> GetSkinStrings(SpineCharaAni tar)//獲得骨架數據的造型陣列
    // {
    //     // Debug.Log("測試==>" + tar.aSkeletonGraphic);
    //     List<string> CollectAniName = new List<string>();
    //     // Debug.Log("偵測動畫=>" + tar.aSkeletonGraphic.skeletonDataAsset.toAnimation);
    //     foreach (var ani in tar.aTarget.aSkeletonGraphic.SkeletonDataAsset.GetAnimationStateData().SkeletonData.Skins)
    //     {
    //         CollectAniName.Add(ani.Name);
    //     }

    //     return CollectAniName;
    // }





}
